using System;
using System.IO;
using SmartRecipes.Mobile.Infrastructure;
using Xamarin.Forms;

[assembly: Dependency(typeof(SmartRecipes.Mobile.iOS.Services.FileHelper))]
namespace SmartRecipes.Mobile.iOS.Services
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
        }
    }
}