using System;
using System.Linq;
using ARKit;
using ARRunner.Xamarin.Extensions;
using CoreGraphics;
using Foundation;
using SceneKit;

namespace ARRunner.Xamarin.SpatialMapping
{
    public class PlaneFinding
    {
        private class HitTestRay : NSObject
        {
            public SCNVector3 Origin { get; set; }

            public SCNVector3 Direction { get; set; }
            public HitTestRay()
            {
            }

            public HitTestRay(SCNVector3 origin, SCNVector3 direction)
            {
                // Initialize
                Origin = origin;
                Direction = direction;
            }
        }

        public static SCNVector3? FindNearestWorldPointToScreenPoint(CGPoint point, ARSCNView sceneView)
        {
            //var planeHitPosition = HitTestExistingPlanes(point, sceneView);
            //if (planeHitPosition.HasValue)
                //return planeHitPosition;

            var pointCloudHitPosition = HitTestPointCloud(point, sceneView, 18, 0.2, 2.0);
            if (pointCloudHitPosition.HasValue)
                return pointCloudHitPosition;

            return null;
        }

        private static SCNVector3? HitTestExistingPlanes(CGPoint point, ARSCNView sceneView)
        {
            var hitResult = sceneView.HitTest(point, ARHitTestResultType.ExistingPlaneUsingExtent);
            if (hitResult.Count() > 0)
            {
                var xPos = hitResult[0].WorldTransform.Column3.X;
                var yPos = hitResult[0].WorldTransform.Column3.Y;
                var zPos = hitResult[0].WorldTransform.Column3.Z;

                return new SCNVector3(xPos, yPos, zPos);
            }

            return null;        
        }

        private static SCNVector3? HitTestPointCloud(CGPoint point, ARSCNView sceneView, double coneOpeningAngleInDegrees, double minDistance = 0, double maxDistance = Double.MaxValue)
        {
            //var results = new List<FeatureHitTestResult>();
            var minHitTestResultDistance = float.MaxValue;
            //FeatureHitTestResult closestFeauture = null;
            SCNVector3? closestFeauturePosition = null;

            if (sceneView.Session == null || ViewController.CurrentFrame == null)
            {
                return null; //results.ToArray();
            }
            var features = ViewController.CurrentFrame.RawFeaturePoints;
            if (features == null)
            {
                return null; //results.ToArray();
            }

            var ray = HitTestRayFromScreenPos(sceneView, point);
            if (ray == null)
            {
                return null; //results.ToArray();
            }

            var maxAngleInDeg = Math.Min(coneOpeningAngleInDegrees, 360) / 2.0;
            var maxAngle = (maxAngleInDeg / 180) * Math.PI;

            foreach (var featurePos in features.Points)
            {
                var scnFeaturePos = new SCNVector3(featurePos.X, featurePos.Y, featurePos.Z);
                var originToFeature = scnFeaturePos - ray.Origin;
                //var crossProduct = originToFeature.Cross(ray.Direction);
                //var featureDistanceFromResult = crossProduct.LengthFast;

                var hitTestResult = ray.Origin + (ray.Direction * (ray.Direction.Dot(originToFeature)));
                var hitTestResultDistance = (hitTestResult - ray.Origin).LengthFast;

                if (hitTestResultDistance < minDistance || hitTestResultDistance > maxDistance)
                {
                    // Skip this feature -- it's too close or too far
                    continue;
                }

                var originToFeatureNormalized = originToFeature.Normalized();
                var angleBetweenRayAndFeature = Math.Acos(ray.Direction.Dot(originToFeatureNormalized));

                if (angleBetweenRayAndFeature > maxAngle)
                {
                    // Skip this feature -- it's outside the cone 
                    continue;
                }

                if(hitTestResultDistance < minHitTestResultDistance)
                {
                    minHitTestResultDistance = hitTestResultDistance;
                    closestFeauturePosition = hitTestResult; //new FeatureHitTestResult(hitTestResult); //, hitTestResultDistance, scnFeaturePos, featureDistanceFromResult);
                }

                // All tests passed: Add the hit against this feature to the results.
                //results.Add(new FeatureHitTestResult(hitTestResult, hitTestResultDistance, scnFeaturePos, featureDistanceFromResult));
            }

            return closestFeauturePosition;

   //         if(closestFeauture != null)
   //         {
   //             results.Add(closestFeauture);
   //         }

            ////// Sort the results by feature distance to the ray.
            ////results.Sort((a, b) => a.DistanceToRayOrigin.CompareTo(b.DistanceToRayOrigin));

            ////// Cap the list to maxResults.
            ////results.GetRange(0, Math.Min(results.Count(), maxResults));
            //return results.ToArray();
        }

