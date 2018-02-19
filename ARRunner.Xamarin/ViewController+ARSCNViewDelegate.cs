using System;
using System.Linq;
using ARKit;
using Foundation;
using SceneKit;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using ARRunner.Xamarin.SpatialMapping;

namespace ARRunner.Xamarin
{
    public partial class ViewController : IARSCNViewDelegate
    {
        SCNNode _cursorNode = null;

        [Export("renderer:updateAtTime:")]
        public void RendererUpdateAtTime(SCNSceneRenderer renderer, double updateAtTime)
        {
            if (Session?.CurrentFrame == null)
            {
                return;
            }
            // Vital for memory: Single location to set current frame! (Note: Assignment disposes existing frame -- see `set`
            ViewController.CurrentFrame = Session.CurrentFrame;
            //UpdateFocusSquare();


            var screenRect = sceneView.Bounds;
            var screenCenter = new CGPoint(screenRect.GetMidX(), screenRect.GetMidY());

            var worldPos = PlaneFinding.FindNearestWorldPointToScreenPoint(screenCenter, sceneView, null);

            if (worldPos.hitType == PlaneFinding.HitType.None)
                return;

            if(_cursorNode == null)
            {
                var box = new SCNBox() { Width = 0.1f, Height = 0.1f, Length = 0.1f, ChamferRadius = 0.0f };

                _cursorNode = new SCNNode();
                _cursorNode.Geometry = box;
                _cursorNode.Position = worldPos.hitPoint.Value;

                sceneView.Scene.RootNode.AddChildNode(_cursorNode);
            }

            _cursorNode.Position = worldPos.hitPoint.Value;
            _cursorNode.Geometry.Materials.First().Diffuse.Contents = worldPos.hitType == PlaneFinding.HitType.Plane ? UIColor.Green : UIColor.Red;
        }

        [Export("renderer:didAddNode:forAnchor:")]
        public void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            //if (!(anchor is ARPlaneAnchor))
            //    return;

            //var planeAnchor = anchor as ARPlaneAnchor;

            //var width = planeAnchor.Extent.X;
            //var height = planeAnchor.Extent.Z;

            //var plane = new SCNPlane() { Width = width, Height = height };
            //plane.Materials.First().Diffuse.Contents = UIColor.Red;

            //var planeNode = new SCNNode() { Geometry = plane };

            //var posX = planeAnchor.Center.X;
            //var posY = planeAnchor.Center.Y;
            //var posZ = planeAnchor.Center.Z;

            //planeNode.Position = new SCNVector3(posX, posY, posZ);
            //planeNode.EulerAngles = new SCNVector3((float)-Math.PI / 2, 0, 0);

            //node.AddChildNode(planeNode);

            //_planeStore.Add(anchor.Identifier, plane);
            ////_spatialStore.Register(anchor.Identifier, new Plane() { Height = planeAnchor.Extent.Z, Width = planeAnchor.Extent.X });
        }

        [Export("renderer:didUpdateNode:forAnchor:")]
        public void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            //if (!(anchor is ARPlaneAnchor))
            //    return;

            //var planeAnchor = anchor as ARPlaneAnchor;
            //var planeNode = node.ChildNodes.First();
            //var plane = planeNode.Geometry as SCNPlane;

            //var newWidth = planeAnchor.Extent.X;
            //var newHeight = planeAnchor.Extent.Z;
            //plane.Width = newWidth;
            //plane.Height = newHeight;

            //var posX = planeAnchor.Center.X;
            //var posY = planeAnchor.Center.Y;
            //var posZ = planeAnchor.Center.Z;

            //planeNode.Position = new SCNVector3(posX, posY, posZ);

            ////_planeStore.[anchor.Identifier] = plane);
            ////_spatialStore.Register(anchor.Identifier, new Plane() { Height = planeAnchor.Extent.Z, Width = planeAnchor.Extent.X });

        }

        //[Export("renderer:didRemoveNode:forAnchor:")]
        //public void DidRemoveNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        //{
        //}
    }
}
