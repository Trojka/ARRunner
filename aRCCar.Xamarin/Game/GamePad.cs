using System;
using System.Linq;
using aRCCar.Xamarin.Util;
using Foundation;
using SpriteKit;
using UIKit;

namespace aRCCar.Xamarin.Game
{
    public class GamePad
    {
        SKScene _scene;
        nuint _currentNumberOfTouches = 0;

        public event EventHandler<EventArgs<SingleFingerTouch>> SingleFingerTouchEvent;
        public event EventHandler<EventArgs<IllegalTouch>> IllegalTouchEvent;

        public GamePad(SKScene scene)
        {
            _scene = scene;
        }

        public void TouchesBegan(NSSet touches, UIEvent evt)
        {
            if(touches.Count == 1 && _currentNumberOfTouches == 0)
            {
                var touch = (UITouch)touches.ElementAt(0);
                var viewTouchPoint = touch.LocationInNode(_scene);

                if (SingleFingerTouchEvent != null)
                {
                    var touchData = new SingleFingerTouch(0, viewTouchPoint, GestureState.Start);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }
            }
            else if(touches.Count >= 1 && _currentNumberOfTouches == 1)
            {
                var touch = (UITouch)touches.ElementAt(0);
                var viewTouchPoint = touch.LocationInNode(_scene);
                if (SingleFingerTouchEvent != null)
                {
                    var touchData = new SingleFingerTouch(0, viewTouchPoint, GestureState.Cancelled);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }
                if (IllegalTouchEvent != null)
                {
                    var touchData = new IllegalTouch(GestureState.Start);
                    IllegalTouchEvent(this, new EventArgs<IllegalTouch>(touchData));
                }
            }
            else
            {
                if (IllegalTouchEvent != null)
                {
                    var touchData = new IllegalTouch(GestureState.Start);
                    IllegalTouchEvent(this, new EventArgs<IllegalTouch>(touchData));
                }
            }
            _currentNumberOfTouches = _currentNumberOfTouches + touches.Count;
        }

        public void TouchesMoved(NSSet touches, UIEvent evt)
        {
        }

        public void TouchesEnded(NSSet touches, UIEvent evt)
        {
            if(touches.Count == 1 && _currentNumberOfTouches == 1)
            {
                var touch = (UITouch)touches.ElementAt(0);
                var viewTouchPoint = touch.LocationInNode(_scene);
                if (SingleFingerTouchEvent != null)
                {
                    var touchData = new SingleFingerTouch(0, viewTouchPoint, GestureState.End);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }
            }
            else if(touches.Count == _currentNumberOfTouches)
            {
                if (IllegalTouchEvent != null)
                {
                    var touchData = new IllegalTouch(GestureState.End);
                    IllegalTouchEvent(this, new EventArgs<IllegalTouch>(touchData));
                }
            }
            _currentNumberOfTouches = _currentNumberOfTouches - touches.Count;
        }
    }
}
