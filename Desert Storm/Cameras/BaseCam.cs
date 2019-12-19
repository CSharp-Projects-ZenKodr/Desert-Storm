using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desert_Storm
{
   public class BaseCam
    {
        protected Game1 game;
        public Matrix viewMatrix;
        public Vector3 position;
        protected float yaw;
        protected float pitch;
        protected Vector3 normal;

        //projection variables
        public Matrix projectionMatrix;
        float fieldOfView;
        float aspectRatio;
        float nearPlane;
        float farPlane;
        protected int speed;
        protected Vector3 direction;
        protected Vector3 right;
        protected Vector3 target;

        protected Vector2 center;

        //public bool active;

        public BaseCam(Game1 game)
        {

            this.game = game;
            center = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
            Mouse.SetPosition((int)center.X, (int)center.Y); //puts the mouse on the middle of the screen

            
            yaw = MathHelper.ToRadians(-120f);
            pitch = 0;
            Vector3 dirDefault = new Vector3(0f, 0f, -1f);
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Vector3 direction = Vector3.Transform(dirDefault, cameraRotation);

            Vector3 target = position + direction;

            normal = Vector3.Up;

            viewMatrix = Matrix.CreateLookAt(position, target, normal);

            fieldOfView = MathHelper.ToRadians(45f);
            aspectRatio = (float)game.GraphicsDevice.Viewport.Width / game.GraphicsDevice.Viewport.Height;
            nearPlane = 1f;
            farPlane = 1000f;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
        }

        public virtual void Update(KeyboardState kb, MouseState mouse, GameTime gameTime)
        {
                float deltaX = mouse.X - center.X;
                float deltaY = mouse.Y - center.Y;
                

                yaw += -(deltaX) * MathHelper.ToRadians(5f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

                pitch -= -(deltaY) * MathHelper.ToRadians(5f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (pitch < -1) pitch = -1;
                else if (pitch > 1) pitch = 1;

                Vector3 dirDefault = new Vector3(0f, 0f, -1f);
                Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
                direction = Vector3.Transform(dirDefault, cameraRotation);

                right = Vector3.Cross(direction, Vector3.Up);

                Vector3 target = position + direction;

            normal = Vector3.Cross(right, direction);
            normal.Normalize();

            viewMatrix = Matrix.CreateLookAt(position, target, normal);


            Mouse.SetPosition((int)center.X, (int)center.Y); //locks the mouse in the center

                //camera Movement
                if (kb.IsKeyDown(Keys.NumPad8)) position += (direction * speed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad5)) position -= (direction * speed) * (float)gameTime.ElapsedGameTime.TotalSeconds;


                Vector3 strafeRight = Vector3.Cross(direction, Vector3.Up);
                strafeRight.Normalize();

                if (kb.IsKeyDown(Keys.NumPad6)) position += strafeRight * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (kb.IsKeyDown(Keys.NumPad4)) position -= strafeRight * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

        }



    }
}
