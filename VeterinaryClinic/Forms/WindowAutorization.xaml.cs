using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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
using VeterinaryClinic.Core.Datebase;
using VeterinaryClinic.Core.Entity;
using VeterinaryClinic.Core.Helper;

namespace VeterinaryClinic.Forms
{
    /// <summary>
    /// Логика взаимодействия для WindowAutorization.xaml
    /// </summary>
    public partial class WindowAutorization : Window
    {
        public WindowAutorization()
        {
            InitializeComponent();
            setTheme();
        }

        private void setTheme()
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetPrimaryColor(Color.FromRgb(223,77,96));

            paletteHelper.SetTheme(theme);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private bool IsMaximized = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height = 720;
                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    IsMaximized = true;
                }
            }
        }


        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnEntry_Click(object sender, RoutedEventArgs e)
        {
            if (isUserCorrect(tbLogin.Text, tbPassword.Password))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Hide();
            }
        }


        /// <summary>
        /// Метод, который возвращает true, если найден пользователь и ему разрешен доступ к 
        /// информационной системе
        /// </summary>
        /// <param name="_login"></param>
        /// <param name="_password"></param>
        /// <returns></returns>
        private bool isUserCorrect(string _login,string _password)
        {
            Command command = new Command();
            command.LoadData($"Select Employee.ID_Employee,Employee.Surname,Employee.Name, Employee.Patronymic,Posts.TitlePost, Employee.PathPhoto From Users Inner Join Employee On Employee.ID_Employee = Users.ID_User Inner Join Posts On Posts.ID_Post = Employee.id_post Where Users.[Login] = '{_login}' AND [Password] = '{_password}' COLLATE SQL_Latin1_General_CP1_CS_AS");

            if(command.MainTable.Rows.Count > 0 )
            {
                Employee employee = new Employee(command.MainTable.Rows[0][0].ToString(), command.MainTable.Rows[0][1].ToString(), command.MainTable.Rows[0][2].ToString(), command.MainTable.Rows[0][3].ToString(), command.MainTable.Rows[0][4].ToString(),"", command.MainTable.Rows[0][5].ToString(), "", "", false, "", "");
                return isCheckRole(employee);
            }
            else
            {
                MessageBox.Show("Вы ввели не верно логин либо пароль!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        /// <summary>
        /// Метод, который проверяет доступ к информационной системе для определённого пользователя
        /// </summary>
        /// <param name="_employee">сотрудник из БД</param>
        /// <returns>Возвращает true, если пользователю разрешено пользоваться информационной системой</returns>
        private bool isCheckRole(Employee _employee)
        {
            switch (_employee.Post)
            {
                case "Администратор":
                    GlobalVar.CurrentUser = _employee;
                    return true;

                case "Ветеринар":
                    GlobalVar.CurrentUser = _employee;
                    return true;
            }
            MessageBox.Show("Доступ запрещен!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }
}
