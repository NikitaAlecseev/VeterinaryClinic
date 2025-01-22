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

namespace VeterinaryClinic.Forms.Editing
{
    /// <summary>
    /// Логика взаимодействия для WindowEditEmployee.xaml
    /// </summary>
    public partial class WindowEditEmployee : Window
    {
        private Employee employee;

        private string formatPhoto = "";
        private string pathPhoto = "";
        private bool isChangePhoto = false;
        public WindowEditEmployee(Employee _employee)
        {
            InitializeComponent();
            employee = _employee;

            loadPosts();
            LoadData(employee);
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void loadPosts()
        {
            Command command = new Command();
            command.LoadData("Select TitlePost From Posts");
            for(int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                cbPost.Items.Add(command.MainTable.Rows[i][0].ToString());
            }
        }

        private void LoadData(Employee _employee)
        {
            tbSurname.Text = _employee.Surname;
            tbName.Text = _employee.Name;
            tbPatronymic.Text = _employee.Patronymic;
            tbPhone.Text = _employee.Phone;
            tbAddress.Text = _employee.Address;
            cbPost.Text = _employee.Post;
            tbNumberOffice.Text = _employee.WorkOffice;
            tbLogin.Text = _employee.Login;
            tbPassword.Password = _employee.Password;
            
            if(_employee.PathPhoto != "" && File.Exists(PhotoDB.GetPathPerson(_employee.PathPhoto)))
            {
                pathPhoto = PhotoDB.GetPathPerson(_employee.PathPhoto);
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            EventSystem.InvokeEventBlackBackground();
            this.Close();
        }

        private void saveImage(string _idUser)
        {
            try
            {
                if (!isChangePhoto) return; // если фотку менять не нужно

                employee.PathPhoto = _idUser + formatPhoto;
                string copyToPath = Environment.CurrentDirectory + Constants.MAIN_PATH_FOLDER_PERSON + $"{_idUser}{formatPhoto}";
                File.Copy(pathPhoto, copyToPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error #100", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ifEmptyTextBoxes())
                {
                    return;
                }

                // проверка на доступность логина
                if (isLoginContains())
                {
                    MessageBox.Show("Такой логин уже существует!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // сохраняем фотку (по необходимости)
                saveImage(employee.ID);

                // меняем учетную запись
                Command command = new Command();
                command.SendCommand($"Update Users Set Login = '{tbLogin.Text}', Password = '{tbPassword.Password}' WHERE Login = '{employee.Login}'");
                command.LoadData($"Select ID_User From Users Where Login = '{tbLogin.Text}'");

                // меняем данные сотрудника
                string idUser = command.MainTable.Rows[0][0].ToString();
                command.SendCommand($"Update Employee Set Surname = '{tbSurname.Text}', Name = '{tbName.Text}', Patronymic = '{tbPatronymic.Text}', Phone = '{tbPhone.Text}', Address = '{tbAddress.Text}', WorkOffice = '{tbNumberOffice.Text}', PathPhoto = '{employee.PathPhoto}', id_post = {cbPost.SelectedIndex + 1}, id_user = {idUser} Where ID_Employee = {employee.ID}");
                

                MessageBox.Show("Сотрудник успешно изменен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                EventSystem.InvokeEventBlackBackground();
                EventSystem.InvokeUpdateEmployee();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool isLoginContains()
        {
            Command command = new Command();
            command.LoadData($"Select * From Users Where Login = '{tbLogin.Text}' AND ID_User <> {employee.ID}");
            if (command.MainTable.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ifEmptyTextBoxes()
        {
            if (tbName.Text == "" || tbSurname.Text == "" || tbPatronymic.Text == "" || tbPhone.Text == "" || tbNumberOffice.Text == "" || tbLogin.Text == "" || tbPassword.Password == "" || cbPost.Text == "" || tbAddress.Text == "")
            {
                MessageBox.Show("Заполните все поля!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }
    }
}
