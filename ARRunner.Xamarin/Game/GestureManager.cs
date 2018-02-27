using System;
using System.Diagnostics;
using System.Linq;
using ARRunner.Xamarin.Util;
using Foundation;
using UIKit;

namespace ARRunner.Xamarin.Game
{
    public class GestureManager
    {
        enum TouchType {
            None,
            SingleTouch,
            DoubleTouch
        }

        UIView view;
        TouchType currentTouchType = TouchType.None;

        public event EventHandler<EventArgs<SingleFingerTouch>> SingleTouchEvent;

        public GestureManager(UIView view)
        {
            this.view = view;
        }

        public void TouchesBegan(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesBegan: " + touches.Count);

            if(touches.Count == 1 && currentTouchType == TouchType.None)
            {
                currentTouchType = TouchType.SingleTouch;
                if(SingleTouchEvent != null)
                {
                    var touch = (UITouch)touches.First();
                    var touchData = new SingleFingerTouch(0, touch.LocationInView(view), GestureState.Start);
                    SingleTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }

            }
            else if(touches.Count == 1 && currentTouchType == TouchType.SingleTouch)
            {
                if (SingleTouchEvent != null)
                {
                    var touch = (UITouch)touches.First();
                    var touchData = new SingleFingerTouch(0, touch.LocationInView(view), GestureState.Cancelled);
                    SingleTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }

                currentTouchType = TouchType.None;
            }
            Debug.WriteLine("TouchesBegan: type: " + currentTouchType);
        }

        public void TouchesMoved(NSSet touches, UIEvent evt)
        {
            //Debug.WriteLine("TouchesMoved: " + touches.Count);
        }

        public void TouchesEnded(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesEnded: " + touches.Count);

            if (touches.Count == 1 && currentTouchType == TouchType.SingleTouch)
            {
                if (SingleTouchEvent != null)
                {
                    var touch = (UITouch)touches.First();
                    var touchData = new SingleFingerTouch(0, touch.LocationInView(view), GestureState.End);
                    SingleTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }

                currentTouchType = TouchType.None;
            }
            Debug.WriteLine("TouchesEnded: type: " + currentTouchType);

        }

        public void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesCancelled: " + touches.Count);
        }
    }
}
