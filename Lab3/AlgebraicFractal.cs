using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Color = System.Drawing.Color;
using System.Numerics;

namespace Lab3
{
  public class AlgebraicFractal
  {
    private Bitmap bitmap;
    private int width;
    private int height;
    private int maxIterations;
    private double offsetX;
    private double offsetY;

    public AlgebraicFractal(int width, int height, int maxIterations)
    {
      this.width = width;
      this.height = height;
      this.maxIterations = maxIterations;
      bitmap = new Bitmap(width, height);
    }

    public Bitmap GenerateFractal(double offsetX, double offsetY, double constantC, int colorSchemeChoice)
    {
      this.offsetX = offsetX;
      this.offsetY = offsetY;

      double xStep = 8.0 / width;
      double yStep = 4.0 / height;
      double xStart = -10.0 + offsetX;
      double yStart = -2.0 + offsetY;

      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          double a = xStart + x * xStep;
          double b = yStart + y * yStep;
          Complex z = new Complex(a, b);

          int iterations = 0;
          while (z.Magnitude < 100 && iterations < maxIterations)
          {
            z = Complex.Add(Complex.Multiply(Complex.Sin(z), Complex.Cos(z)), new Complex(constantC, 0));
            iterations++;
          }

          Color color = GetColor(iterations, maxIterations, colorSchemeChoice);
          bitmap.SetPixel(x, y, color);
        }
      }

      return bitmap;
    }
    private static Color GetColor(int iterations, int maxIterations, int colorSchemeChoice)
    {
      double hue = (double) iterations / maxIterations * 360;
      double saturation = 1.0;
      double lightness = 0.5;

      if (iterations == maxIterations)
        return Color.Black;

      switch (colorSchemeChoice)
      {
        case 1: break;
        case 2: hue = (hue + 240) % 360; break;
        case 3: hue = (hue + 60) % 360; break;
        default: return Color.Black;
      }

      return ColorFromHSL(hue, saturation, lightness);
    }

    private static Color ColorFromHSL(double hue, double saturation, double lightness)
    {
      double chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
      double huePrime = hue / 60;
      double x = chroma * (1 - Math.Abs(huePrime % 2 - 1));

      double r1, g1, b1;
      if (huePrime >= 0 && huePrime < 1)
      {
        r1 = chroma;
        g1 = x;
        b1 = 0;
      }
      else if (huePrime >= 1 && huePrime < 2)
      {
        r1 = x;
        g1 = chroma;
        b1 = 0;
      }
      else if (huePrime >= 2 && huePrime < 3)
      {
        r1 = 0;
        g1 = chroma;
        b1 = x;
      }
      else if (huePrime >= 3 && huePrime < 4)
      {
        r1 = 0;
        g1 = x;
        b1 = chroma;
      }
      else if (huePrime >= 4 && huePrime < 5)
      {
        r1 = x;
        g1 = 0;
        b1 = chroma;
      }
      else
      {
        r1 = chroma;
        g1 = 0;
        b1 = x;
      }

      double m = lightness - chroma / 2;
      int r = (int) ((r1 + m) * 255);
      int g = (int) ((g1 + m) * 255);
      int b = (int) ((b1 + m) * 255);

      return Color.FromArgb(r, g, b);
    }

    public ImageSource BitmapToImageSource(Bitmap bitmap)
    {
      using (MemoryStream memory = new MemoryStream())
      {
        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
        memory.Position = 0;

        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();

        return bitmapImage;
      }
    }
  }
}
