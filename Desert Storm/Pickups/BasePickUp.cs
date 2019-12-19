using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Desert_Storm
{
    public class BasePickup //Pickups are supposed to be ammo/health that the tanks can pickUp
    {
        protected Model model;
        protected Matrix world;
        protected Game1 game;

        protected Vector3 position;
        protected Vector3 direction, dirDefault, normal, right;

        protected Matrix rotation;

        protected Matrix[] boneTransform;

        protected Matrix rotacao;

        protected float yaw;
        protected float pitch;
        protected float scale;

        protected float HoverHeight;
        protected int jumpsLeft;
        protected Vector3 jumpVector;

        protected Vector3 MovVector; //movement vector
        protected bool jumping;

        public BasePickup(Game1 game)
        {
 
            this.game = game;
            world = Matrix.Identity;

            yaw = 0;
            pitch = 0;
            rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0f);
            normal = Vector3.Up;

            dirDefault = new Vector3(0f, 0f, -1f);

            scale = 0.0025f;
            HoverHeight = 2f;

            rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0f);
            direction = Vector3.Transform(dirDefault, rotation);

            right = Vector3.Cross(direction, normal);

            rotacao = Matrix.Identity;
            rotacao.Forward = direction;
            rotacao.Up = normal;
            rotacao.Right = right;

            MovVector = Vector3.Zero;
            jumpsLeft = 3;
            jumpVector = Vector3.Zero;


        }

        public virtual void Update(GameTime gt)
        {

            position += MovVector * (float)gt.ElapsedGameTime.TotalSeconds;

            rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0f);
            direction = Vector3.Transform(dirDefault, rotation);

            right = Vector3.Cross(direction, normal);


            Matrix rotacao = Matrix.Identity;
            rotacao = Matrix.Identity;
            rotacao.Forward = direction;
            rotacao.Up = normal;
            rotacao.Right = right;


            Matrix translacao = Matrix.CreateTranslation(position);
            Matrix escala = Matrix.CreateScale(scale);


            model.Root.Transform = escala * rotacao * translacao;


            model.CopyAbsoluteBoneTransformsTo(boneTransform);


        }

        public virtual void draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransform[mesh.ParentBone.Index];
                    //effect.World = Matrix.CreateTranslation(position);
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;

                    effect.DirectionalLight0.DiffuseColor = game.LightManager.directionalLightDiffuseColor; //Directional Light's Color //Id
                    effect.DirectionalLight0.Direction = game.LightManager.directionalLightDirection; //Directional light's Direction 
                }
                mesh.Draw();
            }
        }

        protected Vector3 JumpStart(int jumpsLeft)
        {
            Vector3 jumpVector = -game.GravityForce * 1.5f;

            return jumpVector;
        }

        protected Vector3 Jump(Vector3 jumpVector)
        {
            return jumpVector * 0.995f;
        }




    }
}
