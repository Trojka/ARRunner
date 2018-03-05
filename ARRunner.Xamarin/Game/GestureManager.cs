using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
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
        List<UITouch> currentTouches = new List<UITouch>();

        public event EventHandler<EventArgs<SingleFingerTouch>> SingleFingerTouchEvent;
        public event EventHandler<EventArgs<TwoFingerTouch>> TwoFingerTouchEvent;

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
                currentTouches.Add((UITouch)touches.First());
                if(SingleFingerTouchEvent != null)
                {
                    var touch = (UITouch)touches.First();
                    var touchData = new SingleFingerTouch(0, touch.LocationInView(view), GestureState.Start);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }

            }
            else if(touches.Count == 1 && currentTouchType == TouchType.SingleTouch)
            {
                var existingTouch = currentTouches[0];
                currentTouches.Add((UITouch)touches.First());
                if (SingleFingerTouchEvent != null)
                {
                    var touch = existingTouch;
                    var touchData = new SingleFingerTouch(0, touch.LocationInView(view), GestureState.Cancelled);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }
                if(TwoFingerTouchEvent != null)
                {
                    var firstTouchData = currentTouches[0];
                    var secondTouchData = currentTouches[1];
                    var touchData = new TwoFingerTouch(0, firstTouchData.LocationInView(view), secondTouchData.LocationInView(view), GestureState.Start);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                currentTouchType = TouchType.DoubleTouch;
            }
            else if(touches.Count == 2 && currentTouchType == TouchType.None)
            {
                currentTouches.AddRange(touches.Cast<UITouch>());
                if (TwoFingerTouchEvent != null)
                {
                    var firstTouchData = currentTouches[0];
                    var secondTouchData = currentTouches[1];
                    var touchData = new TwoFingerTouch(0, firstTouchData.LocationInView(view), secondTouchData.LocationInView(view), GestureState.Start);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                currentTouchType = TouchType.DoubleTouch;
            }
            else if(currentTouchType == TouchType.DoubleTouch)
            {
                currentTouches.AddRange(touches.Cast<UITouch>());
                if (TwoFingerTouchEvent != null)
                {
                    var firstTouchData = currentTouches[0];
                    var secondTouchData = currentTouches[1];
                    var touchData = new TwoFingerTouch(0, firstTouchData.LocationInView(view), secondTouchData.LocationInView(view), GestureState.Cancelled);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                currentTouchType = TouchType.None;
            }
            Debug.WriteLine("TouchesBegan: type: " + currentTouchType);
        }

        public void TouchesMoved(NSSet touches, UIEvent evt)
        {
            if(currentTouchType == TouchType.SingleTouch)
            {
                // where only handling taps, so no need to process this
            }
            if(currentTouchType == TouchType.DoubleTouch)
            {
                if(touches.Count == 1)
                {
                    if (TwoFingerTouchEvent != null)
                    {
                        var firstTouchData = (UITouch)touches.ElementAt(0);
                        var secondTouchData = currentTouches[1];
                        if (currentTouches[1] == firstTouchData)
                        {
                            firstTouchData = currentTouches[1];
                            secondTouchData = (UITouch)touches.ElementAt(0);
                        }
                        var touchData = new TwoFingerTouch(0, firstTouchData.LocationInView(view), secondTouchData.LocationInView(view), GestureState.Change);
                        TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                    }
                }
                if (touches.Count == 2)
                {
                    if (TwoFingerTouchEvent != null)
                    {
                        var firstTouchData = (UITouch)touches.ElementAt(0);
                        var secondTouchData = (UITouch)touches.ElementAt(1);
                        if (currentTouches[1] == firstTouchData)
                        {
                            firstTouchData = (UITouch)touches.ElementAt(1);
                            secondTouchData = (UITouch)touches.ElementAt(0);
                        }
                        var touchData = new TwoFingerTouch(0, firstTouchData.LocationInView(view), secondTouchData.LocationInView(view), GestureState.Change);
                        TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                    }
                }
            }
        }

        public void TouchesEnded(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesEnded: " + touches.Count);

            if (touches.Count == 1 && currentTouchType == TouchType.SingleTouch)
            {
                if (SingleFingerTouchEvent != null)
                {
                    var touch = (UITouch)touches.First();
                    var touchData = new SingleFingerTouch(0, touch.LocationInView(view), GestureState.End);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }

                currentTouchType = TouchType.None;
            }
            if (currentTouchType == TouchType.DoubleTouch)
            {
                if(TwoFingerTouchEvent != null)
                {
                    var firstTouchData = currentTouches[0];
                    var secondTouchData = currentTouches[1];
                    var touchData = new TwoFingerTouch(0, firstTouchData.LocationInView(view), secondTouchData.LocationInView(view), GestureState.End);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
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
