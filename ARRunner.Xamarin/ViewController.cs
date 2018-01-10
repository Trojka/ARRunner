using System;
using System.Linq;
using ARKit;
using UIKit;
using SceneKit;

namespace ARRunner.Xamarin
{
    public partial class ViewController : UIViewController
    {
        private class MyARSCNViewDelegate : ARSCNViewDelegate
        {
            public override void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
            {
                if (!(anchor is ARPlaneAnchor))
                    return;

                var planeAnchor = anchor as ARPlaneAnchor;

                var width = planeAnchor.Extent.X;
                var height = planeAnchor.Extent.Z;

                var plane = new SCNPlane() { Width = width, Height = height};
                plane.Materials.First().Diffuse.Contents = UIColor.Red;

                var planeNode = new SCNNode() { Geometry = plane };

                var posX = planeAnchor.Center.X;
                var posY = planeAnchor.Center.Y;
                var posZ = planeAnchor.Center.Z;

                planeNode.Position = new SCNVector3(posX, posY, posZ);
                planeNode.EulerAngles = new SCNVector3((float)-Math.PI / 2, 0, 0);

                node.AddChildNode(planeNode);
            }

            public override void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
            {
                if (!(anchor is ARPlaneAnchor))
                    return;

                var planeAnchor = anchor as ARPlaneAnchor;
                var planeNode = node.ChildNodes.First();
                var plane = planeNode.Geometry as SCNPlane;

                var newWidth = planeAnchor.Extent.X;
                var newHeight = planeAnchor.Extent.Z;
                plane.Width = newWidth;
                plane.Height = newHeight;

                var posX = planeAnchor.Center.X;
                var posY = planeAnchor.Center.Y;
                var posZ = planeAnchor.Center.Z;

                planeNode.Position = new SCNVector3(posX, posY, posZ);
            }
        }

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            sceneView.Delegate = new MyARSCNViewDelegate();

            sceneView.ShowsStatistics = true;
            sceneView.DebugOptions = ARSCNDebugOptions.ShowWorldOrigin | ARSCNDebugOptions.ShowFeaturePoints;

            var scene = new SCNScene();

            var box = new SCNBox() { Width = 0.1f, Height = 0.1f, Length = 0.1f, ChamferRadius = 0.0f };

            var boxNode = new SCNNode();
            boxNode.Geometry = box;
            boxNode.Position = new SCNVector3(0, 0, -0.5f);

            scene.RootNode.AddChildNode(boxNode);

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
            if(touches.Count <= 0)
                return;

            var touch = (UITouch)touches.First();
            var pointInScene = touch.LocationInView(sceneView);

            var hitResult = sceneView.HitTest(pointInScene, ARHitTestResultType.ExistingPlaneUsingExtent);
            if (hitResult.Count() <= 0)
                return;

            var xPos = hitResult[0].WorldTransform.Column3.X;
            var yPos = hitResult[0].WorldTransform.Column3.Y;
            var zPos = hitResult[0].WorldTransform.Column3.Z;

            var box = new SCNBox() { Width = 0.1f, Height = 0.1f, Length = 0.1f, ChamferRadius = 0.0f };

            var boxNode = new SCNNode();
            boxNode.Geometry = box;
            boxNode.Position = new SCNVector3(xPos, yPos, zPos);

            sceneView.Scene.RootNode.AddChildNode(boxNode);
        }
    }
}
