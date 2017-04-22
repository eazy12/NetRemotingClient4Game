using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace client
{
    class Game : GameWindow
    {
        Tank tank1, tank2;
        private int current_texture;
        private int texture;

        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            tank1 = new Tank(0, 0, 0);
            tank2 = new Tank(0.3f, 0, 0);
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);

           

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            current_texture = LoadTexture("C:\\tank.png", 1);

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
                tank1.x -= 0.1f;
                tank2.x -= 0.1f;
            }
            else if (Keyboard[Key.Left])
            {
                tank1.x += 0.1f;
                tank2.x += 0.1f;
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
            DrawTank(this.tank1, e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            

            SwapBuffers();
        }
        public void DrawTank(Tank tank, FrameEventArgs e)
        {

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.PushMatrix();
            GL.Translate(0, 0, -5);
            GL.Rotate(tank.angleTank, tank.x, tank.y, 4.0f);
         
            GL.Color4(Color.White);

            GL.BindTexture(TextureTarget.Texture2D, current_texture);

            GL.Begin(BeginMode.Quads);

            //Bind texture coordinates to vertices in ccw order

            //Top-Right
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(tank.x + 0.2f, tank.y + 0.2f);


            //Top-Left
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(tank.x - 0.2f, tank.y + 0.2f);

            //Bottom-Left
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(tank.x - 0.2f, tank.y - 0.2f);

            //Bottom-Right
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(tank.x + 0.2f, tank.y - 0.2f);

            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.PopMatrix();

            SwapBuffers();
            //GL.PushMatrix();
            //GL.Rotate(tank.angleTank, tank.x, tank.y, 4.0f);
            //GL.Begin(BeginMode.Quads);
            //GL.Color3(1.0f, 1.0f, 0.0f);
            //GL.Vertex3(tank.x-0.1f, tank.y- 0.05f, 4.0f);
            //GL.Vertex3(tank.x+0.1f, tank.y- 0.05f, 4.0f);
            //GL.Vertex3(tank.x+0.1f, tank.y+ 0.05f, 4.0f);
            //GL.Vertex3(tank.x-0.1f, tank.y+ 0.05f, 4.0f);

            //GL.End();



            //GL.Begin(BeginMode.Triangles);
            //GL.Color3(1.0f, 1.0f, 0.0f);
            //GL.Vertex3(tank.x - 0.05f, tank.y + 0.05f, 4.0f);
            //GL.Vertex3(tank.x , tank.y + 0.15f, 4.0f);
            //GL.Vertex3(tank.x + 0.05f, tank.y + 0.05f, 4.0f);

            //GL.End();

            //    GL.Begin(BeginMode.Lines);
            //    GL.Color3(1.0f, 1.0f, 0.0f);
            //    GL.Vertex3(tank.x + 0.05f / 2.0f, tank.y + 0.1f, 4.0f);
            //    GL.Vertex3(tank.x + 0.05f / 2.0f + 0.1f * Math.Cos(tank.angleDula * Math.PI / 180.0f), tank.y + 0.1f + 0.1f * Math.Sin(tank.angleDula * Math.PI / 180.0f), 4.0f);

            //    GL.End();
            //GL.PopMatrix();
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
