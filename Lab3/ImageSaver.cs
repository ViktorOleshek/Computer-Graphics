using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab3
{
  public static class ImageSaver
  {
    public static void SaveImage(FrameworkElement visual, string filePath, string format)
    {
      if (visual == null)
      {
        MessageBox.Show("Помилка: переданий елемент візуалу є недійсним.");
        return;
      }

      if (string.IsNullOrWhiteSpace(filePath))
      {
        MessageBox.Show("Помилка: не вказаний шлях для збереження файлу.");
        return;
      }

      RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
          (int) visual.ActualWidth,
          (int) visual.ActualHeight,
          96d,
          96d,
          PixelFormats.Default);
      renderBitmap.Render(visual);

      BitmapEncoder encoder = null;
      switch (format.ToLower())
      {
        case "png":
          encoder = new PngBitmapEncoder();
          break;
        case "jpg":
        case "jpeg":
          encoder = new JpegBitmapEncoder();
          break;
        case "bmp":
          encoder = new BmpBitmapEncoder();
          break;
        default:
          MessageBox.Show("Помилка: непідтримуваний формат зображення.");
          return;
      }
      encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

      try
      {
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
          encoder.Save(fs);
        }
        MessageBox.Show($"Зображення успішно збережено за шляхом {filePath}");
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Помилка при збереженні зображення: {ex.Message}");
      }
    }
  }
}
