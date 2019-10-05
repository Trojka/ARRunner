using System;
using CoreGraphics;

namespace aRCCar.Xamarin.Game
{
    public class SingleFingerTouch
    {
        public SingleFingerTouch(int id, CGPoint coord, GestureState state)
        {
            Id = id;
            Coord = coord;
            State = state;
        }

        public int Id { get; private set; }
        public CGPoint Coord { get; private set; }
        public GestureState State { get; private set; }
    }
}
