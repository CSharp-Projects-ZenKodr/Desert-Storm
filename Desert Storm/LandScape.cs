using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desert_Storm
{
    public class LandScape
    {
        BasicEffect effect;
        Color[] greyValues;
        float[] greyMap;
        Game1 game;
        IndexBuffer indexBuffer;
        int vertexCount;
        int indexCount;
        short[] indexes;
        Texture2D HeightMap;
        Texture2D groundTexture;
        VertexBuffer vertexBuffer;
        VertexPositionNormalTexture[] vertices;

        Vector3[] normalMap;

        public Vector2 size;

        public Skybox skybox;
        public Vector3 center;

        public LandScape(Game1 game, Texture2D heightMap)
        {
            this.game = game;

            groundTexture = game.Content.Load<Texture2D>("gTexture");
            HeightMap = heightMap;

            greyValues = new Color[HeightMap.Width * HeightMap.Height];
            HeightMap.GetData<Color>(greyValues);

            effect = new BasicEffect(game.GraphicsDevice);
            effect.LightingEnabled = true;
            effect.TextureEnabled = true;
            effect.Texture = groundTexture;

            size.X = HeightMap.Width;
            size.Y = HeightMap.Height;

            center = new Vector3(size.X / 2f, 0, size.Y / 2f);

            #region Lightning
            //LIGHTING
            effect.DirectionalLight0.Enabled = true;
            effect.DiffuseColor                     = game.LightManager.diffuseColor; //kd
            effect.SpecularColor                    = Vector3.Zero; //Specular Light  //ks
            effect.SpecularPower                    = game.LightManager.specularPower; //Power of Specular Light //s
            effect.AmbientLightColor                = game.LightManager.ambientLightColor; //Ia
            effect.DirectionalLight0.DiffuseColor   = game.LightManager.directionalLightDiffuseColor; //Directional Light's Color //Id
            effect.DirectionalLight0.Direction      = game.LightManager.directionalLightDirection; //Directional light's Direction 
            effect.DirectionalLight0.SpecularColor  = game.LightManager.directionalLightSpecularColor; //Is
            #endregion


            CreateMap(greyValues, HeightMap);

            skybox = new Skybox(game, size);
        }

        #region Map Creation
        void CreateMap(Color[] mapColor, Texture2D map)
        {      
            
            //Atribuição de variáveis
            float verticalScale = 0.05f;
            greyMap = new float[map.Width * map.Height];

            normalMap = new Vector3[map.Width * map.Height];

            vertexCount = (map.Height * map.Width);
            vertices = new VertexPositionNormalTexture[vertexCount];

            indexCount = 2 * (map.Height) * (map.Width - 1);
            indexes = new short[indexCount];

            //Criação de array de mapa de alturas relatiavas às cores
            for (int i = 0; i < vertexCount; i++)
            {
                greyMap[i] = greyValues[i].R * verticalScale;
            }

            //criação do array de normalMap
            for (int i = 0; i < vertexCount; i++)
            {
                normalMap[i] = Vector3.Zero;
            }

            //Criação de array de vertices com todos os vertices e posições do mapa
            int vertexNumber = 0;
            for (int z = 0; z < map.Height; z++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    vertices[vertexNumber] = new VertexPositionNormalTexture(new Vector3(x, greyMap[vertexNumber], z), normalMap[vertexNumber], getTextureCoor(vertexNumber, x, z));
                    vertexNumber++;

                }
            }

            #region Normals
            //Normais miolo
            for (int z = 1; z < map.Height-1; z++)
            {
                for (int x = 1; x < map.Width-1; x++)
                {
                    int current = z * HeightMap.Width + x;
                    normalMap[current] = getNormalFrom8(x, z);
                    vertices[current].Normal = normalMap[current];
                }
            }

            //normals Edge
            //right edge
            for (int z = 1, x = 0; z < map.Height - 1; z++)
            {
                int current = z * HeightMap.Width + x;
                normalMap[current] = getNormalFrom5Left(x, z);
                vertices[current].Normal = normalMap[current];
            }
            //left edge
            for (int z = 1, x = map.Width - 1; z < map.Height - 1; z++)
            {
                int current = z * HeightMap.Width + x;
                normalMap[current] = getNormalFrom5Right(x, z);
                vertices[current].Normal = normalMap[current];
            }
            //bottom edge
            for (int x = 1, z = 0; x < map.Width - 1; x++)
            {
                int current = z * HeightMap.Width + x;
                normalMap[current] = getNormalFrom5Above(x, z);
                vertices[current].Normal = normalMap[current];
            }
            //Top edge
            for (int x = 1, z = map.Height - 1; x < map.Width - 1; x++)
            {
                int current = z * HeightMap.Width + x;
                normalMap[current] = getNormalFrom5Bellow(x, z);
                vertices[current].Normal = normalMap[current];
            }

            //normais Cantos
            topRightNormal(map.Width - 1, map.Height - 1);
            topLeftNormal(0,map.Height - 1);
            bottomRightNormal(map.Width - 1,0);
            bottomLeftNormal(0,0);
            #endregion

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.None); //cria o VertexBuffer
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices); //copia os dados para o VertexBuffer

            //Criação do array de indices 
            for (int i = 0; i < indexCount / 2; i++)
            {
                int j = (i % map.Height) * map.Height + (i / map.Height);
                indexes[2 * i + 0] = (short)(j);
                indexes[2 * i + 1] = (short)(j + 1);
            }


            indexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), indexCount, BufferUsage.None);
            indexBuffer.SetData<short>(indexes);
        }
        #endregion

        Vector2 getTextureCoor(int vertexNumber, int x, int z)
        {
            Vector2 Coor;
            //Atribui as coordenadas da textura
            if (x % 2 == 0 && z % 2 == 0) //se X e Z forem par
                Coor = new Vector2(0, 0);
            else if (x % 2 != 0 && z % 2 == 0) //se X for impar e Z for par
                Coor = new Vector2(1, 0);
            else if (x % 2 == 0 && z % 2 != 0) //se X for par e Z impar
                Coor = new Vector2(0, 1);
            else  //se X e Z forem impar
                Coor = new Vector2(1, 1);
            return Coor;
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            effect.World = Matrix.Identity;

            effect.DirectionalLight0.Direction = game.LightManager.directionalLightDirection; //Directional light's Direction 
            
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            effect.CurrentTechnique.Passes[0].Apply();

            game.GraphicsDevice.SetVertexBuffer(vertexBuffer);

            game.GraphicsDevice.Indices = indexBuffer;
            
            //Sequência de Draw para desenhar cada strip do mapa
            for (int i = 0; i < (HeightMap.Width - 1); i++)
            {

                game.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    i * (2 * HeightMap.Width),
                    2 * HeightMap.Width - 2
                    );
            }

            skybox.draw(viewMatrix, projectionMatrix);
         }

        #region Point Height Bilinear Interpolation
        public float getHeight(float x, float z)
        {
            if (x < size.X - 1 && x - 1 > 0 && z < size.Y - 1 && z - 1 > 0)
            {
                //vertice a - topo esquerdo
                int Xa = (int)x;
                int Za = (int)z;
                //vertice c - baixo esquerdo
                int Xc = Xa;
                int Zc = Za + 1;
                //vertice b - topo direito
                int Xb = Xa + 1;
                int Zb = Za;
                //vertice d - baixo direito
                int Xd = Xa + 1;
                int Zd = Za + 1;

                float da = x - Xa; //distancia horizontal do vertice a á camera
                float db = Xb - x; //distancia horizontal do vertice b á camera
                float dd = db; //distancia horizontal do vertice d á camera
                float dc = da; //distancia horizonta do vertice c á camera

                float Ya = greyMap[Za * (HeightMap.Width) + Xa]; //Altura do vertice a
                float Yb = greyMap[Zb * (HeightMap.Width) + Xb]; //altura do vertice b
                float Yc = greyMap[Zc * (HeightMap.Width) + Xc]; //altura do vertice c
                float Yd = greyMap[Zd * (HeightMap.Width) + Xd]; //altura do vertice d

                float dab = z - Za; //distancia Vertical da camera á aresta de cima
                float dcd = Zc - z; //distancia vertical da cemera á aresta de baixo

                float Yab = da * Yb + db * Ya; //inclinação da aresta ab
                float Ycd = dc * Yd + dd * Yc; //inclinação da aresta cd

                float y = dab * Ycd + dcd * Yab; //altura a que a camera está

                return y;
            }
            else return 0;
        }
        #endregion

        #region point Normal Bilinear interpolation
        public Vector3 getNormal(float x, float z)
        {
            //vertice a - topo esquerdo
            int Xa = (int)x;
            int Za = (int)z;
            //vertice c - baixo esquerdo
            int Xc = Xa;
            int Zc = Za + 1;
            //vertice b - topo direito
            int Xb = Xa + 1;
            int Zb = Za;
            //vertice d - baixo direito
            int Xd = Xa + 1;
            int Zd = Za + 1;

            float da = x - Xa; //distancia horizontal do vertice a á camera
            float db = Xb - x; //distancia horizontal do vertice b á camera
            float dd = db; //distancia horizontal do vertice d á camera
            float dc = da; //distancia horizonta do vertice c á camera

            Vector3 Ya = normalMap[Za * (HeightMap.Width) + Xa]; //normal do vertice a 
            Vector3 Yb = normalMap[Zb * (HeightMap.Width) + Xb]; //normal do vertice b
            Vector3 Yc = normalMap[Zc * (HeightMap.Width) + Xc]; //normal do vertice c
            Vector3 Yd = normalMap[Zd * (HeightMap.Width) + Xd]; //normal do vertice d

            float dab = z - Za; //distancia Vertical da camera á aresta de cima
            float dcd = Zc - z; //distancia vertical da cemera á aresta de baixo

            Vector3 Yab = da * Yb + db * Ya; //Peso da aresta ab
            Vector3 Ycd = dc * Yd + dd * Yc; //Peso da aresta cd

            Vector3 normal = dab * Ycd + dcd * Yab; //Normal da posição
            normal.Normalize();

            //Vector3 normal = (Ya + Yb + Yc + Yd) / 4; //normal da face

            return normal;
        }
        #endregion

        #region Normal Calculation
        #region Middle normals
        Vector3 getNormalFrom8(int x, int z)
        {
            Vector3 midlePoint = new Vector3(x, greyMap[z * (HeightMap.Width) + x], z);
            Vector3 finalNormal = Vector3.Up;
            Vector3[] sidePoints;
            Vector3[] sideVectors;
            Vector3[] sideMiddleVectors;
            sidePoints = new Vector3[8]; //Neighbouring Points
            sideVectors = new Vector3[8]; //Vectors to the neighbouring points
            sideMiddleVectors = new Vector3[8]; //Avarage or the Vectors above

            sidePoints[0] = new Vector3(x - 1, greyMap[(z + 0) * (HeightMap.Width) + (x - 1)], z + 0);
            sidePoints[1] = new Vector3(x - 1, greyMap[(z + 1) * (HeightMap.Width) + (x - 1)], z + 1);
            sidePoints[2] = new Vector3(x + 0, greyMap[(z + 1) * (HeightMap.Width) + (x + 0)], z + 1);
            sidePoints[3] = new Vector3(x + 1, greyMap[(z + 1) * (HeightMap.Width) + (x + 1)], z + 1);
            sidePoints[4] = new Vector3(x + 1, greyMap[(z + 0) * (HeightMap.Width) + (x + 1)], z + 0);
            sidePoints[5] = new Vector3(x + 1, greyMap[(z - 1) * (HeightMap.Width) + (x + 1)], z - 1);
            sidePoints[6] = new Vector3(x + 0, greyMap[(z - 1) * (HeightMap.Width) + (x + 0)], z - 1);
            sidePoints[7] = new Vector3(x - 1, greyMap[(z - 1) * (HeightMap.Width) + (x - 1)], z - 1);


            for (int i = 0; i < 8; i++) //calc Vectors to Neighbouring Points
            {
                sideVectors[i] = Vector3.Subtract(sidePoints[i], midlePoint);
            }

            for (int i = 0; i < 7; i++) //calc avarage of the Vectors calculated Above
            {
                sideMiddleVectors[i] = Vector3.Cross(sideVectors[i], sideVectors[i + 1]);
            }
            sideMiddleVectors[7] = Vector3.Cross(sideVectors[7], sideVectors[0]);

            //calculate the original point's normal by avaraging the vectors calculated above

            Vector3 VectorSum = Vector3.Zero;
            for (int i = 0; i < 8; i++) //sum the Vectors for avaraging
            {
                VectorSum += sideMiddleVectors[i];
            }

            finalNormal = VectorSum / 8; //avarage the Vectors

            return finalNormal;
        }
        #endregion
        #region Edges
        Vector3 getNormalFrom5Left(int x, int z)
        {
            Vector3 midlePoint = new Vector3(x, greyMap[z * (HeightMap.Width) + x], z);
            Vector3 finalNormal = Vector3.Up;
            Vector3[] sidePoints;
            Vector3[] sideVectors;
            Vector3[] sideMiddleVectors;
            sidePoints = new Vector3[5]; //Neighbouring Points
            sideVectors = new Vector3[5]; //Vectors to the neighbouring points
            sideMiddleVectors = new Vector3[4]; //Avarage or the Vectors above

            sidePoints[0] = new Vector3(x + 0, greyMap[(z + 1) * (HeightMap.Width) + (x + 0)], z + 1);
            sidePoints[1] = new Vector3(x + 1, greyMap[(z + 1) * (HeightMap.Width) + (x + 1)], z + 1);
            sidePoints[2] = new Vector3(x + 1, greyMap[(z + 0) * (HeightMap.Width) + (x + 1)], z + 0);
            sidePoints[3] = new Vector3(x + 1, greyMap[(z - 1) * (HeightMap.Width) + (x + 1)], z - 1);
            sidePoints[4] = new Vector3(x + 0, greyMap[(z - 1) * (HeightMap.Width) + (x + 0)], z - 1);

            for (int i = 0; i < 5; i++) //calc Vectors to Neighbouring Points
            {
                sideVectors[i] = Vector3.Subtract(sidePoints[i], midlePoint);
            }

            for (int i = 0; i < 4; i++) //calc avarage of the Vectors calculated Above
            {
                sideMiddleVectors[i] = Vector3.Cross(sideVectors[i], sideVectors[i + 1]);
            }

            //calculate the original point's normal by avaraging the vectors calculated above

            Vector3 VectorSum = Vector3.Zero;
            for (int i = 0; i < 4; i++) //sum the Vectors for avaraging
            {
                VectorSum += sideMiddleVectors[i];
            }

            finalNormal = VectorSum / 4; //avarage the Vectors

            return finalNormal;
        }

        Vector3 getNormalFrom5Right(int x, int z)
        {
            Vector3 midlePoint = new Vector3(x, greyMap[z * (HeightMap.Width) + x], z);
            Vector3 finalNormal = Vector3.Up;
            Vector3[] sidePoints;
            Vector3[] sideVectors;
            Vector3[] sideMiddleVectors;
            sidePoints = new Vector3[5]; //Neighbouring Points
            sideVectors = new Vector3[5]; //Vectors to the neighbouring points
            sideMiddleVectors = new Vector3[4]; //Avarage or the Vectors above

            sidePoints[0] = new Vector3(x + 0, greyMap[(z - 1) * (HeightMap.Width) + (x + 0)], z - 1);
            sidePoints[1] = new Vector3(x - 1, greyMap[(z - 1) * (HeightMap.Width) + (x - 1)], z - 1);
            sidePoints[2] = new Vector3(x - 1, greyMap[(z + 0) * (HeightMap.Width) + (x - 1)], z + 0);
            sidePoints[3] = new Vector3(x - 1, greyMap[(z + 1) * (HeightMap.Width) + (x - 1)], z + 1);
            sidePoints[4] = new Vector3(x + 0, greyMap[(z + 1) * (HeightMap.Width) + (x + 0)], z + 1);

            for (int i = 0; i < 5; i++) //calc Vectors to Neighbouring Points
            {
                sideVectors[i] = Vector3.Subtract(sidePoints[i], midlePoint);
            }

            for (int i = 0; i < 4; i++) //calc avarage of the Vectors calculated Above
            {
                sideMiddleVectors[i] = Vector3.Cross(sideVectors[i], sideVectors[i + 1]);
            }

            //calculate the original point's normal by avaraging the vectors calculated above

            Vector3 VectorSum = Vector3.Zero;
            for (int i = 0; i < 4; i++) //sum the Vectors for avaraging
            {
                VectorSum += sideMiddleVectors[i];
            }

            finalNormal = VectorSum / 4; //avarage the Vectors

            return finalNormal;
        }

        Vector3 getNormalFrom5Above(int x, int z)
        {
            Vector3 midlePoint = new Vector3(x, greyMap[z * (HeightMap.Width) + x], z);
            Vector3 finalNormal = Vector3.Up;
            Vector3[] sidePoints;
            Vector3[] sideVectors;
            Vector3[] sideMiddleVectors;
            sidePoints = new Vector3[5]; //Neighbouring Points
            sideVectors = new Vector3[5]; //Vectors to the neighbouring points
            sideMiddleVectors = new Vector3[4]; //Avarage or the Vectors above

            sidePoints[0] = new Vector3(x - 1, greyMap[(z + 0) * (HeightMap.Width) + (x - 1)], z + 0);
            sidePoints[1] = new Vector3(x - 1, greyMap[(z + 1) * (HeightMap.Width) + (x - 1)], z + 1);
            sidePoints[2] = new Vector3(x + 0, greyMap[(z + 1) * (HeightMap.Width) + (x + 0)], z + 1);
            sidePoints[3] = new Vector3(x + 1, greyMap[(z + 1) * (HeightMap.Width) + (x + 1)], z + 1);
            sidePoints[4] = new Vector3(x + 1, greyMap[(z + 0) * (HeightMap.Width) + (x + 1)], z + 0);

            for (int i = 0; i < 5; i++) //calc Vectors to Neighbouring Points
            {
                sideVectors[i] = Vector3.Subtract(sidePoints[i], midlePoint);
            }

            for (int i = 0; i < 4; i++) //calc avarage of the Vectors calculated Above
            {
                sideMiddleVectors[i] = Vector3.Cross(sideVectors[i], sideVectors[i + 1]);
            }

            //calculate the original point's normal by avaraging the vectors calculated above

            Vector3 VectorSum = Vector3.Zero;
            for (int i = 0; i < 4; i++) //sum the Vectors for avaraging
            {
                VectorSum += sideMiddleVectors[i];
            }

            finalNormal = VectorSum / 4; //avarage the Vectors

            return finalNormal;
        }

        Vector3 getNormalFrom5Bellow(int x, int z)
        {
            Vector3 midlePoint = new Vector3(x, greyMap[z * (HeightMap.Width) + x], z);
            Vector3 finalNormal = Vector3.Up;
            Vector3[] sidePoints;
            Vector3[] sideVectors;
            Vector3[] sideMiddleVectors;
            sidePoints = new Vector3[5]; //Neighbouring Points
            sideVectors = new Vector3[5]; //Vectors to the neighbouring points
            sideMiddleVectors = new Vector3[4]; //Avarage or the Vectors above


            sidePoints[0] = new Vector3(x + 1, greyMap[(z + 0) * (HeightMap.Width) + (x + 1)], z + 0);
            sidePoints[1] = new Vector3(x + 1, greyMap[(z - 1) * (HeightMap.Width) + (x + 1)], z - 1);
            sidePoints[2] = new Vector3(x + 0, greyMap[(z - 1) * (HeightMap.Width) + (x + 0)], z - 1);
            sidePoints[3] = new Vector3(x - 1, greyMap[(z - 1) * (HeightMap.Width) + (x - 1)], z - 1);
            sidePoints[4] = new Vector3(x - 1, greyMap[(z + 0) * (HeightMap.Width) + (x - 1)], z + 0);

            for (int i = 0; i < 5; i++) //calc Vectors to Neighbouring Points
            {
                sideVectors[i] = Vector3.Subtract(sidePoints[i], midlePoint);
            }

            for (int i = 0; i < 4; i++) //calc avarage of the Vectors calculated Above
            {
                sideMiddleVectors[i] = Vector3.Cross(sideVectors[i], sideVectors[i + 1]);
            }

            //calculate the original point's normal by avaraging the vectors calculated above

            Vector3 VectorSum = Vector3.Zero;
            for (int i = 0; i < 4; i++) //sum the Vectors for avaraging
            {
                VectorSum += sideMiddleVectors[i];
            }

            finalNormal = VectorSum / 4; //avarage the Vectors

            return finalNormal;
        }
        #endregion
        #region Corners
        void topRightNormal(int x, int z)
        {
            int current = z * HeightMap.Width + x;

            Vector3 midlePoint = new Vector3(x, greyMap[z * (HeightMap.Width) + x], z);
            Vector3 finalNormal = Vector3.Up;
            Vector3[] sidePoints;
            Vector3[] sideVectors;
            Vector3[] sideMiddleVectors;
            sidePoints = new Vector3[3]; //Neighbouring Points
            sideVectors = new Vector3[3]; //Vectors to the neighbouring points
            sideMiddleVectors = new Vector3[2]; //Avarage or the Vectors above

            sidePoints[0] = new Vector3(x + 0, greyMap[(z - 1) * (HeightMap.Width) + (x + 0)], z - 1);
            sidePoints[1] = new Vector3(x - 1, greyMap[(z - 1) * (HeightMap.Width) + (x - 1)], z - 1);
            sidePoints[2] = new Vector3(x - 1, greyMap[(z + 0) * (HeightMap.Width) + (x - 1)], z + 0);

            for (int i = 0; i < 3; i++) //calc Vectors to Neighbouring Points
            {
                sideVectors[i] = Vector3.Subtract(sidePoints[i], midlePoint);
            }

            for (int i = 0; i < 2; i++) //calc avarage of the Vectors calculated Above
            {
                sideMiddleVectors[i] = Vector3.Cross(sideVectors[i], sideVectors[i + 1]);
            }

            //calculate the original point's normal by avaraging the vectors calculated above

            Vector3 VectorSum = Vector3.Zero;
            for (int i = 0; i < 2; i++) //sum the Vectors for avaraging
            {
                VectorSum += sideMiddleVectors[i];
            }

            finalNormal = VectorSum / 2; //avarage the Vectors

            normalMap[current] = finalNormal;
            vertices[current].Normal = finalNormal;
        }

        void topLeftNormal(int x, int z)
        {
            int current = z * HeightMap.Width + x;

            Vector3 midlePoint = new Vector3(x, greyMap[z * (HeightMap.Width) + x], z);
            Vector3 finalNormal = Vector3.Up;
            Vector3[] sidePoints;
            Vector3[] sideVectors;
            Vector3[] sideMiddleVectors;
            sidePoints = new Vector3[3]; //Neighbouring Points
            sideVectors = new Vector3[3]; //Vectors to the neighbouring points
            sideMiddleVectors = new Vector3[2]; //Avarage or the Vectors above

            sidePoints[0] = new Vector3(x + 1, greyMap[(z + 0) * (HeightMap.Width) + (x + 1)], z + 0);
            sidePoints[1] = new Vector3(x + 1, greyMap[(z - 1) * (HeightMap.Width) + (x + 1)], z - 1);
            sidePoints[2] = new Vector3(x + 0, greyMap[(z - 1) * (HeightMap.Width) + (x + 0)], z - 1);

            for (int i = 0; i < 3; i++) //calc Vectors to Neighbouring Points
            {
                sideVectors[i] = Vector3.Subtract(sidePoints[i], midlePoint);
            }

            for (int i = 0; i < 2; i++) //calc avarage of the Vectors calculated Above
            {
                sideMiddleVectors[i] = Vector3.Cross(sideVectors[i], sideVectors[i + 1]);
            }

            //calculate the original point's normal by avaraging the vectors calculated above

            Vector3 VectorSum = Vector3.Zero;
            for (int i = 0; i < 2; i++) //sum the Vectors for avaraging
            {
                VectorSum += sideMiddleVectors[i];
            }

            finalNormal = VectorSum / 2; //avarage the Vectors

            normalMap[current] = finalNormal;
            vertices[current].Normal = finalNormal;
        }

        void bottomRightNormal(int x, int z)
        {
            int current = z * HeightMap.Width + x;

            Vector3 midlePoint = new Vector3(x, greyMap[z * (HeightMap.Width) + x], z);
            Vector3 finalNormal = Vector3.Up;
            Vector3[] sidePoints;
            Vector3[] sideVectors;
            Vector3[] sideMiddleVectors;
            sidePoints = new Vector3[3]; //Neighbouring Points
            sideVectors = new Vector3[3]; //Vectors to the neighbouring points
            sideMiddleVectors = new Vector3[2]; //Avarage or the Vectors above

            sidePoints[0] = new Vector3(x - 1, greyMap[(z + 0) * (HeightMap.Width) + (x - 1)], z + 0);
            sidePoints[1] = new Vector3(x - 1, greyMap[(z + 1) * (HeightMap.Width) + (x - 1)], z + 1);
            sidePoints[2] = new Vector3(x + 0, greyMap[(z + 1) * (HeightMap.Width) + (x + 0)], z + 1);

            for (int i = 0; i < 3; i++) //calc Vectors to Neighbouring Points
            {
                sideVectors[i] = Vector3.Subtract(sidePoints[i], midlePoint);
            }

            for (int i = 0; i < 2; i++) //calc avarage of the Vectors calculated Above
            {
                sideMiddleVectors[i] = Vector3.Cross(sideVectors[i], sideVectors[i + 1]);
            }

            //calculate the original point's normal by avaraging the vectors calculated above

            Vector3 VectorSum = Vector3.Zero;
            for (int i = 0; i < 2; i++) //sum the Vectors for avaraging
            {
                VectorSum += sideMiddleVectors[i];
            }

            finalNormal = VectorSum / 2; //avarage the Vectors

            normalMap[current] = finalNormal;
            vertices[current].Normal = finalNormal;
        }

        void bottomLeftNormal(int x, int z)
        {
            int current = z * HeightMap.Width + x;

            Vector3 midlePoint = new Vector3(x, greyMap[z * (HeightMap.Width) + x], z);
            Vector3 finalNormal = Vector3.Up;
            Vector3[] sidePoints;
            Vector3[] sideVectors;
            Vector3[] sideMiddleVectors;
            sidePoints = new Vector3[3]; //Neighbouring Points
            sideVectors = new Vector3[3]; //Vectors to the neighbouring points
            sideMiddleVectors = new Vector3[2]; //Avarage or the Vectors above

            sidePoints[0] = new Vector3(x + 0, greyMap[(z + 1) * (HeightMap.Width) + (x + 0)], z + 1);
            sidePoints[1] = new Vector3(x + 1, greyMap[(z + 1) * (HeightMap.Width) + (x + 1)], z + 1);
            sidePoints[2] = new Vector3(x + 1, greyMap[(z + 0) * (HeightMap.Width) + (x + 1)], z + 0);

            for (int i = 0; i < 3; i++) //calc Vectors to Neighbouring Points
            {
                sideVectors[i] = Vector3.Subtract(sidePoints[i], midlePoint);
            }

            for (int i = 0; i < 2; i++) //calc avarage of the Vectors calculated Above
            {
                sideMiddleVectors[i] = Vector3.Cross(sideVectors[i], sideVectors[i + 1]);
            }

            //calculate the original point's normal by avaraging the vectors calculated above

            Vector3 VectorSum = Vector3.Zero;
            for (int i = 0; i < 2; i++) //sum the Vectors for avaraging
            {
                VectorSum += sideMiddleVectors[i];
            }

            finalNormal = VectorSum / 2; //avarage the Vectors

            normalMap[current] = finalNormal;
            vertices[current].Normal = finalNormal;
        }
        #endregion
        #endregion

        //normalFollow - interpolação de normais, ir buscar normais em vez de alturas


    }
}
