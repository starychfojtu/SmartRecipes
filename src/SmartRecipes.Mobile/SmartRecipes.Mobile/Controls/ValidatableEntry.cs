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

        private static void HandleErrorsPropertyChangedDelegate(BindableObject bindable, object oldValue, object isValid)
        {
            var entry = bindable as ValidatableEntry;

            if (entry.NonErrorTextColor == default(Color))
            {
                entry.NonErrorTextColor = entry.TextColor;
            }

            entry.TextColor = (bool)isValid ? entry.NonErrorTextColor : Color.Red;
        }

        private Color NonErrorTextColor { get; set; }

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }
    }
}