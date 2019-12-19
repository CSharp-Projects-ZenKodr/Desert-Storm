using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Desert_Storm
{
    public class Sphere : ICollider
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public Sphere(Vector3 center, float radius)
        {
            Center = center; Radius = radius;
        }

        virtual public bool CollidesWith(Sphere other) //Sphere - Sphere collision
        {
            float dist1 = (Center - other.Center).LengthSquared();
            float dist2 = (float)Math.Pow(Radius + other.Radius, 2f);
            return dist1 <= dist2;
        }

        virtual public bool CollidesWith(OBB other) //Sphere - OBB colision
        {
            return other.CollidesWith(this);
        }

        virtual public bool CollidesWith(Segment other) //Sphere - Segment Colision
        {
            return other.CollidesWith(this);
        }
               
        public bool CollidesWith(ICollider other) //Check with what this is coliding with
        {
            ICollider collider = other.GetCollider();

            switch (collider)
            {
                case Segment seg:
                    return CollidesWith(seg as Segment);
                case OBB o:
                    return CollidesWith(o as OBB);
                case Sphere s:
                    return CollidesWith(s as Sphere);
                default:
                    return false;
            }
        }

        //bellow functions are Implemented but not used, the objects that have colliders are the ones that use them
        public string Name() { return "undef"; }

        public string SubName() { return "undef"; }

        public virtual Vector3 Position() { return Center; }

        public int? ID() { return null; }

        public bool Active() => false;

        public void CollisionWith(ICollider other) { }

        public ICollider GetCollider() => this;
    }
}
