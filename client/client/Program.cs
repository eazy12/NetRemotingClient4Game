﻿using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Timers;
using System.Collections;



namespace client
{
    class Game : GameWindow
    {
        Tank tank1, tank2;
        private int greenTank;
        private int blueTank;
        private int greenTankWeapon;
        private int blueTankWeapon;
        private Timer aTimer;
        private DateTime timeInitial;
        private double bulletXt;
        private double bulletYt = 0.4;
        private ArrayList terrainPoints = new ArrayList();
        private bool initTank = false;
        Random rnd = new Random();
        double rand;
        bool boom = false;
        double boomX = 0.0;
        double boomY = 0.0;

        double initialX = 0;


        private const double TANK_WIDTH = 0.25 / 4;
        private const double TANK_HEIGHT = 0.25 / 4;
        private const double TANK_SPEED = 0.01;
        private const int LEFT_EXTREME_COORD = -1;
        private const int RIGHT_EXTREME_COORD = 1;
        private const int TERRAIN_POINTS_COUNT = 200;

        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            tank1 = new Tank(TANK_WIDTH, TANK_HEIGHT, TANK_SPEED, -1 + TANK_WIDTH, 0, 0);
            tank2 = new Tank(TANK_WIDTH, TANK_HEIGHT, TANK_SPEED, 0.3, 0, 0);
            rand = rnd.Next(8, 10);
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);

