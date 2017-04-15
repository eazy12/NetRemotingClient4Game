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
        float x1, x2, x3, y1, y2, y3, angle;
        double angleValue;

        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            x1 = -1.0f;
            y1 = -1.0f;
            x2 = 1.0f;
            y2 = -1.0f;
            x3 = 0.0f;
            y3 = 1.0f;
            angle = 1;
            angleValue = 1;

            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
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
                x1 = x1 - 0.1f;
                x2 = x2 - 0.1f;
                x3 = x3 - 0.1f;
            }
            else if (Keyboard[Key.Left])
            {
                x1 = x1 + 0.1f;
                x2 = x2 + 0.1f;
                x3 = x3 + 0.1f;
            }
            else if (Keyboard[Key.Up])
            {
                angle = angle + 1;
            }
            else if (Keyboard[Key.Down])
            {
                angle = angle - 1;
            }

            if (Math.Abs(angle) > 360)
            {
                angle = 0;
            }

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            GL.PushMatrix();

            GL.Rotate(angle, 0, 0, 1);

            GL.Begin(BeginMode.Triangles);

            GL.Color3(1.0f, 1.0f, 0.0f); GL.Vertex3(x1, y1, 4.0f);
            GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex3(x2, y2, 4.0f);
            GL.Color3(0.2f, 0.9f, 1.0f); GL.Vertex3(x3, y3, 4.0f);

            GL.End();

            GL.PopMatrix();

            GL.PushMatrix();

            GL.Translate(1.0f, 0, 0);

            GL.Begin(BeginMode.Triangles);

            GL.Color3(1.0f, 1.0f, 0.0f); GL.Vertex3(x1, y1, 4.0f);
            GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex3(x2, y2, 4.0f);
            GL.Color3(0.2f, 0.9f, 1.0f); GL.Vertex3(x3, y3, 4.0f);

            GL.End();

            GL.PopMatrix();

            SwapBuffers();
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
