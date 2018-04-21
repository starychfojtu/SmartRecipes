using System;

namespace SmartRecipes.Mobile.Services
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
    }
}
