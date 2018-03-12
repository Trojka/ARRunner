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
        //List<(IntPtr Id, CGPoint Point)> currentTouches = new List<(IntPtr, CGPoint)>();
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
            Debug.WriteLine("TouchesBegan: " + touches.Count);

            if(touches.Count == 1 && currentTouchType == TouchType.None)
            {
                currentTouchType = TouchType.SingleTouch;
                var touch = (UITouch)touches.ElementAt(0);
                var handle = touch.Handle;
                var viewTouchPoint = touch.LocationInView(view);
                //currentTouches.Add((touch.Handle, viewTouchPoint));
                Debug.WriteLine("TouchesBegan: " + touches.Count + ", TouchType.None, Handle: " + handle + ", Point: " + viewTouchPoint);

                touchDictionary.Add(handle, viewTouchPoint);
                touchList.Add(handle);
                if(SingleFingerTouchEvent != null)
                {
                    //var firstViewTouchPoint = currentTouches[0].Point;
                    var firstViewTouchPoint = touchDictionary[touchList[0]];
                    //var touch = (UITouch)touches.First();
                    var touchData = new SingleFingerTouch(0, firstViewTouchPoint, GestureState.Start);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }

            }
            else if(touches.Count == 1 && currentTouchType == TouchType.SingleTouch)
            {
                //var existingTouch = currentTouches[0];
                var touch = (UITouch)touches.ElementAt(0);
                var handle = touch.Handle;
                var viewTouchPoint = touch.LocationInView(view);
                //currentTouches.Add((touch.Handle, viewTouchPoint));
                //Debug.WriteLine("TouchesBegan: " + touches.Count + ", TouchType.SingleTouch, Handle: " + handle + ", Point: " + viewTouchPoint);
                touchDictionary.Add(handle, viewTouchPoint);
                touchList.Add(handle);
                if (SingleFingerTouchEvent != null)
                {
                    //var existingViewTouchPoint = currentTouches[0].Point;
                    var existingViewTouchPoint = touchDictionary[touchList[0]];
                    var touchData = new SingleFingerTouch(0, existingViewTouchPoint, GestureState.Cancelled);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }
                if(TwoFingerTouchEvent != null)
                {
                    //var firstViewTouchPoint = currentTouches[0].Point;
                    //var secondViewTouchPoint = currentTouches[1].Point;
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

                //currentTouches.AddRange(touches.Cast<UITouch>().Select(a => (a.Handle, a.LocationInView(view))));
                if (TwoFingerTouchEvent != null)
                {
                    //var firstViewTouchPoint = currentTouches[0].Point;
                    //var secondViewTouchPoint = currentTouches[1].Point;
                    var firstViewTouchPoint = touchDictionary[touchList[0]];
                    var secondViewTouchPoint = touchDictionary[touchList[1]];

                    var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Start);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                currentTouchType = TouchType.DoubleTouch;
            }
            else if(currentTouchType == TouchType.DoubleTouch)
            {
                //currentTouches.AddRange(touches.Cast<UITouch>().Select(a => (a.Handle, a.LocationInView(view))));
                foreach (UITouch touch in touches.Cast<UITouch>())
                {
                    var handle = touch.Handle;
                    var viewTouchPoint = touch.LocationInView(view);
                    touchDictionary.Add(handle, viewTouchPoint);
                    touchList.Add(handle);
                }

                if (TwoFingerTouchEvent != null)
                {
                    //var firstViewTouchPoint = currentTouches[0].Point;
                    //var secondViewTouchPoint = currentTouches[1].Point;
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


                // https://developer.xamarin.com/guides/ios/application_fundamentals/touch/touch_tracking/
                //if(touches.Count == 1)
                //{
                //    if (TwoFingerTouchEvent != null)
                //    {
                //        var newTouches = touches.Cast<UITouch>().Select(a => (a.Handle, a.LocationInView(view))).ToList();
                //        if(currentTouches[0].Id == newTouches[0].Handle)
                //        {
                //            currentTouches[0] = newTouches[0];
                //        }
                //        else 
                //        {
                //            currentTouches[1] = newTouches[0];
                //        }

                //        var firstViewTouchPoint = currentTouches[0].Point;
                //        var secondViewTouchPoint = currentTouches[1].Point;
                //        var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Cancelled);

                //        //var firstTouchData = (UITouch)touches.ElementAt(0);
                //        //var secondTouchData = (UITouch)touches.ElementAt(0);
                //        //if (firstTouchData == currentTouches[0])
                //        //{
                //        //     secondTouchData = currentTouches[1];
                //        //}
                //        //else
                //        //{
                //        //    firstTouchData = currentTouches[1];
                //        //}

                //        //var viewTouchPoint1 = /*firstTouchData*/ currentTouches[0].LocationInView(view);
                //        //var viewTouchPoint2 = /*secondTouchData*/ currentTouches[1].LocationInView(view);

                //        Debug.WriteLine("TouchesMoved(1): firstViewTouchPoint" + firstViewTouchPoint.ToString() + ", secondViewTouchPoint: " + secondViewTouchPoint.ToString());

                //        //var touchData = new TwoFingerTouch(0, viewTouchPoint1, viewTouchPoint2, GestureState.Change);
                //        TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                //    }
                //}
                //if (touches.Count == 2)
                //{
                //    if (TwoFingerTouchEvent != null)
                //    {
                //        var newTouches = touches.Cast<UITouch>().Select(a => (a.Handle, a.LocationInView(view))).ToList();
                //        if (currentTouches[0].Id == newTouches[0].Handle)
                //        {
                //            currentTouches[0] = newTouches[0];
                //            currentTouches[1] = newTouches[1];
                //        }
                //        else
                //        {
                //            currentTouches[0] = newTouches[1];
                //            currentTouches[1] = newTouches[0];
                //        }

                //        var firstViewTouchPoint = currentTouches[0].Point;
                //        var secondViewTouchPoint = currentTouches[1].Point;
                //        var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Cancelled);

                //        //var firstTouchData = (UITouch)touches.ElementAt(0);
                //        //var secondTouchData = (UITouch)touches.ElementAt(1);
                //        //if (firstTouchData == currentTouches[1])
                //        //{
                //        //    firstTouchData = (UITouch)touches.ElementAt(1);
                //        //    secondTouchData = (UITouch)touches.ElementAt(0);
                //        //}

                //        //var viewTouchPoint1 = /*firstTouchData*/ currentTouches[0].LocationInView(view);
                //        //var viewTouchPoint2 = /*secondTouchData*/ currentTouches[1].LocationInView(view);

                //        Debug.WriteLine("TouchesMoved(2): firstViewTouchPoint" + firstViewTouchPoint.ToString() + ", secondViewTouchPoint: " + secondViewTouchPoint.ToString());

                //        //var touchData = new TwoFingerTouch(0, viewTouchPoint1, viewTouchPoint2, GestureState.Change);
                //        TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                //    }
                //}
            }
        }

        public void TouchesEnded(NSSet touches, UIEvent evt)
        {
            //Debug.WriteLine("TouchesEnded: " + touches.Count);


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
                    //var newTouches = touches.Cast<UITouch>().Select(a => (a.Handle, a.LocationInView(view))).ToList();
                    //if (currentTouches[0].Id == newTouches[0].Handle)
                    //{
                    //    currentTouches[0] = newTouches[0];
                    //    currentTouches[1] = newTouches[1];
                    //}
                    //else
                    //{
                    //    currentTouches[0] = newTouches[1];
                    //    currentTouches[1] = newTouches[0];
                    //}

                    //var firstViewTouchPoint = currentTouches[0].Point;
                    //var secondViewTouchPoint = currentTouches[1].Point;


                    //var firstTouchData = currentTouches[0];
                    //var secondTouchData = currentTouches[1];

                    Debug.WriteLine("TouchesEnded");

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
