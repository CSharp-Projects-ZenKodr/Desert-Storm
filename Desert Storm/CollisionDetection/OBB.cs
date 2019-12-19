using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Desert_Storm //NOT USED
{
    public class OBB : Sphere, ICollider //NOT USED
    {
        /*
         * 
         * 
         * 
         * NOT USED
         * 
         * 
         * 
         */
        const float EPSILON = 0.001f;
        public Vector3[] Axis { get; private set; }
        public float[] Extend { get; private set; }
        float rotation;

        public OBB(Vector3 center, Vector3 size, float rotation)
                : base(center, size.Length() / 2f)
        {
            Extend = new float[] { size.X / 2f, size.Y / 2f };
            this.rotation = rotation;
            updateAxis();

        }

        private void updateAxis()
        {
            //Axis = new Vector2[] {
            //    new Vector2((float) Math.Sin(Math.PI/2f + rotation),
            //                (float) Math.Cos(Math.PI/2f + rotation)),
            //    new Vector2((float) Math.Sin(rotation),
            //                (float) Math.Cos(rotation))
            //};
        }

        public void Rotate(float angle)
        {
            rotation += angle;
            updateAxis();
        }

        public Vector3 ClosestPoint(Vector3 p)
        {
            Vector3 d = p - this.Center;
            Vector3 q = new Vector3(this.Center.X, this.Center.Y, this.Center.Y);
            for (int i = 0; i < 2; ++i)
            {
                float dist = Vector3.Dot(d, this.Axis[i]);
                if (dist > this.Extend[i]) dist = this.Extend[i];
                if (dist < -this.Extend[i]) dist = -this.Extend[i];  // THERE WAS A MISSING MINUS
                q += dist * this.Axis[i];
            }
            return q;
        }

        override public bool CollidesWith(Sphere c)
        {
            if (base.CollidesWith(c))
            {
                Vector3 p = this.ClosestPoint(c.Center);
                Vector3 v = p - c.Center;
                return Vector3.Dot(v, v) <= c.Radius * c.Radius;
            }
            else
            {
                return false;
            }
        }

        override public bool CollidesWith(OBB other)
        {
            if (base.CollidesWith(other as Sphere))
            {
                float ra, rb;
                float[,] R = new float[2, 2], AbsR = new float[2, 2];

                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                        R[i, j] = Vector3.Dot(this.Axis[i], other.Axis[j]);

                // Compute translation vector t
                Vector3 t = other.Center - this.Center;
                // Bring translation into a's coordinate frame
                ///t = new Vector3(Vector3.Dot(t, this.Axis[0]), Vector3.Dot(t, this.Axis[1]));

                // Compute common subexpressions. Add in an epsilon term to
                // counteract arithmetic errors when two edges are parallel and
                // their cross product is (near) null
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                        AbsR[i, j] = Math.Abs(R[i, j]) + EPSILON;

                // Test axes L = A0,  = A1
                for (int i = 0; i < 2; i++)
                {
                    ra = this.Extend[i];
                    rb = other.Extend[0] * AbsR[i, 0] + other.Extend[1] * AbsR[i, 1];
                    ///if (Math.Abs(t.Pos(i)) > ra + rb) return false;
                }

                // Test axes L = B0, L = B1
                for (int i = 0; i < 2; i++)
                {
                    ra = this.Extend[0] * AbsR[0, i] + this.Extend[1] * AbsR[1, i];
                    rb = other.Extend[i];
                    ///if (Math.Abs(t.Pos(0) * R[0, i] + t.Pos(1) * R[1, i]) > ra + rb) return false;
                }

                // Test axis L = A0 x B0 - Not needed for 2D (?)
                // Test axis L = A0 x B1 - Not needed for 2D (?)
                // Test axis L = A0 x B2 - Not needed for 2D (?)
                // Test axis L = A1 x B0 - Not needed for 2D (?)
                // Test axis L = A1 x B1 - Not needed for 2D (?)
                // Test axis L = A1 x B2 - Not needed for 2D (?)
                // Test axis L = A2 x B0 - Not needed for 2D (?)
                // Test axis L = A2 x B1 - Not needed for 2D (?)
                // Test axis L = A2 x B2 - Not needed for 2D (?)

                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
