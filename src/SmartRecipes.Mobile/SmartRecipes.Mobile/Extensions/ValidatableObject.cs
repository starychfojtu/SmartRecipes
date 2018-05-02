using System.Collections.Generic;
using System;
using System.Linq;

namespace SmartRecipes.Mobile
{
    public class ValidatableObject<T>
    {
        private T data;

        private readonly Action<T> onDataChanged;

        private readonly Func<T, IEnumerable<string>> validate;

        public ValidatableObject(T data, Func<T, IEnumerable<string>> validate, Action<T> onDataChanged)
        {
            this.data = data;
            this.validate = validate;
            this.onDataChanged = onDataChanged;
            Errors = validate(default(T));
        }

        public T Data
        {
            get { return data; }
            set
            {
                data = value;
                Errors = validate(data);
                onDataChanged?.Invoke(data);
            }
        }

        public bool IsValid
        {
            get { return Errors.Count() == 0; }
        }

        public IEnumerable<string> Errors { get; private set; }
    }
}
