using System.Drawing;

namespace ShapesBuilderSingleFile
{
    public class EllipseShape : ShapeBase
    {
        public float Width { get; }
        public float Height { get; }

        public EllipseShape(PointF pos, float width, float height, Color color) : base(pos, color)
        {
            Width = width;
            Height = height;
        }

        public override void Draw(Graphics g)
        {
            using var brush = new SolidBrush(Color);
            g.FillEllipse(brush, Position.X, Position.Y, Width, Height);
            g.DrawEllipse(Pens.Black, Position.X, Position.Y, Width, Height);
        }
    }
}
