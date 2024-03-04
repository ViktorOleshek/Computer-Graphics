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
using System.Windows.Shapes;

namespace Lab1
{
  /// <summary>
  /// Interaction logic for InputCoordinate.xaml
  /// </summary>
  public partial class InputCoordinate : Window
  {
    public Point [] trapezoidVertices
    {
      get; private set;
    }
    public InputCoordinate()
    {
      InitializeComponent();
    }

    private void AddTrapezoid_Click(object sender, RoutedEventArgs e)
    {
      if (!AreInputsValid())
      {
        MessageBox.Show("Будь ласка, введіть коректні числові значення.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      trapezoidVertices = new Point [4];

      trapezoidVertices [0] = new Point(Convert.ToDouble(textBox1X.Text), Convert.ToDouble(textBox1Y.Text));
      trapezoidVertices [1] = new Point(Convert.ToDouble(textBox2X.Text), Convert.ToDouble(textBox2Y.Text));
      trapezoidVertices [2] = new Point(Convert.ToDouble(textBox3X.Text), Convert.ToDouble(textBox3Y.Text));
      trapezoidVertices [3] = new Point(Convert.ToDouble(textBox4X.Text), Convert.ToDouble(textBox4Y.Text));

      if (!TwoLinesParallel())
      {
        MessageBox.Show("Введені координати не утворюють трапецію.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        trapezoidVertices = null;
        return;
      }

      this.Close();
    }
    private bool TwoLinesParallel()
    {
      bool [] result = new bool [6];

      result [0] = AreLinesParallel(trapezoidVertices [3].X, trapezoidVertices [0].X, trapezoidVertices [1].X, trapezoidVertices [0].X,
                                    trapezoidVertices [3].Y, trapezoidVertices [0].Y, trapezoidVertices [1].Y, trapezoidVertices [0].Y);

      result [1] = AreLinesParallel(trapezoidVertices [3].X, trapezoidVertices [0].X, trapezoidVertices [2].X, trapezoidVertices [1].X,
                                    trapezoidVertices [3].Y, trapezoidVertices [0].Y, trapezoidVertices [2].Y, trapezoidVertices [1].Y);

      result [2] = AreLinesParallel(trapezoidVertices [3].X, trapezoidVertices [0].X, trapezoidVertices [3].X, trapezoidVertices [2].X,
                                    trapezoidVertices [3].Y, trapezoidVertices [0].Y, trapezoidVertices [3].Y, trapezoidVertices [2].Y);

      result [3] = AreLinesParallel(trapezoidVertices [1].X, trapezoidVertices [0].X, trapezoidVertices [2].X, trapezoidVertices [1].X,
                                    trapezoidVertices [1].Y, trapezoidVertices [0].Y, trapezoidVertices [2].Y, trapezoidVertices [1].Y);

      result [4] = AreLinesParallel(trapezoidVertices [1].X, trapezoidVertices [0].X, trapezoidVertices [3].X, trapezoidVertices [2].X,
                                    trapezoidVertices [1].Y, trapezoidVertices [0].Y, trapezoidVertices [3].Y, trapezoidVertices [2].Y);

      result [5] = AreLinesParallel(trapezoidVertices [2].X, trapezoidVertices [1].X, trapezoidVertices [3].X, trapezoidVertices [2].X,
                                    trapezoidVertices [2].Y, trapezoidVertices [1].Y, trapezoidVertices [3].Y, trapezoidVertices [2].Y);

      int numberLinesParallel = result.Count(a => a);

      return numberLinesParallel == 1;
    }

    private bool AreLinesParallel(double x1, double x2, double x3, double x4, double y1, double y2, double y3, double y4)
    {
      double numeratorX = DifferenceAndAbs(x2, x1);
      double denominatorX = DifferenceAndAbs(x4, x3);
      double numeratorY = DifferenceAndAbs(y2, y1);
      double denominatorY = DifferenceAndAbs(y4, y3);

      if ((numeratorX == 0 && denominatorX == 0) || (numeratorY == 0 && denominatorY == 0))
      {//вертикальна або горизонтальна трапеція
        return true;
      }
      else if (denominatorX != 0 && denominatorY != 0)
      {
        double slope1 = numeratorX / denominatorX;
        double slope2 = numeratorY / denominatorY;

        return slope1 == slope2;
      }
      else
        return false;
    }

    private double DifferenceAndAbs(double x, double y)
    {
      return x - y;
    }


    private bool AreInputsValid()
    {
      return double.TryParse(textBox1X.Text, out _) &&
             double.TryParse(textBox1Y.Text, out _) &&
             double.TryParse(textBox2X.Text, out _) &&
             double.TryParse(textBox2Y.Text, out _) &&
             double.TryParse(textBox3X.Text, out _) &&
             double.TryParse(textBox3Y.Text, out _) &&
             double.TryParse(textBox4X.Text, out _) &&
             double.TryParse(textBox4Y.Text, out _);
    }
  }
}
