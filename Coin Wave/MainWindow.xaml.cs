using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Coin_Wave
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameWindow window;
        private DispatcherTimer timer;
        private WriteableBitmap bitmap;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetupVievPort()
        {
            this.window = new GameWindow(500, 500, GraphicsMode.Default, "OpenGL Hiddeb Window");
            this.MakeContextCurrent(true);

            this.window.Size = new System.Drawing.Size(500, 500);
            GL.Viewport(0, 0, 500, 500);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 500, 0, 500, -1d, -1d);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            this.MakeContextCurrent(false);

            this.bitmap = new WriteableBitmap(500, 500, 96, 96, PixelFormats.Rgb24, null);
            this.VievPort.Source = this.bitmap;

            this.timer = new DispatcherTimer(DispatcherPriority.Render);
            this.timer.Interval = TimeSpan.FromMilliseconds(1000d / 30d); // 30 fps in millis
            this.timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e) 
        {
            this.MakeContextCurrent(true);

            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            
            //render square

            GL.PushMatrix();
            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(0, 1, 0);


            GL.End();
            GL.PopMatrix();

            // Transfer pixels from OpenGL to WPF

            // We are drawing into back byffers, so read from it
            GL.ReadBuffer(ReadBufferMode.Back);

            // Lock the bitmap, allowing access to the back buffer
            this.bitmap.Lock();

            // Transfer pixels from GL to the bitmap
            GL.ReadPixels(0, 0, 500, 500, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, this.bitmap.BackBuffer);

            // Tell the bitmap where the dirty data is
            this.bitmap.AddDirtyRect(new Int32Rect(0, 0, 500, 500));

            // Unlock the bitmap; lets the WPF engine be notified that bitmap has changes and can be re-render
            this.bitmap.Unlock();

            this.MakeContextCurrent(false);
        }

        public void MakeContextCurrent(bool valid)
        {
            if (valid)
            {
                this.window.MakeCurrent();
            }
            else
            {
                this.window.Context.MakeCurrent(null);
            }
        }
    }
}
