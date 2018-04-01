using System;
using aRCCar.Xamarin.Extensions;
using Foundation;
using SceneKit;

namespace aRCCar.Xamarin.SpatialMapping
{
    public class Ray : NSObject
    {
        public SCNVector3 Origin { get; set; }

        public SCNVector3 Direction { get; set; }
        public Ray()
        {
        }

        public Ray(SCNVector3 origin, SCNVector3 direction)
        {
            // Initialize
            Origin = origin;
            Direction = direction;
        }

        public SCNVector3? IntersectionWithHorizontalPlane(float planeY)
        {
            SCNVector3 rayOrigin = Origin;
            SCNVector3 direction = Direction;

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
