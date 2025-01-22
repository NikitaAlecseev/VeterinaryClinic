using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static MaterialDesignThemes.Wpf.Theme;

namespace VeterinaryClinic.Forms
{
    /// <summary>
    /// Логика взаимодействия для DialogGetPhone.xaml
    /// </summary>
    public partial class DialogGetPhone : Window
    {
        private string phoneNumberInput = "";
        private string inputMaskPhone = "";
        public DialogGetPhone()
        {
            InitializeComponent();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            WindowAddRecord formRecord = new WindowAddRecord(getClientFromPhone(),tbPhone.Text);
            formRecord.Show();
            this.Close();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            EventSystem.InvokeEventBlackBackground();
        }

        private void tbPhone_TextChanged(object sender, TextChangedEventArgs e)
        {

        }




        private Client getClientFromPhone()
        {
            // загружаем клиента по номеру телефона
            Command command = new Command();
            command.LoadData($"Select * From Client Where Phone = '{tbPhone.Text}'");

            if(command.MainTable.Rows.Count == 0 ) return null; // если клиента по номеру телефона не находит, то возвращаем null

            // загружаем питомцев клиента
            string idClient = command.MainTable.Rows[0][0].ToString();
            Command sqlAnimals = new Command();
            sqlAnimals.LoadData($"Select ID_Animal,Nickname,Gender, PathPhoto, TypeAnimals.TitleAnimals, Breed.TitleBreed, Animals.id_typeAnimal, Animals.id_breed From Animals Inner Join Breed On Animals.id_breed = Breed.ID_Breed Inner Join TypeAnimals On Breed.ID_TypeAnimals = TypeAnimals.ID_TypeAnimals Where id_client = {idClient}");
            List<Animal> animals = new List<Animal>();
            for(int i = 0;i<sqlAnimals.MainTable.Rows.Count;i++)
            {
                animals.Add(new Animal(sqlAnimals.MainTable.Rows[i][0].ToString(), sqlAnimals.MainTable.Rows[i][1].ToString(), sqlAnimals.MainTable.Rows[i][2].ToString(),
                    sqlAnimals.MainTable.Rows[i][3].ToString(), sqlAnimals.MainTable.Rows[i][4].ToString(), sqlAnimals.MainTable.Rows[i][5].ToString(), sqlAnimals.MainTable.Rows[i]["id_typeAnimal"].ToString(),
                    sqlAnimals.MainTable.Rows[i]["id_breed"].ToString()));
            }


            Client client = new Client(command.MainTable.Rows[0][0].ToString(), command.MainTable.Rows[0][1].ToString(), command.MainTable.Rows[0][2].ToString(),
                command.MainTable.Rows[0][3].ToString(), command.MainTable.Rows[0][4].ToString(),animals);

            return client;
        }
    }
}
