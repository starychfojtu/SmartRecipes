using System;
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
        
        // TODO: User real icons
        public static Icon Plus() => new Icon("add");
        public static Icon Minus() => new Icon("remove");
        public static Icon Delete() => new Icon("remove");
        public static Icon Edit() => new Icon("add");
        public static Icon Done() => new Icon("add");
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