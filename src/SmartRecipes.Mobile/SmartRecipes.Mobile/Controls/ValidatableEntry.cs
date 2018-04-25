using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;

namespace SmartRecipes.Mobile.Controls
{
    public class ValidatableEntry : Entry
    {
        public static readonly BindableProperty ErrorsProperty = BindableProperty.Create(
            nameof(Errors),
            typeof(IEnumerator<string>),
            typeof(ValidatableEntry),
            Enumerable.Empty<string>(),
            propertyChanged: HandleErrorsPropertyChangedDelegate
        );

        // TODO: THis binding code is fucking hell, investigate it
        private static void HandleErrorsPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
        {
            var entry = bindable as ValidatableEntry;
            var errors = newValue as IEnumerable<string>;
            if (errors.Any())
            {
                entry.NonErrorTextColor = entry.TextColor;
                entry.TextColor = Color.Red;
            }
            else
            {
                entry.TextColor = entry.NonErrorTextColor;
            }
        }

        private Color NonErrorTextColor { get; set; }

        public IEnumerable<string> Errors
        {
            get { return (IEnumerable<string>)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }
    }
}