using System;

namespace SmartRecipes.Mobile.Infrastructure
{
    public sealed class ValidatableObject<T>
    {
        private T data;

        private readonly Action<T> onDataChanged;

        private readonly Predicate<T> validate;

        public ValidatableObject(T data, Predicate<T> validate, Action<T> onDataChanged)
        {
            this.data = data;
            this.validate = validate;
            this.onDataChanged = onDataChanged;
            IsValid = validate(data);
        }

        public T Data
        {
            get { return data; }
            set
            {
                data = value;
                IsValid = validate(data);
                onDataChanged?.Invoke(data);
            }
        }

        public bool IsValid { get; set; }
    }

    public static class ValidatableObject
    {
        public static ValidatableObject<T> Create<T>(Predicate<T> validate, Action<T> onDataChanged)
        {
            return new ValidatableObject<T>(default(T), validate, onDataChanged);
        }
    }
}
