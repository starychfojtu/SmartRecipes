using Xamarin.Forms;
using System.Linq;
using System;

namespace SmartRecipes.Mobile
{
    public partial class NavigationDrawer : ContentPage
    {
        public NavigationDrawer(Page[] pages, Action<int> changePage)
        {
            InitializeComponent();

            Pages.ItemsSource = pages.Select((page, index) => new NavigationLink(page, index));
            Pages.ItemTapped += (s, e) => changePage((e.Item as NavigationLink).Index);
        }

        private class NavigationLink
        {
            public NavigationLink(Page page, int index)
            {
                Page = page;
                Index = index;
            }

            public Page Page { get; }

            public int Index { get; }

            public override string ToString()
            {
                return Page.Title;
            }
        }
    }
}
