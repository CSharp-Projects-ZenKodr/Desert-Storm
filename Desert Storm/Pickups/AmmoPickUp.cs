using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desert_Storm
{
    public class AmmoPickup : BasePickup //Pickups are supposed to be ammo/health that the tanks can pickUp
    {
        public AmmoPickup(Game1 game) : base(game)
        {
            model = game.Content.Load<Model>("Ammo/Ammo");

            position = new Vector3(game.map.size.X / 2, 50, game.map.size.Y / 2);

            //position = Vector3.Zero;

            //terrainHeight = game.map.getHeight(position.X, position.Z);
            //position.Y = terrainHeight + HoverHeight;

     ;

            boneTransform = new Matrix[model.Bones.Count];


            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.Enabled = true;
                    //effect.DiffuseColor = game.LightManager.diffuseColor; //kd
                    //effect.SpecularColor = game.LightManager.specularColor; //Specular Light  //ks
                    //effect.SpecularPower = game.LightManager.specularPower; //Power of Specular Light //s
                    //effect.AmbientLightColor = game.LightManager.ambientLightColor; //Ia
                    //effect.DirectionalLight0.DiffuseColor = game.LightManager.directionalLightDiffuseColor; //Directional Light's Color //Id
                    //effect.DirectionalLight0.Direction = game.LightManager.directionalLightDirection; //Directional light's Direction 
                    //effect.DirectionalLight0.SpecularColor = game.LightManager.directionalLightSpecularColor; //Is
                    effect.EnableDefaultLighting();
                }
            }
        }

        public override void Update(GameTime gt) //The pickups Jump up and down
        {
            

            MovVector = Vector3.Zero;

            MovVector = game.Gravity(position, MovVector, HoverHeight);

            yaw += MathHelper.ToRadians(2);

            if (position.Y - HoverHeight < game.map.getHeight(position.X, position.Z)) { jumpVector = JumpStart(jumpsLeft); jumpsLeft--; jumping = true; }
            if (jumping)
            {
                jumpVector = Jump(jumpVector);
                MovVector += jumpVector;
                if (jumpVector == Vector3.Zero) jumping = false;
            }

            base.Update(gt);
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {


            base.draw(viewMatrix, projectionMatrix);
           

        }

        


    }
}
