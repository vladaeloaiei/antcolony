using System;
using System.Drawing;
using System.Windows.Forms;

namespace AntColony
{
    public partial class AntWorldForm : Form
    {
        private WorldAgent _ownerAgent;
        private Bitmap _doubleBufferImage;

        public AntWorldForm()
        {
            InitializeComponent();
        }

        public void SetOwner(WorldAgent a)
        {
            _ownerAgent = a;
        }

        public void UpdateWorldGui()
        {
            DrawWorld();
        }

        private void DrawWorld()
        {
            var w = pictureBox.Width;
            var h = pictureBox.Height;

            if (_doubleBufferImage != null)
            {
                _doubleBufferImage.Dispose();
                GC.Collect(); // prevents memory leaks
            }

            _doubleBufferImage = new Bitmap(w, h);
            var g = Graphics.FromImage(_doubleBufferImage);
            g.Clear(Utils.BACKGROUND_COLOR);
            
            if (_ownerAgent != null)
            {
                foreach (var node in _ownerAgent.World.Graph.Keys)
                {
                    node.Draw(g);

                    foreach (var edge in _ownerAgent.World.Graph[node])
                    {
                        edge.Draw(g);
                    }
                }

                foreach (var ant in _ownerAgent.World.Ants.Values)
                {
                    ant.Draw(g);
                }
            }

            var pbg = pictureBox.CreateGraphics();
            pbg.DrawImage(_doubleBufferImage, 0, 0);
        }
    }
}