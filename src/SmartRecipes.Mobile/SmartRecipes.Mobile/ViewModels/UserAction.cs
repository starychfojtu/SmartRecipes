using System;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile.ViewModels
{
    public sealed class Icon
    {
        private Icon(string name)
        {
            ImageName = $"{name}.png";
        }
        
        public string ImageName { get; }
        
        public static Icon Plus() => new Icon("add");
        public static Icon Minus() => new Icon("remove");
        public static Icon Delete() => new Icon("delete");
        public static Icon Edit() => new Icon("edit");
        public static Icon Done() => new Icon("done");
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