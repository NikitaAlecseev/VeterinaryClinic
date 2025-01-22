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
using VeterinaryClinic.Core.Entity;
using VeterinaryClinic.Core.Helper;

namespace VeterinaryClinic.Forms
{
    /// <summary>
    /// Логика взаимодействия для WindowView.xaml
    /// </summary>
    public partial class WindowView : Window
    {
        public WindowView(Record record)
        {
            InitializeComponent();
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

        public void LoadData(Record record)
        {
            lName.Text = record.AnimalClient.Nickname;
            lBreed.Text = record.AnimalClient.Breed;
            lNickname.Text = record.AnimalClient.Nickname;
            lFullName.Text = record.FullName;
            lFullNameVeterinar.Text = record.Veterinar.FullName;
            lPhone.Text = record.Phone;
            lDateReception.Text = $"{record.DateReception} {record.TimeReception}";
            tbClaim.Text = record.Claim;

            if (record.AnimalClient.PathPhoto != "" && File.Exists(PhotoDB.GetPathAnimal(record.AnimalClient.PathPhoto)))
            {
                imageBrush.ImageSource = new BitmapImage(new Uri(PhotoDB.GetPathAnimal(record.AnimalClient.PathPhoto), UriKind.Relative));
            }
            else
            {
                imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/emptyAnimal.png"));
            }
        }
    }
}
