using System;
using System.Linq;
using ARKit;
using aRCCar.Xamarin.Extensions;
using aRCCar.Xamarin.Game;
using CoreGraphics;
using Foundation;
using SceneKit;

namespace aRCCar.Xamarin.SpatialMapping
{
    public class PlaneFinding
    {
        public enum HitType
        {
            None,
            Plane,
            FeaturePoint
        }

        public static (SCNVector3? hitPoint, HitType hitType) FindNearestWorldPointToScreenPoint(CGPoint point, ARSCNView sceneView, SCNVector3? pointOnPlane)
        {
            var planeHitPosition = HitTestExistingPlanes(point, sceneView);
            if (planeHitPosition.HasValue)
                return (planeHitPosition, HitType.Plane);

            var pointCloudHitPosition = HitTestPointCloud(point, sceneView, 18, 0.2, 2.0);
            if (pointCloudHitPosition.HasValue)
                return (pointCloudHitPosition, HitType.FeaturePoint);

            //if(pointOnPlane.HasValue)
            //{
            //    var infinitePlaneHitPosition = HitTestWithInfiniteHorizontalPlane(point, sceneView, pointOnPlane.Value);
            //    if (infinitePlaneHitPosition.HasValue)
            //        return infinitePlaneHitPosition.Value;
            //}

            return (null, HitType.None);
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

            if (sceneView.Session == null || ARGamePlay.CurrentFrame == null)
            {
                return null; //results.ToArray();
            }
            var features = ARGamePlay.CurrentFrame.RawFeaturePoints;
            if (features == null)
            {
                return null; //results.ToArray();
            }

            var ray = sceneView.HitTestRayFromScreenPos(point);
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



    }
}
