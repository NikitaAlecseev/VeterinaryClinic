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
    /// Логика взаимодействия для WindowEditVacation.xaml
    /// </summary>
    public partial class WindowEditVacation : Window
    {
        private Employee employee;
        private List<Vacation> vacations = new List<Vacation>();
        public WindowEditVacation(Employee _employee)
        {
            InitializeComponent();
            listBox.SelectionChanged += ListBox_Selected;

            employee = _employee;
            loadData();
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
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Command command = new Command();
            command.LoadData($"Update Vacation Set DateStartVacation = '{dpStart.Text}', DateEndVacation = '{dpEnd.Text}' WHERE id_employee = {employee.ID}");
            MessageBox.Show("Отпуск изменен!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            dpStart.Text = "";
            dpEnd.Text = "";
            loadData();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"Вы уверены, что хотите удалить {dpStart.Text}-{dpEnd.Text}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Command command = new Command();
                command.SendCommand($"Delete Vacation Where id_employee = {employee.ID} AND DateStartVacation = '{dpStart.Text}' AND DateEndVacation = '{dpEnd.Text}'");
                loadData();
            }
        }

        private void loadData()
        {
            listBox.Items.Clear();
            vacations.Clear();

            Command command = new Command();
            command.LoadData($"Select * From Vacation Where id_employee = {employee.ID}");
            for(int i = 0;i<command.MainTable.Rows.Count;i++)
            {
                Vacation vacation = new Vacation(command.MainTable.Rows[i][0].ToString(), command.MainTable.Rows[i][1].ToString(),
                    command.MainTable.Rows[i][2].ToString());
                listBox.Items.Add($"{vacation.DateStart.Replace(" 0:00:00", "")}-{vacation.DateEnd.Replace(" 0:00:00", "")}");
                vacations.Add(vacation);
            }
        }

        private void ListBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex == -1) return;
            dpStart.Text = vacations[listBox.SelectedIndex].DateStart.ToString();
            dpEnd.Text = vacations[listBox.SelectedIndex].DateEnd.ToString();
        }
    }
}
