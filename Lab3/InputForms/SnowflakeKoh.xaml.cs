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
  /// Логика взаимодействия для SnowflakeKoh.xaml
  /// </summary>
  public partial class SnowflakeKoh : Window
  {
    public int NumberOfIterations
    {
      get; private set;
    }
    public double CenterX
    {
      get; private set;
    }
    public double CenterY
    {
      get; private set;
    }
    public double Step
    {
      get; private set;
    }
    public SnowflakeKoh()
    {
      InitializeComponent();
    }
    private void BuildFractalButton_Click(object sender, RoutedEventArgs e)
    {
      // Перевірка коректності введених даних
      if (!int.TryParse(textBox_numbersOfStep.Text, out int numberOfIterations) || numberOfIterations <= 0)
      {
        MessageBox.Show("Невірне значення кількості ітерацій.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      if (!double.TryParse(textBox_centerX.Text, out double centerX)
        || !double.TryParse(textBox_centerY.Text, out double centerY))
      {
        MessageBox.Show("Невірне значення центра генерування.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      if (!double.TryParse(textBox_step.Text, out double step) || step <= 0)
      {
        MessageBox.Show("Невірне значення кроку.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      NumberOfIterations = numberOfIterations - 1;
      CenterX = centerX;
      CenterY = centerY;
      Step = step;
      DialogResult = true;
    }
  }
}
