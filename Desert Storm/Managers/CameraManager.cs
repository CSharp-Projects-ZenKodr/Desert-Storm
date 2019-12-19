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
    class CameraManager
    {
        FreeCamera freeCamera;
        CameraSurfaceFollow cameraSurfaceFollow;
        Camera3Person cameraTank1;
        Camera3Person cameraTank2;

        public Matrix ActiveView, ActiveProjection;
        public Matrix ActiveView2, ActiveProjection2;

        CameraBirdsEye cameraBirdsEye;
        public int actCam;

        public bool splitScreen;

        List<BaseCam> listCam;

        Game1 game;

        public CameraSplitScreen cameraSplitScreen;

        public CameraManager(Game1 game)
        {
            this.game = game;

            actCam = 3;
            splitScreen = true;
            
            listCam = new List<BaseCam>();

            freeCamera = new FreeCamera(game);
            cameraSurfaceFollow = new CameraSurfaceFollow(game);                           

            cameraBirdsEye = new CameraBirdsEye(game);

            cameraTank1 = new Camera3Person(game, game.tank1);
            cameraTank2 = new Camera3Person(game, game.tank2);

            listCam.Add(freeCamera);
            listCam.Add(cameraSurfaceFollow);
            listCam.Add(cameraBirdsEye);
            listCam.Add(cameraTank1);
            listCam.Add(cameraTank2);

            cameraSplitScreen = new CameraSplitScreen(game, cameraTank1, cameraTank2);

            ActiveView = freeCamera.viewMatrix;
            ActiveProjection = freeCamera.projectionMatrix;
        }

        public void Update(GameTime gameTime, KeyboardState kb, MouseState mouse)
        {
            if (game.IsActive)
            {
                ActiveCam(gameTime, kb, mouse);
            }
        }

        private void ActiveCam(GameTime gameTime, KeyboardState kb, MouseState mouse)
        {

            if (kb.IsKeyDown(Keys.F1)) //Free Camera
            {
                splitScreen = false;
                actCam = 0;
            }
            else if (kb.IsKeyDown(Keys.F2)) //SurfaceFollow
            {
                splitScreen = false;
                actCam = 1;
            }
            else if (kb.IsKeyDown(Keys.F3)) //
            {
                splitScreen = false;
                actCam = 2;
            }
            else if (kb.IsKeyDown(Keys.F4))
            {
                splitScreen = false;
                actCam = 3;
            }
            else if (kb.IsKeyDown(Keys.F5))
            {
                splitScreen = false;
                actCam = 4;
            }
            else if (kb.IsKeyDown(Keys.F6))
            {
                splitScreen = true;
            }

            if (!splitScreen)
            {
                ActiveView = listCam[actCam].viewMatrix;
                ActiveProjection = listCam[actCam].projectionMatrix;

                listCam[actCam].Update(kb, mouse, gameTime);
            }

            else
            {
                ActiveView = cameraSplitScreen.camera1.viewMatrix;
                ActiveProjection = cameraSplitScreen.projectionMatrix;

                ActiveView2 = cameraSplitScreen.camera2.viewMatrix;
                ActiveProjection2 = cameraSplitScreen.projectionMatrix;

                cameraSplitScreen.camera1.Update(kb, mouse, gameTime);
                cameraSplitScreen.camera2.Update(kb, mouse, gameTime);


            } 

        }



    }
}
