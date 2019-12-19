using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desert_Storm
{
    public class FreeCamera : BaseCam
    {
        


        public FreeCamera(Game1 game) : base(game)
        {
            this.position = new Vector3(0f, 10f, 5f);

            speed = 10;
            //active = true;
        }

        public override void Update(KeyboardState kb, MouseState mouse, GameTime gameTime)
        {
            base.Update(kb, mouse, gameTime);


            if (kb.IsKeyDown(Keys.NumPad7)) position.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (kb.IsKeyDown(Keys.NumPad1)) position.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;


    
        }



    }
}
