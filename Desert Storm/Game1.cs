using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Desert_Storm
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public LandScape map;
        cls3dAxis centerAxis;

       CameraManager cameraManager;

        public LightingManager LightManager;

        public Tank tank1;
        public Tank tank2;

        Keys[] TankControlsKeyboard1;
        Keys[] TankControlsKeyboard2;

        PlayerIndex pad1 = PlayerIndex.One;
        PlayerIndex pad2 = PlayerIndex.Two;
        
        //AmmoPickup ammoPickUp;
        public Vector3 GravityForce;
        public List<ICollider> colliders;
        public ProjectileManager projectileManager;
        public WeatherManager weather;

        public Vector3 gravity = Vector3.Down * 9.78033f; //Global Gravity force

        public Random rng = new Random();

        Vector3[] spawn;

        SpriteFont font;
        float fps = 0;
        public bool debug = true;
        

        public Game1()
        {
                                                          //this makes it so that every update/draw loop is evenly spaced,
            IsFixedTimeStep = true;                       //setting it to false makes the system start the next frame immidiatelly after the previous one finishes,
                                                          //setting it to true means the system will wait before starting the next one ///improves performance overall
            TargetElapsedTime = TimeSpan.FromMilliseconds(16);//this controls how fast the engine calls the update and draw functions


            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";         

        }


        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            //Tank Controls - these arrays will be loaded onto the tanks and decide which keys control them
            TankControlsKeyboard1 = new Keys[9]; 
            TankControlsKeyboard2 = new Keys[9];

            TankControlsKeyboard1[0] = Keys.W;
            TankControlsKeyboard1[1] = Keys.S;
            TankControlsKeyboard1[2] = Keys.A;
            TankControlsKeyboard1[3] = Keys.D;
            TankControlsKeyboard1[4] = Keys.Up;
            TankControlsKeyboard1[5] = Keys.Down;
            TankControlsKeyboard1[6] = Keys.Left;
            TankControlsKeyboard1[7] = Keys.Right;
            TankControlsKeyboard1[8] = Keys.Space;
            TankControlsKeyboard2[0] = Keys.I;
            TankControlsKeyboard2[1] = Keys.K;
            TankControlsKeyboard2[2] = Keys.J;
            TankControlsKeyboard2[3] = Keys.L;
            TankControlsKeyboard2[4] = Keys.Home;
            TankControlsKeyboard2[5] = Keys.End;
            TankControlsKeyboard2[6] = Keys.Delete;
            TankControlsKeyboard2[7] = Keys.PageDown;
            TankControlsKeyboard2[8] = Keys.LeftControl;
            ///Tank controls

            GravityForce = Vector3.Down * 17;

            base.Initialize();
        }

        Texture2D heightmap; 

        protected override void LoadContent()
        {
            heightmap = Content.Load<Texture2D>("3dMap");

            LightManager = new LightingManager(this, heightmap);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");

            centerAxis = new cls3dAxis(this);
            map = new LandScape(this, heightmap);

            spawn = new Vector3[4];
            spawn[0] = new Vector3(15, 0, 15);
            spawn[1] = new Vector3(map.size.X - 15, 0, 15);
            spawn[2] = new Vector3(15, 0, map.size.Y - 15);
            spawn[3] = new Vector3(map.size.X - 15, 0, map.size.Y - 15);

            colliders = new List<ICollider>(); //Collider list
            projectileManager = new ProjectileManager(this);

            weather = new WeatherManager(this); //Weather Manager

            tank1 = new Tank(this, Content.Load<Model>("tank/tank"), spawn[0], TankControlsKeyboard1, 1, Content.Load<Model>("TankShell/TankShell"), false, pad2, 0f);
            tank2 = new Tank(this, Content.Load<Model>("tank/tank"), spawn[3], TankControlsKeyboard2, 2, Content.Load<Model>("TankShell/TankShell"), debug, pad1, 180f);

            cameraManager = new CameraManager(this);

            //ammoPickUp = new AmmoPickup(this);        
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            fps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            cameraManager.Update(gameTime, kb, mouse);

            LightManager.Update(gameTime);
            weather.Update(gameTime);

            tank1.Update(gameTime, kb, mouse);
            tank2.Update(gameTime, kb, mouse);

            projectileManager.Update(gameTime);

            //ammoPickUp.Update(gameTime);

            //Checks if anything collides
            for (int i = 0; i < colliders.Count - 1; i++) //Collision
            {
                for (int j = 1; j < colliders.Count; j++) //Collision
                {
                    if (colliders[i].CollidesWith(colliders[j]))
                    {
                        colliders[i].CollisionWith(colliders[j]);
                        colliders[j].CollisionWith(colliders[i]);
                    }
                }
            }


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (!cameraManager.splitScreen)
            {
                drawWorld(cameraManager.ActiveView, cameraManager.ActiveProjection);
                DrawText();//Draws fps
                if (cameraManager.actCam == 3) DrawHealth(tank1);
                else if (cameraManager.actCam == 4) DrawHealth(tank2);
            } else
            {
                Viewport original = graphics.GraphicsDevice.Viewport;      //save the Original view port    
                
                graphics.GraphicsDevice.Viewport = cameraManager.cameraSplitScreen.Screen1; //Create the first view
                drawWorld(cameraManager.ActiveView, cameraManager.ActiveProjection);
                DrawHealthSplitScreen(tank1, cameraManager.cameraSplitScreen.Screen1);

                graphics.GraphicsDevice.Viewport = cameraManager.cameraSplitScreen.Screen2; //Create the second view
                drawWorld(cameraManager.ActiveView2, cameraManager.ActiveProjection2);
                DrawHealthSplitScreen(tank2, cameraManager.cameraSplitScreen.Screen2);


                GraphicsDevice.Viewport = original; //return toe original view port

                DrawText(); //draws Fps
            }

            base.Draw(gameTime);
        }

        protected void drawWorld(Matrix ActiveView, Matrix ActiveProjection) //Draws the world
        {

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            map.Draw(ActiveView, ActiveProjection);

            centerAxis.Draw(ActiveView, ActiveProjection);

            tank1.Draw(ActiveView, ActiveProjection);
            tank2.Draw(ActiveView, ActiveProjection);

            projectileManager.Draw(ActiveView, ActiveProjection);
            weather.draw(ActiveView, ActiveProjection);

            //ammoPickUp.Draw(ActiveView, ActiveProjection);
        }

        //-------------------------------------------------HUD--------------------------------------------//
        void DrawText() //Draws Frames per second
        {
            spriteBatch.Begin();
            string text = string.Format("Fps: {00}", fps);
            Vector2 Pos = new Vector2(10f, 10f);
            spriteBatch.DrawString(font, text, Pos, Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        void DrawHealthSplitScreen(Tank tank, Viewport port) //Draws Health In Split screen
        {
            spriteBatch.Begin();
            string text = string.Format("Health: {00}", tank.getHealth());
            Vector2 Pos = new Vector2(20, port.Height - 50f);
            spriteBatch.DrawString(font, text, Pos, Color.Red, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        void DrawHealth(Tank tank) //Draws Health in single Screen
        {
            spriteBatch.Begin();
            string text = string.Format("Health: {00}", tank.getHealth());
            Vector2 Pos = new Vector2(20 , graphics.PreferredBackBufferHeight - 50f);
            spriteBatch.DrawString(font, text, Pos, Color.Red, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
        ///-----------------------------------------------------END HUD--------------------------------------///

        public Vector3 Gravity(Vector3 position, Vector3 movVector, float hoverHeight, float resistence = 1) //Function made for Pickups
        { //Not used
            if (resistence > 1) resistence = 1;
            else if (resistence < 0) resistence = 0;

            float terrainHeight = map.getHeight(position.X, position.Z);
            
            if ((position.Y - hoverHeight) > terrainHeight) movVector += GravityForce * resistence;
            
            return movVector;
        }
       

        public Vector3 Check_Furthest_Spawn(Vector3 deathPos) //Checks which Spawn is the furthest from the Object's Death position
        {
            float dist = 0;
            float max = 0;
            int furthests = 0;

            for (int i = 0; i < spawn.Length; i++)
            {
                dist = (deathPos - spawn[i]).Length();
                if (dist >= max)
                {
                    max = dist;
                    furthests = i;
                }
            }
            return spawn[furthests];
        }


    }
}
