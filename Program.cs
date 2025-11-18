using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShapesBuilderSingleFile
{
public enum ShapeType
{
Rectangle,
Ellipse,
Triangle
}

public interface IShape
{
PointF Position { get; }
SizeF Size { get; }
Color Color { get; }
void Draw(Graphics g);
}

public abstract class ShapeBase : IShape
{
public PointF Position { get; protected set; }
public SizeF Size { get; protected set; }
public Color Color { get; protected set; }

protected ShapeBase(PointF pos, SizeF size, Color color)
{
Position = pos;
Size = size;
Color = color;
}

public abstract void Draw(Graphics g);
}

public class RectangleShape : ShapeBase
{
public RectangleShape(PointF pos, SizeF size, Color color) : base(pos, size, color) { }
public override void Draw(Graphics g)
{
using var brush = new SolidBrush(Color);
g.FillRectangle(brush, Position.X, Position.Y, Size.Width, Size.Height);
g.DrawRectangle(Pens.Black, Position.X, Position.Y, Size.Width, Size.Height);
}
}

public class EllipseShape : ShapeBase
{
public EllipseShape(PointF pos, SizeF size, Color color) : base(pos, size, color) { }
public override void Draw(Graphics g)
{
using var brush = new SolidBrush(Color);
g.FillEllipse(brush, Position.X, Position.Y, Size.Width, Size.Height);
g.DrawEllipse(Pens.Black, Position.X, Position.Y, Size.Width, Size.Height);
}
}

public class TriangleShape : ShapeBase
{
public TriangleShape(PointF pos, SizeF size, Color color) : base(pos, size, color) { }
public override void Draw(Graphics g)
{
var p1 = new PointF(Position.X + Size.Width / 2, Position.Y);
var p2 = new PointF(Position.X, Position.Y + Size.Height);
var p3 = new PointF(Position.X + Size.Width, Position.Y + Size.Height);

PointF[] pts = { p1, p2, p3 };

using var brush = new SolidBrush(Color);
g.FillPolygon(brush, pts);
g.DrawPolygon(Pens.Black, pts);
}
}

// === BUILDER ===
public class ShapeBuilder
{
private ShapeType _type = ShapeType.Rectangle;
private SizeF _size = new SizeF(80, 80);
private Color _color = Color.CornflowerBlue;
private PointF _position = new PointF(10, 10);

public ShapeBuilder SetType(ShapeType t) { _type = t; return this; }
public ShapeBuilder SetSize(float w, float h) { _size = new SizeF(w, h); return this; }
public ShapeBuilder SetColor(Color c) { _color = c; return this; }
public ShapeBuilder SetPosition(float x, float y) { _position = new PointF(x, y); return this; }

public IShape Build()
{
return _type switch
{
ShapeType.Rectangle => new RectangleShape(_position, _size, _color),
ShapeType.Ellipse => new EllipseShape(_position, _size, _color),
ShapeType.Triangle => new TriangleShape(_position, _size, _color),
_ => throw new Exception("Unknown shape")
};
}
}

// === ГОЛОВНА ФОРМА ===
public class MainForm : Form
{
private readonly List<IShape> shapes;
private readonly Panel canvas = new();

public MainForm(List<IShape> initialShapes = null)
{
	Text = "Builder – Shapes Demo";
	Size = new Size(900, 600);

	// initialize shapes list from provided initial shapes or empty
	shapes = initialShapes != null ? new List<IShape>(initialShapes) : new List<IShape>();

public Color Color { get; protected set; }

	protected ShapeBase(PointF pos, Color color)
	{
		Position = pos;
		Color = color;
	}

public abstract void Draw(Graphics g);
}

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

// === BUILDER ===
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

// === ГОЛОВНА ФОРМА ===
public class MainForm : Form
{
private readonly List<IShape> shapes;
private readonly Panel canvas = new();

public MainForm(List<IShape> initialShapes = null)
{
	Text = "Builder – Shapes Demo";
	Size = new Size(900, 600);

	// initialize shapes list from provided initial shapes or empty
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

// === ПУСК ===
internal static class Program
{
[STAThread]
static void Main()
{
	// Interactive creation of shapes before launching the form
	Console.WriteLine("--- Shapes Builder Interactive Setup ---");
	int count = PromptInt("How many shapes do you want to create? (1-6): ", 1, 12);
	var shapes = new List<IShape>();
	for (int i = 0; i < count; i++)
	{
		Console.WriteLine($"\nShape #{i + 1}:");
		var type = PromptShapeType();
		float w = PromptFloat("Enter width (or base) in pixels (number): ", 1, 1000);
		float h = PromptFloat("Enter height in pixels (number): ", 1, 1000);
		var colorName = PromptString("Enter color name (e.g. Red, Blue): ");
		Color color = Color.FromName(colorName);
		if (!color.IsKnownColor && color.A == 0)
		{
			Console.WriteLine("Unknown color name, defaulting to Black.");
			color = Color.Black;

// === ПУСК ===
internal static class Program
{
[STAThread]
static void Main()
{
	// Interactive creation of shapes before launching the form
	Console.WriteLine("--- Shapes Builder Interactive Setup ---");
	int count = PromptInt("How many shapes do you want to create? (1-6): ", 1, 12);
	var shapes = new List<IShape>();
	for (int i = 0; i < count; i++)
	{
		Console.WriteLine($"\nShape #{i + 1}:");
		var type = PromptShapeType();
		float w = PromptFloat("Enter width (or base) in pixels (number): ", 1, 1000);
		float h = PromptFloat("Enter height in pixels (number): ", 1, 1000);
		var colorName = PromptString("Enter color name (e.g. Red, Blue): ");
		Color color = Color.FromName(colorName);
		if (!color.IsKnownColor && color.A == 0)
		{
			Console.WriteLine("Unknown color name, defaulting to Black.");
			color = Color.Black;
		}

		// compute a position so shapes are not overlapping too much
		float x = 50 + i * 200 % Math.Max(200, (int)w + 20);
		float y = 50 + (i / 4) * 150;

		var shape = new ShapeBuilder()
			.SetType(type)
			.SetPosition(x, y)
			.SetColor(color)
			.SetSize(w, h)
			.Build();

		shapes.Add(shape);
	}

	ApplicationConfiguration.Initialize();
	Application.Run(new MainForm(shapes));
}

static int PromptInt(string prompt, int min, int max)
{
	while (true)
	{
		Console.Write(prompt);
		var s = Console.ReadLine();
		if (int.TryParse(s, out var v) && v >= min && v <= max) return v;
		Console.WriteLine($"Enter integer between {min} and {max}.");
	}
}

static float PromptFloat(string prompt, float min, float max)
{
	while (true)
	{
		Console.Write(prompt);
		var s = Console.ReadLine();
		if (float.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v) || float.TryParse(s, out v))
		{
			if (v >= min && v <= max) return v;
		}
		Console.WriteLine($"Enter number between {min} and {max}.");
	}
}

		// compute a position so shapes are not overlapping too much
		float x = 50 + i * 200 % Math.Max(200, (int)w + 20);
		float y = 50 + (i / 4) * 150;

		var shape = new ShapeBuilder()
			.SetType(type)
			.SetPosition(x, y)
			.SetColor(color)
			.SetSize(w, h)
			.Build();

		shapes.Add(shape);
	}

	ApplicationConfiguration.Initialize();
	Application.Run(new MainForm(shapes));
}

static int PromptInt(string prompt, int min, int max)
static string PromptString(string prompt)
{
	while (true)
	{
		Console.Write(prompt);
		var s = Console.ReadLine();
		if (int.TryParse(s, out var v) && v >= min && v <= max) return v;
		Console.WriteLine($"Enter integer between {min} and {max}.");
	}
}

static float PromptFloat(string prompt, float min, float max)
{
	while (true)
	{
		Console.Write(prompt);
		var s = Console.ReadLine();
		if (float.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v) || float.TryParse(s, out v))
		{
			if (v >= min && v <= max) return v;
		}
		Console.WriteLine($"Enter number between {min} and {max}.");
	}
}

static string PromptString(string prompt)
{
	while (true)
	{
		Console.Write(prompt);
		var s = Console.ReadLine();
		if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
		Console.WriteLine("Please enter a non-empty value.");
	}
}

static ShapeType PromptShapeType()
{
	while (true)
	{
		Console.WriteLine("Choose shape type: (1) Rectangle  (2) Ellipse  (3) Triangle");
		Console.Write("Enter number or name: ");
		var input = Console.ReadLine()?.Trim();
		if (string.IsNullOrEmpty(input)) continue;

		if (int.TryParse(input, out var n))
		{
			if (n == 1) return ShapeType.Rectangle;
			if (n == 2) return ShapeType.Ellipse;
			if (n == 3) return ShapeType.Triangle;
		}
		if (Enum.TryParse<ShapeType>(input, true, out var t)) return t;
		Console.WriteLine("Invalid selection. Try again.");
	}
}
		if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
		Console.WriteLine("Please enter a non-empty value.");
	}
}

static ShapeType PromptShapeType()
{
	while (true)
	{
		Console.WriteLine("Choose shape type: (1) Rectangle  (2) Ellipse  (3) Triangle");
		Console.Write("Enter number or name: ");
		var input = Console.ReadLine()?.Trim();
		if (string.IsNullOrEmpty(input)) continue;

		if (int.TryParse(input, out var n))
		{
			if (n == 1) return ShapeType.Rectangle;
			if (n == 2) return ShapeType.Ellipse;
			if (n == 3) return ShapeType.Triangle;
		}
		if (Enum.TryParse<ShapeType>(input, true, out var t)) return t;
		Console.WriteLine("Invalid selection. Try again.");
	}
}
}
}
