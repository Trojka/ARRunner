using System;
using ARKit;
using aRCCar.Xamarin.Game;
using CoreGraphics;
using SceneKit;
using aRCCar.Xamarin.SpatialMapping;

namespace aRCCar.Xamarin.Extensions
{
    public static class ARSCNViewEx
    {
        public static Ray HitTestRayFromScreenPos(this ARSCNView sceneView, CGPoint point)
        {
            //if (sceneView.Session == null || ViewController.CurrentFrame == null)
            //{
            //    return null;
            //}

            var frame = ARGamePlay.CurrentFrame;
            //var frame = ViewController.CurrentFrame;
            if (frame == null || frame.Camera == null || frame.Camera.Transform == null)
            {
                return null;
            }

            var cameraPos = SCNVector3Ex.PositionFromTransform(frame.Camera.Transform);

            // Note: z: 1.0 will unproject() the screen position to the far clipping plane.
            var positionVec = new SCNVector3((float)point.X, (float)point.Y, 1.0f);
            var screenPosOnFarClippingPlane = sceneView.UnprojectPoint(positionVec);

            var rayDirection = screenPosOnFarClippingPlane - cameraPos; //screenPosOnFarClippingPlane.Subtract(cameraPos);
            rayDirection.Normalize();

            return new Ray(cameraPos, rayDirection);
        } 

        public static SCNVector3? HitTestWithInfiniteHorizontalPlane(this ARSCNView sceneView, CGPoint point, SCNVector3 pointOnPlane)
        {
            //if (sceneView.Session == null || ARGamePlay.CurrentFrame == null)
            //{
            //    return null;
            //}
            //
            //var currentFrame = ARGamePlay.CurrentFrame;

            var ray = sceneView.HitTestRayFromScreenPos(point);
            if (ray == null)
            {
                return null;
            };

            // Do not intersect with planes above the camera or if the ray is almost parallel to the plane.
            if (ray.Direction.Y > -0.03f)
            {
                return null;
            }

            return ray.IntersectionWithHorizontalPlane(pointOnPlane.Y);
        }

    }
}
