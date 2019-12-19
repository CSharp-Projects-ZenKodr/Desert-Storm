using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desert_Storm
{
    public class LightingManager
    {
        public Vector3 diffuseColor;
        public Vector3 specularColor;
        public float specularPower;
        public Vector3 ambientLightColor;
        public Vector3 directionalLightDiffuseColor;
        public Vector3 directionalLightDirection;
        public Vector3 directionalLightDirectionSky; //Light's direction for purposes of lighting the sky
        public Vector3 directionalLightSpecularColor;

        Vector2 mapsize;
        Vector3 sun;
        Vector3 earth;

        float sunAngle;

        float timer;
        float IntensityChanger;
        float currentIntensity;
        float itimer;

        public LightingManager(Game1 game, Texture2D map)
        {
            IntensityChanger = 1f;
            currentIntensity = 1f;
            sunAngle = 0;

            mapsize = new Vector2(map.Width, map.Height);
            earth = new Vector3(mapsize.X / 2, 0, mapsize.Y / 2);
            sun = new Vector3(earth.X + (float)Math.Cos(sunAngle) * 100, earth.X + (float)Math.Sin(sunAngle) * 100, 0);

            diffuseColor                  = Color.White.ToVector3() * 1f; //kd
            specularColor                 = Color.White.ToVector3() * 0.2f; //Specular Light  //ks
            specularPower                 = 128f; //Power of Specular Light //s
            ambientLightColor             = Color.White.ToVector3() * 0.2f; //Ia
            directionalLightDiffuseColor  = (Color.White.ToVector3() * 0.8f + Color.Orange.ToVector3() * 0.4f) * currentIntensity; //Directional Light's Color //Id
            directionalLightDirection     = Vector3.Normalize(Vector3.Subtract(earth, sun)); //Directional light's Direction 
            directionalLightDirectionSky  = Vector3.Normalize(Vector3.Subtract(sun, earth)); //Directional light's Counter Direction 
            directionalLightSpecularColor = Color.White.ToVector3() * 1.0f; //Is
            timer = 0;
           
        }

        public LightingManager(Game1 game, Texture2D map, Vector3 diffuseColor, Vector3 specularColor, float specularPower, Vector3 ambientLightColor,
            Vector3 directionalLightDiffuseColor, Vector3 directionalLightSpecularColor)
        {
            IntensityChanger = 1f;
            sunAngle = 0; //sun's starting position

            mapsize = new Vector2(map.Width, map.Height);
            earth = new Vector3(mapsize.X / 2, 0, mapsize.Y / 2);
            sun = new Vector3(earth.X + (float)Math.Cos(sunAngle) * 100, earth.X + (float)Math.Sin(sunAngle) * 100, 0);

            this.diffuseColor = diffuseColor;
            this.specularColor = specularColor;
            this.specularPower = specularPower;
            this.ambientLightColor = ambientLightColor;
            this.directionalLightDiffuseColor = directionalLightDiffuseColor * currentIntensity;
            directionalLightDirection = Vector3.Normalize(Vector3.Subtract(earth, sun)); //Directional light's Direction
            directionalLightDirectionSky = Vector3.Normalize(Vector3.Subtract(sun, earth)); //Directional light's Counter Direction 
            this.directionalLightSpecularColor = directionalLightSpecularColor;

            timer = 0;
            
        }

        public void Update(GameTime gt)
        {
            if (itimer > 0) itimer -= (float)gt.ElapsedGameTime.TotalMilliseconds;
            if (IntensityChanger != 1 && itimer < 0) IntensityChanger = 1;
            if (currentIntensity > IntensityChanger) currentIntensity -= 1f * (float)gt.ElapsedGameTime.TotalMilliseconds;
            else if (currentIntensity < IntensityChanger) currentIntensity += 1f * (float)gt.ElapsedGameTime.TotalMilliseconds;
            timer += (float)gt.ElapsedGameTime.TotalMilliseconds; //counts the frames

            if (timer > 100) //Every 0.1 secs, the sun's position changes
            {
                sunAngle += 1;
                if (sunAngle == 360) sunAngle = 0;

                if (sunAngle > 220 && sunAngle < 300) directionalLightDiffuseColor = Color.White.ToVector3() * 0f + Color.Orange.ToVector3() * 0f;
                else directionalLightDiffuseColor = (Color.White.ToVector3() * 0.8f + Color.Orange.ToVector3() * 0.4f) * currentIntensity; //Directional Light's Color //Id  


                sun.X = earth.X + (float)Math.Cos(MathHelper.ToRadians(sunAngle)) * 100;
                sun.Y = earth.Y + (float)Math.Sin(MathHelper.ToRadians(sunAngle)) * 100;


                directionalLightDirection = Vector3.Normalize(Vector3.Subtract(earth, sun)); //Directional light's Direction 
                directionalLightDirectionSky = Vector3.Normalize(Vector3.Subtract(sun, earth)); //Directional light's Counter Direction

                timer = 0;
            }
        }

        public void changeIntensity(float newIntensity, float time)
        {
            IntensityChanger = newIntensity;
            itimer = time;
        }

        public void changeIntensity(float newIntensity)
        {
            IntensityChanger = newIntensity;
        }


    }
}
