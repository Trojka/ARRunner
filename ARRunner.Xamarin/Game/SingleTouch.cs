using System;
using CoreGraphics;

namespace ARRunner.Xamarin.Game
{
    public class SingleTouch
    {
        public SingleTouch(int id, CGPoint coord, GestureState state)
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
