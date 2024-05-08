using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lab5
{
  /// <summary>
  /// wfrfw Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private double currentScale = 1.0;
    int intervalCount = 13;
    double intervalSize = 35;

    private Square square;
    private DispatcherTimer animationTimer;
    private double currentAngle = 0;
    private double scaleX = 1.0;
    private double scaleY = 1.0;
    public MainWindow()
    {
      InitializeComponent();
      animationTimer = new DispatcherTimer();
      animationTimer.Interval = TimeSpan.FromMilliseconds(10);
      animationTimer.Tick += AnimationTimer_Tick;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      DrawCoordinateSystem();
    }
    private void ClearCanvas<T>() where T : UIElement
    {
      var elementsToRemove = canvas.Children
          .OfType<T>()
          .ToList();

      foreach (var elementToRemove in elementsToRemove)
      {
        canvas.Children.Remove(elementToRemove);
      }
    }
    private void DrawCoordinateSystem()
    {
      ClearCanvas<Label>();

      double width = mainGrid.ActualWidth;
      double height = mainGrid.RowDefinitions [0].ActualHeight;
      double centerX = width / 2;
      double centerY = height / 2;

      DrawAxis(centerX, centerY, width, height, true, "X");
      DrawAxis(centerX, centerY, width, height, false, "Y");

      DrawUnitIntervals(centerX, centerY, width, height, true);
      DrawUnitIntervals(centerX, centerY, width, height, false);

      DrawUnitLabels(centerX, centerY, true); // Перемальовуємо мітки для горизонтальної осі
      DrawUnitLabels(centerX, centerY, false);

      InvalidateVisual();
    }
    private void DrawAxis(double centerX, double centerY, double width, double height, bool isXAxis, string label)
    {
      Line axis = new Line
      {
        Stroke = Brushes.Black,
        StrokeThickness = 1
      };

      Polygon axisArrow = new Polygon
      {
        Fill = Brushes.Black,
        Points = new PointCollection()
      };

      Label axisLabel = new Label
      {
        Content = label
      };

      double arrowSize = 10; // Размер стрелки

      if (isXAxis)
      {
        axis.X1 = 0;
        axis.Y1 = centerY;
        axis.X2 = width;
        axis.Y2 = centerY;

        axisArrow.Points.Add(new Point(width, centerY));
        axisArrow.Points.Add(new Point(width - arrowSize, centerY - arrowSize));
        axisArrow.Points.Add(new Point(width - arrowSize, centerY + arrowSize));

        axisLabel.Margin = new Thickness(width - 20, centerY - 20, 0, 0);
      }
      else
      {
        axis.X1 = centerX;
        axis.Y1 = 0;
        axis.X2 = centerX;
        axis.Y2 = height;

        axisArrow.Points.Add(new Point(centerX, 0));
        axisArrow.Points.Add(new Point(centerX - arrowSize, arrowSize));
        axisArrow.Points.Add(new Point(centerX + arrowSize, arrowSize));

        axisLabel.Margin = new Thickness(centerX + 10, 0, 0, 0);
      }

      axisArrow.Fill = Brushes.Black;

      canvas.Children.Add(axis);
      canvas.Children.Add(axisArrow);
      canvas.Children.Add(axisLabel);
    }
    private void DrawUnitLabels(double centerX, double centerY, bool isXAxis)
    {
      double width = mainGrid.ActualWidth;
      double height = mainGrid.RowDefinitions [0].ActualHeight;

      if (isXAxis)
      {
        for (int i = -intervalCount; i <= intervalCount; i++)
        {
          double x = centerX + i * intervalSize;

          if (x >= 0 && x <= width)
          {
            Label label = new Label
            {
              Content = (i * scaleFactor).ToString("F2"),
              Margin = new Thickness(x - 5, centerY + 10, 0, 0)
            };

            canvas.Children.Add(label);
          }
        }
      }
      else
      {
        for (int i = -intervalCount; i <= intervalCount; i++)
        {
          double y = centerY - i * intervalSize;

          if (y >= 0 && y <= height)
          {
            Label label = new Label
            {
              Content = (i * scaleFactor).ToString("F2"),
              Margin = new Thickness(centerX - 20, y - 5, 0, 0)
            };
            canvas.Children.Add(label);
          }
        }
      }
    }
    private void DrawUnitIntervals(double centerX, double centerY, double width, double height, bool isXAxis)
    {
      if (isXAxis)
      {
        for (int i = -intervalCount; i <= intervalCount; i++)
        {
          double x = centerX + i * intervalSize;
          double y1 = centerY - 5;
          double y2 = centerY + 5;


          if (x >= 0 && x <= width)
          {
            Line intervalLine = new Line
            {
              Stroke = Brushes.Black,
              X1 = x,
              Y1 = y1,
              X2 = x,
              Y2 = y2,
              StrokeThickness = 1
            };
            canvas.Children.Add(intervalLine);
          }
        }
      }
      else
      {
        for (int i = -intervalCount; i <= intervalCount; i++)
        {
          double x1 = centerX - 5;
          double x2 = centerX + 5;
          double y = centerY - i * intervalSize;

          if (y >= 0 && y <= height)
          {
            Line intervalLine = new Line
            {
              Stroke = Brushes.Black,
              X1 = x1,
              Y1 = y,
              X2 = x2,
              Y2 = y,
              StrokeThickness = 1
            };
            canvas.Children.Add(intervalLine);
          }
        }
      }
    }

    private Polygon polySquare;
    private void DrawSquare_Click(object sender, RoutedEventArgs e)
    {
      SetSquareCoordinate input = new SetSquareCoordinate();
      input.ShowDialog();

      if (input.square != null && input.square.IsSquare())
      {
        square = input.square;
        polySquare = new Polygon
        {
          Fill = new SolidColorBrush(Colors.Red),
          Points = new PointCollection(TransformCoordinates(square.Points))
        };
        canvas.Children.Add(polySquare);
      }
      else
      {
        MessageBox.Show("Введені точки не утворюють квадрат. Спробуйте ще раз.");
      }
    }
    private Point [] TransformCoordinates(Point [] vertices)
    {
      Point [] transformedVertices = new Point [vertices.Length];

      double centerX = canvas.ActualWidth / 2;
      double centerY = canvas.ActualHeight / 2;

      for (int i = 0; i < vertices.Length; i++)
      {
        double x = centerX + vertices [i].X * currentScale * intervalSize;
        double y = centerY - vertices [i].Y * currentScale * intervalSize;
        transformedVertices [i] = new Point(x, y);
      }

      return transformedVertices;
    }
    Matrix transformMatrix;
    double time = 0;
    /// <summary>
    /// need fix
    /// </summary>
    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
      if (square == null)
        return;

      time += animationTimer.Interval.TotalSeconds;
      currentAngle += 0.3; // Швидкість повороту (в радіанах)
      scaleX = 1 + 0.4 * Math.Sin(time); // Коефіцієнт масштабування по осі X
      scaleY = 1 - 0.01 * Math.Cos(time); // Коефіцієнт масштабування по осі Y

      Matrix rotationMatrix = new Matrix();
      rotationMatrix.RotateAt(currentAngle, square.Center.X, square.Center.Y);

      Matrix scaleMatrix = new Matrix(scaleX, 0, 0, scaleY, 0, 0);

      Matrix transformMatrix = rotationMatrix * scaleMatrix;

      Point [] transformedVertices = new Point [square.Points.Length];
      for (int i = 0; i < square.Points.Length; i++)
      {
        Point vertex = square.Points [i];
        transformedVertices [i] = transformMatrix.Transform(vertex);
      }

      Point [] canvasVertices = TransformCoordinates(transformedVertices);
      polySquare.Points = new PointCollection(canvasVertices);
    }
    private void StartAnimation_Click(object sender, RoutedEventArgs e)
    {
      animationTimer.Start();
    }
    private void StopAnimation_Click(object sender, RoutedEventArgs e)
    {
      animationTimer.Stop();
    }


    double scaleFactor = 1;
    double scaleFactor1 = 1;
    private void ScaleSquare(bool zoomIn)
    {
      scaleFactor *= zoomIn ? 1.3 : 0.85;

      scaleFactor1 *= zoomIn ? 0.99 : 1.005;
      Matrix scaleMatrix = new Matrix(scaleFactor1, 0, 0, scaleFactor1, 0, 0);

      for (int i = 0; i < square.Points.Length; i++)
      {
        Point vertex = square.Points [i];
        Point scaledVertex = scaleMatrix.Transform(vertex);
        square.Points [i] = scaledVertex;
      }


      Point [] canvasVertices = TransformCoordinates(square.Points);
      polySquare.Points = new PointCollection(canvasVertices);
    }

    private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      int direction = e.Delta > 0 ? 1 : -1;
      currentScale *= direction > 0 ? 1.2 : 0.8;

      ScaleSquare(direction < 0);
      DrawCoordinateSystem();
    }
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
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



    //save
    private void SaveTransformMatrix_Click(object sender, RoutedEventArgs e)
    {
      FileManager.SaveData<string>("Text files (*.txt)|*.txt|All files (*.*)|*.*", "TransformationMatrix.txt",
          (filePath) =>
          {
            StringBuilder matrixStringBuilder = new StringBuilder();
            matrixStringBuilder.AppendLine($"Transformation Matrix:");
            matrixStringBuilder.AppendLine($"{transformMatrix.M11:F4}, {transformMatrix.M12:F4}, {transformMatrix.OffsetX:F4}");
            matrixStringBuilder.AppendLine($"{transformMatrix.M21:F4}, {transformMatrix.M22:F4}, {transformMatrix.OffsetY:F4}");
            matrixStringBuilder.AppendLine($"0, 0, 1");
            File.WriteAllText(filePath, matrixStringBuilder.ToString());
            return "";
          },
          "Матриця перетворення була успішно збережена у файл.",
          "Під час збереження матриці перетворення сталася помилка");
    }
    private void SavePhoto_Click(object sender, RoutedEventArgs e)
    {
      FileManager.SaveData<bool>("JPEG files (*.jpg)|*.jpg|All files (*.*)|*.*", "TransformedPhoto.jpg",
          (filePath) =>
          {
            SaveCanvasAsImage(filePath);
            return true;
          },
          "Фото було успішно збережено.",
          "Під час збереження фото сталася помилка");
    }
    private void SaveCanvasAsImage(string filePath)
    {
      RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int) canvas.ActualWidth, (int) canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
      renderTargetBitmap.Render(canvas);

      BitmapEncoder encoder = new JpegBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

      using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
      {
        encoder.Save(fileStream);
      }
    }
  }

}
