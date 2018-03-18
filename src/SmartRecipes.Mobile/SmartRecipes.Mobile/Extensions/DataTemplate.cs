namespace SmartRecipes.Mobile.Extensions
{
    public class DataTemplate<T> : Xamarin.Forms.DataTemplate
    {
        public DataTemplate() : base(typeof(T))
        {
        }
    }
}
