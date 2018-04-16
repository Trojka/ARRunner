using System;
namespace aRCCar.Xamarin.Game
{
    public class IllegalTouch
    {
        public IllegalTouch(GestureState state)
        {
            State = state;
        }

        public GestureState State { get; private set; }
    }
}
