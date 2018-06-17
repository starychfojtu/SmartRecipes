using System;
using System.Threading.Tasks;
using LanguageExt;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Infrastructure
{
    public class UserMessage
    {
        private UserMessage(string title, string text)
        {
            Title = title;
            Text = text;
        }

        public string Title { get; }
        
        public string Text { get; }

        public static async Task PopupAction(Func<Task<Option<UserMessage>>> action)
        {
            var result = await action();
            await result
                .MapAsync(r => Application.Current.MainPage.DisplayAlert(r.Title, r.Text, "Ok"))
                .IfNone(Task.CompletedTask);
        }
        
        public static UserMessage Error(Exception e)
        {
            return new UserMessage("Error", e.Message);
        }

        public static UserMessage Deleted()
        {
            return new UserMessage("Success", "Successfully deleted.");
        }

        public static UserMessage Added()
        {
            return new UserMessage("Success", "Successfully added.");
        }
    }
}