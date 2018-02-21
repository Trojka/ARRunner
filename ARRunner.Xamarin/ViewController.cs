using System;
using System.Linq;
using ARKit;
using UIKit;
using SceneKit;
using ARRunner.Xamarin.SpatialMapping;
using Foundation;
using ARRunner.Xamarin.SpatialQuerying;
using System.Collections.Generic;
using ARRunner.Xamarin.Game;

namespace ARRunner.Xamarin
{
    public partial class ViewController : UIViewController
    {
        //private class MyARSCNViewDelegate : ARSCNViewDelegate
        //{
        //    SpatialStore _spatialStore;
        //    Dictionary<NSUuid, SCNPlane> _planeStore = new Dictionary<NSUuid, SCNPlane>();

        //    public MyARSCNViewDelegate(SpatialStore spatialStore) 
        //        : base()
        //    {
        //        _spatialStore = spatialStore;
        //    }

        //    public override void Update(ISCNSceneRenderer renderer, double timeInSeconds)
        //    {
        //        base.Update(renderer, timeInSeconds);
        //    }

        //    public override void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        //    {
        //        if (!(anchor is ARPlaneAnchor))
        //            return;

        //        var planeAnchor = anchor as ARPlaneAnchor;

        //        var width = planeAnchor.Extent.X;
        //        var height = planeAnchor.Extent.Z;

        //        var plane = new SCNPlane() { Width = width, Height = height};
        //        plane.Materials.First().Diffuse.Contents = UIColor.Red;

        //        var planeNode = new SCNNode() { Geometry = plane };

        //        var posX = planeAnchor.Center.X;
        //        var posY = planeAnchor.Center.Y;
        //        var posZ = planeAnchor.Center.Z;

        //        planeNode.Position = new SCNVector3(posX, posY, posZ);
        //        planeNode.EulerAngles = new SCNVector3((float)-Math.PI / 2, 0, 0);

        //        node.AddChildNode(planeNode);

        //        _planeStore.Add(anchor.Identifier, plane);
        //        _spatialStore.Register(anchor.Identifier, new Plane(){ Height = planeAnchor.Extent.Z, Width = planeAnchor.Extent.X });
        //    }

        //    public override void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        //    {
        //        if (!(anchor is ARPlaneAnchor))
        //            return;

        //        var planeAnchor = anchor as ARPlaneAnchor;
        //        var planeNode = node.ChildNodes.First();
        //        var plane = planeNode.Geometry as SCNPlane;

        //        var newWidth = planeAnchor.Extent.X;
        //        var newHeight = planeAnchor.Extent.Z;
        //        plane.Width = newWidth;
        //        plane.Height = newHeight;

        //        var posX = planeAnchor.Center.X;
        //        var posY = planeAnchor.Center.Y;
        //        var posZ = planeAnchor.Center.Z;

        //        planeNode.Position = new SCNVector3(posX, posY, posZ);

        //        //_planeStore.[anchor.Identifier] = plane);
        //        _spatialStore.Register(anchor.Identifier, new Plane() { Height = planeAnchor.Extent.Z, Width = planeAnchor.Extent.X });
        //    }

        //    public SCNPlane FindPlane(NSUuid id)
        //    {
        //        return _planeStore[id];
        //    }
        //}

        //ISpatialStore _spatialStore;
        //SpatialQL _spatialQuerying;
        //int _queryId;
        //NSUuid _spatialObjectId;
        //MyARSCNViewDelegate _viewDelegate;

        ARGamePlay gamePlay = new ARGamePlay();
        GestureManager gestureManager = new GestureManager();

        Dictionary<NSUuid, SCNPlane> _planeStore = new Dictionary<NSUuid, SCNPlane>();

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        //void _spatialQuerying_QueryFulfilled(object sender, Util.EventArgs<(int queryId, NSUuid spatialObjectId)> e)
        //{
        //    (var queryId, var spatialObjectId) = e.Value;
        //    if (queryId != _queryId)
        //        return;

