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

namespace Lab5
{
  /// <summary>
  /// Логика взаимодействия для SetSquareCoordinate.xaml
  /// </summary>
  public partial class SetSquareCoordinate : Window
  {
    public Square square {  get; private set; }

    public SetSquareCoordinate()
    {
      InitializeComponent();
    }

    private void AddSquare_Click(object sender, RoutedEventArgs e)
    {
      if (!AreInputsValid())
      {
        MessageBox.Show("Будь ласка, введіть коректні числові значення.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      Point [] squareVertices = new Point [4];
      squareVertices [0] = new Point(Convert.ToDouble(textBox1X.Text), Convert.ToDouble(textBox1Y.Text));
      squareVertices [1] = new Point(Convert.ToDouble(textBox2X.Text), Convert.ToDouble(textBox2Y.Text));
      squareVertices [2] = new Point(Convert.ToDouble(textBox3X.Text), Convert.ToDouble(textBox3Y.Text));
      square = new Square(squareVertices [0], squareVertices [1], squareVertices [2]);

      if (!square.IsSquare())
      {
        MessageBox.Show("Введені координати не утворюють квадрат.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        square = null;
        squareVertices = null;
        return;
      }

      this.Close();
    }
    private bool AreInputsValid()
    {
      return double.TryParse(textBox1X.Text, out _) &&
             double.TryParse(textBox1Y.Text, out _) &&
             double.TryParse(textBox2X.Text, out _) &&
             double.TryParse(textBox2Y.Text, out _) &&
             double.TryParse(textBox3X.Text, out _) &&
             double.TryParse(textBox3Y.Text, out _);
    }
  }
}
