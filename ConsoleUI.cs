using System;
using System.Globalization;
using System.Threading;

namespace CreationalDesignPatterns.UI
{
    /// <summary>
    /// Handles console interactions for building and displaying shapes.
    /// </summary>
    public class ConsoleUi
    {
        private readonly Services.ShapeRenderer _renderer = new Services.ShapeRenderer();

        /// <summary>
        /// Start the interactive console UI.
        /// </summary>
        public void Run()
        {
            Console.WriteLine("--- Shape Builder Interactive Demo ---");

            var type = PromptShapeType();
            CreationalDesignPatterns.Shape shape = null;

            try
            {
                switch (type)
                {
                    case CreationalDesignPatterns.ShapeType.Circle:
                        var radius = PromptDouble("Enter radius (number): ", min: 0.1);
                        var colorC = PromptString("Enter color: ");
                        shape = new CreationalDesignPatterns.Builders.CircleBuilder().Reset().SetRadius(radius).SetColor(colorC).Build();
                        break;
                    case CreationalDesignPatterns.ShapeType.Rectangle:
                        var width = PromptDouble("Enter width (number): ", min: 1);
                        var height = PromptDouble("Enter height (number): ", min: 1);
                        var colorR = PromptString("Enter color: ");
                        shape = new CreationalDesignPatterns.Builders.RectangleBuilder().Reset().SetWidthHeight(width, height).SetColor(colorR).Build();
                        break;
                    case CreationalDesignPatterns.ShapeType.Triangle:
                        var b = PromptDouble("Enter base (number): ", min: 1);
                        var h = PromptDouble("Enter height (number): ", min: 1);
                        var colorT = PromptString("Enter color: ");
                        shape = new CreationalDesignPatterns.Builders.TriangleBuilder().Reset().SetBaseHeight(b, h).SetColor(colorT).Build();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error building shape: {ex.Message}");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Built shape:");
            Console.WriteLine(shape);
            Console.WriteLine();
            Console.WriteLine("Drawing (ASCII):");
            Console.WriteLine(_renderer.RenderAscii(shape));

            Console.WriteLine();
            Console.Write("Open graphical window to display shape? (y/n): ");
            var ans = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (ans == "y" || ans == "yes")
            {
                ShowShapeWindow(shape);
            }
        }

        private void ShowShapeWindow(CreationalDesignPatterns.Shape shape)
        {
            var t = new Thread(() =>
            {
                try
                {
                    System.Windows.Forms.Application.EnableVisualStyles();
                    System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                    System.Windows.Forms.Application.Run(new CreationalDesignPatterns.ShapeForm(shape));
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

        private CreationalDesignPatterns.ShapeType PromptShapeType()
        {
            while (true)
            {
                Console.WriteLine("Choose shape type: (1) Circle  (2) Rectangle  (3) Triangle");
                Console.Write("Enter number or name: ");
                var input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input)) continue;

                if (int.TryParse(input, out var n))
                {
                    if (n == 1) return CreationalDesignPatterns.ShapeType.Circle;
                    if (n == 2) return CreationalDesignPatterns.ShapeType.Rectangle;
                    if (n == 3) return CreationalDesignPatterns.ShapeType.Triangle;
                }

                if (Enum.TryParse<CreationalDesignPatterns.ShapeType>(input, true, out var t)) return t;

                Console.WriteLine("Invalid selection. Try again.");
            }
        }

        private double PromptDouble(string prompt, double min = double.MinValue, double max = double.MaxValue)
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

        private string PromptString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.WriteLine("Please enter a non-empty value.");
            }
        }
    }
}
