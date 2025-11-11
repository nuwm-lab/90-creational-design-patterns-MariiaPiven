using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace CreationalDesignPatterns
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("--- Shape Builder Interactive Demo ---");

			var type = PromptShapeType();
			Shape shape = null;

			switch (type)
			{
				case ShapeType.Circle:
					var radius = PromptDouble("Enter radius (number): ", min: 0.1);
					var colorC = PromptString("Enter color: ");
					shape = new CircleBuilder().Reset().SetRadius(radius).SetColor(colorC).Build();
					break;
				case ShapeType.Rectangle:
					var width = PromptDouble("Enter width (number): ", min: 1);
					var height = PromptDouble("Enter height (number): ", min: 1);
					var colorR = PromptString("Enter color: ");
					shape = new RectangleBuilder().Reset().SetWidthHeight(width, height).SetColor(colorR).Build();
					break;
				case ShapeType.Triangle:
					var b = PromptDouble("Enter base (number): ", min: 1);
					var h = PromptDouble("Enter height (number): ", min: 1);
					var colorT = PromptString("Enter color: ");
					shape = new TriangleBuilder().Reset().SetBaseHeight(b, h).SetColor(colorT).Build();
					break;
			}

			// Basic null/validation checks after construction
			if (shape == null)
			{
				Console.WriteLine("Shape construction failed or returned null.");
				return;
			}

			if (!IsValidShape(shape, out var reason))
			{
				Console.WriteLine($"Constructed shape is invalid: {reason}");
				return;
			}

			Console.WriteLine();
			Console.WriteLine("Built shape:");
			Console.WriteLine(shape);
			Console.WriteLine();
			Console.WriteLine("Drawing (ASCII):");
			DrawShape(shape);

			Console.WriteLine();
			Console.Write("Open graphical window to display shape? (y/n): ");
			var ans = Console.ReadLine()?.Trim().ToLowerInvariant();
			if (ans == "y" || ans == "yes")
			{
				ShowShapeWindow(shape);
			}
		}

		static void ShowShapeWindow(Shape shape)
		{
			// Run WinForms on an STA thread
			var t = new Thread(() =>
			{
				try
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new ShapeForm(shape));
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to show window: {ex.Message}");
				}
			});
			t.SetApartmentState(ApartmentState.STA);
			t.Start();
			t.Join();
		}

	// --- Merged types from Shapes/ and UI/ ---

	public enum ShapeType
	{
		Circle,
		Rectangle,
		Triangle
	}

	public class Shape
	{
		// Type of the shape
		public ShapeType Type { get; set; }

		// Primary size: radius for Circle, width for Rectangle, base for Triangle
		public double Size1 { get; set; }

		// Secondary size: height for Rectangle/Triangle; unused for Circle
		public double Size2 { get; set; }

		// Color as a simple string for demo purposes
		public string Color { get; set; }

		public override string ToString()
		{
			return Type switch
			{
				ShapeType.Circle => $"Circle (radius={Size1}, color={Color})",
				ShapeType.Rectangle => $"Rectangle (width={Size1}, height={Size2}, color={Color})",
				ShapeType.Triangle => $"Triangle (base={Size1}, height={Size2}, color={Color})",
				_ => $"Unknown shape (color={Color})"
			};
		}
	}

	public interface IShapeBuilder
	{
		// Reset internal builder state
		IShapeBuilder Reset();

		// Set the shape type (Circle, Rectangle, Triangle)
		IShapeBuilder SetType(ShapeType type);

		// Set sizes (one or two values depending on shape)
		IShapeBuilder SetSize(double size1, double size2 = 0);

		// Set color
		IShapeBuilder SetColor(string color);

		// Return built Shape
		Shape Build();
	}

	// A generic/fluent builder implementation that can produce different shapes
	public class ShapeBuilder : IShapeBuilder
	{
		protected Shape shape;

		public ShapeBuilder()
		{
			Reset();
		}

		public virtual IShapeBuilder Reset()
		{
			shape = new Shape();
			return this;
		}

		public virtual IShapeBuilder SetType(ShapeType type)
		{
			shape.Type = type;
			return this;
		}

		public virtual IShapeBuilder SetSize(double size1, double size2 = 0)
		{
			shape.Size1 = size1;
			shape.Size2 = size2;
			return this;
		}

		public virtual IShapeBuilder SetColor(string color)
		{
			shape.Color = color;
			return this;
		}

		public virtual Shape Build()
		{
			// Return a copy to avoid later mutation by the builder
			var result = new Shape
			{
				Type = shape.Type,
				Size1 = shape.Size1,
				Size2 = shape.Size2,
				Color = shape.Color
			};
			// Prepare builder for possible reuse
			Reset();
			return result;
		}
	}

	// Convenience concrete builder for Circle
	public class CircleBuilder : ShapeBuilder
	{
		public CircleBuilder()
		{
			SetType(ShapeType.Circle);
		}

		// Ensure Reset returns the concrete builder so fluent chains keep concrete methods like SetRadius
		public new CircleBuilder Reset()
		{
			base.Reset();
			SetType(ShapeType.Circle);
			return this;
		}

		public CircleBuilder SetRadius(double radius)
		{
			SetSize(radius);
			return this;
		}

		public new CircleBuilder SetColor(string color)
		{
			base.SetColor(color);
			return this;
		}
	}

	// Convenience concrete builder for Rectangle
	public class RectangleBuilder : ShapeBuilder
	{
		public RectangleBuilder()
		{
			SetType(ShapeType.Rectangle);
		}

		public new RectangleBuilder Reset()
		{
			base.Reset();
			SetType(ShapeType.Rectangle);
			return this;
		}

		public RectangleBuilder SetWidthHeight(double width, double height)
		{
			SetSize(width, height);
			return this;
		}

		public new RectangleBuilder SetColor(string color)
		{
			base.SetColor(color);
			return this;
		}
	}

	// Convenience concrete builder for Triangle
	public class TriangleBuilder : ShapeBuilder
	{
		public TriangleBuilder()
		{
			SetType(ShapeType.Triangle);
		}

		public new TriangleBuilder Reset()
		{
			base.Reset();
			SetType(ShapeType.Triangle);
			return this;
		}

		public TriangleBuilder SetBaseHeight(double b, double h)
		{
			SetSize(b, h);
			return this;
		}

		public new TriangleBuilder SetColor(string color)
		{
			base.SetColor(color);
			return this;
		}
	}

	// Director orchestrates common construction sequences
	public class ShapeDirector
	{
		public Shape ConstructDefaultRedCircle(CircleBuilder builder, double radius)
		{
			return builder
				.Reset()
				.SetRadius(radius)
				.SetColor("Red")
				.Build();
		}

		public Shape ConstructBlueRectangle(RectangleBuilder builder, double width, double height)
		{
			return builder
				.Reset()
				.SetWidthHeight(width, height)
				.SetColor("Blue")
				.Build();
		}

		public Shape ConstructGreenTriangle(TriangleBuilder builder, double b, double h)
		{
			return builder
				.Reset()
				.SetBaseHeight(b, h)
				.SetColor("Green")
				.Build();
		}
	}

	public class ShapeForm : Form
	{
		private readonly Shape shape;

		public ShapeForm(Shape shape)
		{
			this.shape = shape ?? throw new ArgumentNullException(nameof(shape));
			Text = $"Shape Viewer - {shape.Type} ({shape.Color})";
			ClientSize = new Size(600, 400);
			StartPosition = FormStartPosition.CenterScreen;
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// Determine drawing area
			var rect = new Rectangle(10, 10, ClientSize.Width - 20, ClientSize.Height - 20);

			// Parse color name to a System.Drawing.Color (fallback to Black)
			Color drawColor = Color.Black;
			try
			{
				var parsed = Color.FromName(shape.Color ?? string.Empty);
				if (parsed.IsKnownColor || parsed.A > 0)
					drawColor = parsed;
			}
			catch { /* keep black */ }

			using var pen = new Pen(drawColor, 2);
			using var brush = new SolidBrush(Color.FromArgb(40, drawColor));

			switch (shape.Type)
			{
				case ShapeType.Circle:
					DrawCircle(g, rect, (float)shape.Size1, pen, brush);
					break;
				case ShapeType.Rectangle:
					DrawRectangle(g, rect, (float)shape.Size1, (float)shape.Size2, pen, brush);
					break;
				case ShapeType.Triangle:
					DrawTriangle(g, rect, (float)shape.Size1, (float)shape.Size2, pen, brush);
					break;
				default:
					g.DrawString("Unknown shape", Font, Brushes.Black, rect.Location);
					break;
			}
		}

		private void DrawCircle(Graphics g, Rectangle area, float radius, Pen pen, Brush brush)
		{
			// Scale radius to fit area
			float maxR = Math.Min(area.Width, area.Height) / 2f - 10f;
			float scale = maxR / Math.Max(1f, radius);
			float drawR = radius * scale;
			var cx = area.Left + area.Width / 2f;
			var cy = area.Top + area.Height / 2f;
			var rRect = new RectangleF(cx - drawR, cy - drawR, drawR * 2, drawR * 2);
			g.FillEllipse(brush, rRect);
			g.DrawEllipse(pen, rRect);
		}

		private void DrawRectangle(Graphics g, Rectangle area, float width, float height, Pen pen, Brush brush)
		{
			// Scale to fit area while preserving aspect
			float sx = (area.Width - 20f) / Math.Max(1f, width);
			float sy = (area.Height - 20f) / Math.Max(1f, height);
			float scale = Math.Min(sx, sy);
			float w = width * scale;
			float h = height * scale;
			var x = area.Left + (area.Width - w) / 2f;
			var y = area.Top + (area.Height - h) / 2f;
			var r = new RectangleF(x, y, w, h);
			g.FillRectangle(brush, r);
			g.DrawRectangle(pen, Rectangle.Round(r));
		}

		private void DrawTriangle(Graphics g, Rectangle area, float b, float h, Pen pen, Brush brush)
		{
			// Scale base and height to fit
			float sx = (area.Width - 20f) / Math.Max(1f, b);
			float sy = (area.Height - 20f) / Math.Max(1f, h);
			float scale = Math.Min(sx, sy);
			float baseW = b * scale;
			float heightH = h * scale;
			float cx = area.Left + area.Width / 2f;
			float topY = area.Top + (area.Height - heightH) / 2f;
			PointF p1 = new PointF(cx - baseW / 2f, topY + heightH);
			PointF p2 = new PointF(cx + baseW / 2f, topY + heightH);
			PointF p3 = new PointF(cx, topY);
			var pts = new[] { p1, p2, p3 };
			g.FillPolygon(brush, pts);
			g.DrawPolygon(pen, pts);
		}
	}

		static ShapeType PromptShapeType()
		{
			while (true)
			{
				Console.WriteLine("Choose shape type: (1) Circle  (2) Rectangle  (3) Triangle");
				Console.Write("Enter number or name: ");
				var input = Console.ReadLine()?.Trim();
				if (string.IsNullOrEmpty(input)) continue;

				if (int.TryParse(input, out var n))
				{
					if (n == 1) return ShapeType.Circle;
					if (n == 2) return ShapeType.Rectangle;
					if (n == 3) return ShapeType.Triangle;
				}

				if (Enum.TryParse<ShapeType>(input, true, out var t)) return t;

				Console.WriteLine("Invalid selection. Try again.");
			}
		}

		static double PromptDouble(string prompt, double min = double.MinValue, double max = double.MaxValue)
		{
			while (true)
			{
				Console.Write(prompt);
				var s = Console.ReadLine();
				if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) || double.TryParse(s, out v))
				{
					if (v < min)
					{
						Console.WriteLine($"Value must be >= {min}");
						continue;
					}
					if (v > max)
					{
						Console.WriteLine($"Value must be <= {max}");
						continue;
					}
					return v;
				}
				Console.WriteLine("Invalid number, try again.");
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

		static void DrawShape(Shape shape)
		{
			switch (shape.Type)
			{
				case ShapeType.Circle:
					DrawCircle((int)Math.Round(shape.Size1), shape.Color);
					break;
				case ShapeType.Rectangle:
					DrawRectangle((int)Math.Round(shape.Size1), (int)Math.Round(shape.Size2), shape.Color);
					break;
				case ShapeType.Triangle:
					DrawTriangle((int)Math.Round(shape.Size1), (int)Math.Round(shape.Size2), shape.Color);
					break;
				default:
					Console.WriteLine("No drawing available for this shape.");
					break;
			}
		}

		static void DrawRectangle(int width, int height, string color)
		{
			width = Math.Clamp(width, 1, 80);
			height = Math.Clamp(height, 1, 40);
			Console.WriteLine($"[{color}] Rectangle {width}x{height}");
			for (int r = 0; r < height; r++)
			{
				for (int c = 0; c < width; c++) Console.Write("#");
				Console.WriteLine();
			}
		}

		static void DrawTriangle(int baseWidth, int height, string color)
		{
			baseWidth = Math.Clamp(baseWidth, 1, 79);
			height = Math.Clamp(height, 1, 40);
			Console.WriteLine($"[{color}] Triangle base={baseWidth} height={height}");
			// Draw as isosceles triangle
			for (int row = 0; row < height; row++)
			{
				double t = (double)row / Math.Max(1, height - 1);
				int stars = 1 + (int)Math.Round(t * (baseWidth - 1));
				int pad = (baseWidth - stars) / 2;
				Console.Write(new string(' ', pad));
				Console.WriteLine(new string('*', Math.Max(1, stars)));
			}
		}

		static void DrawCircle(int radius, string color)
		{
			radius = Math.Clamp(radius, 1, 20);
			Console.WriteLine($"[{color}] Circle radius={radius}");
			int diameter = radius * 2 + 1;
			for (int y = 0; y < diameter; y++)
			{
				for (int x = 0; x < diameter; x++)
				{
					double dx = x - radius;
					double dy = y - radius;
					double d = Math.Sqrt(dx * dx + dy * dy);
					Console.Write(d <= radius + 0.3 ? '*' : ' ');
				}
				Console.WriteLine();
			}
		}

		static bool IsValidShape(Shape shape, out string reason)
		{
			reason = string.Empty;
			if (shape == null)
			{
				reason = "shape is null";
				return false;
			}

			if (double.IsNaN(shape.Size1) || double.IsInfinity(shape.Size1))
			{
				reason = "Size1 is not a valid number";
				return false;
			}

			switch (shape.Type)
			{
				case ShapeType.Circle:
					if (shape.Size1 <= 0)
					{
						reason = "Circle radius must be > 0";
						return false;
					}
					break;
				case ShapeType.Rectangle:
					if (double.IsNaN(shape.Size2) || double.IsInfinity(shape.Size2))
					{
						reason = "Size2 is not a valid number";
						return false;
					}
					if (shape.Size1 <= 0 || shape.Size2 <= 0)
					{
						reason = "Rectangle width and height must be > 0";
						return false;
					}
					break;
				case ShapeType.Triangle:
					if (double.IsNaN(shape.Size2) || double.IsInfinity(shape.Size2))
					{
						reason = "Size2 is not a valid number";
						return false;
					}
					if (shape.Size1 <= 0 || shape.Size2 <= 0)
					{
						reason = "Triangle base and height must be > 0";
						return false;
					}
					break;
				default:
					reason = "Unknown shape type";
					return false;
			}

			if (string.IsNullOrWhiteSpace(shape.Color))
			{
				reason = "Color is empty";
				return false;
			}

			return true;
		}
	}
}

