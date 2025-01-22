using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
using VeterinaryClinic.Core.Helper;
using VeterinaryClinic.Forms;
using VeterinaryClinic.Forms.Editing;

namespace VeterinaryClinic.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageHistory.xaml
    /// </summary>
    public partial class PageHistory : UserControl
    {
        private string sqlSorting = "";
        private string sqlEmployee = ""; // если авторизован ветеринар, то тут будет условие сортировки
        public PageHistory()
        {
            InitializeComponent();
            loadList();
        }

        private void loadList()
        {
            if (GlobalVar.CurrentUser.Post != "Администратор")
            {
                sqlEmployee = $"AND Record.id_employee = {GlobalVar.CurrentUser.ID}";
            }

            if (listRecord != null)
                listRecord.ItemsSource = getRecords();
        }

        /// <summary>
        /// Метод для загрузки истории приемов
        /// </summary>
        /// <returns></returns>
        private List<Record> getRecords()
        {
            string sqlRequest = "Select ID_Record,Client.Surname as 'ClientSurname',Client.Name as 'ClientName', " +
                "Client.Patronymic as 'ClientPatronymic',Client.Phone,DateReception,TimeReception,Claim,Animals.ID_Animal,Nickname," +
                "Animals.Gender,Animals.PathPhoto,TypeAnimals.TitleAnimals,Breed.TitleBreed, Record.id_statusRecord," +
                "Employee.ID_Employee,Employee.Surname as 'EmployeeSurname',Employee.Name as 'EmployeeName'," +
                "Employee.Patronymic as 'EmployeePatronymic', Client.ID_Client, Animals.id_typeAnimal, Animals.id_breed From Record " +
                "Inner Join Client On Record.id_client = Client.ID_Client " +
                "Inner Join Employee On Employee.ID_Employee = Record.id_employee " +
                "Inner Join Animals On Animals.ID_Animal = Record.id_animal " +
                "Inner Join Breed On Animals.id_breed = Breed.ID_Breed " +
                $"Inner Join TypeAnimals On TypeAnimals.ID_TypeAnimals = Animals.id_typeAnimal WHERE id_statusRecord = 2 {sqlEmployee} " +
                $"AND (Client.Surname like '%{tbSearch.Text}%' Or Client.[Name] like '%{tbSearch.Text}%' Or Client.Patronymic like '%{tbSearch.Text}%' Or Breed.TitleBreed like '%{tbSearch.Text}%' Or Animals.Nickname like '%{tbSearch.Text}%' Or Client.Phone like '%{tbSearch.Text}%') {sqlSorting}";

            List<Record> records = new List<Record>();
            Command command = new Command();
            command.LoadData(sqlRequest);

            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                Animal animal = new Animal(command.MainTable.Rows[i]["ID_Animal"].ToString(), command.MainTable.Rows[i]["Nickname"].ToString(),
                    command.MainTable.Rows[i]["Gender"].ToString(), command.MainTable.Rows[i]["PathPhoto"].ToString(), command.MainTable.Rows[i]["TitleAnimals"].ToString(),
                    command.MainTable.Rows[i]["TitleBreed"].ToString(), command.MainTable.Rows[i]["id_typeAnimal"].ToString(),
                    command.MainTable.Rows[i]["id_breed"].ToString());

                Employee veterinar = new Employee(command.MainTable.Rows[i]["ID_Employee"].ToString(), command.MainTable.Rows[i]["EmployeeSurname"].ToString(),
                    command.MainTable.Rows[i]["EmployeeName"].ToString(), command.MainTable.Rows[i]["EmployeePatronymic"].ToString(), "", "", "", "", "", false, "", "");

                records.Add(new Record(command.MainTable.Rows[i]["ID_Record"].ToString(), command.MainTable.Rows[i]["ClientSurname"].ToString(),
                    command.MainTable.Rows[i]["ClientName"].ToString(), command.MainTable.Rows[i]["ClientPatronymic"].ToString(),
                    command.MainTable.Rows[i]["Phone"].ToString(),
                    command.MainTable.Rows[i]["DateReception"].ToString(),
                    command.MainTable.Rows[i]["TimeReception"].ToString(),
                    command.MainTable.Rows[i]["Claim"].ToString(),
                    command.MainTable.Rows[i]["id_statusRecord"].ToString(),
                    command.MainTable.Rows[i]["ID_Client"].ToString(), animal, veterinar));
            }

            return records;
        }


        private void cbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbSorting.SelectedIndex)
            {
                case 0:
                    sqlSorting = "";
                    break;
                case 1:
                    sqlSorting = "Order By Client.Surname ASC,Client.[Name] ASC,Client.Patronymic ASC";
                    break;
                case 2:
                    sqlSorting = "Order By Client.Surname DESC,Client.[Name] DESC,Client.Patronymic DESC";
                    break;
                case 3:
                    sqlSorting = "Order By Record.DateReception ASC,Record.TimeReception ASC";
                    break;
                case 4:
                    sqlSorting = "Order By Record.DateReception DESC,Record.TimeReception ASC";
                    break;
            }
            loadList();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            loadList();
        }

        private void menuItem_Click_Animals(object sender, RoutedEventArgs e)
        {
            Record record = listRecord.SelectedItem as Record;
            WindowEditAnimals windowEditAnimal = new WindowEditAnimals(record.IDClient);
            windowEditAnimal.Show();
            EventSystem.InvokeEventBlackBackground();
        }
        private void menuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            Record record = listRecord.SelectedItem as Record;

            if (MessageBox.Show($"Вы уверены, что хотите удалить {record.FullName}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Command command = new Command();
                command.SendCommand($"Delete Record Where ID_Record = {record.ID}");
                MessageBox.Show("Запись удалена!", "Инфомрация", MessageBoxButton.OK, MessageBoxImage.Information);
                loadList();
            }
        }

        private void menuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            Record record = listRecord.SelectedItem as Record;
            WindowView wView = new WindowView(record);
            wView.Show();
            EventSystem.InvokeEventBlackBackground();
        }
    }
}
