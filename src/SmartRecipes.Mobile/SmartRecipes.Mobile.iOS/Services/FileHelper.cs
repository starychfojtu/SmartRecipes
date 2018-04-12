using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SmartRecipes.Mobile.iOS.Service.FileHelper))]
namespace SmartRecipes.Mobile.iOS.Service
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
        }
    }
}