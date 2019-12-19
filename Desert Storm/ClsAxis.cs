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
    class cls3dAxis
    {
        VertexPositionColor[] vertices;
        BasicEffect effect;
        Game1 game;

        public cls3dAxis(Game1 game)
        {
            this.game = game;
            //  Vamos usar um efeito básico
            effect = new BasicEffect(game.GraphicsDevice);
            //  Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = (float)game.GraphicsDevice.Viewport.Width / game.GraphicsDevice.Viewport.Height;
            effect.View = Matrix.CreateLookAt(new Vector3(1, 2.0f, 2.0f), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10.0f);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            //  Cria os eixos 3D
            CreateGeometry();
        }


        private void CreateGeometry()
        {
            float axisLenght = 1f;
            //  Tamanho da linha em cada sinal do eixo 
            int vertexCount = 6;
            //  Vamos usar 6 vértices
            vertices = new VertexPositionColor[vertexCount];
            // Linha sobre o eixo X
            vertices[0] = new VertexPositionColor(new Vector3(-axisLenght, 0.0f, 0.0f), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(axisLenght, 0.0f, 0.0f), Color.Red);
            // Linha sobre o eixo Y
            vertices[2] = new VertexPositionColor(new Vector3(0.0f, -axisLenght, 0.0f), Color.White);
            vertices[3] = new VertexPositionColor(new Vector3(0.0f, axisLenght, 0.0f), Color.White);
            // Linha sobre o eixo Z
            vertices[4] = new VertexPositionColor(new Vector3(0.0f, 0.0f, -axisLenght), Color.Blue);
            vertices[5] = new VertexPositionColor(new Vector3(0.0f, 0.0f, axisLenght), Color.Blue);
        }



        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            //angulo += 1;
            // WorldMatrix
            effect.World = Matrix.Identity;


            //camera
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 3);
        }
    }
}
