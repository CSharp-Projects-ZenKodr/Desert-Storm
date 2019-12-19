using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desert_Storm
{
    public class DustLineEmitter : baseEmitter
    {
        Vector3 start, end;

        VertexPositionColor[] lineVertices;
        BasicEffect LineEffect; 
        Vector3 lineDirection; //Direction for building the line
        float scalar; //particle emiter scalar
        Vector3 divergence; //Optional divergence on the emitter's center (in case of the tank, itsso that it goes to the bottom of the wheel)
        Vector3 particleDirection; //Particles' direction

        protected VertexPositionColor[] particleVertices;
        int particleVertexCount;
        

        public DustLineEmitter(Game1 game, Vector3 center, Vector3 lineDirection, float scalar, Vector3 normal, int maxParticlesPerSec, Color particleColor, Vector3 particleDirection) : base(game, center, normal, maxParticlesPerSec, particleColor)
        {
            this.particleDirection = particleDirection;
            this.scalar = scalar;
            this.lineDirection = lineDirection;
            this.lineDirection *= this.scalar;

             divergence = Vector3.Zero;  //Optional divergence on the emitter's center (in case of the tank, its so that it goes to the bottom of the wheel)

            this.center = center; //middle of the line
            this.center += divergence;
            start = this.center - this.lineDirection;    //Start of the line    

            end = start + this.lineDirection; //end of the line

            this.maxParticlesPerSec = maxParticlesPerSec;
            this.particlesPerSec = this.maxParticlesPerSec;

            LineEffect = new BasicEffect(device);
            LineEffect.VertexColorEnabled = true;

            LineCreateGeometry();
        }

        public bool Update(GameTime gt, Vector3 center, Vector3 LineDirection, Vector3 particleDirection ,  Vector3? optional_divergence = null , bool creating = true)
        {
            this.particleDirection = particleDirection;
            if (optional_divergence == null) divergence = Vector3.Zero;
            else divergence = (Vector3)optional_divergence;

            if (creating) createParticle();

            LineUpdatePosition(center, LineDirection);


            updateParticle(gt);

            particleUpdateGeometry();

            return active;
        }

        public override bool Update(GameTime gt)
        {
            throw new NotImplementedException();
        }

        void createParticle()
        {
            for (int i = 0; i < particlesPerSec; i++)
            {
                Vector3 addedVector = end - start;
                Vector3 pPos = this.start;

                float randomAngle = game.rng.Next(360); //random angle from 0 to 360 (next return from 0 to max-1)
                Vector3 pDirection =  new Vector3((float)Math.Cos(randomAngle), 0, (float)Math.Sin(randomAngle)); //random Direction; this.particleDirection +
                pDirection.Normalize();

                pPos += addedVector * (float)game.rng.NextDouble();

                DustParticle p = new DustParticle(pPos, pDirection);
                particles.Add(p);
            }
            particleCount = particles.Count(); //ammouot of particles
        }



        public override void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            particleEffect.World = Matrix.Identity;

            //camera
            particleEffect.View = viewMatrix;
            particleEffect.Projection = projectionMatrix;

            // Indica o efeito para desenhar os eixos
            particleEffect.CurrentTechnique.Passes[0].Apply();

            if (particleCount > 0)
            {
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, particleVertices, 0, particleCount);
            }

            if (game.debug) device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, lineVertices, 0, 1);
        }

        void particleUpdateGeometry()
        {
            particleVertexCount = particleCount * 2;

            particleVertices = new VertexPositionColor[particleVertexCount];

            for (int i = 0, currentVertice = 0; i < particleCount; i++)
            {
                particleVertices[currentVertice] = new VertexPositionColor(particles[i].previousPos, particleColor);
                particleVertices[currentVertice + 1] = new VertexPositionColor(particles[i].position, particleColor);
                currentVertice += 2;
            }
        }

        void updateParticle(GameTime gt)
        {
            foreach (var p in particles.ToArray())
            {
                if (!p.Update(gt, gravity, game)) {
                    particles.Remove(p);
                    particleCount--;
                }
            }
        }


        //---------------------------------------EMITTER REPRESENTATION--------------------------//
        void LineCreateGeometry()
        {
            int vertexCount = 2;
            lineVertices = new VertexPositionColor[vertexCount];

            lineVertices[0] = new VertexPositionColor(start, Color.Yellow);
            lineVertices[1] = new VertexPositionColor(end, Color.Yellow);
        }

        void LineUpdateGeometry()
        {
            lineVertices[0] = new VertexPositionColor(start, Color.Yellow);
            lineVertices[1] = new VertexPositionColor(end, Color.Yellow);
        }

        public void LineUpdatePosition(Vector3 center, Vector3 direction)
        {
            this.center = center;
            this.center += divergence;
            this.lineDirection = direction;
            this.lineDirection *= scalar;
            start = this.center - this.lineDirection;

            end = start + this.lineDirection;

            LineUpdateGeometry();
        }
    }
}
