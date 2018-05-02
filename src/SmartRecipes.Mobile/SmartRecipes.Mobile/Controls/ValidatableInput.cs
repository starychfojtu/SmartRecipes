using Xamarin.Forms;
using SmartRecipes.Mobile.Controls;

namespace SmartRecipes.Mobile
{
    // TODO: Not used so far...
    public class ValidatableInput<T> : ContentView
    {
        public ValidatableInput()
        {
            DataEntry = new ValidatableEntry();
            ErrorLabel = new Label
            {
                TextColor = Color.Red
            };

            var layout = new StackLayout();
            var context = ValidatableBindingContext;

            //context.Bind(DataEntry, Entry.TextProperty, c => c.Data);
            //context.Bind(DataEntry, ValidatableEntry.ErrorsProperty, c => c.Errors);
            //context.Bind(ErrorLabel, Label.TextProperty, c => c.Errors.FirstOrDefault());

            layout.Children.Add(DataEntry);
            layout.Children.Add(ErrorLabel);

            Content = layout;
        }

        public ValidatableEntry DataEntry { get; }

        public Label ErrorLabel { get; }

        private ValidatableObject<T> ValidatableBindingContext
        {
            get { return BindingContext as ValidatableObject<T>; }
        }
    }
}

