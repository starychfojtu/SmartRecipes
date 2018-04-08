using Xamarin.Forms;

namespace SmartRecipes.Mobile.Extensions
{
    public static class PageFactory
    {
        public static TabbedPage CreateTabbed(string title, params Page[] pages)
        {
            var tabbedPage = new TabbedPage { Title = title };
            return tabbedPage.Tee(p => p.Children.AddRange(pages));
        }
    }
}
