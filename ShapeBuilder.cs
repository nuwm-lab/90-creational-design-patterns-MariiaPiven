using System;
using System.Drawing;

namespace ShapesBuilderSingleFile
{
    public class ShapeBuilder
    {
        private ShapeType _type = ShapeType.Rectangle;
        private float _width = 80f;
        private float _height = 80f;
        private Color _color = Color.CornflowerBlue;
        private PointF _position = new PointF(10, 10);

        public ShapeBuilder SetType(ShapeType t) { _type = t; return this; }
        public ShapeBuilder SetSize(float w, float h) { _width = w; _height = h; return this; }
        public ShapeBuilder SetColor(Color c) { _color = c; return this; }
        public ShapeBuilder SetPosition(float x, float y) { _position = new PointF(x, y); return this; }

        public IShape Build()
        {
            return _type switch
            {
                ShapeType.Rectangle => new RectangleShape(_position, _width, _height, _color),
                ShapeType.Ellipse => new EllipseShape(_position, _width, _height, _color),
                ShapeType.Triangle => new TriangleShape(_position, _width, _height, _color),
                _ => throw new Exception("Unknown shape")
            };
        }
    }
}
