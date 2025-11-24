using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Drawing;

namespace ConsoleApp3
{
    class Window3D : GameWindow
    {
        private const float rotationSpeed = 1.0f; // radians per second
        private const float moveStep = 0.1f;

        private float angle2 = 0.0f;

        // Light blue cube position (movable)
        private Vector3 lightBlueCubePos1 = new Vector3(2.5f, 0.0f, 0.0f);

        public Window3D() : base(1280, 768, new GraphicsMode(32, 24, 0, 8))
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);

            GL.ClearColor(Color.FromArgb(49, 50, 51));

            // Console instructions for movement
            Console.WriteLine("Use W/S/A/D to move the light blue cube.");
            Console.WriteLine("W/S: move up/down   A/D: move left/right");
            Console.WriteLine("ESC: exit");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, (float)Width / Height, 1.0f, 1024.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            Matrix4 lookat = Matrix4.LookAt(
                new Vector3(5, 5, 10), // Camera position
                Vector3.Zero,          // Target
                Vector3.UnitY);        // Up vector
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            angle2 += (float)e.Time * rotationSpeed;

            var keyboard = Keyboard.GetState();
            if (keyboard[Key.W])
                lightBlueCubePos1.Y += moveStep;
            if (keyboard[Key.S])
                lightBlueCubePos1.Y -= moveStep;
            if (keyboard[Key.A])
                lightBlueCubePos1.X -= moveStep;
            if (keyboard[Key.D])
                lightBlueCubePos1.X += moveStep;

            if (keyboard[Key.Escape])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Light blue cube 2 rotates on YZ
            float radius = 2.5f;
            Vector3 lightBlueCubePos2 = new Vector3(
                0.0f,
                (float)Math.Cos(angle2) * radius,
                (float)Math.Sin(angle2) * radius
            );

            // Set up Light0 (light blue, movable)
            float[] lightBlue = { 0.68f, 0.85f, 0.90f, 1.0f }; // light blue
            float[] light0_position = { lightBlueCubePos1.X, lightBlueCubePos1.Y, lightBlueCubePos1.Z, 1.0f };
            GL.Light(LightName.Light0, LightParameter.Position, light0_position);
            GL.Light(LightName.Light0, LightParameter.Diffuse, lightBlue);
            GL.Light(LightName.Light0, LightParameter.Specular, lightBlue);

            // Set up Light1 (light blue, rotates on YZ)
            float[] light1_position = { lightBlueCubePos2.X, lightBlueCubePos2.Y, lightBlueCubePos2.Z, 1.0f };
            GL.Light(LightName.Light1, LightParameter.Position, light1_position);
            GL.Light(LightName.Light1, LightParameter.Diffuse, lightBlue);
            GL.Light(LightName.Light1, LightParameter.Specular, lightBlue);

            // Draw the gray cube
            ResetMaterial();
            GL.PushMatrix();
            GL.Translate(0.0f, 0.0f, 0.0f);
            DrawCube(1.0f, Color.Gray);
            GL.PopMatrix();

            // Draw light blue cube for Light0 (movable)
            SetLightBlueMaterial();
            GL.PushMatrix();
            GL.Translate(lightBlueCubePos1.X, lightBlueCubePos1.Y, lightBlueCubePos1.Z);
            DrawCube(0.3f, Color.LightBlue);
            GL.PopMatrix();

            // Draw light blue cube for Light1 (rotating)
            SetLightBlueMaterial();
            GL.PushMatrix();
            GL.Translate(lightBlueCubePos2.X, lightBlueCubePos2.Y, lightBlueCubePos2.Z);
            DrawCube(0.3f, Color.LightBlue);
            GL.PopMatrix();

            SwapBuffers();
        }

        private void DrawCube(float size, Color color)
        {
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(color);
            // Front face
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(-size, size, size);

            // Back face
            GL.Normal3(0.0f, 0.0f, -1.0f);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, -size, -size);

            // Left face
            GL.Normal3(-1.0f, 0.0f, 0.0f);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(-size, size, -size);

            // Right face
            GL.Normal3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, -size, size);

            // Top face
            GL.Normal3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, size, -size);

            // Bottom face
            GL.Normal3(0.0f, -1.0f, 0.0f);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(-size, -size, size);

            GL.End();
        }

        // Resets material to default for the gray cube
        private void ResetMaterial()
        {
            float[] ambient = { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] diffuse = { 0.5f, 0.5f, 0.5f, 1.0f }; // gray
            float[] specular = { 0.0f, 0.0f, 0.0f, 1.0f };
            float shininess = 0.0f;

            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, ambient);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, diffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, specular);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, shininess);
        }

        // Sets material for light blue cubes (light source)
        private void SetLightBlueMaterial()
        {
            float[] ambient = { 0.68f, 0.85f, 0.90f, 1.0f };
            float[] diffuse = { 0.68f, 0.85f, 0.90f, 1.0f };
            float[] specular = { 0.68f, 0.85f, 0.90f, 1.0f };
            float shininess = 32.0f;

            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, ambient);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, diffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, specular);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, shininess);
        }
    }
}
