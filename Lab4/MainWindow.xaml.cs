using Microsoft.Win32;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;
using Color = System.Drawing.Color;
using Rectangle = System.Windows.Shapes.Rectangle;
using Image = System.Windows.Controls.Image;

namespace Lab4
{
  public partial class MainWindow : Window
  {
    private Bitmap originalImage;
    private Bitmap processedImage;

    public MainWindow()
    {
      InitializeComponent();
    }

    private void OpenImage_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
        openFileDialog.Filter = "Image files (*.png;*.jpeg;*.bmp)|*.png;*.jpeg;*.bmp";
        if (openFileDialog.ShowDialog() == true)
        {
          originalImage = new Bitmap(openFileDialog.FileName);
          canvas.Background = new ImageBrush(ToBitmapSource(originalImage));
          processedImage = (Bitmap) originalImage.Clone();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Сталася помилка при відкритті зображення: {ex.Message}");
      }
    }
    private void ProcessImage_Click(object sender, RoutedEventArgs e)
    {
      if (originalImage != null)
      {
        string selectedItem = ((ComboBoxItem) conversionComboBox.SelectedItem).Content.ToString();
        if (selectedItem == "RGB в Lab")
        {
          processedImage = ConvertToLab(originalImage);
        }
        else if (selectedItem == "Lab в RGB")
        {
          processedImage = ConvertToRGB(originalImage);
        }
        canvas.Background = new ImageBrush(ToBitmapSource(processedImage));
      }
    }

    private void DisplayColorsButton_Click(object sender, RoutedEventArgs e)
    {
      if (originalImage != null)
      {
        int x = int.Parse(textBox_X.Text);
        int y = int.Parse(textBox_Y.Text);
        Color originalColor = originalImage.GetPixel(x, y);
        Color processedColor = processedImage.GetPixel(x, y);

        MessageBox.Show($"Оригінальний колір (RGB): {originalColor.R}, {originalColor.G}, {originalColor.B}\n" +
                       $"Перетворений колір (Lab): {ConvertRGBToLab(originalColor).L}, {ConvertRGBToLab(originalColor).A}, {ConvertRGBToLab(originalColor).B}");
      }
    }

    private void SaveImage_Click(object sender, RoutedEventArgs e)
    {
      if (processedImage != null)
      {
        string selectedItem = ((ComboBoxItem) photoComboBox.SelectedItem).Content.ToString();
        Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
        saveFileDialog.Filter = $"{selectedItem} files (*.{selectedItem.ToLower()})|*.{selectedItem.ToLower()}";
        if (saveFileDialog.ShowDialog() == true)
        {
          switch (selectedItem)
          {
            case "PNG":
              processedImage.Save(saveFileDialog.FileName, ImageFormat.Png);
              break;
            case "JPEG":
              processedImage.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
              break;
            case "BMP":
              processedImage.Save(saveFileDialog.FileName, ImageFormat.Bmp);
              break;
          }
        }
      }
    }

