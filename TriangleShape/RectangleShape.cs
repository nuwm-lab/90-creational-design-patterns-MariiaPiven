using System.Drawing;

namespace ShapesBuilderSingleFile
{
    public class RectangleShape : ShapeBase
    {
        public float Width { get; }
        public float Height { get; }

        public RectangleShape(PointF pos, float width, float height, Color color) : base(pos, color)
        {
            Width = width;
            Height = height;
        }

        public override void Draw(Graphics g)
        {
            using var brush = new SolidBrush(Color);
            g.FillRectangle(brush, Position.X, Position.Y, Width, Height);
            g.DrawRectangle(Pens.Black, Position.X, Position.Y, Width, Height);
        }
    }
}
