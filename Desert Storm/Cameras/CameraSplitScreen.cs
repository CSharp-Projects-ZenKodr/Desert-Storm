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
    class CameraSplitScreen
    {
        protected Game1 game;

        public Camera3Person camera1;
        public Camera3Person camera2;

        public Viewport Screen1;
        public Viewport Screen2;

        public Matrix projectionMatrix;
        float fieldOfView;
        float aspectRatio;
        float nearPlane;
        float farPlane;



        public CameraSplitScreen(Game1 game, Camera3Person camera1, Camera3Person camera2)
        {
            Vector2 screenSize = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height);

            fieldOfView = MathHelper.ToRadians(45f);
            aspectRatio = (float)(game.GraphicsDevice.Viewport.Width/2) / game.GraphicsDevice.Viewport.Height;
            nearPlane = 1f;
            farPlane = 1000f;

            //the projection matrix is the same for both cameras
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);

            this.game = game;

            this.camera1 = camera1;
            this.camera2 = camera2;         

            //SCREEN 1 SIZE - LEFT SCREEN
            Screen1.X = 0;
            Screen1.Y = 0;
            Screen1.Width = (int)screenSize.X;
            Screen1.Height = (int)screenSize.Y;
            Screen1.MinDepth = 0;
            Screen1.MaxDepth = 1;

            //SCREEN 2 SIZE - RIGHT SCREEN
            Screen2.X = (int)screenSize.X;
            Screen2.Y = 0;
            Screen2.Width = (int)screenSize.X;
            Screen2.Height = (int)screenSize.Y;
            Screen2.MinDepth = 0;
            Screen2.MaxDepth = 1;
        }




    }
}
