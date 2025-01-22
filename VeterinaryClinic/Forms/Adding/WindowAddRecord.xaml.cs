using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VeterinaryClinic.Core;
using VeterinaryClinic.Core.Datebase;
using VeterinaryClinic.Core.Entity;
using VeterinaryClinic.Core.Helper;

namespace VeterinaryClinic.Forms
{
    /// <summary>
    /// Логика взаимодействия для FormRecord.xaml
    /// </summary>
    public partial class WindowAddRecord : Window
    {
        private List<Employee> veterinars = new List<Employee>();
        private List<Breed> breeds = new List<Breed>();
        private Client client;

        private string pathPhoto = "";
        private string namePhoto = "";
        private string formatPhoto = "";

        private bool isChangePhoto = false;
        public WindowAddRecord(Client _client, string _phone)
        {
            InitializeComponent();

            loadCbTypeAnimals();
            loadCbVenerinar();
            loadUser(_client);
            tbPhone.Text = _phone;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        
        private void loadCbTypeAnimals()
        {
            Command command = new Command();
            command.LoadData("Select TitleAnimals From TypeAnimals");
            for(int i = 0;i<command.MainTable.Rows.Count;i++)
            {
                cbTypeAnimal.Items.Add(command.MainTable.Rows[i][0].ToString());
            }
        }
        private void loadCbBreed()
        {
            cbBreed.Items.Clear();
            breeds.Clear();
            Command command = new Command();
            command.LoadData($"Select * From Breed WHERE id_typeAnimals = {cbTypeAnimal.SelectedIndex + 1}");
            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                cbBreed.Items.Add(command.MainTable.Rows[i][1].ToString());
                breeds.Add(new Breed(command.MainTable.Rows[i][0].ToString(), command.MainTable.Rows[i][1].ToString(), command.MainTable.Rows[i][2].ToString()));
            }

            if (client == null) return;
            if (client.Animals.Count > 0) cbBreed.Text = client.Animals[0].Breed;

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


        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                namePhoto = System.IO.Path.GetFileName(files[0]);
                lAddPhoto.Visibility = Visibility.Collapsed;
                photo.ImageSource = new BitmapImage(new Uri(files[0], UriKind.Relative));
                ellipsePhoto.Visibility = Visibility.Visible;

                pathPhoto = files[0];
                formatPhoto = System.IO.Path.GetExtension(pathPhoto);
                isChangePhoto = true;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            EventSystem.InvokeEventBlackBackground();
            this.Close();
        }

        private void loadUser(Client _client)
        {
            client = _client;
            if (client == null) return;

            tbSurname.Text = client.Surname;
            tbName.Text = client.Name;
            tbPatronymic.Text = client.Patronymic;
            tbPhone.Text = client.Phone;

            if(GlobalVar.CurrentUser.Post != "Администратор")
            {
                cbVeterinarian.Text = $"{GlobalVar.CurrentUser.Surname} {GlobalVar.CurrentUser.Name} {GlobalVar.CurrentUser.Patronymic}";
            }
            

            // загружаем питомца хозяина
            if (client.Animals.Count > 0)
            {
                tbNickname.Text = client.Animals[0].Nickname;
                cbGender.Text = client.Animals[0].Gender;
                cbTypeAnimal.Text = client.Animals[0].TypeAnimal;

                if (client.Animals[0].PathPhoto != "" && File.Exists(PhotoDB.GetPathAnimal(client.Animals[0].PathPhoto)))
                {
                    lAddPhoto.Visibility = Visibility.Collapsed;
                    photo.ImageSource = new BitmapImage(new Uri(PhotoDB.GetPathAnimal(client.Animals[0].PathPhoto), UriKind.Relative));
                    ellipsePhoto.Visibility = Visibility.Visible;
                }
            }          
        }

        private void cbTypeAnimal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadCbBreed();
        }

        private void bttRecord_Click(object sender, RoutedEventArgs e)
        {
            if (client != null) sendAppointmentClient();
            else sendAppointmentNewClient();
        }

