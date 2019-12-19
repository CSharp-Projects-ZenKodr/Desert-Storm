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
    public class Segment : Sphere, ICollider
    {
        public Vector3 start;
        Vector3 direction;
        Vector3 end;
        Vector3 segment;

        VertexPositionColor[] vertices;
        BasicEffect effect;
        GraphicsDevice device;

        public Segment(Game1 game, Vector3 start, Vector3 direction, Vector3 center, float radius) :base(center, radius)
        {
            this.start = start; //Initial Point
            this.direction = -direction;

            end = start + this.direction * 2; //end Point

            segment = end - start; //segment's Vector
            this.Center = Vector3.Zero;

            this.device = game.GraphicsDevice;
            effect = new BasicEffect(device);
            effect.VertexColorEnabled = true;

            CreateGeometry();

        }

        public void UpdatePosition(Vector3 position)
        {
            start = end;
            end = position + direction *2;
            

            segment = end - start;
            UpdateGeometry();
        }

        public override bool CollidesWith(Sphere other)
        {
            Vector3 bVector = other.Center - end; //Segment's End to Sphere's center Vector
            //bVector.Normalize();
            Vector3 aVector = other.Center - start; //Segment's start to Sphere's center Vector
            //aVector.Normalize();

            float bAngle = Vector3.Dot(direction, bVector); //Cos of the angle =  dot product of Segment's End to Sphere's center Vector
            float aAngle = Vector3.Dot(direction, aVector); //cos of the angle = dot product of Segment's start to Sphere's center Vector
            

            if (aAngle > 0 && bAngle < 0) //Check if angle of start-center is +90 and if end-center is -90
            {
                //Heron's Formula
                float a = (other.Center - end).Length(); //Lenght of center to segment's end
                float b = (other.Center - start).Length(); //lenght of center to segment's start
                float c = segment.Length(); // (start - end).Length(); //Lenght of segment                        
                    
                float s = (a + b + c) / 2f;
                float area = (float)Math.Sqrt(s * (s - a) * (s - b) * (s - c));
                //Console.WriteLine("area: " + area + " c: " + c);
                float h = (2f * area) / c;


                return (h < other.Radius);
            }
            else return false;
        }

        public override bool CollidesWith(Segment other)
        {
            return false;
        }

        void CreateGeometry()
        {
            int vertexCount = 2;
            vertices = new VertexPositionColor[vertexCount];

            vertices[0] = new VertexPositionColor(start, Color.Red);
            vertices[1] = new VertexPositionColor(end, Color.Red);
        }

        void UpdateGeometry()
        {
            vertices[0] = new VertexPositionColor(start, Color.Red);
            vertices[1] = new VertexPositionColor(end, Color.Red);
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            effect.World = Matrix.Identity;

            //camera
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            // Indica o efeito para desenhar os eixos
            effect.CurrentTechnique.Passes[0].Apply();


            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }


        public override Vector3 Position() { return end; }


    }
}
