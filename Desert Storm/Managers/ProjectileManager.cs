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
    public class ProjectileManager
    {
        List<BaseProjectile> projectiles;
        Game1 game;


        public ProjectileManager(Game1 game)
        {
            this.game = game;
            projectiles = new List<BaseProjectile>();
        }

        public void TankShoot(BaseProjectile projectile)
        {
            projectiles.Add(projectile as TProjectile);
            game.colliders.Add(projectile as ICollider);
        }


        public void Update(GameTime gameTime)
        {
            foreach (var projectile in projectiles.ToArray())
            {
                if (!projectile.Update(gameTime))
                {
                    projectiles.Remove(projectile);
                    game.colliders.Remove(projectile as ICollider);
                }
            }
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            foreach (var projectile in projectiles)
            {
                projectile.Draw(viewMatrix, projectionMatrix);
            }
        }


    }
}
