using Xamarin.Forms;

namespace SmartRecipes.Mobile.Extensions
{
    public static class PageFactory
    {
        public static TabbedPage CreateTabbed(string title, params Page[] pages)
        {
            var tabbedPage = new TabbedPage { Title = title };
            foreach (var page in pages)
            {
                tabbedPage.Children.Add(page);
            }
            return tabbedPage;
        }
    }
}
