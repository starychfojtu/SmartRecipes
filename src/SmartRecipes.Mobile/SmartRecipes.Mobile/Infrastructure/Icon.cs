namespace SmartRecipes.Mobile.Infrastructure
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
}