using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VeterinaryClinic.Core.Helper
{
    public class PhotoDB
    {
        public static string GetPathPerson(string _nameFile)
        {
            return Environment.CurrentDirectory + Constants.MAIN_PATH_FOLDER_PERSON + _nameFile;
        }

        public static string GetPathAnimal(string _nameFile)
        {
            return Environment.CurrentDirectory + Constants.MAIN_PATH_FOLDER_ANIMALS + _nameFile;
        }

        public static BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
