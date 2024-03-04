using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
  internal class Matrix
  {
    private readonly double [,] data;

    public Matrix(int rows, int columns)
    {
      data = new double [rows, columns];
    }

    public double this [int row, int column]
    {
      get
      {
        return data [row, column];
      }
      set
      {
        data [row, column] = value;
      }
    }

    public static Matrix operator *(Matrix m1, Matrix m2)
    {
      int rows1 = m1.data.GetLength(0);
      int cols1 = m1.data.GetLength(1);
      int cols2 = m2.data.GetLength(1);

      Matrix result = new Matrix(rows1, cols2);

      for (int i = 0; i < rows1; ++i)
      {
        for (int j = 0; j < cols2; ++j)
        {
          double sum = 0;
          for (int k = 0; k < cols1; ++k)
          {
            sum += m1 [i, k] * m2 [k, j];
          }
          result [i, j] = sum;
        }
      }

      return result;
    }

    public static double [,] CreateCoefMatrix(int n)
    {
      double [,] matrix;
      switch (n)
      {
        case 2:
          matrix = new double [3, 3] {
                        { 1,  0, 0 },
                        { -2, 2, 0 },
                        { 1, -2, 1 }
                    };

          break;
        case 3:
          matrix = new double [4, 4] {
                        { 1.0, 0.0, 0.0, 0.0 },
                        { -3.0, 3.0, 0.0, 0.0 },
                        { 3.0, -6.0, 3.0, 0.0 },
                        { -1.0, 3.0, -3.0, 1.0 }
                    };

          break;
        case 4:
          matrix = new double [5, 5] {
                        { 1, 0, 0, 0, 0 },
                        { -4, 4, 0, 0, 0 },
                        { 6, -12, 6, 0, 0 },
                        { -4, 12, -12, 4, 0 },
                        { 1, -4, 6, -4, 1 }
                    };

          break;
        case 5:
          matrix = new double [6, 6]
          {
                        { 1, 0, 0, 0, 0, 0 },
                        { -5, 5, 0, 0, 0, 0 },
                        { 10, -20, 10, 0, 0, 0 },
                        { -10, 30, -30, 10, 0, 0 },
                        { 5, -20, 30, -20, 5, 0 },
                        { -1, 5, -10, 10, -5, 1 }
      };

          break;
        case 6:
          matrix = new double [7, 7]
          {
                        { 1, 0, 0, 0, 0, 0, 0 },
                        { -6, 6, 0, 0, 0, 0, 0 },
                        { 15, -30, 15, 0, 0, 0, 0 },
                        { -20, 60, -60, 20, 0, 0, 0 },
                        { 15, -60, 90, -60, 15, 0, 0 },
                        { -6, 30, -60, 60, -30, 6, 0 },
                        { 1, -6, 15, -20, 15, -6, 1 }
          };

          break;
        case 7:
          matrix = new double [8, 8]
          {
                        { 1, 0, 0, 0, 0, 0, 0, 0 },
                        { -7, 7, 0, 0, 0, 0, 0, 0 },
                        { 21, -42, 21, 0, 0, 0, 0, 0 },
                        { -35, 105, -105, 35, 0, 0, 0, 0 },
                        { 35, -140, 210, -140, 35, 0, 0, 0 },
                        { -21, 105, -210, 210, -105, 21, 0, 0 },
                        { 7, -42, 105, -140, 105, -42, 7, 0 },
                        { -1, 7, -21, 35, -35, 21, -7, 1 }
          };

          break;
        default:
          throw new ArgumentException("Invalid n");
      }

      return matrix;
    }
  }
}
