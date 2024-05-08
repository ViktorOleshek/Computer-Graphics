using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lab5
{
  public class Square
  {
    public Point [] Points { get; }
    public Point Center
    {
      get
      {
        double centerX = (Points [0].X + Points [2].X) / 2;
        double centerY = (Points [0].Y + Points [2].Y) / 2;
        return new Point(centerX, centerY);
      }
    }


    public Square(Point point1, Point point2, Point point3)
    {
      Points = new Point [4];
      Points [0] = point1;
      Points [1] = point2;
      Points [2] = point3;
      Points [3] = CalculateFourthPoint();
    }
    private Point CalculateFourthPoint()
    {
      // Реалізація алгоритму для знаходження четвертої точки
      double x4 = Points [0].X + Points [2].X - Points [1].X;
      double y4 = Points [0].Y + Points [2].Y - Points [1].Y;
      return new Point(x4, y4);
    }

    public bool IsSquare()
    {
      double side1 = Distance(Points [0], Points [1]);
      double side2 = Distance(Points [1], Points [2]);
      double side3 = Distance(Points [2], Points [3]);
      double side4 = Distance(Points [3], Points [0]);

      return Math.Abs(side1 - side2) < 0.0001 &&
             Math.Abs(side2 - side3) < 0.0001 &&
             Math.Abs(side3 - side4) < 0.0001 &&
             Math.Abs(side4 - side1) < 0.0001 &&
             IsRightAngle();
    }

    private bool IsRightAngle()
    {
      double diagonal1 = Distance(Points [0], Points [2]);
      double diagonal2 = Distance(Points [1], Points [3]);

      return Math.Abs(diagonal1 - diagonal2) < 0.0001;
    }
    private double Distance(Point p1, Point p2)
    {
      return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }
  }
}
