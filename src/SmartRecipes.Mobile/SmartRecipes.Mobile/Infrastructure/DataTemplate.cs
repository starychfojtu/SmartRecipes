namespace SmartRecipes.Mobile.Infrastructure
{
    public class DataTemplate<T> : Xamarin.Forms.DataTemplate
    {
        public DataTemplate() : base(typeof(T))
        {
        }
    }
}