        /// <summary>
        /// Отправляет запись на прием зарегистрированного клиента
        /// </summary>
        private void sendAppointmentClient()
        {
            // проверка на заполнение полей
            if (isEmptyTextBoxes())
            {
                return;
            }

            // проверка на существующий телефон
            if (isPhoneExist(client.IDClient))
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

            // ищем питомца клиента
            Command command = new Command();
            command.LoadData($"Select * From Animals Where id_client = {client.IDClient} AND Nickname = '{tbNickname.Text}' AND id_breed = {breeds[cbBreed.SelectedIndex].IDBreed}");

            // если питомец с таким именем есть, то записываем на прием
            if(command.MainTable.Rows.Count > 0)
            {
                command.AddParameter("@DateReception", SqlDbType.Date, dpDate.Text);
                command.AddParameter("@TimeReception", SqlDbType.Time, tpTime.Text);
                command.AddParameter("@Claim", SqlDbType.NVarChar, tbClaim.Text);
                command.AddParameter("@idClient", SqlDbType.Int, client.IDClient);
                command.AddParameter("@idEmployee", SqlDbType.Int, veterinars[cbVeterinarian.SelectedIndex].ID);
                command.AddParameter("@idAnimal", SqlDbType.Int, command.MainTable.Rows[0][0].ToString());
                saveImage(command.MainTable.Rows[0][0].ToString());
                command.SendCommand($"Insert Into Record VALUES(@DateReception,@TimeReception,@Claim,@idClient,@idEmployee,@idAnimal,1)");
            }
            else // если питомца с таким именем нет, то создаем питомца и записываем на прием
            {
                // создаем питомца
                command.LoadData($"Select MAX(ID_Animal) + 1 From Animals");
                string idAnimal = command.MainTable.Rows[0][0].ToString();
                saveImage(idAnimal);

                command.AddParameter("@Nickname", SqlDbType.NVarChar, tbNickname.Text);
                command.AddParameter("@Gender", SqlDbType.NVarChar, cbGender.Text);
                command.AddParameter("@PathPhoto", SqlDbType.NVarChar, namePhoto);
                command.AddParameter("@idClient", SqlDbType.Int, client.IDClient);
                command.AddParameter("@idTypeAnimal", SqlDbType.Int, (cbTypeAnimal.SelectedIndex + 1).ToString());
                command.AddParameter("@idBreed", SqlDbType.Int, breeds[cbBreed.SelectedIndex].IDBreed);
                command.SendCommand("Insert Into Animals VALUES(@Nickname,@Gender,@PathPhoto,@idClient,@idTypeAnimal,@idBreed)");

                // записываем на прием
                command.LoadData($"Select * From Animals Where id_client = {client.IDClient} AND Nickname = '{tbNickname.Text}' AND id_breed = {breeds[cbBreed.SelectedIndex].IDBreed}");
                command.AddParameter("@DateReception", SqlDbType.Date, dpDate.Text);
                command.AddParameter("@TimeReception", SqlDbType.Time, tpTime.Text);
                command.AddParameter("@Claim", SqlDbType.NVarChar, tbClaim.Text);
                command.AddParameter("@idClient", SqlDbType.Int, client.IDClient);
                command.AddParameter("@idEmployee", SqlDbType.Int, veterinars[cbVeterinarian.SelectedIndex].ID);
                command.AddParameter("@idAnimal", SqlDbType.Int, command.MainTable.Rows[0][0].ToString());
                command.SendCommand($"Insert Into Record VALUES(@DateReception,@TimeReception,@Claim,@idClient,@idEmployee,@idAnimal, 1)");
            }

            UpdateDateClient();

            MessageBox.Show($"{tbNickname.Text} успешно записан на прием! Дата записи {dpDate.Text} {tpTime.Text}","Информация", MessageBoxButton.OK,MessageBoxImage.Information);
            EventSystem.InvokeEventBlackBackground();
            EventSystem.InvokeUpdateRecords();
            this.Close();
        }


