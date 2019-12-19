using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desert_Storm
{
    public class Tank : ICollider
    {
        Model myModel;
        Model shotModel;
        Matrix world;
        Game1 game;

        public enum AImode
        {
            WANDER,
            PERSUIT
        }
        public AImode aiMode;

        public Vector3 position, direction, dirDefault, terrainNormal, trueDirection, right;
        float scale, speed;
        //BONES
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone rightWheel;
        ModelBone leftWheel;
        ModelBone rightBackWheel;
        ModelBone leftBackWheel;
        ModelBone hatch;

        Matrix cannonTransform, turretTransform, rightWheelTransform, rightBackWheelTransform, leftWheelTransform, leftBackWheelTransform, hatchTransform;
        Matrix rotation;
        Matrix turretRotation;
        public Vector3 turretDirection;

        //GUN VARIABLES
        Matrix GunRotation;
        Vector3 GunDirection;

        //ANGLES
        float turretAngle;
        float cannonAngle;
        float rightWheelAngle;
        float leftWheelAngle;
        float rightBackWheelAngle;
        float leftBackWheelAngle;
        float hatchAngle;
        public float yaw;

        Matrix[] boneTransform;
        Keys[] Controls;
        PlayerIndex pad;

        GamePadState gamepad;
        GamePadCapabilities capabilities;
        Sphere collider;
        bool Tankcoliding;
        bool Hit;
        float hittime;
        public int id;

        Vector3 velocity;
        Vector3 blockingDirection;

        bool reloading;
        float reloadTimer;

        public Matrix rotacao;
        KeyboardState kbPreviousState;
        GamePadState padPreviousState;

        DustLineEmitter[] dustEmitters;
        bool moving;
        public bool AIControlled;
        float idleTimer;
        BoidRadar radar;
        Vector3 defaultDirection;
        Vector3 IdealDirection;
        bool keyboard;
        int Health;
        public bool alive;

        public Tank(Game1 game, Model m, Vector3 position, Keys[] Controls, int id, Model Shot, bool ai, PlayerIndex pad, float initialYaw)
        {
            this.game = game;
            myModel = m;
            this.shotModel = Shot;
            this.id = id;
            world = Matrix.Identity;

            this.Controls = Controls;
            this.pad = pad;

            dirDefault = new Vector3(0f, 0f, -1f); //default direction
            yaw = initialYaw;
            rotation = Matrix.CreateFromYawPitchRoll(yaw, 0f, 0f);
            defaultDirection = Vector3.Transform(dirDefault, rotation);
            direction = Vector3.Transform(dirDefault, rotation);
            this.position = position;

            terrainNormal = game.map.getNormal(position.X, position.Z); //current position's terrain normal
            right = Vector3.Cross(direction, terrainNormal); //tank's Right
            trueDirection = Vector3.Cross(right, terrainNormal); //Direction which the tank faces when he is in the terrain
            trueDirection.Normalize();

            scale = 0.005f; //Model's SCalling
            speed = 5f; //Tank's default Speed
            velocity = Vector3.Zero;
            blockingDirection = Vector3.Zero;

            moving = false;

            #region Bones Creation
            //Bones Variables Creation
            turretBone = myModel.Bones["turret_geo"];
            cannonBone = myModel.Bones["canon_geo"];
            rightWheel = myModel.Bones["r_front_wheel_geo"];
            leftWheel = myModel.Bones["l_front_wheel_geo"];
            rightBackWheel = myModel.Bones["r_back_wheel_geo"];
            leftBackWheel = myModel.Bones["l_back_wheel_geo"];
            hatch = myModel.Bones["hatch_geo"];
            #endregion

            #region Bones Transforms
            //bones Transforms
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            rightWheelTransform = rightWheel.Transform;
            leftWheelTransform = leftWheel.Transform;
            rightBackWheelTransform = rightBackWheel.Transform;
            leftBackWheelTransform = leftBackWheel.Transform;
            hatchTransform = hatch.Transform;
            #endregion

            boneTransform = new Matrix[myModel.Bones.Count];
            rotacao = Matrix.Identity;

            #region bone Angles
            //Bone Angles
            turretAngle = 0f;
            cannonAngle = 0f;
            rightWheelAngle = 0f;
            leftWheelAngle = 0f;
            rightBackWheelAngle = 0f;
            leftBackWheelAngle = 0f;
            hatchAngle = 0f;
            #endregion

            turretRotation = Matrix.CreateFromYawPitchRoll(turretAngle, 0f, 0f); //turret's rotation matrix
            turretDirection = Vector3.Transform(dirDefault, turretRotation); //turret's direction for third person camera purposes

            //COLISION
            collider = new Sphere(position, 2f); //Radius is fine tuned
            game.colliders.Add(this as ICollider); //add the tank to the collider list 
            Tankcoliding = false;
            Hit = false;
            hittime = 0;

            reloading = false;
            reloadTimer = 0;

            capabilities = GamePad.GetCapabilities(pad); //controller capabilities check
            gamepad = GamePad.GetState(pad);  //controller
            kbPreviousState = Keyboard.GetState(); //THIS SAVES THE PREVIOUS KEYBOARD STATE SO THAT SINGLE BUTTON PRESSES ARE POSSIBLE
            padPreviousState = gamepad; //THIS SAVES THE PREVIOUS GAMEPAD STATE SO THAT SINGLE BUTTON PRESSES ARE POSSIBLE
            keyboard = true;

            dustEmitters = new DustLineEmitter[4];
            Color dustColor = Color.SandyBrown;
            dustEmitters[0] = new DustLineEmitter(game, boneTransform[rightWheel.Index].Translation, rotation.Left, 0.8f, Vector3.Forward, 10, dustColor, Vector3.Cross(rotation.Backward, rotation.Up));
            dustEmitters[1] = new DustLineEmitter(game, boneTransform[leftWheel.Index].Translation, rotation.Right, 0.8f, Vector3.Forward, 10, dustColor, Vector3.Cross(rotation.Backward, rotation.Up));
            dustEmitters[2] = new DustLineEmitter(game, boneTransform[rightBackWheel.Index].Translation, rotation.Right, 1f, Vector3.Forward, 10, dustColor, Vector3.Cross(rotation.Backward, rotation.Up));
            dustEmitters[3] = new DustLineEmitter(game, boneTransform[leftBackWheel.Index].Translation, rotation.Left, 1f, Vector3.Forward, 10, dustColor, Vector3.Cross(rotation.Backward, rotation.Up));

            AIControlled = ai;
            idleTimer = 0;
            radar = new BoidRadar(game, this);
            IdealDirection = direction;
            aiMode = AImode.WANDER;

            Health = 100;
            alive = true;

            //Lights
            #region Lighting
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.Enabled = true;
                    effect.DiffuseColor = game.LightManager.diffuseColor; //kd
                    effect.SpecularColor = game.LightManager.specularColor; //Specular Light  //ks
                    effect.SpecularPower = game.LightManager.specularPower; //Power of Specular Light //s
                    effect.AmbientLightColor = game.LightManager.ambientLightColor; //Ia
                    effect.DirectionalLight0.DiffuseColor = game.LightManager.directionalLightDiffuseColor; //Directional Light's Color //Id
                    effect.DirectionalLight0.Direction = game.LightManager.directionalLightDirection; //Directional light's Direction 
                    effect.DirectionalLight0.SpecularColor = game.LightManager.directionalLightSpecularColor; //Is                    
                }
            }
            #endregion
        }

        public void Update(GameTime gt, KeyboardState kb, MouseState ms)
        {
            if (!AIControlled)
            {
                idleTimer += (float)gt.ElapsedGameTime.TotalSeconds;
                if (idleTimer > 30) AIControlled = true;
            }
            else idleTimer = 0f;
            velocity = Vector3.Zero;

            capabilities = GamePad.GetCapabilities(pad); //controller capabilities check
            gamepad = GamePad.GetState(pad);  //controller

            //---------------------------------------------Player Controlled Movement-----------------------------//

            //KEYBOARD
            #region KeyBoard Controls
            #region Tank Rotation
            if (kb.IsKeyDown(Controls[2])) //Rotate Left
            {
                yaw += MathHelper.ToRadians(2);
                Animate_Wheels_Left(gt);
                AIControlOff();
                keyboard = true;
            }
            if (kb.IsKeyDown(Controls[3])) //Rotate Right
            {
                yaw -= MathHelper.ToRadians(2);
                Animate_Wheels_Right(gt);
                AIControlOff();
                keyboard = true;
            }

            #endregion

            #region Movement Forward and Backwards
            //Move Forward
            if (kb.IsKeyDown(Controls[0]))
            {
                velocity -= (((direction - blockingDirection) * speed) * (float)gt.ElapsedGameTime.TotalSeconds);

                moving = true;
                Animate_Wheels_ForWard(gt);
                AIControlOff();
                keyboard = true;
            }
            //move BackWards
            else if (kb.IsKeyDown(Controls[1]))
            {
                velocity += (((direction + blockingDirection) * speed) * (float)gt.ElapsedGameTime.TotalSeconds);

                moving = true;
                Animate_Wheels_BackWards(gt);
                AIControlOff();
                keyboard = true;
            }
            #endregion

            #region turret Movement
            //Turret Movement
            if (kb.IsKeyDown(Controls[6])) //Move turret Right
            {
                turretAngle += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                AIControlOff();
                keyboard = true;
            }
            if (kb.IsKeyDown(Controls[7])) //more turret Left
            {
                turretAngle -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                AIControlOff();
                keyboard = true;
            }
            if (kb.IsKeyDown(Controls[4]) && cannonAngle > -0.75f) //Move cannon Up
            {
                cannonAngle -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                AIControlOff();
                keyboard = true;
            }
            if (kb.IsKeyDown(Controls[5]) && cannonAngle < 0.3f) //move cannon Down
            {
                cannonAngle += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                AIControlOff();
                keyboard = true;
            }
            #endregion

            #endregion
            ///KEYBOARD

            //XBOX CONTROLLER
            #region controllerControls
            if (capabilities.GamePadType == GamePadType.GamePad)
            {
                #region tank rotation
                if (!keyboard && gamepad.ThumbSticks.Left.X < -0.5f) //Rotate Left
                {
                    yaw += MathHelper.ToRadians(2);
                    Animate_Wheels_Left(gt);
                    AIControlOff();
                }
                if (!keyboard && gamepad.ThumbSticks.Left.X > 0.5f) //Rotate Right
                {
                    yaw -= MathHelper.ToRadians(2);
                    Animate_Wheels_Right(gt);
                    AIControlOff();
                }


                #endregion

                #region Movement Forward and Backwards
                //Move Forward
                if (!keyboard && gamepad.ThumbSticks.Left.Y > 0.5f)
                {
                    velocity -= (((direction - blockingDirection) * speed) * (float)gt.ElapsedGameTime.TotalSeconds);

                    moving = true;
                    Animate_Wheels_ForWard(gt);
                    AIControlOff();
                }
                //move BackWards
                else if (!keyboard && gamepad.ThumbSticks.Left.Y < -0.5f)
                {
                    velocity += (((direction + blockingDirection) * speed) * (float)gt.ElapsedGameTime.TotalSeconds);

                    moving = true;
                    Animate_Wheels_BackWards(gt);
                    AIControlOff();
                }
                #endregion

                #region turret Movement
                //Turret Movement
                if (!keyboard && gamepad.ThumbSticks.Right.X < -0.5f) //Move turret Right
                {
                    turretAngle += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                    AIControlOff();
                }
                if (!keyboard && gamepad.ThumbSticks.Right.X > 0.5f) //more turret Left
                {
                    turretAngle -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                    AIControlOff();
                }
                if (!keyboard && gamepad.ThumbSticks.Right.Y > 0.5f && cannonAngle > -0.75f) //Move cannon Up
                {
                    cannonAngle -= 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                    AIControlOff();
                }
                if (!keyboard && gamepad.ThumbSticks.Right.Y < -0.5f && cannonAngle < 0.3f) //move cannon Down
                {
                    cannonAngle += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
                    AIControlOff();
                }
                #endregion
            }
            #endregion
            ///XBOX CONTROLLER

            if (!AIControlled)
            {
                rotation = Matrix.CreateFromYawPitchRoll(yaw, 0f, 0f);
                direction = Vector3.Transform(dirDefault, rotation); //tank's 2d Direction
                direction.Normalize();
            }

            //---------------------------------------------END PLAYER CONTROLLED MOMENT--------------------------//

            //-------------------------------------------AI Controlled Movement--------------------------------// (float)Math.Cos(angleDiference)
            #region AI
            if (AIControlled)
            {


                switch (aiMode)
                {
                    case AImode.WANDER:
                        float angleDiference = Vector3.Dot(direction, radar.newDirection); //Angle Dirference between the old direction and the new

                        yaw += (float)Math.Cos(angleDiference) * radar.newDir * (float)gt.ElapsedGameTime.TotalSeconds; //adds the new direciton to the old

                        rotation = Matrix.CreateFromYawPitchRoll(yaw, 0f, 0f);
                        direction = Vector3.Transform(dirDefault, rotation); //tank's 2d Direction
                        direction.Normalize();

                        Vector3 AIdirection = direction - blockingDirection;

                        velocity -= ((AIdirection * speed) * (float)gt.ElapsedGameTime.TotalSeconds);
                        moving = true;

                        Animate_Wheels_ForWard(gt);
                        break;
                    case AImode.PERSUIT:


                        break;

                }

            }


            #endregion
            //-----------------------------------END AI CONTROLED MOVEMENT------------------------------------------//

            Vector3 nextPos = position + velocity;  //Position in which the tank might be next frame if the player moves it
            //velocity += blockingDirection * (float)gt.ElapsedGameTime.TotalSeconds; //this makes the tanks push each others
            if (nextPos.X < game.map.size.X - 1 && nextPos.X - 1 > 0 && nextPos.Z < game.map.size.Y - 1 && nextPos.Z - 1 > 0) position += velocity;
            collider.Center = position; //Colider Update

            radar.update(gt);

            terrainNormal = game.map.getNormal(position.X, position.Z); //Current position's terrain normal

            if (!AIControlled)
            {
                formatDirection(direction, terrainNormal); //tank's rotation function
            }
            else
            {
                formatDirection(direction, terrainNormal); //tank's rotation function
            }

            position.Y = game.map.getHeight(position.X, position.Z); //tank's Height

            Matrix translacao = Matrix.CreateTranslation(position);
            Matrix escala = Matrix.CreateScale(scale);
            myModel.Root.Transform = escala * rotacao * translacao;



            //TURRET DIRECTION FOR CAMERA PUPOSES
            turretRotation = Matrix.CreateFromYawPitchRoll(turretAngle, 0f, 0f);
            turretDirection = Vector3.Transform(direction, turretRotation);
            turretDirection.Normalize();

            #region BONE TRANSFORMS
            //BONE TRANSFORMS
            turretBone.Transform = Matrix.CreateRotationY(turretAngle) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;
            rightWheel.Transform = Matrix.CreateRotationX(rightWheelAngle) * rightWheelTransform;
            leftWheel.Transform = Matrix.CreateRotationX(leftWheelAngle) * leftWheelTransform;
            rightBackWheel.Transform = Matrix.CreateRotationX(rightBackWheelAngle) * rightBackWheelTransform;
            leftBackWheel.Transform = Matrix.CreateRotationX(leftBackWheelAngle) * leftBackWheelTransform;
            hatch.Transform = Matrix.CreateRotationX(hatchAngle) * hatchTransform;

            myModel.CopyAbsoluteBoneTransformsTo(boneTransform);
            #endregion

            //GUN DIRECTION FOR SHOOTING PURPOSES
            GunDirection = boneTransform[cannonBone.Index].Forward;
            GunDirection.Normalize();



            //-----------------------------------SHOOTING LOGIC-----------------------------//
            if (reloading)
            {
                reloadTimer += (float)gt.ElapsedGameTime.TotalSeconds;
                if (reloadTimer >= 1f) { reloading = false; reloadTimer = 0; }
            }

            //-------------------------------------------------KEYBOARD CONTROLS-----------------------------//
            if (!reloading && !kbPreviousState.IsKeyDown(Controls[8]) && kb.IsKeyDown(Controls[8])) //SHOOTING CONTROLS
            {
                Shoot();
                AIControlOff();
                keyboard = true;
            }
            ///---------------------------------------------END KEYBOARD CONTROLS----------------------------//

            //-----------------------------------------XBOX PAD CONTROLS----------------------------------//
            if (!keyboard && capabilities.GamePadType == GamePadType.GamePad)
            {
                if (!reloading && !padPreviousState.IsButtonDown(Buttons.RightTrigger) && gamepad.IsButtonDown(Buttons.RightTrigger))
                {
                    Shoot();
                    AIControlOff();
                }
            }
            ///---------------------------------------END XBOX PAD CONTROLS-----------------------------------//

            if (hatchAngle < 0) //close Hatch
            {
                hatchAngle += 1f * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            if (Hit) hittime += (float)gt.ElapsedGameTime.TotalSeconds;
            if (hittime > 0.3f) { hittime = 0; Hit = false; }
            ///-------------------------------------END SHOOOTING---------------------------//

            //DUST EMMITER
            dustEmitters[0].Update(gt, boneTransform[rightWheel.Index].Translation, rotation.Left, Vector3.Cross(direction, rotation.Up), rotation.Down * 0.25f, moving);
            dustEmitters[1].Update(gt, boneTransform[leftWheel.Index].Translation, rotation.Right, Vector3.Cross(direction, rotation.Up), rotation.Down * 0.25f, moving);
            dustEmitters[2].Update(gt, boneTransform[rightBackWheel.Index].Translation, rotation.Right, Vector3.Cross(direction, rotation.Up), rotation.Down * 0.45f, moving);
            dustEmitters[3].Update(gt, boneTransform[leftBackWheel.Index].Translation, rotation.Left, Vector3.Cross(direction, rotation.Up), rotation.Down * 0.45f, moving);
            ///DUST EMMITERS

            Tankcoliding = false;
            moving = false;

            blockingDirection = Vector3.Zero;

            kbPreviousState = kb; //Keyboard state This frame
            padPreviousState = gamepad; //padState this frame
            keyboard = false;

            if (!alive) { Respawn(); }
        }


        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransform[mesh.ParentBone.Index];
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;

                    if (Tankcoliding) effect.AmbientLightColor = Color.Blue.ToVector3(); //turns tanks blue when they are colling
                    else if (Hit && hittime < 0.3f) effect.AmbientLightColor = Color.Red.ToVector3(); //turns tanks red when they are hit by projectiles
                    else if (moving) effect.AmbientLightColor = Color.Yellow.ToVector3();
                    else effect.AmbientLightColor = game.LightManager.ambientLightColor; //Ia


                    effect.DirectionalLight0.DiffuseColor = game.LightManager.directionalLightDiffuseColor; //Directional Light's Color //Id
                    effect.DirectionalLight0.Direction = game.LightManager.directionalLightDirection; //Directional light's Direction 
                }
                mesh.Draw();
            }

            for (int i = 0; i < dustEmitters.Length; i++)
            {
                dustEmitters[i].Draw(viewMatrix, projectionMatrix);
            }

            if (AIControlled) radar.draw(viewMatrix, projectionMatrix);
        }

        //----------------------------------------------------------------/COLLISIONS/--------------------------------------------------//
        public string Name() => "Tank";

        public string SubName() => "Tank";

        public int? ID() => id;

        public Vector3 Position() => position;

        public bool Active() => alive; //Tells others that it's alive

        public void CollisionWith(ICollider other) //THIS IS WHAT THE TANK DOES WHEN IT COLIDES WITH SOMETHING
        {
            if (other.Active() && other.Name() == "Tank" && other.ID() != id) //IN CASE THE OTHER IS A TANK
            {
                Tankcoliding = true;
                Vector3 otherDirection = position - other.Position(); //Direction from which the other object is coming from
                otherDirection.Normalize();
                blockingDirection = otherDirection;
            }

            if (other.Name() == "Projectile" && other.ID() != id) //IN CASE ITS A PROJECTILE
            {
                Hit = true;
                int damage;
                //damage = 10 * (game.rng.Next(4,6));
                damage = 50;
                Health -= damage;
                if (Health <= 0) alive = false; //DIES
            }
        }

        public bool CollidesWith(ICollider other) => collider.CollidesWith(other);

        public ICollider GetCollider() => collider;

        public int getHealth() => Health;
        ///------------------------------------------------------------------/END COLLISIONS/------------------------------------------------///

        //------------------------------------------------------------------/ANIMATIONS/----------------------------------------------------///
        void Animate_Wheels_Left(GameTime gt) //ANIMATE LEFT
        {
            rightWheelAngle += 10f * (float)gt.ElapsedGameTime.TotalSeconds;
            rightBackWheelAngle += 10f * (float)gt.ElapsedGameTime.TotalSeconds;

            leftWheelAngle -= 8f * (float)gt.ElapsedGameTime.TotalSeconds;
            leftBackWheelAngle -= 8f * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        void Animate_Wheels_Right(GameTime gt) //ANIMATE RIGHT
        {
            leftWheelAngle += 10f * (float)gt.ElapsedGameTime.TotalSeconds;
            leftBackWheelAngle += 10f * (float)gt.ElapsedGameTime.TotalSeconds;

            rightWheelAngle -= 8f * (float)gt.ElapsedGameTime.TotalSeconds;
            rightBackWheelAngle -= 8f * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        void Animate_Wheels_ForWard(GameTime gt) //ANIMATE FORWARD
        {
            rightWheelAngle += 10f * (float)gt.ElapsedGameTime.TotalSeconds; //ANimates the wheels
            rightBackWheelAngle += 10f * (float)gt.ElapsedGameTime.TotalSeconds; //ANimates the wheels

            leftWheelAngle += 10f * (float)gt.ElapsedGameTime.TotalSeconds; //ANimates the wheels
            leftBackWheelAngle += 10f * (float)gt.ElapsedGameTime.TotalSeconds; //ANimates the wheels
        }

        void Animate_Wheels_BackWards(GameTime gt) //ANIMATE BACKWARDS
        {
            rightWheelAngle -= 10f * (float)gt.ElapsedGameTime.TotalSeconds; //ANimates the wheels
            rightBackWheelAngle -= 10f * (float)gt.ElapsedGameTime.TotalSeconds; //ANimates the wheels

            leftWheelAngle -= 10f * (float)gt.ElapsedGameTime.TotalSeconds; //ANimates the wheels
            leftBackWheelAngle -= 10f * (float)gt.ElapsedGameTime.TotalSeconds; //ANimates the wheels
        }
        ///-----------------------------------------------------------/END ANIMATIONS/--------------------------------------------------------------------///


        void Shoot()
        {
            Vector3 turretPosition = boneTransform[cannonBone.Index].Translation;
            hatchAngle = -0.90f;

            TProjectile proj = new TProjectile(turretPosition, GunDirection, id, game, shotModel);
            game.projectileManager.TankShoot(proj);
            reloading = true;
        }


        void AIControlOff() //SHUT DOWN AI CONTROL
        {
            AIControlled = false;
            idleTimer = 0f;
        }

        void formatDirection(Vector3 direction, Vector3 terrainNormal) //FORMAT TANK'S ROTATION MATRIX USING A DIRECTION
        {
            right = Vector3.Cross(direction, terrainNormal); //tank's Right
            right.Normalize();

            trueDirection = Vector3.Cross(terrainNormal, right); //Tank's 3d Direction
            trueDirection.Normalize();

            rotacao = Matrix.Identity; //Rotation matrix

            rotacao.Forward = trueDirection;
            rotacao.Up = terrainNormal;
            rotacao.Right = right;
            rotation.Right = -right;
            rotation.Down = -terrainNormal;
            rotation.Backward = -trueDirection;
        }

        void Respawn()
        {
            position = game.Check_Furthest_Spawn(position);
            radar.updateLocation();
            Health = 100;
            aiMode = AImode.WANDER;
            radar.EnemyPosition = null;
            alive = true;
            if (!AIControlled) idleTimer = 0f;
        }

    }
}
