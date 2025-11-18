using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShapesBuilderSingleFile
{
	// Program entrypoint only — types moved to dedicated files.
	internal static class Program
	{
		[STAThread]
		static void Main()
		{
			Console.WriteLine("--- Shapes Builder Interactive Setup ---");
			int count = PromptInt("How many shapes do you want to create? (1-6): ", 1, 6);
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

				float x = 50 + (i % 4) * 200;
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
	}
}
