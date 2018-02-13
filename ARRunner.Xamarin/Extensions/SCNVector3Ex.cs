using System;
using OpenTK;
using SceneKit;

namespace ARRunner.Xamarin.Extensions
{
    public static class SCNVector3Ex
    {
        public static SCNVector3 PositionFromTransform(NMatrix4 transform)
        {
            var pFromComponents = new SCNVector3(transform.M14, transform.M24, transform.M34);
            return pFromComponents;
        }

        public static SCNVector3 Normalized(this SCNVector3 vector3)
        {

            if (vector3.Length < Double.Epsilon)
            {
                return vector3;
            }
            else
            {
                return vector3 / vector3.Length;
            }
        }

        public static float Dot(this SCNVector3 vector3, SCNVector3 vec)
        {
            return (vector3.X * vec.X) + (vector3.Y * vec.Y) + (vector3.Z * vec.Z);
        }

        public static SCNVector3 Cross(this SCNVector3 vector3, SCNVector3 vec)
        {
            return new SCNVector3(vector3.Y * vec.Z - vector3.Z * vec.Y, vector3.Z * vec.X - vector3.X * vec.Z, vector3.X * vec.Y - vector3.Y * vec.X);
        }
    }
}
