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
    public abstract class BaseProjectile
    {
        protected Vector3 position;
        protected Vector3 direction;
        protected Vector3 velocity;
        protected bool exists;
        protected int ownerId;
        protected float UpdateTimer;
        protected Game1 game;
        protected float life;

        public BaseProjectile(Vector3 position, Vector3 direction, int ownerId, Game1 game)
        {
            this.game = game;
            this.position = position;
            this.direction = direction;
            this.ownerId = ownerId;

            UpdateTimer = 0;
            life = 2f;

            exists = true;
        }

        public abstract bool Update(GameTime gt);

        public abstract void Draw(Matrix viewMatrix, Matrix projectionMatrix);


        }
}
