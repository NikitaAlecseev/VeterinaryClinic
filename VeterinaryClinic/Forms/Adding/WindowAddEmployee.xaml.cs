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

namespace VeterinaryClinic.Forms
{
    /// <summary>
    /// Логика взаимодействия для WindowAddEmployee.xaml
    /// </summary>
    public partial class WindowAddEmployee : Window
    {
        private string formatPhoto = "";
        private string pathPhoto = "";
        public WindowAddEmployee()
        {
            InitializeComponent();
            loadPosts();
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
            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                cbPost.Items.Add(command.MainTable.Rows[i][0].ToString());
            }
        }
        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                lAddPhoto.Visibility = Visibility.Collapsed;
                photo.ImageSource = new BitmapImage(new Uri(files[0], UriKind.Relative));
                ellipsePhoto.Visibility = Visibility.Visible;
                pathPhoto = files[0];
                formatPhoto = System.IO.Path.GetExtension(pathPhoto);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            EventSystem.InvokeEventBlackBackground();
            this.Close();
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isEmptyTextBoxes())
                {
                    return;
                }

                // проверка на доступность логина
                if (isLoginContains())
                {
                    MessageBox.Show("Такой логин уже существует!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // добавляем аккаунт в базу
                Command command = new Command();
                command.AddParameter("@Login", System.Data.SqlDbType.NVarChar, tbLogin.Text);
                command.AddParameter("@Password", System.Data.SqlDbType.NVarChar, tbPassword.Password);
                command.SendCommand("Insert Into Users VALUES (@Login,@Password)");
                command.LoadData($"Select ID_User From Users Where Login = '{tbLogin.Text}'");

                // добавляем сотрудника в базу
                string idUser = command.MainTable.Rows[0][0].ToString();
                command.AddParameter("@Surname", System.Data.SqlDbType.NVarChar, tbSurname.Text);
                command.AddParameter("@Name", System.Data.SqlDbType.NVarChar, tbName.Text);
                command.AddParameter("@Patronymic", System.Data.SqlDbType.NVarChar, tbPatronymic.Text);
                command.AddParameter("@Phone", System.Data.SqlDbType.NVarChar, tbPhone.Text);
                command.AddParameter("@Address", System.Data.SqlDbType.NVarChar, tbAddress.Text);
                command.AddParameter("@WorkOffice", System.Data.SqlDbType.NVarChar, tbNumberOffice.Text);
                command.AddParameter("@PathPhoto", System.Data.SqlDbType.NVarChar, $"{idUser}{formatPhoto}");
                command.AddParameter("@IDPost", System.Data.SqlDbType.Int, (cbPost.SelectedIndex + 1).ToString());
                command.AddParameter("@IDUser", System.Data.SqlDbType.NVarChar, idUser);

                command.SendCommand("Insert Into Employee VALUES(@Surname,@Name,@Patronymic,@Phone,@Address,@WorkOffice,@PathPhoto,@IDPost,@IDUser)");
                if(pathPhoto != "") saveImage(idUser); // копируем фотку в папку с приложением

                MessageBox.Show("Сотрудник успешно добавлен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                EventSystem.InvokeEventBlackBackground();
                EventSystem.InvokeUpdateEmployee();
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveImage(string _idUser)
        {
            try
            {
                string copyToPath = Environment.CurrentDirectory + Constants.MAIN_PATH_FOLDER_PERSON + $"{_idUser}{formatPhoto}";
                File.Copy(pathPhoto, copyToPath, true);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool isLoginContains()
        {
            Command command = new Command();
            command.LoadData($"Select * From Users Where Login = '{tbLogin.Text}'");
            if(command.MainTable.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isEmptyTextBoxes()
        {
            if(tbName.Text == "" || tbSurname.Text == "" || tbPatronymic.Text == "" || tbPhone.Text == "" || tbNumberOffice.Text == "" || tbLogin.Text == "" || tbPassword.Password == "" || cbPost.Text == "" || tbAddress.Text == "")
            {
                MessageBox.Show("Заполните все поля!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            return false;
        }
    }
}
