using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desert_Storm
{
    public class CameraBirdsEye : BaseCam
    {
        public CameraBirdsEye(Game1 game) : base(game) //camera that sees the whole map from above
        {
            this.position = new Vector3(-10, game.map.skybox.height, game.map.size.Y / 2);
            yaw = MathHelper.ToRadians(-90f);
            speed = 10;
        }

        public override void Update(KeyboardState kb, MouseState mouse, GameTime gameTime)
        {
            //base.Update(kb, mouse, gameTime);
            pitch = -1f;

            Vector3 dirDefault = new Vector3(0f, 0f, -1f);


            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);


            direction = Vector3.Transform(dirDefault, cameraRotation);

            target = position + direction;

            right = Vector3.Cross(direction, Vector3.Up);

            normal = Vector3.Cross(right, direction);
            normal.Normalize();

            viewMatrix = Matrix.CreateLookAt(position, target, normal);


            Mouse.SetPosition((int)center.X, (int)center.Y);
        }
    }
}
