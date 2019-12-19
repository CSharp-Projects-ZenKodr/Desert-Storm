using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desert_Storm
{
    public class CameraSurfaceFollow : BaseCam
    {


        public CameraSurfaceFollow(Game1 game) : base(game)
        {

            position = new Vector3(10f, 10f, 5f);


            //active = false;


            speed = 20;
        }

        public override void Update(KeyboardState kb, MouseState mouse, GameTime gameTime)
        {
            base.Update(kb, mouse, gameTime);


            if (position.X < game.map.size.X - 1 && position.X > 0 && position.Z < game.map.size.Y - 1 && position.Z > 0)
            {


                position.Y = 2 + game.map.getHeight(position.X, position.Z);
            }

            else { position.Y = 5; }


        }
    }
}
