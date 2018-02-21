using System;
using System.Diagnostics;
using Foundation;
using UIKit;

namespace ARRunner.Xamarin.Game
{
    public class GestureManager
    {
        public void TouchesBegan(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesBegan: " + touches.Count);

            if(touches.Count == 1)
            {
                
            }
        }

        public void TouchesMoved(NSSet touches, UIEvent evt)
        {
            //Debug.WriteLine("TouchesMoved: " + touches.Count);
        }

        public void TouchesEnded(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesEnded: " + touches.Count);
        }

        public void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            Debug.WriteLine("TouchesCancelled: " + touches.Count);
        }
    }
}
