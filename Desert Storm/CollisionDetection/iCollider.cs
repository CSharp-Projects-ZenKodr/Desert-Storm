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
    public interface ICollider
    {
        //obtain name of object with collider
        string Name();
        //obtain name of object with collider
        string SubName();
        //Obtain ID of object with Collider
        int? ID();
        //Obtain object's position;
        Vector3 Position();
        //Obtain if Object's active
        bool Active();
        //notify object of collision
        void CollisionWith(ICollider other);
        // validate collisions (exists/doesn't exists)
        bool CollidesWith(ICollider other);
        //retorna o collider do objecto (circle ou OBB)
        ICollider GetCollider();
    }
}
