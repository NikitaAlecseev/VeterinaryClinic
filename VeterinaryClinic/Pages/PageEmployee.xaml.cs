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
using VeterinaryClinic.Forms;
using VeterinaryClinic.Forms.Editing;

namespace VeterinaryClinic.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageEmployee.xaml
    /// </summary>
    public partial class PageEmployee : UserControl
    {
        private string sqlSorting = "";
        private string selectedPost = "";
        public PageEmployee()
        {
            InitializeComponent();
            EventSystem.EventUpdateEmployee += loadData;

            loadPosts();
            loadData(null,null);
        }


        private void loadData(object sender, EventArgs e)
        {
            if (listEmployee == null) return;

            List<Employee> employee = getEmployee();
            List<Employee> employeeVacation = getEmployeeVacation();
            for(int i = 0; i < employeeVacation.Count; i++)
            {
                employee.Add(employeeVacation[i]);
            }
            listEmployee.ItemsSource = employee;
        }

        private void loadPosts()
        {
            cbPost.Items.Add("Все должности");
            cbPost.SelectedIndex = 0;
            Command command = new Command();
            command.LoadData("Select TitlePost From Posts");
            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                cbPost.Items.Add(command.MainTable.Rows[i][0].ToString());
            }
        }


        /// <summary>
        /// Возвращает сотрудников, которые не состоят в отпуске
        /// </summary>
        /// <returns></returns>
        private List<Employee> getEmployee()
        {
            string requestSQL = "SELECT DISTINCT e.ID_Employee, e.Surname, e.Name, e.Patronymic, e.Phone,e.Address,e.WorkOffice," +
                "e.PathPhoto, p.TitlePost, u.Login,u.Password FROM Employee e " +
                "Inner Join Posts p ON p.ID_Post = e.id_post " +
                "Inner Join Users u ON e.id_user = u.ID_User" +
                " LEFT JOIN Vacation v ON v.id_employee = e.ID_Employee " +
                "WHERE (v.id_employee IS NULL OR NOT(GETDATE() BETWEEN v.DateStartVacation AND v.DateEndVacation))" +
                $"AND (e.Surname like '%{tbSearch.Text}%' OR e.[Name] like '%{tbSearch.Text}%' OR e.Patronymic like '%{tbSearch.Text}%' OR e.Phone like '%{tbSearch.Text}%' OR e.[Address] like '%{tbSearch.Text}%' OR e.WorkOffice like '%{tbSearch.Text}%' OR u.[Login] like '%{tbSearch.Text}%') AND p.TitlePost like '%{selectedPost}%' {sqlSorting}";

            Clipboard.SetText(requestSQL);
            List<Employee> employees = new List<Employee>();
            Command command = new Command();
            command.LoadData(requestSQL);
            for(int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                employees.Add(new Employee(command.MainTable.Rows[i][0].ToString(), command.MainTable.Rows[i][1].ToString(), command.MainTable.Rows[i][2].ToString(),
                    command.MainTable.Rows[i][3].ToString(), command.MainTable.Rows[i][8].ToString(), command.MainTable.Rows[i][4].ToString(),
                    command.MainTable.Rows[i][7].ToString(), command.MainTable.Rows[i][5].ToString(), command.MainTable.Rows[i][6].ToString(),
                    false, command.MainTable.Rows[i][9].ToString(), command.MainTable.Rows[i][10].ToString()));
            }
            return employees;
        }

        private void cbSorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbSorting.SelectedIndex)
            {
                case 0:
                    sqlSorting = "";
                    break;
                case 1:
                    sqlSorting = "ORDER BY e.Surname ASC, e.[Name] ASC, e.Patronymic ASC";
                    break;
                case 2:
                    sqlSorting = "ORDER BY e.Surname DESC, e.[Name] DESC, e.Patronymic DESC";
                    break;
            }
            loadData(null, null);
        }

        private void cbPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbPost.SelectedIndex)
            {
                case 0:
                    selectedPost = "";
                    break;
                default:
                    selectedPost = cbPost.Items[cbPost.SelectedIndex].ToString();
                    break;
            }
            
            loadData(null, null);
        }

        /// <summary>
        /// Возвращает сотрудников, которые состоят в отпуске
        /// </summary>
        /// <returns></returns>
        private List<Employee> getEmployeeVacation()
        {
            string requestSQL = "SELECT DISTINCT e.ID_Employee, e.Surname, e.Name, e.Patronymic, e.Phone,e.Address,e.WorkOffice," +
                "e.PathPhoto, p.TitlePost, u.Login,u.Password FROM Employee e " +
                "Inner Join Posts p ON p.ID_Post = e.id_post " +
                "Inner Join Users u ON e.id_user = u.ID_User" +
                " LEFT JOIN Vacation v ON v.id_employee = e.ID_Employee " +
                "WHERE (GETDATE() BETWEEN v.DateStartVacation AND v.DateEndVacation) " +
                $"AND (e.Surname like '%{tbSearch.Text}%' OR e.[Name] like '%{tbSearch.Text}%' OR e.Patronymic like '%{tbSearch.Text}%' OR e.Phone like '%{tbSearch.Text}%' OR e.[Address] like '%{tbSearch.Text}%' OR e.WorkOffice like '%{tbSearch.Text}%' OR u.[Login] like '%{tbSearch.Text}%') AND p.TitlePost like '%{selectedPost}%' {sqlSorting}";


            List<Employee> employees = new List<Employee>();
            Command command = new Command();
            command.LoadData(requestSQL);
            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                employees.Add(new Employee(command.MainTable.Rows[i][0].ToString(), command.MainTable.Rows[i][1].ToString(), command.MainTable.Rows[i][2].ToString(),
                    command.MainTable.Rows[i][3].ToString(), command.MainTable.Rows[i][8].ToString(), command.MainTable.Rows[i][4].ToString(),
                    command.MainTable.Rows[i][7].ToString(), command.MainTable.Rows[i][5].ToString(), command.MainTable.Rows[i][6].ToString(),
                    true, command.MainTable.Rows[i][9].ToString(), command.MainTable.Rows[i][10].ToString()));
            }
            return employees;
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            WindowAddEmployee windowAddEmployee = new WindowAddEmployee();
            windowAddEmployee.Show();
            EventSystem.InvokeEventBlackBackground(); 
        }

        private void menuItem_Click_SetVacation(object sender, RoutedEventArgs e)
        {
            Employee employee = listEmployee.SelectedItem as Employee;
            WindowAddVacation windowAddVacation = new WindowAddVacation(employee);
            windowAddVacation.Show();
            EventSystem.InvokeEventBlackBackground();
        }
        private void menuItem_Click_EditVacation(object sender, RoutedEventArgs e)
        {
            Employee employee = listEmployee.SelectedItem as Employee;
            WindowEditVacation windowEditVacation = new WindowEditVacation(employee);
            windowEditVacation.Show();
            EventSystem.InvokeEventBlackBackground();
        }
        private void menuItem_Click_EditUser(object sender, RoutedEventArgs e)
        {
            Employee employee = listEmployee.SelectedItem as Employee;
            WindowEditEmployee editEmployee = new WindowEditEmployee(employee);
            editEmployee.Show();
            EventSystem.InvokeEventBlackBackground();
        }
        private void menuItem_Click_DeleteUser(object sender, RoutedEventArgs e)
        {
            Employee employee = listEmployee.SelectedItem as Employee;
            
            if(MessageBox.Show($"Вы уверены, что хотите удалить {employee.FullName}?","Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Command command = new Command();
                command.SendCommand($"Delete From Record Where id_employee = {employee.ID}");
                command.SendCommand($"Delete From Vacation Where id_employee = {employee.ID}");
                command.SendCommand($"Delete From Employee Where ID_Employee = {employee.ID}");
                MessageBox.Show($"{employee.FullName} удален!", "Информация", MessageBoxButton.OK,MessageBoxImage.Information);
                EventSystem.InvokeUpdateEmployee();
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            loadData(null, null);
        }
    }
}
