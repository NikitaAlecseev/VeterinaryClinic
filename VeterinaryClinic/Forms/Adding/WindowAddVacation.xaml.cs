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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VeterinaryClinic.Core;
using VeterinaryClinic.Core.Datebase;
using VeterinaryClinic.Core.Entity;

namespace VeterinaryClinic.Forms
{
    /// <summary>
    /// Логика взаимодействия для WindowAddVacation.xaml
    /// </summary>
    public partial class WindowAddVacation : Window
    {
        private Employee employee;
        public WindowAddVacation(Employee _employee)
        {
            InitializeComponent();
            employee = _employee;
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

        private void btnVacation_Click(object sender, RoutedEventArgs e)
        {
            Command command = new Command();
            command.SendCommand($"Insert Into Vacation VALUES('{dateStart.Text}','{dateEnd.Text}',{employee.ID})");
            MessageBox.Show($"Отпуск успешно добавлен для {employee.FullName} на период {dateStart.Text}-{dateEnd.Text}","Информация", MessageBoxButton.OK,MessageBoxImage.Information);
            EventSystem.InvokeEventBlackBackground();
            this.Close();
        }
    }
}
