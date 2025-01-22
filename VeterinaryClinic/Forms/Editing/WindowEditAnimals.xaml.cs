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
    /// Логика взаимодействия для WindowEditAnimals.xaml
    /// </summary>
    public partial class WindowEditAnimals : Window
    {
        private string formatPhoto = "";
        private string pathPhoto = "";
        private bool isChangePhoto = false;

        private string idClient;
        private List<Animal> animals = new List<Animal>();

        public WindowEditAnimals(string _idClient)
        {
            InitializeComponent();

            idClient = _idClient;

            listBox.SelectionChanged += ListBox_Selected;
            loadCbTypeAnimals();
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

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            Command command = new Command();

            command.AddParameter("@Nickname", System.Data.SqlDbType.NVarChar, tbNickname.Text);
            command.AddParameter("@Gender", System.Data.SqlDbType.NVarChar, cbGender.Text);
            command.AddParameter("@PathPhoto", System.Data.SqlDbType.NVarChar, pathPhoto + formatPhoto);
            command.AddParameter("@idClient", System.Data.SqlDbType.NVarChar, idClient);
            command.AddParameter("@idTypeAnimal", System.Data.SqlDbType.NVarChar, (cbTypeAnimal.SelectedIndex + 1).ToString());
            command.AddParameter("@Breed", System.Data.SqlDbType.NVarChar, cbBreed.Text);

            command.SendCommand("Insert Into Animals VALUES(@Nickname,@Gender,@PathPhoto,@idClient,@idTypeAnimal,(Select ID_Breed From Breed Where TitleBreed = @Breed))");
            loadData();
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            Command command = new Command();
            saveImage();
            command.SendCommand($"Update Animals Set Nickname = '{tbNickname.Text}',Gender = '{cbGender.Text}'," +
                $"id_typeAnimal = {(cbTypeAnimal.SelectedIndex + 1)},id_breed = (Select ID_Breed From Breed Where TitleBreed = '{cbBreed.Text}') Where ID_Animal = {animals[listBox.SelectedIndex].IDAnimal}");
            MessageBox.Show($"Данные изменены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            loadData();
            EventSystem.InvokeUpdateRecords();
        }
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"Вы уверены, что хотите удалить {animals[listBox.SelectedIndex].Nickname}?","Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Command command = new Command();
                command.SendCommand($"Delete Record Where id_animal = {animals[listBox.SelectedIndex].IDAnimal}");
                command.SendCommand($"Delete Animals Where ID_Animal = {animals[listBox.SelectedIndex].IDAnimal}");
                MessageBox.Show($"{animals[listBox.SelectedIndex].Nickname} удален!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                loadData();
                EventSystem.InvokeUpdateRecords();
            }
        }

        private void loadData()
        {
            formatPhoto = "";
            pathPhoto = "";
            animals.Clear();
            listBox.Items.Clear();

            Command command = new Command();
            command.LoadData($"Select ID_Animal,Nickname,Gender,PathPhoto,TitleAnimals, TitleBreed, id_typeAnimal,Animals.id_breed From Animals Inner Join Breed On Breed.ID_Breed = Animals.id_breed Inner Join TypeAnimals On TypeAnimals.ID_TypeAnimals = Animals.id_typeAnimal Where Animals.id_client = {idClient}");
            for(int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                Animal animal = new Animal(command.MainTable.Rows[i][0].ToString(), command.MainTable.Rows[i][1].ToString(),
                    command.MainTable.Rows[i][2].ToString(), command.MainTable.Rows[i][3].ToString(), command.MainTable.Rows[i][4].ToString(),
                    command.MainTable.Rows[i][5].ToString(), command.MainTable.Rows[i][6].ToString(), command.MainTable.Rows[i][7].ToString());

                listBox.Items.Add($"{animal.Nickname}");
                animals.Add(animal);
            }
        }

        private void ListBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex == -1) return;
            tbNickname.Text = animals[listBox.SelectedIndex].Nickname;
            cbTypeAnimal.Text = animals[listBox.SelectedIndex].TypeAnimal;
            cbBreed.Text = animals[listBox.SelectedIndex].Breed;
            cbGender.Text = animals[listBox.SelectedIndex].Gender;

            string namePhoto = animals[listBox.SelectedIndex].PathPhoto;
            if (namePhoto != "" && File.Exists(PhotoDB.GetPathAnimal(namePhoto)))
            {
                pathPhoto = PhotoDB.GetPathAnimal(namePhoto);
                formatPhoto = System.IO.Path.GetExtension(pathPhoto);

                lAddPhoto.Visibility = Visibility.Collapsed;

                var bytes = File.ReadAllBytes(pathPhoto);
                photo.ImageSource = PhotoDB.ToImage(bytes);

                ellipsePhoto.Visibility = Visibility.Visible;
            }
            else
            {
                pathPhoto = "";
                formatPhoto = "";

                lAddPhoto.Visibility = Visibility.Visible;
                photo.ImageSource = null;
                ellipsePhoto.Visibility = Visibility.Collapsed;
            }
        }

        private void loadCbTypeAnimals()
        {
            Command command = new Command();
            command.LoadData("Select TitleAnimals From TypeAnimals");
            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                cbTypeAnimal.Items.Add(command.MainTable.Rows[i][0].ToString());
            }
        }
        private void loadCbBreed()
        {
            cbBreed.Items.Clear();
            Command command = new Command();
            command.LoadData($"Select TitleBreed From Breed WHERE id_typeAnimals = {cbTypeAnimal.SelectedIndex + 1}");
            for (int i = 0; i < command.MainTable.Rows.Count; i++)
            {
                cbBreed.Items.Add(command.MainTable.Rows[i][0].ToString());
            }
        }

        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                lAddPhoto.Visibility = Visibility.Collapsed;

                pathPhoto = files[0];
                var bytes = File.ReadAllBytes(files[0]);
                photo.ImageSource = PhotoDB.ToImage(bytes);

                ellipsePhoto.Visibility = Visibility.Visible;

                formatPhoto = System.IO.Path.GetExtension(pathPhoto);
                isChangePhoto = true;
            }
        }

        private void cbTypeAnimal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadCbBreed();
        }

        private void saveImage()
        {
            try
            {
                if (!isChangePhoto) return; // если фотку менять не нужно

                string copyToPath = Environment.CurrentDirectory + Constants.MAIN_PATH_FOLDER_ANIMALS + $"{animals[listBox.SelectedIndex].IDAnimal}{formatPhoto}";
                File.Copy(pathPhoto, copyToPath, true);

                Command command = new Command();
                command.SendCommand($"Update Animals Set PathPhoto = '{animals[listBox.SelectedIndex].IDAnimal}{formatPhoto}' WHERE ID_Animal = {animals[listBox.SelectedIndex].IDAnimal}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
