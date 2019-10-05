using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using aRCCar.Xamarin.Util;
using Foundation;
using UIKit;
using CoreGraphics;

namespace aRCCar.Xamarin.Game
{
    public class GestureManager
    {
        enum TouchType {
            None,
            OneFingerTouch,
            TwoFingerTouch
        }

        UIView _view;
        TouchType _currentTouchType = TouchType.None;
        Dictionary<IntPtr, CGPoint> _touchDictionary = new Dictionary<IntPtr, CGPoint>();
        List<IntPtr> _touchList = new List<IntPtr>();

        public event EventHandler<EventArgs<SingleFingerTouch>> SingleFingerTouchEvent;
        public event EventHandler<EventArgs<TwoFingerTouch>> TwoFingerTouchEvent;

        public GestureManager(UIView view)
        {
            this._view = view;
        }

        public void TouchesBegan(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesBegan: " + touches.Count);
            if(touches.Count == 1 && _currentTouchType == TouchType.None)
            {
                _currentTouchType = TouchType.OneFingerTouch;
                var touch = (UITouch)touches.ElementAt(0);
                var handle = touch.Handle;
                var viewTouchPoint = touch.LocationInView(_view);
                Debug.WriteLine("TouchesBegan (TouchType.None): " + touches.Count + ", TouchType: " + _currentTouchType + ", Handle: " + handle + ", Point: " + viewTouchPoint);

                _touchDictionary.Add(handle, viewTouchPoint);
                _touchList.Add(handle);
                if(SingleFingerTouchEvent != null)
                {
                    var firstViewTouchPoint = _touchDictionary[_touchList[0]];
                    var touchData = new SingleFingerTouch(0, firstViewTouchPoint, GestureState.Start);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }

            }
            else if(touches.Count == 1 && _currentTouchType == TouchType.OneFingerTouch)
            {
                var touch = (UITouch)touches.ElementAt(0);
                var handle = touch.Handle;
                var viewTouchPoint = touch.LocationInView(_view);
                Debug.WriteLine("TouchesBegan: " + touches.Count + ", TouchType.SingleTouch, Handle: " + handle + ", Point: " + viewTouchPoint);
                _touchDictionary.Add(handle, viewTouchPoint);
                _touchList.Add(handle);
                if (SingleFingerTouchEvent != null)
                {
                    var existingViewTouchPoint = _touchDictionary[_touchList[0]];
                    var touchData = new SingleFingerTouch(0, existingViewTouchPoint, GestureState.Cancelled);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }
                if(TwoFingerTouchEvent != null)
                {
                    var firstViewTouchPoint = _touchDictionary[_touchList[0]];
                    var secondViewTouchPoint = _touchDictionary[_touchList[1]];

                    Debug.WriteLine("firstViewTouchPoint: " + firstViewTouchPoint.ToString() + ", secondViewTouchPoint:" + secondViewTouchPoint.ToString());

                    var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Start);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                _currentTouchType = TouchType.TwoFingerTouch;
            }
            else if(touches.Count == 2 && _currentTouchType == TouchType.None)
            {
                foreach (UITouch touch in touches.Cast<UITouch>())
                {
                    var handle = touch.Handle;
                    var viewTouchPoint = touch.LocationInView(_view);
                    _touchDictionary.Add(handle, viewTouchPoint);
                    _touchList.Add(handle);
                }

                if (TwoFingerTouchEvent != null)
                {
                    var firstViewTouchPoint = _touchDictionary[_touchList[0]];
                    var secondViewTouchPoint = _touchDictionary[_touchList[1]];

                    var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Start);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                _currentTouchType = TouchType.TwoFingerTouch;
            }
            else if(_currentTouchType == TouchType.TwoFingerTouch)
            {
                foreach (UITouch touch in touches.Cast<UITouch>())
                {
                    var handle = touch.Handle;
                    var viewTouchPoint = touch.LocationInView(_view);
                    _touchDictionary.Add(handle, viewTouchPoint);
                    _touchList.Add(handle);
                }

                if (TwoFingerTouchEvent != null)
                {
                    var firstViewTouchPoint = _touchDictionary[_touchList[0]];
                    var secondViewTouchPoint = _touchDictionary[_touchList[1]];

                    var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Cancelled);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }
                _currentTouchType = TouchType.None;
            }
            Debug.WriteLine("TouchesBegan: type: " + _currentTouchType);
        }

        public void TouchesMoved(NSSet touches, UIEvent evt)
        {
            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                // Add point to path
                _touchDictionary[touch.Handle] = touch.LocationInView(_view);
            }

            if(_currentTouchType == TouchType.OneFingerTouch)
            {
                // where only handling taps, so no need to process this
            }
            if(_currentTouchType == TouchType.TwoFingerTouch)
            {
                var firstViewTouchPoint = _touchDictionary[_touchList[0]];
                var secondViewTouchPoint = _touchDictionary[_touchList[1]];

                var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.Change);
                TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
            }
        }

        public void TouchesEnded(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesEnded: " + touches.Count);
            if (touches.Count == 1 && _currentTouchType == TouchType.OneFingerTouch)
            {
                Debug.WriteLine("TouchesEnded (TouchType.SingleTouch): " + touches.Count);
                if (SingleFingerTouchEvent != null)
                {
                    var touch = (UITouch)touches.First();
                    var touchData = new SingleFingerTouch(0, touch.LocationInView(_view), GestureState.End);
                    SingleFingerTouchEvent(this, new EventArgs<SingleFingerTouch>(touchData));
                }

                _currentTouchType = TouchType.None;
            }
            if (_currentTouchType == TouchType.TwoFingerTouch)
            {
                if(TwoFingerTouchEvent != null)
                {
                    Debug.WriteLine("TouchesEnded");

                    var firstViewTouchPoint = _touchDictionary[_touchList[0]];
                    var secondViewTouchPoint = _touchDictionary[_touchList[1]];

                    var touchData = new TwoFingerTouch(0, firstViewTouchPoint, secondViewTouchPoint, GestureState.End);
                    TwoFingerTouchEvent(this, new EventArgs<TwoFingerTouch>(touchData));
                }

                _currentTouchType = TouchType.None;
            }

            foreach (UITouch touch in touches.Cast<UITouch>())
            {
                var handle = touch.Handle;
                _touchDictionary.Remove(handle);
                _touchList.Remove(handle);
            }
            Debug.WriteLine("TouchesEnded: type: " + _currentTouchType);

        }

        public void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesCancelled: " + touches.Count);
        }
    }
}
