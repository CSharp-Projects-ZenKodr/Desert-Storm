using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Desert_Storm
{
    public class RainParticle : baseParticle
    {

        public RainParticle(Vector3 position, Vector3 direction) : base(position, direction)
        {
            previousPos = position;
        }

        public override bool Update(GameTime gt, Vector3 gravity, Game1 game)
        {
            previousPos = position;
            velocity += gravity * (float)gt.ElapsedGameTime.TotalSeconds;
            position += direction * (float)gt.ElapsedGameTime.TotalSeconds;
            position += velocity * (float)gt.ElapsedGameTime.TotalSeconds;

            return position.Y > game.map.getHeight(position.X, position.Z);
        }
    }
}
