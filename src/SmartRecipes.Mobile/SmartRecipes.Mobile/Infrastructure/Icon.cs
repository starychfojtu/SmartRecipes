namespace SmartRecipes.Mobile.Infrastructure
{
    public struct Icon
    {
        private Icon(string name)
        {
            ImageName = name;
        }

        public string ImageName { get; }

        // TODO: User real icons
        public static Icon Plus() => new Icon("add");
        public static Icon Minus() => new Icon("remove");
        public static Icon CartRemove() => new Icon("remove_cart");
        public static Icon CartAdd() => new Icon("add_cart");
        public static Icon Edit() => new Icon("edit");
        public static Icon Done() => new Icon("done");
        public static Icon Delete() => new Icon("delete");
    }
}