using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VeterinaryClinic.Core;
using VeterinaryClinic.Core.Datebase;
using VeterinaryClinic.Core.Entity;
using VeterinaryClinic.Core.Helper;
using VeterinaryClinic.Core.Interface;

namespace VeterinaryClinic.Forms.Editing
{
    /// <summary>
    /// Логика взаимодействия для WindowEditRecord.xaml
    /// </summary>
    public partial class WindowEditRecord : Window
    {
        private Record record;
        private List<Employee> veterinars = new List<Employee>();

        private string formatPhoto = "";
        private string pathPhoto = "";
        private bool isChangePhoto = false;

        public WindowEditRecord(Record _record)
        {
            InitializeComponent();

            record = _record;

            loadCbTypeAnimals();
            loadCbVenerinar();
            LoadData(record);
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            EventSystem.InvokeEventBlackBackground();
            this.Close();
        }
        private void loadCbTypeAnimals()
        {
            Command command = new Command();
            command.LoadData("Select TitleAnimals From TypeAnimals");
            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                cbTypeAnimal.Items.Add(command.MainTable.Rows[i][0].ToString());
            }
        }
        private void loadCbBreed()
        {
            cbBreed.Items.Clear();
            Command command = new Command();
            command.LoadData($"Select TitleBreed From Breed WHERE id_typeAnimals = {cbTypeAnimal.SelectedIndex + 1}");
            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                cbBreed.Items.Add(command.MainTable.Rows[i][0].ToString());
            }
        }

        private void loadCbVenerinar()
        {
            Command command = new Command();
            command.LoadData("SELECT DISTINCT e.ID_Employee, e.Surname, e.Name, e.Patronymic FROM Employee e LEFT JOIN Posts p ON p.ID_Post = e.ID_Post LEFT JOIN Vacation v ON v.id_employee = e.ID_Employee WHERE (v.id_employee IS NULL OR NOT(GETDATE() BETWEEN v.DateStartVacation AND v.DateEndVacation)) AND p.TitlePost = 'Ветеринар'");
            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                veterinars.Add(new Employee(command.MainTable.Rows[i][0].ToString(), command.MainTable.Rows[i][1].ToString(),
                    command.MainTable.Rows[i][2].ToString(), command.MainTable.Rows[i][3].ToString(), "", "", "", "", "", false, "", ""));

                cbVeterinarian.Items.Add(veterinars[i].FullName);
            }
        }

        private void LoadData(Record record)
        {
            tbSurname.Text = record.Surname;
            tbName.Text = record.Name;
            tbPatronymic.Text = record.Patronymic;
            tbPhone.Text = record.Phone;
            cbTypeAnimal.Text = record.AnimalClient.TypeAnimal;
            cbBreed.Text = record.AnimalClient.Breed;
            tbNickname.Text = record.AnimalClient.Nickname;
            cbVeterinarian.Text = record.Veterinar.FullName; // TODO: сделать по нормальному
            dpDate.Text = record.DateReception.ToString();
            tpTime.Text = record.TimeReception.ToString();
            tbClaim.Text = record.Claim;
            cbGender.Text = record.AnimalClient.Gender;

            if (record.AnimalClient.PathPhoto != "" && File.Exists(PhotoDB.GetPathAnimal(record.AnimalClient.PathPhoto)))
            {
                pathPhoto = PhotoDB.GetPathAnimal(record.AnimalClient.PathPhoto);
                formatPhoto = System.IO.Path.GetExtension(pathPhoto);

                lAddPhoto.Visibility = Visibility.Collapsed;

                var bytes = File.ReadAllBytes(pathPhoto);
                photo.ImageSource = PhotoDB.ToImage(bytes);

                ellipsePhoto.Visibility = Visibility.Visible;
            }
        }

        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                lAddPhoto.Visibility = Visibility.Collapsed;

                var bytes = File.ReadAllBytes(files[0]);
                photo.ImageSource = PhotoDB.ToImage(bytes);
                ellipsePhoto.Visibility = Visibility.Visible;

                pathPhoto = files[0];
                formatPhoto = System.IO.Path.GetExtension(pathPhoto);
                isChangePhoto = true;
            }
        }

        private void cbTypeAnimal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadCbBreed();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // проверка на заполнение полей
            if (isEmptyTextBoxes())
            {
                return;
            }

            // проверка на существующий телефон
            if (isPhoneExist(record.IDClient))
            {
                MessageBox.Show("Этот телефон уже есть в базе! Операция отменена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // проверка на доступность записи на это время
            if (isCheckRecordTime())
            {
                MessageBox.Show("Выберите другое время, так как в это время ветеринар обслуживает другого клиента", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // сохраняем фотку (по необходимости)
            saveImage(record.AnimalClient.IDAnimal);

            // Редактирование данных клиента
            Command command = new Command();
            command.SendCommand($"Update Client Set Surname = '{tbSurname.Text}',[Name] = '{tbName.Text}', Patronymic = '{tbPatronymic.Text}', Phone = '{tbPhone.Text}' Where ID_Client = {record.IDClient}");

            // Ищем питомца клиента либо добавляем нового
            if (!isAnimalExist(tbNickname.Text,record.AnimalClient.Breed))
            {
                // если питомца нет, то создаем нового
                command.AddParameter("@Nickname", System.Data.SqlDbType.NVarChar, tbNickname.Text);
                command.AddParameter("@Gender", System.Data.SqlDbType.NVarChar, cbGender.Text);
                command.AddParameter("@PathPhoto", System.Data.SqlDbType.NVarChar, record.AnimalClient.PathPhoto);
                command.AddParameter("@idClient", System.Data.SqlDbType.NVarChar, record.IDClient);
                command.AddParameter("@TypeAnimal", System.Data.SqlDbType.NVarChar, cbTypeAnimal.Text);
                command.AddParameter("@Breed", System.Data.SqlDbType.NVarChar, cbBreed.Text);

                command.SendCommand("Insert Into Animals VALUES (@Nickname,@Gender,@PathPhoto,@idClient,(Select ID_TypeAnimals From TypeAnimals Where TitleAnimals = @TypeAnimal),(Select ID_Breed From Breed Where TitleBreed = @Breed))");
                command.LoadData($"Select ID_Animal From Animals Where Nickname = '{tbNickname.Text}' AND id_client = {record.IDClient}");
                string idAnimal = command.MainTable.Rows[0][0].ToString();
                command.SendCommand($"Update Record Set DateReception = '{dpDate.Text}', TimeReception = '{tpTime.Text}', " +
                    $"Claim = '{tbClaim.Text}', id_client = {record.IDClient},id_employee = {veterinars[cbVeterinarian.SelectedIndex].ID}," +
                    $"id_animal = {idAnimal},id_statusRecord = 1 Where ID_Record = {record.ID}");
            }
            else // если питомец уже есть
            {
                command.SendCommand($"Update Record Set DateReception = '{dpDate.Text}', TimeReception = '{tpTime.Text}', " +
                    $"Claim = '{tbClaim.Text}', id_client = {record.IDClient},id_employee = {veterinars[cbVeterinarian.SelectedIndex].ID}," +
                    $"id_animal = {record.AnimalClient.IDAnimal},id_statusRecord = 1 Where ID_Record = {record.ID}");
            }
            
            MessageBox.Show("Запись изменена!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            EventSystem.InvokeEventBlackBackground();
            EventSystem.InvokeUpdateRecords();
            this.Close();
        }

        private void saveImage(string _idAnimal)
        {
            try
            {
                if (!isChangePhoto) return; // если фотку менять не нужно

                record.AnimalClient.PathPhoto = _idAnimal + formatPhoto;
                string copyToPath = Environment.CurrentDirectory + Constants.MAIN_PATH_FOLDER_ANIMALS + $"{_idAnimal}{formatPhoto}";
                File.Copy(pathPhoto, copyToPath, true);

                Command command = new Command();
                command.SendCommand($"Update Animals Set PathPhoto = '{record.AnimalClient.PathPhoto}' WHERE ID_Animal = {record.AnimalClient.IDAnimal}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Возвращает true, если телефон уже существует
        /// </summary>
        /// <returns></returns>
        private bool isPhoneExist(string _idCurrentClient)
        {
            Command command = new Command();
            command.LoadData($"Select * From Client Where Phone = '{tbPhone.Text}' And ID_Client <> {_idCurrentClient}");
            if (command.MainTable.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Возвращает true, если питомец уже существует
        /// </summary>
        /// <returns></returns>
        private bool isAnimalExist(string _nameAnimal, string _breed)
        {
            Command command = new Command();
            command.LoadData($"Select * From Animals Where id_client = {record.IDClient} AND Nickname = '{_nameAnimal}' AND id_breed = (SELECT ID_Breed From Breed Where TitleBreed = '{_breed}')");
            if (command.MainTable.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Возвращает true, если запись на это время уже есть
        /// </summary>
        /// <returns></returns>
        private bool isCheckRecordTime()
        {
            Command command = new Command();
            command.LoadData($"Select * From Record Where DateReception = '{dpDate.Text}' AND '{tpTime.Text}' BETWEEN TimeReception AND DATEADD(MINUTE,40,TimeReception)  AND id_employee = {veterinars[cbVeterinarian.SelectedIndex].ID} AND id_client <> {record.IDClient}");

            return command.MainTable.Rows.Count > 0;
        }


        /// <summary>
        /// Возвращает true, если есть пустые поля в главных текстовых полях
        /// </summary>
        /// <returns></returns>
        private bool isEmptyTextBoxes()
        {
            if(tbSurname.Text == "" || tbName.Text == "" || tbPatronymic.Text == "" || tbPhone.Text == "" || tbClaim.Text == "" || cbGender.Text == "" || tbNickname.Text == "" || cbTypeAnimal.Text == "" || cbBreed.Text == "" || cbVeterinarian.Text == "" || tpTime.Text == "" || dpDate.Text == "")
            {
                MessageBox.Show("Заполните все поля!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }
    }
}
