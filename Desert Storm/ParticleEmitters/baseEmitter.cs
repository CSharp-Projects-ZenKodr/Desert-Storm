using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desert_Storm
{
    public abstract class baseEmitter
    {
        protected Game1 game;
        protected Vector3 center;
        protected float radius;

        protected List<baseParticle> particles;
        protected Vector3 normal;

        protected int maxParticlesPerSec;
        protected int particlesPerSec;

        protected Vector3 gravity;

        
        protected BasicEffect particleEffect;
        protected GraphicsDevice device;

        protected float timer;
        protected float genInterval;
        protected const float minGenInterval = 80f; //80 milliseconds is the minimum interval between partile generation

        protected Color particleColor;
        protected bool active;
        protected int particleCount;
        protected bool shuttingDown;

        public baseEmitter(Game1 game, Vector3 center, Vector3 normal, int maxParticlesPerSec, Color particleColor)
        {
            this.game = game;
            this.center = center; //emiter's center
            this.normal = normal;
            gravity = game.gravity; //game's gravity

            particleCount = 0; //ammount of particles that the emitor has emited

            this.particleColor = particleColor; //particle's color

            this.maxParticlesPerSec = maxParticlesPerSec; //the max particles per second

            particles = new List<baseParticle>(); //particle's List

            device = game.GraphicsDevice;
            particleEffect = new BasicEffect(device);

            particleEffect.VertexColorEnabled = true;

            active = true;
            shuttingDown = false;
        }

        public abstract bool Update(GameTime gt);

        public abstract void Draw(Matrix viewMatrix, Matrix projectionMatrix);

    }
}
