using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using ARRunner.Xamarin.Util;
using Foundation;
using UIKit;
using CoreGraphics;

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
        Dictionary<IntPtr, CGPoint> touchDictionary = new Dictionary<IntPtr, CGPoint>();
        List<IntPtr> touchList = new List<IntPtr>();

        public event EventHandler<EventArgs<SingleFingerTouch>> SingleFingerTouchEvent;
        public event EventHandler<EventArgs<TwoFingerTouch>> TwoFingerTouchEvent;

        public GestureManager(UIView view)
        {
            this.view = view;
        }

        public void TouchesBegan(NSSet touches, UIEvent evt)
        {
            //Debug.WriteLine("TouchesBegan: " + touches.Count);

            if(touches.Count == 1 && currentTouchType == TouchType.None)
            {
                currentTouchType = TouchType.SingleTouch;
                var touch = (UITouch)touches.ElementAt(0);
                var handle = touch.Handle;
                var viewTouchPoint = touch.LocationInView(view);
                //Debug.WriteLine("TouchesBegan (TouchType.None): " + touches.Count + ", TouchType: " + currentTouchType + ", Handle: " + handle + ", Point: " + viewTouchPoint);

                touchDictionary.Add(handle, viewTouchPoint);
                touchList.Add(handle);
                if(SingleFingerTouchEvent != null)
                {
                    var firstViewTouchPoint = touchDictionary[touchList[0]];
                    var touchData = new SingleFingerTouch(0, firstViewTouchPoint, GestureState.Start);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }

            }
            else if(touches.Count == 1 && currentTouchType == TouchType.SingleTouch)
            {
                var touch = (UITouch)touches.ElementAt(0);
                var handle = touch.Handle;
                var viewTouchPoint = touch.LocationInView(view);
                //Debug.WriteLine("TouchesBegan: " + touches.Count + ", TouchType.SingleTouch, Handle: " + handle + ", Point: " + viewTouchPoint);
                touchDictionary.Add(handle, viewTouchPoint);
                touchList.Add(handle);
                if (SingleFingerTouchEvent != null)
                {
                    var existingViewTouchPoint = touchDictionary[touchList[0]];
                    var touchData = new SingleFingerTouch(0, existingViewTouchPoint, GestureState.Cancelled);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }
                if(TwoFingerTouchEvent != null)
                {
                    var firstViewTouchPoint = touchDictionary[touchList[0]];
                    var secondViewTouchPoint = touchDictionary[touchList[1]];

                    //Debug.WriteLine("firstViewTouchPoint: " + firstViewTouchPoint.ToString() + ", secondViewTouchPoint:" + secondViewTouchPoint.ToString());

                    var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Start);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                currentTouchType = TouchType.DoubleTouch;
            }
            else if(touches.Count == 2 && currentTouchType == TouchType.None)
            {
                foreach (UITouch touch in touches.Cast<UITouch>())
                {
                    var handle = touch.Handle;
                    var viewTouchPoint = touch.LocationInView(view);
                    touchDictionary.Add(handle, viewTouchPoint);
                    touchList.Add(handle);
                }

                if (TwoFingerTouchEvent != null)
                {
                    var firstViewTouchPoint = touchDictionary[touchList[0]];
                    var secondViewTouchPoint = touchDictionary[touchList[1]];

                    var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Start);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                currentTouchType = TouchType.DoubleTouch;
            }
            else if(currentTouchType == TouchType.DoubleTouch)
            {
                foreach (UITouch touch in touches.Cast<UITouch>())
                {
                    var handle = touch.Handle;
                    var viewTouchPoint = touch.LocationInView(view);
                    touchDictionary.Add(handle, viewTouchPoint);
                    touchList.Add(handle);
                }

                if (TwoFingerTouchEvent != null)
                {
                    var firstViewTouchPoint = touchDictionary[touchList[0]];
                    var secondViewTouchPoint = touchDictionary[touchList[1]];

                    var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Cancelled);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                currentTouchType = TouchType.None;
            }
            //Debug.WriteLine("TouchesBegan: type: " + currentTouchType);
        }

        public void TouchesMoved(NSSet touches, UIEvent evt)
        {
            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                // Add point to path
                touchDictionary[touch.Handle] = touch.LocationInView(view);
            }

            if(currentTouchType == TouchType.SingleTouch)
            {
                // where only handling taps, so no need to process this
            }
            if(currentTouchType == TouchType.DoubleTouch)
            {
                var firstViewTouchPoint = touchDictionary[touchList[0]];
                var secondViewTouchPoint = touchDictionary[touchList[1]];

                var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Change);
                TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
            }
        }

        public void TouchesEnded(NSSet touches, UIEvent evt)
        {
            //Debug.WriteLine("TouchesEnded: " + touches.Count);


            if (touches.Count == 1 && currentTouchType == TouchType.SingleTouch)
            {
                //Debug.WriteLine("TouchesEnded (TouchType.SingleTouch): " + touches.Count);
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
                    //Debug.WriteLine("TouchesEnded");

                    var firstViewTouchPoint = touchDictionary[touchList[0]];
                    var secondViewTouchPoint = touchDictionary[touchList[1]];

                    var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.End);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }

                currentTouchType = TouchType.None;
           }


            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                var handle = touch.Handle;
                touchDictionary.Remove(handle);
                touchList.Remove(handle);
            }

           //Debug.WriteLine("TouchesEnded: type: " + currentTouchType);

        }

        public void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            //Debug.WriteLine("TouchesCancelled: " + touches.Count);
        }
    }
}
