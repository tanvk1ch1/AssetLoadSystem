using System;

namespace ProjectS
{
    public class ValueObserver<T>
    {
        public event Action<T> OnChange;
        private T _value;
        
        public T Value
        {
            get
            {
                return _value;
            }
            set
            { 
                this._value = value;
                OnChange?.Invoke(value);
            }
        }
        public ValueObserver(T value)
        {
            this._value = value;
        }
    }
}

