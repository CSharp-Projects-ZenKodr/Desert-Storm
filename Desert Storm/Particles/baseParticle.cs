using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desert_Storm
{
    public abstract class baseParticle
    {
        public Vector3 position;
        protected Vector3 velocity;
        protected Vector3 direction;
        public Vector3 previousPos;

        public baseParticle(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
            this.velocity = Vector3.Zero;
        }

        public abstract bool Update(GameTime gt, Vector3 gravity, Game1 game);



    }
}
