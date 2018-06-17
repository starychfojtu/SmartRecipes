using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile.ViewModels
{
    public sealed class Icon
    {
        private Icon(string name)
        {
            ImageName = name;
        }
        
        public string ImageName { get; }
        
        public static Icon Plus() => new Icon("add");
        public static Icon Minus() => new Icon("remove");
    }
    
    public sealed class UserAction<T>
    {
        
        
        public UserAction(Func<T, Task> action, Icon icon, int order)
        {
            Action = action;
            Icon = icon;
            Order = order;
        }

        public Func<T, Task> Action { get; }
        
        public Icon Icon { get; }
        
        public int Order { get; }
    }
}