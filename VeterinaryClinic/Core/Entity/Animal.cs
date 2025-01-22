using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VeterinaryClinic.Core.Helper;
using VeterinaryClinic.Core.Interface;

namespace VeterinaryClinic.Core.Entity
{
    public class Animal : IAnimals
    {
        public string IDAnimal { get; set; }
        public string Nickname { get; set; }
        public string Gender { get; set; }
        public string PathPhoto { get; set; }
        public string TypeAnimal { get; set; }
        public string Breed { get; set; }
        public string IDTypeAnimal { get; set; }
        public string IDBreed { get; set; }


        public BitmapImage GetFullPathPhoto
        {
            get
            {
                if (PathPhoto != "" && File.Exists(PhotoDB.GetPathAnimal(PathPhoto)))
                {
                    var bytes = File.ReadAllBytes(PhotoDB.GetPathAnimal(PathPhoto));
                    return PhotoDB.ToImage(bytes);
                }
                else
                {
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.CacheOption = BitmapCacheOption.OnLoad;
                    logo.UriSource = new Uri("pack://application:,,,/Images/emptyAnimal.png");
                    logo.EndInit();
                    return logo;
                }

            }
        }



        public Animal(string iDAnimal, string nickname, string gender, string pathPhoto, string typeAnimal, string breed, string idTypeAnimal,string idBreed)
        {
            IDAnimal = iDAnimal;
            Nickname = nickname;
            Gender = gender;
            PathPhoto = pathPhoto;
            TypeAnimal = typeAnimal;
            Breed = breed;
            IDTypeAnimal = idTypeAnimal;
            IDBreed = idBreed;
        }
    }
}
