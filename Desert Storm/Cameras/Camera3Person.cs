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
    class Camera3Person : BaseCam //third person camera
    {
        Tank tank;

        public Camera3Person(Game1 game, Tank tank) : base(game)
        {
            this.tank = tank;

            position = tank.position - tank.direction * 7f;
        }

        public override void Update(KeyboardState kb, MouseState mouse, GameTime gameTime)
        {
            base.Update(kb, mouse, gameTime);


            position = tank.position + tank.turretDirection * 7f;

            position.Y = tank.position.Y + 4f;

            target = tank.position;

            target.Y += 2; //put the camera above the tank's position
            if (position.X < game.map.size.X - 1 && position.X > 0 && position.Z < game.map.size.Y - 1 && position.Z > 0) //Checks if camera position is inside the map
            {
                float mapheight = game.map.getHeight(position.X, position.Z); //gets map's height at camera's position
                if (position.Y < mapheight + 1) { position.Y = mapheight + 1; } //if the camera's height goes to low, this will force it to stay above the map
            }

            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);

        }

    }
}