    private Bitmap ConvertToLab(Bitmap image)
    {
      Bitmap labImage = new Bitmap(image.Width, image.Height);
      for (int x = 0; x < image.Width; x++)
      {
        for (int y = 0; y < image.Height; y++)
        {
          Color rgb = image.GetPixel(x, y);
          LabColor lab = ConvertRGBToLab(rgb);
          labImage.SetPixel(x, y, ConvertLabToRGB(lab));
        }
      }
      return labImage;
    }
    private Bitmap ConvertToRGB(Bitmap image)
    {
      Bitmap rgbImage = new Bitmap(image.Width, image.Height);
      for (int x = 0; x < image.Width; x++)
      {
        for (int y = 0; y < image.Height; y++)
        {
          LabColor lab = new LabColor
          {
            L = image.GetPixel(x, y).R,
            A = (sbyte) image.GetPixel(x, y).G,
            B = (sbyte) image.GetPixel(x, y).B
          };
          Color rgb = ConvertLabToRGB(lab);
          rgbImage.SetPixel(x, y, rgb);
        }
      }
      return rgbImage;
    }
    private LabColor ConvertRGBToLab(Color rgb)
    {
      // Перетворення з RGB в XYZ
      double r = rgb.R / 255.0;
      double g = rgb.G / 255.0;
      double bComponent = rgb.B / 255.0;

      if (r > 0.04045)
        r = Math.Pow((r + 0.055) / 1.055, 2.4);
      else
        r /= 12.92;

      if (g > 0.04045)
        g = Math.Pow((g + 0.055) / 1.055, 2.4);
      else
        g /= 12.92;

      if (bComponent > 0.04045)
        bComponent = Math.Pow((bComponent + 0.055) / 1.055, 2.4);
      else
        bComponent /= 12.92;

      double x = r * 0.4124 + g * 0.3576 + bComponent * 0.1805;
      double y = r * 0.2126 + g * 0.7152 + bComponent * 0.0722;
      double z = r * 0.0193 + g * 0.1192 + bComponent * 0.9505;

      // Перетворення з XYZ в Lab
      double epsilon = 0.008856;
      double kappa = 903.3;

      double l = (y > epsilon) ? (116 * Math.Pow(y, 1.0 / 3.0) - 16) : (kappa * y);
      double a = 500 * ((x > epsilon ? Math.Pow(x, 1.0 / 3.0) : (7.787 * x + 16.0 / 116)) -
                      (y > epsilon ? Math.Pow(y, 1.0 / 3.0) : (7.787 * y + 16.0 / 116)));
      double bComponentLab = 200 * ((y > epsilon ? Math.Pow(y, 1.0 / 3.0) : (7.787 * y + 16.0 / 116)) -
                      (z > epsilon ? Math.Pow(z, 1.0 / 3.0) : (7.787 * z + 16.0 / 116)));

      return new LabColor
      {
        L = (byte) Math.Round(l),
        A = (sbyte) Math.Round(a),
        B = (sbyte) Math.Round(bComponentLab)
      };
    }
    private Color ConvertLabToRGB(LabColor lab)
    {
      // Перетворення з Lab в XYZ
      double y = (lab.L + 16) / 116.0;
      double x = lab.A / 500.0 + y;
      double z = y - lab.B / 200.0;

      double epsilon = 0.008856;
      double kappa = 903.3;

      double xPow3 = (x > epsilon) ? Math.Pow(x, 3) : (x - 16.0 / 116) / 7.787;
      double yPow3 = (y > epsilon) ? Math.Pow(y, 3) : (y - 16.0 / 116) / 7.787;
      double zPow3 = (z > epsilon) ? Math.Pow(z, 3) : (z - 16.0 / 116) / 7.787;

      double r = xPow3 * 3.2406 + yPow3 * -1.5372 + zPow3 * -0.4986;
      double g = xPow3 * -0.9689 + yPow3 * 1.8758 + zPow3 * 0.0415;
      double b = xPow3 * 0.0557 + yPow3 * -0.2040 + zPow3 * 1.0570;

      // Перетворення з XYZ в RGB
      r = Math.Max(0, Math.Min(1, r));
      g = Math.Max(0, Math.Min(1, g));
      b = Math.Max(0, Math.Min(1, b));

      r = (r > 0.0031308) ? (1.055 * Math.Pow(r, 1.0 / 2.4) - 0.055) : 12.92 * r;
      g = (g > 0.0031308) ? (1.055 * Math.Pow(g, 1.0 / 2.4) - 0.055) : 12.92 * g;
      b = (b > 0.0031308) ? (1.055 * Math.Pow(b, 1.0 / 2.4) - 0.055) : 12.92 * b;

      return System.Drawing.Color.FromArgb(
          (int) Math.Round(r * 255),
          (int) Math.Round(g * 255),
          (int) Math.Round(b * 255));
    }
    private BitmapSource ToBitmapSource(Bitmap bitmap)
    {
      try
      {
        using (MemoryStream stream = new MemoryStream())
        {
          bitmap.Save(stream, ImageFormat.Png);
          stream.Position = 0;

          BitmapSource bitmapSource = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
          return bitmapSource;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Сталася помилка при створенні BitmapSource: {ex.Message}");
        return null;
      }
    }




    private void SaturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (originalImage != null)
      {
        processedImage = (Bitmap) originalImage.Clone();
        ChangeSaturation(processedImage, (float) saturationSlider.Value, selectionrectangle);
        canvas.Background = new ImageBrush(ToBitmapSource(processedImage));
      }
    }
    private void ChangeSaturation(Bitmap image, float saturationValue, Rectangle selection)
    {
      int startX = (int) point1.X;
      int startY = (int) point1.Y;

      double xScale = originalImage.Width / canvas.ActualWidth;
      double yScale = originalImage.Height / canvas.ActualHeight;
      int endX = (int) (startX + selection.Width * xScale);
      int endY = (int) (startY + selection.Height * yScale);

      Color darkBlueColor = Color.FromArgb(0, 0, 139); // RGB значення для темно синього кольору

      for (int x = startX; x < endX; x++)
      {
        for (int y = startY; y < endY; y++)
        {
          Color color = image.GetPixel(x, y);
          if (color == darkBlueColor)
          {
            float [] hsv = RGBtoHSV(color);
            hsv [1] *= saturationValue;
            image.SetPixel(x, y, HSVtoRGB(hsv));
          }
        }
      }
    }
    private Color HSVtoRGB(float [] hsv)
    {
      float h = hsv [0];
      float s = hsv [1];
      float v = hsv [2];

      int hi = (int) Math.Floor(h / 60) % 6;
      float f = h / 60 - (float) Math.Floor(h / 60);
      float p = v * (1 - s);
      float q = v * (1 - f * s);
      float t = v * (1 - (1 - f) * s);

      float r, g, b;

      switch (hi)
      {
        case 0:
          r = v;
          g = t;
          b = p;
          break;
        case 1:
          r = q;
          g = v;
          b = p;
          break;
        case 2:
          r = p;
          g = v;
          b = t;
          break;
        case 3:
          r = p;
          g = q;
          b = v;
          break;
        case 4:
          r = t;
          g = p;
          b = v;
          break;
        default:
          r = v;
          g = p;
          b = q;
          break;
      }

      return Color.FromArgb(255, (byte) (r * 255), (byte) (g * 255), (byte) (b * 255));
    }

