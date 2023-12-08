using System;
using UnityEngine;

namespace Coordinates
{
    public struct PolarCoord
    {
        public float radius;
        // angle can be expressed in radians or degrees, although we use radians internally
        public float radians;
        public float degrees;

        public PolarCoord(float r, float radians)
        {
            radius = r;
            this.radians = radians;
            degrees = RadiansToDegrees(radians);
        }

        public static PolarCoord FromCartesian(float x, float y)
        {
            float radius = Mathf.Sqrt(x * x + y * y); // we find hypotenuse of triangle formed from x, y
            float radians = Mathf.Atan2(y, x); // inverse tan gives the angle in radians

            return new PolarCoord(radius, radians);
        }

        public static PolarCoord FromCartesian(CartesianCoord c)
        {
            float radius = Mathf.Sqrt(c.x * c.x + c.y * c.y); // we find hypotenuse of triangle formed from x, y
            float radians = Mathf.Atan2(c.y, c.x); // inverse tan gives the angle in radians

            return new PolarCoord(radius, radians);
        }

        public static float RadiansToDegrees(float radians) => radians * (180f / Mathf.PI);
        public static float DegreesToRadians(float degrees) => degrees * (Mathf.PI / 180f);
    }
}