            greenTank = LoadTexture("./tank.png", 1);
            blueTank = LoadTexture("./tank1.png", 1);
            greenTankWeapon = LoadTexture("./weapon.png", 1);
            blueTankWeapon = LoadTexture("./weapon1.png", 1);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);

        }

        private int LoadTexture(string path, int quality = 0, bool repeat = true, bool flip_y = false)
        {
            Bitmap bitmap = new Bitmap(path);

            if (flip_y)
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            int texture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

            System.Drawing.Imaging.BitmapData bitmap_data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bitmap.Width, bitmap.Height, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap_data.Scan0);
            bitmap.UnlockBits(bitmap_data);
            bitmap.Dispose();
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return texture;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();
            else if (Keyboard[Key.Right])
            {
                moveTankRight(tank1);
               
            }
            else if (Keyboard[Key.Left])
            {
                moveTankLeft(tank1);
            }
            else if (Keyboard[Key.Up])
            {
                tank1.angleDula += 10;
                tank2.angleDula += 10;
            }
            else if (Keyboard[Key.Down])
            {
                tank1.angleDula -= 10;
                tank2.angleDula -= 10;
            }
            else if (Keyboard[Key.Space])
            {
                fireBullet(tank1);
            }

            if (tank1.angleDula > 65)
            {
                tank1.angleDula = 65;
                tank2.angleDula = 65;
            }
            else if (tank1.angleDula < -15)
            {
                tank1.angleDula = -15;
                tank2.angleDula = -15;
            }

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref modelview);
            drawTerrain(ClientRectangle.Width, ClientRectangle.Height / 4, ClientRectangle.Height / 100, 0.8);
            drawTank(tank1, 1, e);
            //drawTank(tank2, 2, e);
            SwapBuffers();

            if (initTank == false)
            {
                initTank = true;
                moveTankRight(tank1);
            }
        }

        private void moveTankRight(Tank tank)
        {
            if (tank.x + TANK_SPEED + TANK_WIDTH > RIGHT_EXTREME_COORD)
            {
                tank.x = RIGHT_EXTREME_COORD - TANK_WIDTH;
            }

            tank.x += TANK_SPEED;
            tank.y = getHeightByX(tank.x);
            tank.terrainHeight = getHeightByX(tank.x);
        }

        private void moveTankLeft(Tank tank)
        {
            if (tank.x - TANK_SPEED - TANK_WIDTH < LEFT_EXTREME_COORD)
            {
                tank.x = LEFT_EXTREME_COORD + TANK_WIDTH;
            }

            tank.x -= TANK_SPEED;
            tank.y = getHeightByX(tank.x);
            tank.terrainHeight = getHeightByX(tank.x);
        }

        public void drawTank(Tank tank, int number, FrameEventArgs e)
        {
            if (number == 1)
            {
                GL.LoadIdentity();
                GL.PushMatrix();
                GL.Translate(tank.x, tank.y, 0);
                GL.Rotate(tank.angleTank, 0, 0, 1.0f);
                GL.BindTexture(TextureTarget.Texture2D, greenTank);
                GL.Begin(BeginMode.Quads);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(0.25f / 4, 0.25f / 4);
                GL.TexCoord2(0f, 0f); GL.Vertex2(-0.25f / 4, 0.25f / 4);
                GL.TexCoord2(0f, 1f); GL.Vertex2(-0.25f / 4, -0.25f / 4);
                GL.TexCoord2(1f, 1f); GL.Vertex2(0.25f / 4, -0.25f / 4);
                GL.End();


                GL.BindTexture(TextureTarget.Texture2D, greenTankWeapon);
                GL.Translate(0.05 / 4, 0.1 / 4, -0.001f);
                GL.Rotate(tank.angleDula, 0, 0, 4.0f);
                GL.Begin(BeginMode.Quads);

                GL.TexCoord3(1.0f, 0.0f, 0.0f); GL.Vertex2(0.4f / 4, 0.0f / 4);
                GL.TexCoord3(0f, 0f, 0.0f); GL.Vertex2(0.0f / 4, 0.0f / 4);
                GL.TexCoord3(0f, 1f, 0.0f); GL.Vertex2(0.0f / 4, 0.05f / 4);
                GL.TexCoord3(1f, 1f, 0.0f); GL.Vertex2(0.4f / 4, 0.05f / 4);
                GL.End();
                GL.PopMatrix();

            }
            else
            {
                GL.LoadIdentity();
                GL.PushMatrix();
                GL.Translate(tank.x, tank.y, 0);
                GL.Rotate(tank.angleTank, 0, 0, 1.0f);
                GL.BindTexture(TextureTarget.Texture2D, blueTank);
                GL.Begin(BeginMode.Quads);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(0.25f / 4, 0.25f / 4);
                GL.TexCoord2(0f, 0f); GL.Vertex2(-0.25f / 4, 0.25f / 4);
                GL.TexCoord2(0f, 1f); GL.Vertex2(-0.25f / 4, -0.25f / 4);
                GL.TexCoord2(1f, 1f); GL.Vertex2(0.25f / 4, -0.25f / 4);
                GL.End();


                GL.BindTexture(TextureTarget.Texture2D, blueTankWeapon);
                GL.Translate(0.05 / 4, 0.1 / 4, -0.001f);
                GL.Rotate(tank.angleDula + 180.0f, 0, 0, -4.0f);
                GL.Begin(BeginMode.Quads);

                GL.TexCoord3(1.0f, 0.0f, 0.0f); GL.Vertex2(0.4f / 4, 0.0f / 4);
                GL.TexCoord3(0f, 0f, 0.0f); GL.Vertex2(0.0f / 4, 0.0f / 4);
                GL.TexCoord3(0f, 1f, 0.0f); GL.Vertex2(0.0f / 4, 0.05f / 4);
                GL.TexCoord3(1f, 1f, 0.0f); GL.Vertex2(0.4f / 4, 0.05f / 4);
                GL.End();
                GL.PopMatrix();


            }
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.LoadIdentity();
            GL.PushMatrix();
            GL.PointSize(15);
            GL.Color3(1.0f, 1.0f, 0.0f);
            //Console.WriteLine(initialX + " " + bulletYt);
            GL.Begin(BeginMode.Points);
            GL.Vertex2(initialX, bulletYt);
            GL.End();
            GL.PopMatrix();

            GL.LoadIdentity();

            if (boom == true)
            {
                GL.LoadIdentity();
                GL.PushMatrix();
                GL.PointSize(25);
                GL.Color3(1.0f, 0.0f, 0.0f);
                //Console.WriteLine(initialX + " " + bulletYt);
                GL.Begin(BeginMode.Points);
                GL.Vertex2(boomX, boomY);
                GL.End();
                GL.PopMatrix();

                GL.LoadIdentity();
            }

        }

        private void fireBullet(Tank tank)
        {
            initialX = tank.x;
            Console.WriteLine(initialX);
            double angle = tank.angleDula + tank.angleTank;
            timeInitial = DateTime.Now;

            aTimer = new Timer(1000);
            aTimer.Elapsed += delegate { drawBullet(tank, angle, tank.x, tank.y + 0.01); };
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
       
        private void drawTerrain(double width, double height, double displace, double roughness)
        {
            double amplitude = 0.1;

            if (terrainPoints.Count == 0)
            {
                Random rnd = new Random();
                double rand = rnd.Next(8, 10);
                Console.WriteLine(rand);
                for (int x = 0; x < TERRAIN_POINTS_COUNT; x++)
                {
                    double val = amplitude * Math.Sin(x / rand);
                    terrainPoints.Add(val);
                }
            }


            GL.LoadIdentity();
            GL.PushMatrix();
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.LineWidth(5);
            GL.Begin(BeginMode.LineStrip);

            for (double i = 0; i < TERRAIN_POINTS_COUNT; i++)
            {
                double pointValue = Convert.ToDouble(terrainPoints[(int)i]);
                double x = (i - 100) / 100;
                double y = pointValue;
                
                GL.Vertex2(x, y);
            }
            
            GL.End();
            GL.PopMatrix();
            GL.LoadIdentity();
        }

        private double getHeightByX(double x)
        {
            x = (x + 1) * 100;
            return Convert.ToDouble(terrainPoints[(int)x]);
        }

        private void drawBullet(Tank tank, double angle, double X0, double Y0)
        {
            // УБРАТЬ t, ПРИНЯТЬ ЗА T XT, А ПРИРАЩЕНИЕ ИКСА ЭТО СКОРОСТЬ 
            if(LEFT_EXTREME_COORD > initialX  || initialX >RIGHT_EXTREME_COORD )
            {
                Console.WriteLine("За границей");
                aTimer.Enabled = false;
                aTimer.AutoReset = false;
                return;
            }
            
            TimeSpan currentTime = DateTime.Now - timeInitial;
            double V0 = 0.01;
            int t = 10; //currentTime.Seconds * 1000 + currentTime.Milliseconds;
            double g = 1.0;

            double Vx0 = V0 * Math.Cos(angle * 180.0 / Math.PI);
            double Vy0 = V0 * Math.Sin(angle * 180.0 / Math.PI);
            
            double Yt;
            Yt = Y0 + Vy0 * initialX - g * initialX * initialX / 2;
            initialX = initialX + V0;
            double yterrain;
            yterrain = 0.1 * Math.Sin(initialX / rand);
            bulletYt = Yt;
            //Console.WriteLine(yterrain + " " + rand  + " " + Yt);
            if (Math.Abs(yterrain - Yt)  < 0.25f )
            {
                Console.WriteLine("Boom");
                if ( boom == false ){
                    boom = true;
                    boomX = initialX;
                    boomY = bulletYt;
                }
                
                aTimer.Enabled = false;
                aTimer.AutoReset = false;
                return;
            }

            //Console.WriteLine("Math.Cos(angle): " + Math.Cos(angle));
            //Console.WriteLine("angle: " + angle);
            //Console.WriteLine("t: " + t);
            //Console.WriteLine("Vx0: " + Vx0 + "; Vy0: " + Vy0);
            //Console.WriteLine("Xt: " + Xt + "; Yt: " + Yt);
            //Console.WriteLine("-------------------------");

            Console.WriteLine(Yt);
            
            /*
            GL.LoadIdentity();
            GL.PushMatrix();
            GL.PointSize(25);
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Begin(BeginMode.Points);
            Console.WriteLine(initialX + " " + Yt);
            GL.Vertex2(initialX, Yt);
            GL.End();
            GL.PopMatrix();
            */
    }

    [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
    }
}
