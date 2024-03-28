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

namespace Lab3.InputForms
{
  /// <summary>
  /// Логика взаимодействия для AlgebraicFractal.xaml
  /// </summary>
  public partial class AlgebraicFractal : Window
  {
    public double ConstantC
    {
      get; private set;
    }
    public int Iterations
    {
      get; private set;
    }
    public int ColorScheme
    {
      get; private set;
    }

    public AlgebraicFractal()
    {
      InitializeComponent();
    }

    private void BuildFractalButton_Click(object sender, RoutedEventArgs e)
    {
      // Перевірка введених даних
      if (!double.TryParse(textBox_constant.Text, out double constantC))
      {
        MessageBox.Show("Невірний формат константи (c).");
        return;
      }

      if (!int.TryParse(textBox_iterations.Text, out int iterations))
      {
        MessageBox.Show("Невірний формат кількості ітерацій.");
        return;
      }

      if (!int.TryParse(textBox_colorScheme.Text, out int colorScheme))
      {
        MessageBox.Show("Невірний формат кількості ітерацій.");
        return;
      }

      // Збереження введених даних
      ConstantC = constantC;
      Iterations = iterations;
      ColorScheme = colorScheme;

      // Закриття вікна
      DialogResult = true;
    }
  }
}
