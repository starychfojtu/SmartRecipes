using Xamarin.Forms;

namespace SmartRecipes.Mobile.Controls
{
    public class ValidatableEntry : Entry
    {
        public static readonly BindableProperty IsValidProperty = BindableProperty.Create(
            nameof(IsValid),
            typeof(bool),
            typeof(ValidatableEntry),
            true,
            propertyChanged: HandleErrorsPropertyChangedDelegate
        );

        // TODO: THis binding code is fucking hell, refactor it
        private static void HandleErrorsPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
        {
            // TODO: refactor this - very horrible code
            var entry = bindable as ValidatableEntry;
            var isValid = (bool)newValue;
            if (!isValid && !entry.IsError)
            {
                entry.NonErrorTextColor = entry.TextColor;
                entry.TextColor = Color.Red;
                entry.IsError = true;
            }
            else if (!isValid && entry.IsError)
            {
                entry.TextColor = entry.NonErrorTextColor;
                entry.IsError = false;
            }
        }

        private bool IsError { get; set; }

        private Color NonErrorTextColor { get; set; }

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }
    }
}