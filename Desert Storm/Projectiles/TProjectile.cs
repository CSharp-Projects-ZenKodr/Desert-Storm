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
    class TProjectile : BaseProjectile,ICollider
    {
        //model stuff
        Model myModel;
        float scale;

        //model tranformation stuff
        Matrix rotation;
        Matrix[] boneTransform;

        //moving stuff
        float pitch;

        Segment collider;
        Matrix rotacao;


        public TProjectile(Vector3 position, Vector3 direction, int ownerId, Game1 game, Model m) : base(position, direction, ownerId, game)
        {
            myModel = m; //Bullet's Model
            pitch = 0f; //Pitch and rotation so that the bullet spins arround itself, purelly aesthetic
            rotation = Matrix.CreateFromYawPitchRoll(pitch, 0f, 0f);

            scale = 0.0015f; //Scale of the bullet

            boneTransform = new Matrix[myModel.Bones.Count];
            rotacao = Matrix.Identity;
            velocity = -direction * 80;

            collider = new Segment(game , position, direction, Vector3.Zero, 0);
        }

        public override bool Update(GameTime gt)
        {
            UpdateTimer += (float)gt.ElapsedGameTime.TotalMilliseconds;
            life -= (float)gt.ElapsedGameTime.TotalSeconds;
            if (life <= 0) exists = false;

            pitch += MathHelper.ToRadians(20);
            rotation = Matrix.CreateFromYawPitchRoll(0f, 0f, pitch);

            //Movement
            velocity += (direction + game.gravity) * (float)gt.ElapsedGameTime.TotalSeconds;
            position += velocity * (float)gt.ElapsedGameTime.TotalSeconds;
            ///Movement

            rotation.Forward = -direction;

            Matrix translacao = Matrix.CreateTranslation(position);
            Matrix escala = Matrix.CreateScale(scale);
            myModel.Root.Transform = escala * rotation * translacao;

            myModel.CopyAbsoluteBoneTransformsTo(boneTransform);

            if (UpdateTimer > 10)
                {
                    collider.UpdatePosition(position);
                    UpdateTimer = 0;
                }

            if (collider.start.X < game.map.size.X - 1 && collider.start.X > 0 && collider.start.Z < game.map.size.Y - 1 && collider.start.Z > 0)
            { //Checks if Segment position is inside the map
                if (collider.start.Y < 0 || collider.start.Y < game.map.getHeight(collider.start.X, collider.start.Z)) exists = false;
            }

            return exists;
        }

        public override void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
                foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransform[mesh.ParentBone.Index];
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }

            if (game.debug) collider.Draw(viewMatrix, projectionMatrix);
        }

        public string Name() => "Projectile";

        public string SubName() => "TProjectile";

        public int? ID() => ownerId;

        public Vector3 Position() => position;

        public bool Active() => exists;

        public void CollisionWith(ICollider other)
        {
            if (other.ID() != ownerId && other.Name() == "Tank")
            {
                //Console.WriteLine("projectile colided with a tank");
                exists = false;
            }
        }

        public bool CollidesWith(ICollider other) => collider.CollidesWith(other);

        public ICollider GetCollider() => collider;
    }
}