        //    _spatialQuerying.QueryFulfilled -= _spatialQuerying_QueryFulfilled;

        //    _spatialObjectId = spatialObjectId;
        //    var scnPlane =_viewDelegate.FindPlane(_spatialObjectId);
        //    scnPlane.Materials.First().Diffuse.Contents = UIColor.Green;
        //}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            //var spatialStore = new SpatialStore();
            //_spatialStore = spatialStore;

            //_spatialQuerying = new SpatialQL(_spatialStore);
            //_spatialQuerying.QueryFulfilled += _spatialQuerying_QueryFulfilled;
            //_queryId = _spatialQuerying.RegisterQuery(p => p.Width >= 0.8 && p.Height >= 0.3);

            //_viewDelegate = new MyARSCNViewDelegate(spatialStore);

            gamePlay.SceneView = sceneView;

            //sceneView.Delegate = _viewDelegate;
            sceneView.Delegate = this;

            sceneView.ShowsStatistics = true;
            //sceneView.DebugOptions = ARSCNDebugOptions.ShowWorldOrigin | ARSCNDebugOptions.ShowFeaturePoints;
            sceneView.DebugOptions = ARSCNDebugOptions.ShowFeaturePoints;
            sceneView.Session = gamePlay.Session;

            var scene = new SCNScene();

            //var box = new SCNBox() { Width = 0.1f, Height = 0.1f, Length = 0.1f, ChamferRadius = 0.0f };
            //var boxNode = new SCNNode();
            //boxNode.Geometry = box;
            //boxNode.Position = new SCNVector3(0, 0, -1.0f);

            //scene.RootNode.AddChildNode(boxNode);

            //SCNReferenceNode candle = new SCNReferenceNode(NSUrl.FromString(
            //    NSBundle.MainBundle.BundleUrl.AbsoluteString + $"Models.scnassets/candle/candle.scn"));
            //candle.Load();
            //candle.Position = new SCNVector3(0, 0, -1.0f); //new SCNVector3(xPos, yPos, zPos);

            //scene.RootNode.AddChildNode(candle);

            sceneView.Scene = scene;
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

            //if(touches.Count <= 0)
            //    return;

            //var touch = (UITouch)touches.First();
            //var pointInScene = touch.LocationInView(sceneView);

            //var hitResult = sceneView.HitTest(pointInScene, ARHitTestResultType.ExistingPlaneUsingExtent);
            //if (hitResult.Count() <= 0)
            //    return;

            //var xPos = hitResult[0].WorldTransform.Column3.X;
            //var yPos = hitResult[0].WorldTransform.Column3.Y;
            //var zPos = hitResult[0].WorldTransform.Column3.Z;

            //var box = new SCNBox() { Width = 0.1f, Height = 0.1f, Length = 0.1f, ChamferRadius = 0.0f };

            //var boxNode = new SCNNode();
            //boxNode.Geometry = box;
            //boxNode.Position = new SCNVector3(xPos, yPos, zPos);

            //sceneView.Scene.RootNode.AddChildNode(boxNode);

            //SCNReferenceNode candle = new SCNReferenceNode(NSUrl.FromString(
            //    NSBundle.MainBundle.BundleUrl.AbsoluteString + $"Models.scnassets/candle/candle.scn"));
            //candle.Load();
            //candle.Position = new SCNVector3(xPos, yPos, zPos); //new SCNVector3(0, 0, -1.0f); //

            //sceneView.Scene.RootNode.AddChildNode(candle);
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
            //var scnPlane = _viewDelegate.FindPlane(_spatialObjectId);
            //scnPlane.Materials.First().Diffuse.Contents = UIColor.Orange;        
        }

        [Action("rightclicked:")]
        void RightClicked(NSObject sender)
        {
            //var scnPlane = _viewDelegate.FindPlane(_spatialObjectId);
            //scnPlane.Materials.First().Diffuse.Contents = UIColor.Magenta;        
        }
    }
}
