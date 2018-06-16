using System;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile.ViewModels
{
    public class UserAction<T>
    {
        public UserAction(Func<T, Task> action, string icon, int order)
        {
            Action = action;
            Icon = icon;
            Order = order;
        }

        public Func<T, Task> Action { get; }
        
        public string Icon { get; }
        
        public int Order { get; }
    }
}