    private void MainCanvans_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!isSelecting)
        return;

      Point relativePoint = e.GetPosition(canvas);
      double xScale = originalImage.Width / canvas.ActualWidth;
      double yScale = originalImage.Height / canvas.ActualHeight;

      if (isSelecting)
      {
        if (selectionrectangle != null)
          canvas.Children.Remove(selectionrectangle);
        if (point1.X == 0 && point1.Y == 0)
        {
          point1.X = relativePoint.X * xScale;
          point1.Y = relativePoint.Y * yScale;
        }
        else
        {
          point2.X = relativePoint.X * xScale;
          point2.Y = relativePoint.Y * yScale;
          selectionrectangle = new Rectangle();
          selectionrectangle.Stroke = System.Windows.Media.Brushes.Red;
          selectionrectangle.StrokeThickness = 2;
          selectionrectangle.Width = Math.Abs(point1.X - point2.X) / xScale;
          selectionrectangle.Height = Math.Abs(point1.Y - point2.Y) / yScale;
          canvas.Children.Add(selectionrectangle);
          Canvas.SetLeft(selectionrectangle, Math.Min(point1.X, point2.X) / xScale);
          Canvas.SetTop(selectionrectangle, Math.Min(point1.Y, point2.Y) / yScale);
        }
      }
    }
    private float [] RGBtoHSV(Color rgb)
    {
      float r = rgb.R / 255f;
      float g = rgb.G / 255f;
      float b = rgb.B / 255f;

      float max = Math.Max(r, Math.Max(g, b));
      float min = Math.Min(r, Math.Min(g, b));
      float delta = max - min;

      float h = 0, s = 0, v = max;

      if (max != 0)
        s = delta / max;

      if (delta != 0)
      {
        if (r == max)
          h = (g - b) / delta;
        else if (g == max)
          h = 2 + (b - r) / delta;
        else
          h = 4 + (r - g) / delta;

        h *= 60;
        if (h < 0)
          h += 360;
      }

      return new float [] { h, s, v };
    }

    private Point point1, point2;
    private bool isSelecting = false;
    private Rectangle selectionrectangle;
    private void SelectFragmentButton_Click(object sender, RoutedEventArgs e)
    {
      isSelecting = true;
      point1 = new Point(0, 0);
      point2 = new Point(0, 0);
    }

  }

  public class LabColor
  {
    public byte L { get; set; }
    public sbyte A { get; set; }
    public sbyte B { get; set; }
  }
}