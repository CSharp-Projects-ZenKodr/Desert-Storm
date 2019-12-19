using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desert_Storm
{
    public class Skybox
    {
        Game1 game;
        LandScape map;
        Vector2 size;
        public int height;
        int roofheight;

        BasicEffect effect;

        VertexBuffer wallVertexBuffer;
        IndexBuffer wallIndexBuffer;
        VertexPositionNormalTexture[] Wallvertices;
        int wallVertexCount;
        int wallIndexCount;
        short[] wallIndices;

        VertexBuffer roofVertexBuffer;
        IndexBuffer roofIndexBuffer;
        VertexPositionNormalTexture[] roofvertices;
        int roofVertexCount;
        int roofIndexCount;
        short[] roofIndices;

        Texture2D skyTexture;
        Texture2D WallTexture;
        float radius, roofRadius;

        public Skybox(Game1 game, Vector2 size)
        {
            this.game = game;
            this.map = game.map;

            skyTexture = game.Content.Load<Texture2D>("SkyBox/Sky1");
            WallTexture = game.Content.Load<Texture2D>("SkyBox/Sky16");

            this.size.X = size.X;
            this.size.Y = size.Y;
            height = 100;
            roofheight = height - height / 4;


            effect = new BasicEffect(game.GraphicsDevice);
            effect.LightingEnabled = true;
            effect.TextureEnabled = true;


            #region Lighting
            //LIGHTING
            effect.DirectionalLight0.Enabled = true;
            effect.DiffuseColor = game.LightManager.diffuseColor; //kd
            effect.SpecularColor = game.LightManager.specularColor; //Specular Light  //ks
            effect.SpecularPower = game.LightManager.specularPower; //Power of Specular Light //s
            effect.AmbientLightColor = game.LightManager.ambientLightColor; //Ia
            effect.DirectionalLight0.DiffuseColor = game.LightManager.directionalLightDiffuseColor; //Directional Light's Color //Id
            effect.DirectionalLight0.Direction = game.LightManager.directionalLightDirectionSky; //Directional light's Direction 
            effect.DirectionalLight0.SpecularColor = game.LightManager.directionalLightSpecularColor; //Is
            #endregion

            radius = size.X * 2;
            roofRadius = radius * 2;
            

            effect.Texture = WallTexture;
            CreateWalls();
            effect.Texture = skyTexture;
            createRoof();
        }

        void CreateWalls()
        {
            Vector3 center = new Vector3(size.X / 2, 0, size.Y / 2);
            int sides = 20;
            wallVertexCount = sides * 2 + 2;
            
            Wallvertices = new VertexPositionNormalTexture[wallVertexCount];


            for (int i = 0; i < sides + 1; i++)
            {
                float Angle = ((float)i * (2 * MathHelper.Pi / (float)sides)) + MathHelper.Pi / 4;
                float x = radius * (float)Math.Cos(-Angle) + size.X / 2;
                float z = radius * -(float)Math.Sin(-Angle) + size.Y / 2;
                               
                Wallvertices[2 * i + 0] = new VertexPositionNormalTexture(new Vector3(x, -height/2, z), Vector3.Down, new Vector2(1f / sides * i, 1));
                Wallvertices[2 * i + 1] = new VertexPositionNormalTexture(new Vector3(x, height, z), Vector3.Down, new Vector2(1f / sides * i, 0));
            }

            //Vector3 normal = Vector3.Subtract(new Vector3(size.X / 2, 0, size.Y / 2),new Vector3(x, heigth * 4, z)); //calculates a normal very  close to Vector3.Down
            //normal.Normalize();                                                                                      //Using Vector3.down looks better

            wallVertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalTexture), wallVertexCount, BufferUsage.None); //cria o VertexBuffer
            wallVertexBuffer.SetData<VertexPositionNormalTexture>(Wallvertices); //copia os dados para o VertexBuffer

            wallIndexCount = sides * 2 + 2;
            wallIndices = new short[wallIndexCount];

            for (int i = 0; i < sides + 1; i++)
            {
                wallIndices[2 * i + 0] = (short)(2 * i + 0);

                wallIndices[2 * i + 1] = (short)(2 * i + 1);
            }

            wallIndexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), wallIndexCount, BufferUsage.None); //cria o IndexBuffer
            wallIndexBuffer.SetData<short>(wallIndices); //copia os dados para o IndexBuffer
        }        

        void createRoof()
        {
            int sides = 4;

            roofVertexCount = sides + 1;
            roofvertices = new VertexPositionNormalTexture[roofVertexCount];

            for (int i = 0; i < sides; i++)
            {
                float Angle = -((float)i * (2 * MathHelper.Pi / (float)sides)) + (MathHelper.Pi / 4);
                float x = roofRadius * (float)Math.Cos(Angle) + size.X / 2;
                float z = roofRadius * -(float)Math.Sin(Angle) + size.Y / 2;
                
                //Vector3 normal = Vector3.Subtract(new Vector3(size.X / 2, 0, size.Y / 2), new Vector3(x, heigth * 4, z)); //calculates a normal very  close to Vector3.Down
                //normal.Normalize();                                                                                       //Using Vector3.down looks better

                roofvertices[i] = new VertexPositionNormalTexture(new Vector3(x, roofheight, z), Vector3.Down, new Vector2(0.5f + 0.5f * (float)Math.Cos(Angle), 0.5f - 0.5f * (float)Math.Sin(Angle)));
            }
            roofvertices[sides] = new VertexPositionNormalTexture(new Vector3(size.X/2, roofheight, size.Y/2), Vector3.Down, new Vector2(0.5f, 0.5f));

            roofVertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalTexture), roofVertexCount, BufferUsage.None); //cria o VertexBuffer
            roofVertexBuffer.SetData<VertexPositionNormalTexture>(roofvertices); //copia os dados para o VertexBuffer


            //indices
            roofIndexCount = 2 * (sides + 1);
            roofIndices = new short[roofIndexCount];

            for (int i = 0; i < sides + 1; i++)
            {
                roofIndices[2 * i + 0] = (short)(i % sides);

                roofIndices[2 * i + 1] = (short)sides;
            }

            roofIndexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), roofIndexCount, BufferUsage.None); //cria o IndexBuffer
            roofIndexBuffer.SetData<short>(roofIndices); //copia os dados para o IndexBuffer
        }
        

        public void draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            effect.World = Matrix.Identity;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            effect.DirectionalLight0.Direction = game.LightManager.directionalLightDirectionSky; //Directional light's Direction 

            effect.CurrentTechnique.Passes[0].Apply();

            effect.Texture = WallTexture;
            effect.CurrentTechnique.Passes[0].Apply();
            drawWalls();
            effect.Texture = skyTexture;
            effect.CurrentTechnique.Passes[0].Apply();
            drawRoof();

        }

        void drawWalls()
        {
            game.GraphicsDevice.SetVertexBuffer(wallVertexBuffer);
            game.GraphicsDevice.Indices = wallIndexBuffer;
            game.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    0,
                    wallIndexCount
                    );
        }

        void drawRoof()
        {
            game.GraphicsDevice.SetVertexBuffer(roofVertexBuffer);
            game.GraphicsDevice.Indices = roofIndexBuffer;
            game.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    0,
                    roofIndexCount
                    );
        }





    }
}
