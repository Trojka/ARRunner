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

            var pointCloudHitPosition = HitTextPointCloud(point, sceneView, 18, 0.2, 2.0);
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

        private static SCNVector3? HitTextPointCloud(CGPoint point, ARSCNView sceneView, double coneOpeningAngleInDegrees, double minDistance = 0, double maxDistance = Double.MaxValue)
        {
            SCNVector3? result = null;
            float maxhitTestResultDistance = float.MinValue;

            if (sceneView.Session.CurrentFrame == null || sceneView.Session.CurrentFrame.RawFeaturePoints == null)
                return null;

            var featureCloud = sceneView.Session.CurrentFrame.RawFeaturePoints;

            var hitTestRay = HitTestRayFromScreenPos(sceneView, point);

            var maxAngleInDeg = Math.Min(coneOpeningAngleInDegrees, 360) / 2.0;
            var maxAngle = (maxAngleInDeg / 180) * Math.PI;

            foreach (var featurePos in featureCloud.Points)
            {
                var scnFeaturePos = new SCNVector3(featurePos.X, featurePos.Y, featurePos.Z);
                var originToFeature = scnFeaturePos - hitTestRay.Origin;
                var crossProduct = originToFeature.Cross(hitTestRay.Direction);
                var featureDistanceFromResult = crossProduct.LengthFast;

                var hitTestResult = hitTestRay.Origin + (hitTestRay.Direction * (hitTestRay.Direction.Dot(originToFeature)));
                var hitTestResultDistance = (hitTestResult - hitTestRay.Origin).LengthFast;

                if (hitTestResultDistance < minDistance || hitTestResultDistance > maxDistance)
                {
                    // Skip this feature -- it's too close or too far
                    continue;
                }

                var originToFeatureNormalized = originToFeature.Normalized();
                var angleBetweenRayAndFeature = Math.Acos(hitTestRay.Direction.Dot(originToFeatureNormalized));

                if (angleBetweenRayAndFeature > maxAngle)
                {
                    // Skip this feature -- it's outside the cone 
                    continue;
                }

                // All tests passed: Add the hit against this feature to the results.
                if(hitTestResultDistance > maxhitTestResultDistance) {
                    result = hitTestResult;
                }

                //results.Add(new FeatureHitTestResult(hitTestResult, hitTestResultDistance, scnFeaturePos, featureDistanceFromResult));
            }

            return result;

            //// Sort the results by feature distance to the ray.
            //results.Sort((a, b) => a.DistanceToRayOrigin.CompareTo(b.DistanceToRayOrigin));

            //// Cap the list to maxResults.
            //results.GetRange(0, Math.Min(results.Count(), maxResults));
            //return results.ToArray();




            //var highQualityfeatureHitTestResults = sceneView.HitTestWithFeatures(position, 18, 0.2, 2.0);
            //if (highQualityfeatureHitTestResults.Count() > 0)
            //{
            //    var highQualityFeatureHit = highQualityfeatureHitTestResults.First();
            //    return highQualityFeatureHit.Position;
            //}


            //return null;
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
    }
}
