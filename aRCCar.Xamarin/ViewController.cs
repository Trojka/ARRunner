using System;
using System.Linq;
using ARKit;
using UIKit;
using SceneKit;
using Foundation;
using System.Collections.Generic;
using aRCCar.Xamarin.Game;
using System.Diagnostics;

namespace aRCCar.Xamarin
{
    public partial class ViewController : UIViewController
    {
        ARGamePlay gamePlay = new ARGamePlay();
        GestureManager gestureManager;

        Dictionary<NSUuid, SCNPlane> _planeStore = new Dictionary<NSUuid, SCNPlane>();

        OverlayScene _overlayScene;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            gestureManager = new GestureManager(sceneView);
            gestureManager.SingleFingerTouchEvent += gamePlay.GestureManager_SingleFingerTouchEvent;
            gestureManager.TwoFingerTouchEvent += gamePlay.GestureManager_TwoFingerTouchEvent;

            gamePlay.SceneView = sceneView;

            sceneView.Delegate = this;

            sceneView.ShowsStatistics = true;
            sceneView.DebugOptions = ARSCNDebugOptions.ShowFeaturePoints;
            sceneView.Session = gamePlay.Session;

            var scene = new SCNScene();

            _overlayScene = new OverlayScene(sceneView.Bounds.Size);
            _overlayScene.UserInteractionEnabled = false;
            gamePlay.OverlayScene = _overlayScene;

            sceneView.Scene = scene;
            sceneView.OverlayScene = _overlayScene;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Create a session configuration
            var configuration = new ARWorldTrackingConfiguration
            {
                PlaneDetection = ARPlaneDetection.Horizontal,
                LightEstimationEnabled = true
            };

            // Run the view's session
            sceneView.Session.Run(configuration, ARSessionRunOptions.ResetTracking);
        }

        public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
        {
            gestureManager.TouchesBegan(touches, evt);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            gestureManager.TouchesMoved(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            gestureManager.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            gestureManager.TouchesCancelled(touches, evt);
        }

        [Action("leftclicked:")]
        void LeftClicked(NSObject sender)
        {
            Debug.WriteLine("Left clicked");
            gamePlay.LeftActuator();
        }

        [Action("rightclicked:")]
        void RightClicked(NSObject sender)
        {
            Debug.WriteLine("Right clicked");
            gamePlay.RightActuator();
        }
    }
}
