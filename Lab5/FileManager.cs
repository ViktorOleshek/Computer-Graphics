using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lab5
{
  public static class FileManager
  {
    public static void SaveData<T>(string filter, string defaultFileName, Func<string, T> saveAction, string successMessage, string errorMessage)
    {
      Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
      saveFileDialog.Filter = filter;
      saveFileDialog.FileName = defaultFileName;
      bool? result = saveFileDialog.ShowDialog();

      if (result == true)
      {
        try
        {
          string filePath = saveFileDialog.FileName;
          saveAction(filePath);
          MessageBox.Show(successMessage, "Успішно", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"{errorMessage}: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
    }
  }
}
