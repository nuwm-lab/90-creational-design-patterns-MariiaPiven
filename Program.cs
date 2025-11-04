using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

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

