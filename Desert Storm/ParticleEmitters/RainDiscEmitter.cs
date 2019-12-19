using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desert_Storm
{
    public class RainDiscEmitter : baseEmitter
    {
        float rainInterval;
        float height;
        protected VertexPositionColor[] particleVertices;
        float? time = null;
        float? timeLeft = null;

        public RainDiscEmitter(Game1 game, Vector3 center, float height ,  float radius, Vector3 normal, int maxParticlesPerSec, Color particleColor) : base(game, center, normal, maxParticlesPerSec, particleColor)
        {
            timer = 1f;
            rainInterval = 1;
            this.radius = radius;
            this.height = height;

            this.center.Y = height;
        } //Infinite emmiter

        public RainDiscEmitter(Game1 game, Vector3 center, float height, float radius, Vector3 normal, int maxParticlesPerSec, Color particleColor, float time) : base(game, center, normal, maxParticlesPerSec, particleColor)
        {
            timer = 1f;
            rainInterval = 1;
            this.radius = radius;
            this.height = height;

            this.center.Y = height;
            this.time = time;
            this.timeLeft = this.time;
        } //Timed emmiter

        void generateRain()
        {
            for (int i = 0; i < particlesPerSec; i++)
            {
                float randomDis = (float)game.rng.NextDouble(); //random distance 
                double randomAngle = game.rng.Next(361); //random angle from 0 to 360 (next return from 0 to max-1)
                float randomheight = (float)game.rng.NextDouble();
                Vector3 randomPos = new Vector3((float)Math.Cos(randomAngle), randomheight, (float)Math.Sin(randomAngle)); //random position inside the disc
                Vector3 ParticlePos = center + radius * randomDis * randomPos;


                randomAngle = game.rng.Next(361); //random angle from 0 to 360 (next return from 0 to max-1)
                Vector3 direction = new Vector3((float)Math.Cos(randomAngle), 0, (float)Math.Sin(randomAngle)); //random Direction

                RainParticle p = new RainParticle(ParticlePos, direction);
                particles.Add(p);
            }
            if (particlesPerSec < maxParticlesPerSec) particlesPerSec++;
            particleCount = particles.Count(); //ammou ot of particles
        }

        void updateGeometry()
        {
            int vertexCount = particleCount * 2;

            particleVertices = new VertexPositionColor[vertexCount];

            for (int i = 0, currentVertice = 0; i < particleCount; i++)
            {
                particleVertices[currentVertice] = new VertexPositionColor(particles[i].previousPos, particleColor);
                particleVertices[currentVertice + 1] = new VertexPositionColor(particles[i].position, particleColor);
                currentVertice += 2;
            }

        }

        void updateRain(GameTime gt)
        {
            foreach (var p in particles.ToArray())
            {
                if (!p.Update(gt, gravity, game)) {
                    particles.Remove(p);
                    particleCount--;
                }     //if the particle is bellow 0, it return false, removes it and decreases rain particles count           
            }

        }

        public override bool Update(GameTime gt)
        {
            timer += (float)gt.ElapsedGameTime.TotalMilliseconds; //timer for creating particles
            if (time != null)
            {
                timeLeft -= (float)gt.ElapsedGameTime.TotalSeconds; //time left in the emitter
                if (timeLeft < 0)
                {
                    shuttingDown = true;
                    if (particleCount == 0) active = false;
                }
                
            }


            if (!shuttingDown && timer >= rainInterval )
            { //Only create new particles at x time
                generateRain(); timer = 0;
            }

            updateRain(gt); //Update particle Values
            updateGeometry(); //Update particle Geometry for drawing



            if (rainInterval >= minGenInterval) //Realistic Rain start
            {
                rainInterval -= 10f;
            }

           return active;
        }

        public override void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {

            particleEffect.World = Matrix.Identity;            

            //camera
            particleEffect.View = viewMatrix;
            particleEffect.Projection = projectionMatrix;

            particleEffect.CurrentTechnique.Passes[0].Apply();

            if (particleCount > 0) device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertexData: particleVertices, vertexOffset: 0, primitiveCount: particleCount);


        }


    }
}
