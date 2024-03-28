using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Image = System.Windows.Controls.Image;

namespace Lab3
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    double sideLength = 200;
    Polyline allFractalPoints = new Polyline(); // Переміщення сюди
    public MainWindow()
    {
      InitializeComponent();
    }
    private string? UserChoose(ComboBox comboBox)
    {
      var selectedComboBoxItem = comboBox.SelectedItem as ComboBoxItem;

      if (selectedComboBoxItem == null) { return null; }

      return selectedComboBoxItem.Content?.ToString();
    }
    private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      double delta = e.Delta;

      int direction = delta > 0 ? 1 : -1;

      ScaleFractal(direction);
    }
    double scaleFactor = 1.0;
    private void Window_PreviewKey(object sender, KeyEventArgs e)
    {
      double delta = 0;

      if (e.Key == Key.Left || e.Key == Key.Right)
      {
        delta = (e.Key == Key.Left) ? 50 : -50;

        foreach (var child in canvas.Children)
        {
          if (child is UIElement uiElement)
          {
            double currentLeft = Canvas.GetLeft(uiElement);
            if (double.IsNaN(currentLeft))
            {
              currentLeft = 0;
            }
            Canvas.SetLeft(uiElement, currentLeft + delta);
          }
        }
      }
      else if (e.Key == Key.Up || e.Key == Key.Down)
      {
        delta = (e.Key == Key.Up) ? 20 : -20;

        foreach (var child in canvas.Children)
        {
          if (child is UIElement uiElement)
          {
            double currentTop = Canvas.GetTop(uiElement);
            if (double.IsNaN(currentTop))
            {
              currentTop = 0;
            }
            Canvas.SetTop(uiElement, currentTop + delta);
          }
        }
      }
    }


    private void ScaleFractal(int direction)
    {
      scaleFactor += direction > 0 ? 0.2 : -0.2;

      string? fractalName = UserChoose(fractalComboBox);
      if (fractalName == null)
      {
        MessageBox.Show("Виберіть фрактал для побудови.");
        return;
      }
      switch (fractalName)
      {
        case "Сніжинка Коха звичайна":
        case "Сніжинка Коха «навпаки»":
          ScaleKochSnowflake();
          break;
        case "Фрактал sin(z)*cos(z)+с":
          ScaleAlgebraicFractal(direction > 0);
          break;
      }
    }
    void ScaleAlgebraicFractal(bool zoomIn)
    {
      double canvasWidth = canvas.ActualWidth;
      double canvasHeight = canvas.ActualHeight;

      Image fractalImage = canvas.Children.OfType<Image>().FirstOrDefault();
      if (fractalImage == null)
      {
        MessageBox.Show("Фрактал не знайдено.");
        return;
      }

      double initialWidth = fractalImage.Width;
      double initialHeight = fractalImage.Height;

      double scaleChange = zoomIn ? 1.2 : 0.8;

      double newWidth = initialWidth * scaleChange;
      double newHeight = initialHeight * scaleChange;

      fractalImage.Width = newWidth;
      fractalImage.Height = newHeight;

      double deltaX = (initialWidth - newWidth) / 2;
      double deltaY = (initialHeight - newHeight) / 2;

      double newX = Canvas.GetLeft(fractalImage) + deltaX;
      double newY = Canvas.GetTop(fractalImage) + deltaY;

      Canvas.SetLeft(fractalImage, newX);
      Canvas.SetTop(fractalImage, newY);
    }

    void ScaleKochSnowflake()
    {
      double canvasWidth = canvas.ActualWidth;
      double canvasHeight = canvas.ActualHeight;

      List<Point> scaledPoints = new List<Point>();
      foreach (Point point in allFractalPoints.Points)
      {
        double newX = (point.X - canvasWidth / 2) * scaleFactor + canvasWidth / 2;
        double newY = (point.Y - canvasHeight / 2) * scaleFactor + canvasHeight / 2;

        scaledPoints.Add(new Point(newX, newY));
      }

      Polyline newPolyline = new Polyline
      {
        Points = new PointCollection(scaledPoints),
        Stroke = System.Windows.Media.Brushes.Red,
        StrokeThickness = 1
      };

      canvas.Children.Clear();
      canvas.Children.Add(newPolyline);
    }

    private void BuildFractalButton_Click(object sender, RoutedEventArgs e)
    {
      string? fractalName = UserChoose(fractalComboBox);
      if (fractalName == null)
      {
        MessageBox.Show("Виберіть фрактал для побудови.");
        return;
      }
      canvas.Children.Clear();
      allFractalPoints.Points.Clear();
      switch (fractalName)
      {
        case "Сніжинка Коха звичайна":
          DrawKochSnowflake(true);
          break;
        case "Сніжинка Коха «навпаки»":
          DrawKochSnowflake(false);
          break;
        case "Фрактал sin(z)*cos(z)+с":
          DrawSinCosFractal();
          break;
        default:
          MessageBox.Show("Не вибрано фрактал для побудови.");
          break;
      }
    }

    /// <summary>
    /// Отримати введені дані для сніжинок Коха
    /// </summary>
    private void SnowflakeKohInput(out int numberOfIterations, out double centerX, out double centerY, out double step)
    {
      var inputWindow = new Lab3.InputForms.SnowflakeKoh();
      if (inputWindow.ShowDialog() == true)
      {
        numberOfIterations = inputWindow.NumberOfIterations;
        centerX = inputWindow.CenterX;
        centerY = inputWindow.CenterY;
        step = inputWindow.Step;
      }
      else
      {
        numberOfIterations = 0;
        centerX = 0;
        centerY = 0;
        step = 0;
      }
    }
    /// <summary>
    /// Метод для додавання ліній на Canvas
    /// </summary>
    private void AddPolyline(PointCollection points)
    {
      allFractalPoints.Points = points;
      allFractalPoints.Stroke = System.Windows.Media.Brushes.Red; // Зміна кольору контуру на червоний
      allFractalPoints.StrokeThickness = 1;

      canvas.Children.Add(allFractalPoints);
    }


    /// <summary>
    /// Малювання сніжинки Коха або інвертованої сніжинки Коха
    /// </summary>
    private void DrawKochSnowflake(bool isRegular)
    {
      SnowflakeKohInput(out int numberOfIterations, out double centerX, out double centerY, out double step);

      // Координати вершин трикутника
      Point point1 = new Point(centerX - sideLength / 2, centerY + Math.Sqrt(3) * sideLength / 6);
      Point point2 = new Point(centerX + sideLength / 2, centerY + Math.Sqrt(3) * sideLength / 6);
      Point point3 = new Point(centerX, centerY - Math.Sqrt(3) * sideLength / 3);

      // Генерація точок кривої Коха для кожної сторони трикутника
      PointCollection points = new PointCollection();
      GenerateKochCurveRecursive(point1, point2, numberOfIterations, isRegular, points);
      GenerateKochCurveRecursive(point2, point3, numberOfIterations, isRegular, points);
      GenerateKochCurveRecursive(point3, point1, numberOfIterations, isRegular, points);

      // Додавання точки початку для крайнього лівого нижнього трикутника
      points.Add(point1);

      AddPolyline(points);
    }
    /// <summary>
    /// Генерування точок кривої Коха або її інвертованої версії рекурсивно
    /// </summary>
    private void GenerateKochCurveRecursive(Point startPoint, Point endPoint, int iterations, bool isRegular, PointCollection points)
    {
      if (iterations == 0)
      {
        points.Add(startPoint);
        return;
      }

      double deltaX = (endPoint.X - startPoint.X) / 3;
      double deltaY = (endPoint.Y - startPoint.Y) / 3;

      Point p1 = new Point(startPoint.X + deltaX, startPoint.Y + deltaY);
      Point p2 = new Point(startPoint.X + 2 * deltaX, startPoint.Y + 2 * deltaY);
      Point p3;

      if (isRegular)
      {
        // звичайна сніжинка Коха
        p3 = new Point(startPoint.X + 3 * deltaX / 2 - Math.Sqrt(3) * deltaY / 2, startPoint.Y + 3 * deltaY / 2 + Math.Sqrt(3) * deltaX / 2);
      }
      else
      {
        // "навпаки" сніжинка Коха
        p3 = new Point(startPoint.X + 3 * deltaX / 2 + Math.Sqrt(3) * deltaY / 2, startPoint.Y + 3 * deltaY / 2 - Math.Sqrt(3) * deltaX / 2);
      }

      GenerateKochCurveRecursive(startPoint, p1, iterations - 1, isRegular, points);
      GenerateKochCurveRecursive(p1, p3, iterations - 1, isRegular, points);
      GenerateKochCurveRecursive(p3, p2, iterations - 1, isRegular, points);
      GenerateKochCurveRecursive(p2, endPoint, iterations - 1, isRegular, points);
    }


    private void DrawSinCosFractal()
    {
      AlgebraicFractalInput(out double constantC, out int iterations, out int colorScheme);

      AlgebraicFractal newtonFractal = new AlgebraicFractal(Convert.ToInt32(canvas.ActualWidth), Convert.ToInt32(canvas.ActualHeight), iterations);
      Bitmap fractalBitmap = newtonFractal.GenerateFractal(0, 0, constantC, colorScheme);

      BitmapImage bitmapImage = (BitmapImage) newtonFractal.BitmapToImageSource(fractalBitmap);
      Image img = new Image
      {
        Source = bitmapImage,
        Width = bitmapImage.PixelWidth,
        Height = bitmapImage.PixelHeight
      };
      canvas.Children.Add(img);
    }
    private void AlgebraicFractalInput(out double constantC, out int iterations, out int colorScheme)
    {
      var inputWindow = new Lab3.InputForms.AlgebraicFractal();
      if (inputWindow.ShowDialog() == true)
      {
        constantC = inputWindow.ConstantC;
        iterations = inputWindow.Iterations;
        colorScheme = inputWindow.ColorScheme;
      }
      else
      {
        constantC = 0;
        iterations = 0;
        colorScheme = 0;
      }
    }

    private void SaveImage_Click(object sender, RoutedEventArgs e)
    {
      string selectedFormat = UserChoose(photoComboBox);

      if (selectedFormat == null)
      {
        MessageBox.Show("Виберіть формат для збереження зображення.");
        return;
      }

      string format = selectedFormat.ToUpper();

      var dialog = new Microsoft.Win32.SaveFileDialog();
      dialog.Filter = $"{format} files (*.{format.ToLower()})|*.{format.ToLower()}|All files (*.*)|*.*";

      if (dialog.ShowDialog() == true)
      {
        ImageSaver.SaveImage(canvas, dialog.FileName, format.ToLower());
      }
    }
  }
}
