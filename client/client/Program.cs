using System;
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
        private int nEventsFired = 0;
        private DateTime timeInitial;
        private double bulletXt;
        private double bulletYt;
        private ArrayList points = new ArrayList();

        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            tank1 = new Tank(1.0f , 0.0f , 0);
            tank2 = new Tank(0.3f, 0, 0);
            VSync = VSyncMode.On;
        }

        public static Func<TResult> Apply<TResult, TArg>(Func<TArg, TResult> func, TArg arg)
        {
            return () => func(arg);
        }

        public static Func<TResult> Apply<TResult, TArg1, TArg2>(Func<TArg1, TArg2, TResult> func,
                                                                  TArg1 arg1, TArg2 arg2)
        {
            return () => func(arg1, arg2);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);

           

            //GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            greenTank = LoadTexture("./tank.png", 1);
            blueTank = LoadTexture("./tank1.png", 1);
            greenTankWeapon = LoadTexture("./weapon.png", 1);
            blueTankWeapon = LoadTexture("./weapon1.png", 1);
            GL.Enable(EnableCap.Texture2D);
            //Basically enables the alpha channel to be used in the color buffer
            GL.Enable(EnableCap.Blend);
            //The operation/order to blend
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //Use for pixel depth comparing before storing in the depth buffer
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
            switch (quality)
            {
                case 0:
                default:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                    break;
                case 1:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                    break;
            }

            if (repeat)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            }

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
                tank1.x += 0.1f;
                tank2.x += 0.1f;
            }
            else if (Keyboard[Key.Left])
            {
                tank1.x -= 0.1f;
                tank2.x -= 0.1f;
            }
            else if (Keyboard[Key.Up])
            {
                tank1.angleDula +=  10;
                tank2.angleDula += 10;
            }
            else if (Keyboard[Key.Down])
            {
                tank1.angleDula -= 10;
                tank2.angleDula -= 10;
            }
            else if (Keyboard[Key.PageUp])
            {
                tank1.angleTank += 10;
                tank2.angleTank += 10;
            }
            else if (Keyboard[Key.PageDown])
            {
                tank1.angleTank -= 10;
                tank2.angleTank -= 10;
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
            drawTerrain(ClientRectangle.Width / 100, ClientRectangle.Height / 80, ClientRectangle.Height / 20, 0.9);
            drawTank(tank1, 1,e);
            drawTank(tank2, 2,e);
            SwapBuffers();
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
                GL.Rotate(tank.angleDula+180.0f, 0, 0, -4.0f);
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

        }

        private void fireBullet(Tank tank)
        {

            int angle = tank.angleDula + tank.angleTank;
            timeInitial = DateTime.Now;

            aTimer = new Timer(30);
            aTimer.Elapsed += delegate { drawBullet(tank, angle, tank.x, tank.y); };
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
       
        private void drawTerrain(double width, double height, double displace, double roughness)
        {
            if (points.Count == 0)
            {
                // Gives us a power of 2 based on our width
                int power = (int)Math.Pow(2, Math.Ceiling(Math.Log(width) / (Math.Log(2))));
                Random rnd = new Random();
                double rndValue1 = rnd.Next(0, 100);
                double rndValue2 = rnd.Next(0, 100);

                // Set the initial left point
                points.Add(height / 2 + (rndValue1 / 100 * displace * 2) - displace);
                // set the initial right point
                for (int k = 1; k < power; k++)
                {
                    points.Add(0);
                }
                points.Add(height / 2 + (rndValue2 / 100 * displace * 2) - displace);

                displace *= roughness;

                for (int i = 1; i < power; i *= 2)
                {
                    // Iterate through each segment calculating the center point
                    for (int j = (power / i) / 2; j < power; j += power / i)
                    {
                        double rndValueLocal = rnd.Next(0, 100);
                        double res = ((Convert.ToDouble(points[j - (power / i) / 2]) + Convert.ToDouble(points[j + (power / i) / 2])) / 2);
                        res += (rndValueLocal / 100 * displace * 2) - displace;
                        points.Insert(j, res);
                    }
                    // reduce our random range
                    displace *= roughness;
                }
            }


            
            GL.PushMatrix();
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.LineWidth(2);
            GL.Begin(BeginMode.LineStrip);
            for (double i = 0; i < points.Count; i++)
            {
                double pointValue = Convert.ToDouble(points[(int)i]);
                double x = i / 10;
                double y = pointValue / 1000;
                GL.Vertex2(x, y);
                Console.WriteLine(x + " " + y);
            }
            GL.End();
            GL.PopMatrix();
        }

        private void drawBullet(Tank tank, int angle, float X0, float Y0)
        {
            TimeSpan currentTime = DateTime.Now - timeInitial;
            int V0 = 3;
            int t = 10; //currentTime.Seconds * 1000 + currentTime.Milliseconds;
            double g = 9.8;

            double Vx0 = V0 * Math.Cos(angle);
            double Vy0 = V0 * Math.Sin(angle);

            double Xt = X0 + Vx0 * t;
            double Yt = Y0 + Vy0 * t - g * t * t / 2;

            Console.WriteLine("Math.Cos(angle): " + Math.Cos(angle));
            Console.WriteLine("angle: " + angle);
            Console.WriteLine("t: " + t);
            Console.WriteLine("Vx0: " + Vx0 + "; Vy0: " + Vy0);
            Console.WriteLine("Xt: " + Xt + "; Yt: " + Yt);
            Console.WriteLine("-------------------------");

            bulletXt = Xt;
            bulletYt = Yt;

        /*
            GL.PushMatrix();
            GL.Begin(BeginMode.Quads);
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Vertex2(bulletXt + 0.1f, bulletYt + 0.1f);
            GL.Vertex2(bulletXt - 0.1f, bulletYt + 0.1f);
            GL.Vertex2(bulletXt - 0.1f, bulletYt - 0.1f);
            GL.Vertex2(bulletXt + 0.1f, bulletYt - 0.1f);
            GL.End();
            GL.PopMatrix();
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