        /// <summary>
        /// Отправляет запись на прием нового клиента
        /// </summary>
        private void sendAppointmentNewClient()
        {
            // проверка на заполнение полей
            if (isEmptyTextBoxes())
            {
                return;
            }

            // проверка на существующий телефон
            if (isPhoneExist("-1")) // ID -1 означает, что текущего клиента пока что не существует
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

            // создаем нового клиента
            Command command = new Command();
            command.AddParameter("@Surname", SqlDbType.NVarChar, tbSurname.Text);
            command.AddParameter("@Name", SqlDbType.NVarChar, tbName.Text);
            command.AddParameter("@Patronymic", SqlDbType.NVarChar, tbPatronymic.Text);
            command.AddParameter("@Phone", SqlDbType.NVarChar, tbPhone.Text);
            command.SendCommand("Insert Into Client VALUES(@Surname,@Name,@Patronymic,@Phone)");

            // добавляем питомца новому клиенту
            command.LoadData($"Select MAX(ID_Animal) + 1 From Animals");
            string idAnimal = command.MainTable.Rows[0][0].ToString();
            saveImage(idAnimal);
            command.LoadData($"Select ID_Client From Client Where Phone = '{tbPhone.Text}'");
            string idNewClient = command.MainTable.Rows[0][0].ToString();

            command.AddParameter("@Nickname", SqlDbType.NVarChar, tbNickname.Text);
            command.AddParameter("@Gender", SqlDbType.NVarChar, cbGender.Text);
            command.AddParameter("@PathPhoto", SqlDbType.NVarChar, namePhoto);
            command.AddParameter("@idClient", SqlDbType.Int, idNewClient);
            command.AddParameter("@idTypeAnimal", SqlDbType.Int, (cbTypeAnimal.SelectedIndex + 1).ToString());
            command.AddParameter("@idBreed", SqlDbType.Int, breeds[cbBreed.SelectedIndex].IDBreed);
            command.SendCommand("Insert Into Animals VALUES(@Nickname,@Gender,@PathPhoto,@idClient,@idTypeAnimal,@idBreed)");

            // записываем на прием
            command.LoadData($"Select ID_Animal From Animals Where id_client = {idNewClient} AND Nickname = '{tbNickname.Text}' AND id_breed = {breeds[cbBreed.SelectedIndex].IDBreed}");
            command.AddParameter("@DateReception", SqlDbType.Date, dpDate.Text);
            command.AddParameter("@TimeReception", SqlDbType.Time, tpTime.Text);
            command.AddParameter("@Claim", SqlDbType.NVarChar, tbClaim.Text);
            command.AddParameter("@idClient", SqlDbType.Int, idNewClient);
            command.AddParameter("@idEmployee", SqlDbType.Int, veterinars[cbVeterinarian.SelectedIndex].ID);
            command.AddParameter("@idAnimal", SqlDbType.Int, command.MainTable.Rows[0][0].ToString());
            command.SendCommand($"Insert Into Record VALUES(@DateReception,@TimeReception,@Claim,@idClient,@idEmployee,@idAnimal,1)");

            MessageBox.Show($"{tbNickname.Text} успешно записан на прием! Дата записи {dpDate.Text} {tpTime.Text}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            EventSystem.InvokeEventBlackBackground();
            EventSystem.InvokeUpdateRecords();
            this.Close();
        }

        private void UpdateDateClient()
        {
            // если данные клиента были изменены, то обновляем его данные
            if(tbSurname.Text != client.Surname || tbName.Text != client.Name || tbPatronymic.Text != client.Patronymic || tbPhone.Text != client.Phone)
            {
                Command command = new Command();
                command.SendCommand($"Update Client Set Surname = '{tbSurname.Text}', Name = '{tbName.Text}',Patronymic = '{tbPatronymic.Text}'," +
                    $"Phone = '{tbPhone.Text}' Where ID_Client = {client.IDClient}");
                MessageBox.Show($"Данные клиента успешно обновлены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if(command.MainTable.Rows.Count > 0 )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void saveImage(string _idAmimal)
        {
            try
            {
                if (!isChangePhoto) return; // если фотку менять не нужно

                string copyToPath = Environment.CurrentDirectory + Constants.MAIN_PATH_FOLDER_ANIMALS + $"{_idAmimal}{formatPhoto}";
                File.Copy(pathPhoto, copyToPath, true);

                Command command = new Command();
                command.SendCommand($"Update Animals Set PathPhoto = '{_idAmimal}{formatPhoto}' WHERE ID_Animal = {_idAmimal}");

                namePhoto = _idAmimal + formatPhoto;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Возвращает true, если запись на это время уже есть
        /// </summary>
        /// <returns></returns>
        private bool isCheckRecordTime()
        {
            Command command = new Command();
            command.LoadData($"Select * From Record Where DateReception = '{dpDate.Text}' AND '{tpTime.Text}' BETWEEN TimeReception AND DATEADD(MINUTE,40,TimeReception)  AND id_employee = {veterinars[cbVeterinarian.SelectedIndex].ID}");

            return command.MainTable.Rows.Count > 0;
        }

        /// <summary>
        /// Возвращает true, если есть пустые поля в главных текстовых полях
        /// </summary>
        /// <returns></returns>
        private bool isEmptyTextBoxes()
        {
            if (tbSurname.Text == "" || tbName.Text == "" || tbPatronymic.Text == "" || tbPhone.Text == "" || tbClaim.Text == "" || cbGender.Text == "" || tbNickname.Text == "" || cbTypeAnimal.Text == "" || cbBreed.Text == "" || cbVeterinarian.Text == "" || tpTime.Text == "" || dpDate.Text == "")
            {
                MessageBox.Show("Заполните все поля!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }
    }
}
