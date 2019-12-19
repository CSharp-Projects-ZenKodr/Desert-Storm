using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desert_Storm
{
    public class WeatherManager
    {
        Game1 game;
        List<baseEmitter> WeatherEmitters;
        float nextWeatherCheck;
        float currentWeatherDuration;
        float weatherTimer;
        bool clear;       

        public WeatherManager(Game1 game)
        {
            this.game = game;
            WeatherEmitters = new List<baseEmitter>();
            clear = false;

            nextWeatherCheck = 5f;
            SelectNextWeather();
        }

        public void Update(GameTime gt)
        {
            if (weatherTimer > 0) weatherTimer -= (float)gt.ElapsedGameTime.TotalSeconds;
            else setClear();

            if (clear && nextWeatherCheck > 0)  nextWeatherCheck -= (float)gt.ElapsedGameTime.TotalSeconds;
            if (nextWeatherCheck < 0 && clear)
            {
                if (WeatherChangeChance())
                {                    
                    SelectNextWeather();
                }
            }

            updateWeather(gt);

        }

        public void draw(Matrix viewMatrix, Matrix projectionMatrix)
        {

            drawWeather(viewMatrix, projectionMatrix);

        }

        void setClear(float time) //set weather to clear
        {
            game.LightManager.changeIntensity(1f, time);
        }
        void setClear()
        {
            clear = true;
            game.LightManager.changeIntensity(1f);
        }

        void createRain(float time) //set weather to rain
        {
            RainDiscEmitter rain = new RainDiscEmitter(game, game.map.center, game.map.skybox.height, game.map.size.X, Vector3.Up, 100, Color.Blue, time);
            WeatherEmitters.Add(rain);
            game.LightManager.changeIntensity(0.6f, time);
        }

        void SelectNextWeather()
        {
            float rng = game.rng.Next(1, 3);
            float randomTime = game.rng.Next(60, 121);
            weatherTimer = randomTime;
            currentWeatherDuration = randomTime;
            clear = false;

            switch (rng)
            {
                case 2:                    
                    createRain(randomTime);                    
                    break;
                default:
                    setClear(randomTime);
                    break;
            }
        }

        void updateWeather(GameTime gt)
        {
            foreach (var w in WeatherEmitters.ToArray())
            {
                if (!w.Update(gt)) WeatherEmitters.Remove(w);
            }
        }

        void drawWeather(Matrix viewMatrix, Matrix projectionMatrix)
        {
            foreach (var w in WeatherEmitters)
            {
                w.Draw(viewMatrix, projectionMatrix);
            }
        }

        bool WeatherChangeChance()
        {
            return game.rng.Next(0, 4) == 2;     


        }

    }
}
