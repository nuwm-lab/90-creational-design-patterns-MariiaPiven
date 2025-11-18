using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShapesBuilderSingleFile
{
    public class MainForm : Form
    {
        private readonly List<IShape> shapes;
        private readonly Panel canvas = new();

        public MainForm(List<IShape> initialShapes = null)
        {
            Text = "Builder – Shapes Demo";
            Size = new Size(900, 600);

            shapes = initialShapes != null ? new List<IShape>(initialShapes) : new List<IShape>();

            canvas.Location = new Point(10, 10);
            canvas.Size = new Size(860, 540);
            canvas.BackColor = Color.White;
            canvas.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                foreach (var shape in shapes)
                    shape.Draw(e.Graphics);
            };
            Controls.Add(canvas);
        }
    }
}
