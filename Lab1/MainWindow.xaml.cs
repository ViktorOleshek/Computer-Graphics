using System;
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

namespace Lab1
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }
    private double currentScale = 1.0;
    Color newColor = Colors.Red;

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      DrawCoordinateSystem();
    }
    private void DrawCoordinateSystem()
    {
      double width = mainGrid.ActualWidth;
      double height = mainGrid.RowDefinitions [0].ActualHeight;
      double centerX = width / 2;
      double centerY = height / 2;

      DrawAxis(centerX, centerY, width, height, true, "X");
      DrawAxis(centerX, centerY, width, height, false, "Y");

      DrawUnitIntervals(centerX, centerY, width, height, true);
      DrawUnitIntervals(centerX, centerY, width, height, false);

      InvalidateVisual();
    }

    /// <summary>
    /// побудова одиничних проміжків на осі
    /// </summary>
    private void DrawUnitIntervals(double centerX, double centerY, double width, double height, bool isXAxis)
    {
      int intervalCount = 10;
      double intervalSize = 40;

      if (isXAxis)
      {
        for (int i = -intervalCount; i <= intervalCount; i++)
        {
          double x = centerX + i * intervalSize * currentScale;
          double y1 = centerY - 5;
          double y2 = centerY + 5;

          if (x >= 0 && x <= width)
          {
            Label label = new Label
            {
              Content = (i * currentScale).ToString("F2"), // Використовуйте формат "F2" для двох знаків після коми
              Margin = new Thickness(x - 5, centerY + 10, 0, 0)
            };

            canvas.Children.Add(label);
          }

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
          double y = centerY - i * intervalSize * currentScale;

          if (y >= 0 && y <= height)
          {
            Label label = new Label
            {
              Content = (i * currentScale).ToString("F2"),
              Margin = new Thickness(centerX - 20, y - 5, 0, 0)
            };
            canvas.Children.Add(label);
          }

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
    /// <summary>
    /// побудова осі, стрілки і підпису
    /// </summary>
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

      if (isXAxis)
      {
        axis.X1 = 0;
        axis.Y1 = centerY;
        axis.X2 = width;
        axis.Y2 = centerY;

        //стрілка
        axisArrow.Points.Add(new Point(width, centerY));
        axisArrow.Points.Add(new Point(width - 10, centerY - 5));
        axisArrow.Points.Add(new Point(width - 10, centerY + 5));

        axisLabel.Margin = new Thickness(width - 20, centerY - 20, 0, 0);
      }
      else
      {
        axis.X1 = centerX;
        axis.Y1 = 0;
        axis.X2 = centerX;
        axis.Y2 = height;

        //стрілка
        axisArrow.Points.Add(new Point(centerX, 0));
        axisArrow.Points.Add(new Point(centerX - 5, 10));
        axisArrow.Points.Add(new Point(centerX + 5, 10));

        axisLabel.Margin = new Thickness(centerX + 10, 0, 0, 0);
      }

      axisArrow.Fill = Brushes.Black;

      canvas.Children.Add(axis);
      canvas.Children.Add(axisArrow);
      canvas.Children.Add(axisLabel);
    }
    private void Button_Add(object sender, RoutedEventArgs e)
    {
      InputCoordinate input = new InputCoordinate();
      input.ShowDialog();
      Point [] trapezoidVertices = input.trapezoidVertices;

      if (trapezoidVertices != null)
      {
        DrawTrapezoid(trapezoidVertices, newColor);
        DrawCoordinateSystem();
      }
    }

    private Point [] TransformCoordinates(Point [] vertices)
    {
      Point [] transformedVertices = new Point [vertices.Length];

      double centerX = canvas.ActualWidth / 2;
      double centerY = canvas.ActualHeight / 2;

      for (int i = 0; i < vertices.Length; i++)
      {
        double x = centerX + vertices [i].X * currentScale * 40;
        double y = centerY - vertices [i].Y * currentScale * 40;
        transformedVertices [i] = new Point(x, y);
      }

      return transformedVertices;
    }
    private void DrawTrapezoid(Point [] vertices, Color color)
    {
      Point [] transformedVertices = TransformCoordinates(vertices);

      Polygon trapezoid = new Polygon
      {
        Fill = new SolidColorBrush(color),
        Points = new PointCollection(transformedVertices)
      };

      canvas.Children.Add(trapezoid);

      DrawMedians(trapezoid);

      InvalidateVisual();
    }
    /// <summary>
    /// будуємо 8 медіан трапеції
    /// </summary>
    private void DrawMedians(Polygon trapezoid)
    {
      PointCollection vertices = trapezoid.Points;
      int vertexCount = vertices.Count;

      for (int i = 0; i < vertexCount; i++)
      {
        int opposite1 = (i + 1) % vertexCount;

        double midX = (vertices [i].X + vertices [opposite1].X) / 2;
        double midY = (vertices [i].Y + vertices [opposite1].Y) / 2;

        Line median1 = new Line
        {
          X1 = midX,
          Y1 = midY,
          X2 = vertices [(i + 2) % vertexCount].X,
          Y2 = vertices [(i + 2) % vertexCount].Y,
          Stroke = Brushes.Black,
          StrokeThickness = 1
        };

        canvas.Children.Add(median1);

        int opposite2 = (i + 3) % vertexCount;

        midX = (vertices [i].X + vertices [opposite2].X) / 2;
        midY = (vertices [i].Y + vertices [opposite2].Y) / 2;

        Line median2 = new Line
        {
          X1 = midX,
          Y1 = midY,
          X2 = vertices [(i + 2) % vertexCount].X,
          Y2 = vertices [(i + 2) % vertexCount].Y,
          Stroke = Brushes.Black,
          StrokeThickness = 1
        };

        canvas.Children.Add(median2);
      }
    }

    private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      double delta = e.Delta > 0 ? 0.2 : -0.2;
      currentScale += delta;
      testLabel.Content = currentScale;

      foreach (UIElement child in canvas.Children.OfType<Label>())
      {
        Label label = (Label) child;
        if (double.TryParse(label.Content.ToString(), out double value))
        {
          value *= currentScale;
          label.Content = value.ToString("F2"); // Використовуйте формат "F2" для двох знаків після коми
        }
      }

      // Оновлення масштабу проміжків по осі X
      double centerX = canvas.Width / 2;
      double centerY = canvas.Height / 2;
      foreach (UIElement child in canvas.Children.OfType<Line>().Where(l => l.Name == "XIntervalLine"))
      {
        Line intervalLine = (Line) child;
        double originalX = (double) intervalLine.GetValue(Canvas.LeftProperty);
        double newX = centerX + (originalX - centerX) * currentScale;
        intervalLine.X1 = newX;
        intervalLine.X2 = newX;
      }

      // Оновлення масштабу проміжків по осі Y
      foreach (UIElement child in canvas.Children.OfType<Line>().Where(l => l.Name == "YIntervalLine"))
      {
        Line intervalLine = (Line) child;
        double originalY = (double) intervalLine.GetValue(Canvas.TopProperty);
        double newY = centerY + (originalY - centerY) * currentScale;
        intervalLine.Y1 = newY;
        intervalLine.Y2 = newY;
      }
    }

    private void ColorPicker_ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      newColor = (Color) e.NewValue;

    }
  }
}
