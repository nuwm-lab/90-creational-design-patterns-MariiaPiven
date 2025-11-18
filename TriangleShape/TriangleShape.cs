using System.Drawing;

namespace ShapesBuilderSingleFile
{
    public class TriangleShape : ShapeBase
    {
        public float BaseWidth { get; }
        public float Height { get; }

        public TriangleShape(PointF pos, float baseWidth, float height, Color color) : base(pos, color)
        {
            BaseWidth = baseWidth;
            Height = height;
        }

        public override void Draw(Graphics g)
        {
            var p1 = new PointF(Position.X + BaseWidth / 2, Position.Y);
            var p2 = new PointF(Position.X, Position.Y + Height);
            var p3 = new PointF(Position.X + BaseWidth, Position.Y + Height);

            PointF[] pts = { p1, p2, p3 };

            using var brush = new SolidBrush(Color);
            g.FillPolygon(brush, pts);
            g.DrawPolygon(Pens.Black, pts);
        }
    }
}
