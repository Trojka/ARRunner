using System;
using CoreGraphics;

namespace ARRunner.Xamarin.Game
{
    public class TwoFingerTouch
    {
        public TwoFingerTouch(int id, CGPoint coord1, CGPoint coord2, GestureState state)
        {
            Id = id;
            Coord1 = coord1;
            Coord2 = coord2;
            State = state;
        }

        public int Id { get; private set; }
        public CGPoint Coord1 { get; private set; }
        public CGPoint Coord2 { get; private set; }
        public GestureState State { get; private set; }
   }
}
