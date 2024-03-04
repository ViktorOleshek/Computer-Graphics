using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab2
{
  public partial class MainWindow : Window
  {
    private ObservableCollection<Point> controlPoints = new ObservableCollection<Point>();
    private Polyline polyline;
    private double currentScale = 1.0;
    int intervalCount = 13;
    double intervalSize = 35;
    private Ellipse selectedControlPoint;
    private Point [] transformedPoints; // Додайте це як поле класу

    public MainWindow()
    {
      InitializeComponent();
      InitializeUI();
    }

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

    private void DrawUnitIntervals(double centerX, double centerY, double width, double height, bool isXAxis)
    {
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
              Content = (i * currentScale).ToString("F2"),
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

    private Point [] TransformCoordinates(Point [] vertices, double originX, double originY)
    {
      Point [] transformedVertices = new Point [vertices.Length];

      for (int i = 0; i < vertices.Length; i++)
      {
        double x = originX + vertices [i].X * currentScale * intervalSize;
        double y = originY - vertices [i].Y * currentScale * intervalSize;
        transformedVertices [i] = new Point(x, y);
      }

      return transformedVertices;
    }

    private void InitializeUI()
    {
      //btnDrawCurve.Click += BtnDrawCurve_Click;

      polyline = new Polyline
      {
        Stroke = Brushes.Black,
        StrokeThickness = 2
      };
      canvas.Children.Add(polyline);

      lstControlPoints.ItemsSource = controlPoints;

      foreach (var controlPoint in canvas.Children.OfType<Ellipse>())
      {
        controlPoint.MouseDown += ControlPoint_MouseDown;
        controlPoint.MouseUp += ControlPoint_MouseUp;
        controlPoint.MouseMove += ControlPoint_MouseMove;
      }

      canvas.MouseUp += Canvas_MouseUp;
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
      double x, y;
      if (double.TryParse(txtX.Text, out x) && double.TryParse(txtY.Text, out y))
      {
        controlPoints.Add(new Point(x, y));
        CreateControlPoint(x, y);
      }
      else
      {
        MessageBox.Show("Некоректні координати точки. Введіть числові значення.");
      }
    }

    private void BtnDrawCurve_Click(object sender, RoutedEventArgs e)
    {
      MatrixInfo.Text = null;
      DrawBezierCurve();
    }

    private void DrawBezierCurve()
    {
      polyline.Points.Clear();

      if (controlPoints.Count < 2)
      {
        MessageBox.Show("Для побудови кривої потрібно не менше двох точок.");
        return;
      }

      double centerX = canvas.ActualWidth / 2;
      double centerY = canvas.ActualHeight / 2;

      transformedPoints = TransformCoordinates(controlPoints.ToArray(), centerX, centerY);

      for (double t = 0; t <= 1; t += 0.01)
      {
        Point? curvePoint = CalculateBezierPoint(t);
        if (curvePoint == null) { return; }
        polyline.Points.Add((Point) curvePoint);
      }

      DisplayMatrixInformation();
      HighlightPolygon();
    }

    private void DisplayMatrixInformation()
    {
      StringBuilder matrixInfo = new StringBuilder();
      int n = controlPoints.Count - 1;
      double [,] matrix = Matrix.CreateCoefMatrix(n);


      for (int i = 0; i <= n; i++)
      {
        matrixInfo.AppendLine($"Для {i + 1} рядка:");

        for (int j = 0; j <= n; j++)
        {
          double coefficient = BinomialCoefficient(n, j) * Math.Pow((1 - i), (n - j)) * Math.Pow(i, j);
          //matrix [i, j] = coefficient;
          if (matrix [i, j] != 0)
          {
            matrixInfo.AppendLine($"  Коефіцієнт {j}: {matrix [i, j]}");

          }
        }
      }

      // Виведення інформації про матрицю
      matrixInfo.AppendLine("\nМатриця коефіцієнтів:");
      for (int i = 0; i <= n; i++)
      {
        for (int j = 0; j <= n; j++)
        {
          matrixInfo.Append($"{matrix [i, j]:F2}\t");
        }
        matrixInfo.AppendLine();
      }

      double sumOfDiagonal = 0;

      // Обчислення суми діагональних елементів
      for (int i = 0; i <= n; i++)
      {
        sumOfDiagonal += matrix [i, i];
      }

      matrixInfo.AppendLine($"\nСума діагональних елементів: {sumOfDiagonal:F2}");

      MatrixInfo.Text = matrixInfo.ToString();
    }

    private Point? CalculateBezierPoint(double t)
    {
      var str = Formula.SelectedValue?.ToString();
      if (str == null)
      {
        MessageBox.Show("Виберіть за якою формулою обчислювати.");
        return null;
      }
      else if (str.Equals("Параметрична формула"))
      {
        return CalculateParametricFormula(t);
      }
      else
      {
        return CalculateMatrixFormula(t);
      }
    }


    private void HighlightPolygon()
    {
      Point [] transformedPoints = TransformCoordinates(controlPoints.ToArray(), canvas.ActualWidth / 2, canvas.ActualHeight / 2);

      Polygon polygon = new Polygon
      {
        Stroke = Brushes.Blue,
        StrokeThickness = 2,
        Opacity = 0.7
      };

      PointCollection polygonPoints = new PointCollection();
      foreach (var point in transformedPoints)
      {
        polygonPoints.Add(point);
      }

      polygon.Points = polygonPoints;
      canvas.Children.Add(polygon);
    }
    private Point CalculateMatrixFormula(double t)
    {
      int n = transformedPoints.Length - 1;
      Matrix matrix = new Matrix(1, n + 1);

      for (int i = 0; i <= n; i++)
      {
        matrix [0, i] = BinomialCoefficient(n, i) * Math.Pow((1 - t), (n - i)) * Math.Pow(t, i);
      }

      Matrix controlPointsMatrix = new Matrix(n + 1, 2);
      for (int i = 0; i <= n; i++)
      {
        controlPointsMatrix [i, 0] = transformedPoints [i].X;
        controlPointsMatrix [i, 1] = transformedPoints [i].Y;
      }

      Matrix result = matrix * controlPointsMatrix;

      return new Point(result [0, 0], result [0, 1]);
    }

    private Point CalculateParametricFormula(double t)
    {
      int n = transformedPoints.Length - 1;
      double x = 0, y = 0;

      for (int i = 0; i <= n; i++)
      {
        double coefficient = BinomialCoefficient(n, i) * Math.Pow((1 - t), (n - i)) * Math.Pow(t, i);
        x += coefficient * transformedPoints [i].X;
        y += coefficient * transformedPoints [i].Y;
      }

      return new Point(x, y);
    }

    private double BinomialCoefficient(int n, int i)
    {
      return Factorial(n) / (Factorial(i) * Factorial(n - i));
    }

    private double Factorial(int n)
    {
      double result = 1;
      for (int i = 1; i <= n; i++)
      {
        result *= i;
      }
      return result;
    }

    private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
      selectedControlPoint = null;
    }

    private void ControlPoint_MouseDown(object sender, MouseButtonEventArgs e)
    {
      selectedControlPoint = (Ellipse) sender;
      e.Handled = true;
    }

    private void ControlPoint_MouseUp(object sender, MouseButtonEventArgs e)
    {
      selectedControlPoint = null;
    }

    private void ControlPoint_MouseMove(object sender, MouseEventArgs e)
    {
      if (selectedControlPoint != null && e.LeftButton == MouseButtonState.Pressed)
      {
        var ellipse = (Ellipse) sender;
        var position = e.GetPosition(canvas);
        var index = canvas.Children.IndexOf(ellipse);

        controlPoints [index] = new Point(
            (position.X - canvas.ActualWidth / 2) / (currentScale * intervalSize),
            (canvas.ActualHeight / 2 - position.Y) / (currentScale * intervalSize)
        );

        DrawBezierCurve();
      }
    }

    private void CreateControlPoint(double x, double y)
    {
      Ellipse controlPoint = new Ellipse
      {
        Width = 10,
        Height = 10,
        Fill = Brushes.Red,
        Stroke = Brushes.Black,
        StrokeThickness = 1,
        DataContext = new Point(x, y)
      };

      Canvas.SetLeft(controlPoint, (x * intervalSize * currentScale) + canvas.ActualWidth / 2 - controlPoint.Width / 2);
      Canvas.SetTop(controlPoint, canvas.ActualHeight / 2 - (y * intervalSize * currentScale) - controlPoint.Height / 2);

      controlPoint.MouseUp += ControlPoint_MouseUp;
      controlPoint.MouseDown += ControlPoint_MouseDown;
      controlPoint.MouseMove += ControlPoint_MouseMove;

      canvas.Children.Add(controlPoint);
    }

    private void CalculateBezierPointsInterval(object sender, RoutedEventArgs e)
    {
      StringBuilder pointsStringBuilder = new StringBuilder();

      double minT, maxT, step;
      if (double.TryParse(minValue.Text, out minT) && double.TryParse(maxValue.Text, out maxT) && double.TryParse(stepLenght.Text, out step))
      {
        if (maxT < minT)
        {
          MessageBox.Show("Максимальне значення t повинно бути більше мінімального значення t.");
          return;
        }

        double t = minT;
        while (t <= maxT)
        {
          Point? curvePoint = CalculateBezierPoint(t);
          if (curvePoint == null) { return; }

          double x = ((Point) curvePoint).X * currentScale;
          double y = ((Point) curvePoint).Y * currentScale;

          pointsStringBuilder.AppendLine($"Точка для t = {t}: X = {x}, Y = {y}");
          t += step;
        }

        MessageBox.Show(pointsStringBuilder.ToString(), "Координати точок кривої");
      }
      else
      {
        MessageBox.Show("Некоректно введені значення мінімального, максимального або кроку.");
      }
    }

  }
}