        //public static SCNVector3? HitTextPointCloud(CGPoint pt, ARSCNView self)
        //{
        //    var results = new List<FeatureHitTestResult>();
        //    var ray = self.HitTestRayFromScreenPos(pt);
        //    if (ray == null)
        //    {
        //        return results.ToArray();
        //    }
        //    var result = self.HitTestFromOrigin(ray.Origin, ray.Direction);
        //    if (result != null)
        //    {
        //        results.Add(result);
        //    }
        //    return results.ToArray();
        //}

        public static SCNVector3? HitTestWithInfiniteHorizontalPlane(CGPoint point, ARSCNView sceneView, SCNVector3 pointOnPlane)
        {
            if (sceneView.Session == null || ViewController.CurrentFrame == null)
            {
                return null;
            }

            var currentFrame = ViewController.CurrentFrame;

            var ray = HitTestRayFromScreenPos(sceneView, point);
            if (ray == null)
            {
                return null;
            };

            // Do not intersect with planes above the camera or if the ray is almost parallel to the plane.
            if (ray.Direction.Y > -0.03f)
            {
                return null;
            }

            return RayIntersectionWithHorizontalPlane(ray.Origin, ray.Direction, pointOnPlane.Y);
        }

        private static HitTestRay HitTestRayFromScreenPos(ARSCNView sceneView, CGPoint point)
        {
            //if (sceneView.Session == null || ViewController.CurrentFrame == null)
            //{
            //    return null;
            //}

            var frame = sceneView.Session.CurrentFrame;
            //var frame = ViewController.CurrentFrame;
            //if (frame == null || frame.Camera == null || frame.Camera.Transform == null)
            //{
            //    return null;
            //}

            var cameraPos = SCNVector3Ex.PositionFromTransform(frame.Camera.Transform);

            // Note: z: 1.0 will unproject() the screen position to the far clipping plane.
            var positionVec = new SCNVector3((float)point.X, (float)point.Y, 1.0f);
            var screenPosOnFarClippingPlane = sceneView.UnprojectPoint(positionVec);

            var rayDirection = screenPosOnFarClippingPlane - cameraPos; //screenPosOnFarClippingPlane.Subtract(cameraPos);
            rayDirection.Normalize();

            return new HitTestRay(cameraPos, rayDirection);
        }

        public static SCNVector3? RayIntersectionWithHorizontalPlane(SCNVector3 rayOrigin, SCNVector3 direction, float planeY)
        {
            // Normalize direction
            direction = direction.Normalized();

            // Special case handling: Check if the ray is horizontal as well.
            if (direction.Y == 0)
            {
                if (rayOrigin.Y == planeY)
                {
                    // The ray is horizontal and on the plane, thus all points on the ray intersect with the plane.
                    // Therefore we simply return the ray origin.
                    return rayOrigin;
                }
                else
                {
                    // The ray is parallel to the plane and never intersects.
                    return null;
                }
            }

            // The distance from the ray's origin to the intersection point on the plane is:
            //   (pointOnPlane - rayOrigin) dot planeNormal
            //  --------------------------------------------
            //          direction dot planeNormal

            // Since we know that horizontal planes have normal (0, 1, 0), we can simplify this to:
            var dist = (planeY - rayOrigin.Y) / direction.Y;

            // Do not return intersections behind the ray's origin.
            if (dist < 0)
            {
                return null;
            }

            // Return the intersection point.
            return rayOrigin.Add(direction * dist);
        }


    }
}
