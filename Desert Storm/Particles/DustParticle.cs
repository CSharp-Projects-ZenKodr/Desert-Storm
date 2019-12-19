using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Desert_Storm
{
    public class DustParticle : baseParticle
    {
        float life;
        Vector3 launchForce;

        public DustParticle(Vector3 position, Vector3 direction) : base(position, direction)
        {
            life = 0f;
            this.direction = direction;

            previousPos = position;
            launchForce = Vector3.Up * (15); //Force with which the particle is launched
        }  

        public override bool Update(GameTime gt, Vector3 gravity, Game1 game)
        {
            previousPos = position;
            life += (float)gt.ElapsedGameTime.TotalSeconds;
            velocity += launchForce * (float)gt.ElapsedGameTime.TotalSeconds; //Velocity of the particle
            launchForce += Vector3.Down; //Launch force is decreased
            position += direction * (float)gt.ElapsedGameTime.TotalSeconds;
            position += velocity * (float)gt.ElapsedGameTime.TotalSeconds;

            return ((position.Y > 0) && (life < 0.8f));
        }
    }
}
