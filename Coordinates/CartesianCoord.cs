using System;
using UnityEngine;

namespace Coordinates
{
    public struct CartesianCoord
    {
        public float x;
        public float y;

        public CartesianCoord(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static CartesianCoord FromPolar(float r, float radians)
        {
            float x = Mathf.Cos(radians) * r;
            float y = Mathf.Sin(radians) * r;

            return new CartesianCoord(x, y);
        }

        public static CartesianCoord FromPolar(PolarCoord p)
        {
            float x = Mathf.Cos(p.radians) * p.radius;
            float y = Mathf.Sin(p.radians) * p.radius;

            return new CartesianCoord(x, y);
        }
    }
}