using System;
using System.Linq;
using ARKit;
using CoreGraphics;
using SceneKit;

namespace ARRunner.Xamarin.SpatialMapping
{
    public class PlaneFinding
    {
        public static SCNVector3? FindNearestWorldPointToScreenPoint(CGPoint point, ARSCNView sceneView)
        {
            var hitResult = sceneView.HitTest(point, ARHitTestResultType.ExistingPlaneUsingExtent);
            if (hitResult.Count() <= 0)
                return null;

            var xPos = hitResult[0].WorldTransform.Column3.X;
            var yPos = hitResult[0].WorldTransform.Column3.Y;
            var zPos = hitResult[0].WorldTransform.Column3.Z;

            return new SCNVector3(xPos, yPos, zPos);
        }
    }
}
