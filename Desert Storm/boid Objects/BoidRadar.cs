using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Desert_Storm
{

    class BoidRadar : ICollider
    {
        Vector3 center;
        float radius;
        Vector3 targetPos;
        public Vector3 newDirection;
        VertexPositionColor[] vertices;
        VertexPositionColor[] lineVertices;
        BasicEffect effect;
        Game1 game;
        Matrix world;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        int indexCount;
        short[] indices;
        float wanderTimer;
        public float newDir;
        Sphere Radar;
        Tank tank;

        int sides = 40;
        public Vector3? EnemyPosition;

        public BoidRadar(Game1 game, Tank tank)
        {
            Radar = new Sphere(tank.position, 30);
            game.colliders.Add(this as ICollider);
            this.tank = tank;
            this.center = tank.position - tank.direction * 4;
            this.radius = 3;
            newDirection = tank.direction;

            this.game = game;
            effect = new BasicEffect(game.GraphicsDevice);
            effect.VertexColorEnabled = true;

            wanderTimer = 10f;

            EnemyPosition = null;

            createLineGeometry();
            createGeometry();
       
            world = Matrix.CreateScale(1f) * Matrix.CreateFromYawPitchRoll(0, 0, 0) * Matrix.CreateTranslation(center);
        }


        public void update(GameTime gt)
        {
            if (tank.aiMode == Tank.AImode.WANDER) wanderTimer += (float)gt.ElapsedGameTime.TotalSeconds;


            updateLocation();

            if (wanderTimer > 0.5f) {
                    CreateNewTarget(); wanderTimer = 0;
            }

            if (tank.aiMode == Tank.AImode.PERSUIT) { CreateEnemyDirection(); }

        }

        public void updateLocation()
        {
            center = tank.position - tank.direction * 4;
            Radar.Center = tank.position;

            world = Matrix.CreateScale(1f) * Matrix.CreateFromYawPitchRoll(tank.yaw, 0, 0) * Matrix.CreateTranslation(center);
        }

        void CreateNewTarget()
        {
            float newAngle;

            if (center.X < game.map.size.X - 10 && center.X - 10 > 0 && center.Z < game.map.size.Y - 10 && center.Z - 10 > 0)
            {
                 newAngle = game.rng.Next(45, 136);
            } else
            {
                newAngle = 0; //Forces the tank to turn arround if at the edge of the map
            }
            if (newAngle < 90) { newDir = 1f; }
            else { newDir = -1f;  }
            newAngle = MathHelper.ToRadians(newAngle);
            targetPos = new Vector3(center.X + radius * (float)Math.Cos(newAngle), center.Y, center.Z + radius * (float)Math.Sin(newAngle));
            newDirection = targetPos - center;
            newDirection.Normalize();
            
            UpdateLineGeometry();
        }        

        void CreateEnemyDirection()
        {
            targetPos = (Vector3)EnemyPosition;
            newDirection = tank.position - targetPos;
            newDirection.Normalize();

            UpdateLineGeometry();
        }


        public void draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            effect.World = world;

            //camera
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            effect.CurrentTechnique.Passes[0].Apply();

            if (game.debug) game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, lineVertices, 0, 1);


            game.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            game.GraphicsDevice.Indices = indexBuffer;

            if (game.debug) game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineStrip,0,0,indexCount);
 

        }


        void createGeometry()
        {
            int vertexCount = (sides * 2);
            vertices = new VertexPositionColor[vertexCount];

            for (int i = 0; i <= sides; i++)
            {
                float Angle = ((float)i * (2 * MathHelper.Pi / (float)sides));
                float x = radius * (float)Math.Cos(Angle);
                float z = radius * -(float)Math.Sin(Angle);


                vertices[i] = new VertexPositionColor(new Vector3(x, 2, z), Color.Blue);
            }

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColor), vertexCount, BufferUsage.None); //cria o VertexBuffer
            vertexBuffer.SetData<VertexPositionColor>(vertices); //copia os dados para o VertexBuffer

            //indices
            indexCount = 2 * (sides);
            indices = new short[indexCount];


            for (int i = 0; i < sides + 1; i++)
            {
                indices[i] = (short)i;
            }

            indexBuffer = new IndexBuffer(game.GraphicsDevice, typeof(short), indexCount, BufferUsage.None); //cria o IndexBuffer
            indexBuffer.SetData<short>(indices); //copia os dados para o IndexBuffer
        }

        void createLineGeometry()
        {
            int vertexCount = 2;
            lineVertices = new VertexPositionColor[vertexCount];
            Vector3 start = Vector3.Zero;
            start.Y += 2;
            Vector3 temptarget = start + newDirection;

            lineVertices[0] = new VertexPositionColor(start, Color.Pink);
            lineVertices[1] = new VertexPositionColor(temptarget, Color.Pink);
        }

        void UpdateLineGeometry()
        {
            Vector3 start = Vector3.Zero;
            start.Y += 2;
            Vector3 temptarget = start + newDirection;




            lineVertices[0] = new VertexPositionColor(start, Color.Pink);
            lineVertices[1] = new VertexPositionColor(temptarget, Color.Pink);
        }

        public string Name() => "Radar";


        public string SubName() => "Radar";


        public int? ID() => tank.id;


        public Vector3 Position() => tank.position;

        public bool Active() => true;


        public void CollisionWith(ICollider other)
        {
            
            if (other.Active() && other.ID() != tank.id && other.Name() == "Tank" && tank.AIControlled == true)
            {
               // tank.aiMode = Tank.AImode.PERSUIT;
                EnemyPosition = other.Position();
            }


        }

        public bool CollidesWith(ICollider other) => Radar.CollidesWith(other);


        public ICollider GetCollider() => Radar;
    }
}
