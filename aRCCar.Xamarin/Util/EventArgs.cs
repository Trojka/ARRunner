using System;
namespace aRCCar.Xamarin.Util
{
    public class EventArgs<T> : EventArgs
    {
        public T Value { get; private set; }

        public EventArgs(T val)
        {
            Value = val;
        }
    }
}
