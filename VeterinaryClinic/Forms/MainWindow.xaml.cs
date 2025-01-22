using MaterialDesignThemes.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VeterinaryClinic.Core;
using VeterinaryClinic.Core.Helper;
using VeterinaryClinic.Forms;

namespace VeterinaryClinic
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            loadUser();

            EventSystem.EventBlackBackground += EventSystem_EventBlackBackground;
            fContainer.Navigate(new System.Uri("Pages/PageAppointment.xaml", UriKind.RelativeOrAbsolute));
        }

        private void loadUser()
        {
            if(GlobalVar.CurrentUser != null)
            {
                lPost.Text = GlobalVar.CurrentUser.Post;
                lName.Text = $"{GlobalVar.CurrentUser.Name} {GlobalVar.CurrentUser.Patronymic}";
                string pathPhoto = PhotoDB.GetPathPerson(GlobalVar.CurrentUser.PathPhoto);
                if (File.Exists(pathPhoto))
                {
                    userAvatar.ImageSource = new BitmapImage(new Uri(PhotoDB.GetPathPerson(GlobalVar.CurrentUser.PathPhoto)));
                }
                else
                {
                    Uri imageUri = new Uri("pack://application:,,,/Images/emptyUser.png");
                    BitmapImage image = new BitmapImage(imageUri);
                    userAvatar.ImageSource = image;
                }
                if(GlobalVar.CurrentUser.Post == "Администратор")
                {
                    btnEmployee.Visibility = Visibility.Visible;
                }
            }
        }
        private void EventSystem_EventBlackBackground(object sender, EventArgs e)
        {
            if(blackBackground.Visibility == Visibility.Visible)
            {
                blackBackground.Visibility = Visibility.Collapsed;
            }
            else
            {
                blackBackground.Visibility = Visibility.Visible;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private bool IsMaximized = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                if(IsMaximized)
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/PageAppointment.xaml", UriKind.RelativeOrAbsolute));
        }
        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/PageHistory.xaml", UriKind.RelativeOrAbsolute));
        }
        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("Pages/PageEmployee.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            WindowAutorization windowAutorization = new WindowAutorization();
            windowAutorization.Show();
            this.Close();
        }
    }
}
