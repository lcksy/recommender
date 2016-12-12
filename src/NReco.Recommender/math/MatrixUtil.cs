using System;
using System.Collections.Generic;

namespace NReco.Math3
{
    /// <summary>
    /// 
    /// </summary>
    public static class MatrixUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static IEnumerable<double> NonZeroes(double[] vector)
        {
            for (int i = 0; i < vector.Length; i++)
                if (vector[i] != 0.0)
                    yield return vector[i];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double VectorDot(double[] v1, double[] v2)
        {
            double r = 0d;
            for (int i = 0; i < v1.Length; i++)
            {
                r += (v1[i] * v2[i]);
            }
            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="v"></param>
        public static void WriteVector(string msg, double[] v)
        {
            Console.Write("{0}: ", msg);
            foreach (var x in v)
                Console.Write("{0} ", x);
            Console.WriteLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static double[] ViewColumn(double[,] m, int column)
        {
            var v = new double[m.GetLength(0)];
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = m[i, column];
            }
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static double[] ViewRow(double[,] m, int row)
        {
            var v = new double[m.GetLength(1)];
            for (int i = 0; i < v.Length; i++)
            {
                v[i] = m[row, i];
            }
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double[] ViewDiagonal(double[,] m)
        {
            var v = new double[Math.Min(m.GetLength(0), m.GetLength(1))];
            for (int i = 0; i < v.Length; i++)
                v[i] = m[i, i];
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double Norm2(double[] v)
        {
            return Math.Sqrt(VectorDot(v, v));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double Norm1(double[] v)
        {
            double res = 0;
            for (int i = 0; i < v.Length; i++)
                res += Math.Abs(v[i]);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="rowOff"></param>
        /// <param name="rowRequested"></param>
        /// <param name="colOff"></param>
        /// <param name="colRequested"></param>
        /// <returns></returns>
        public static double[,] ViewPart(double[,] m, int rowOff, int rowRequested, int colOff, int colRequested)
        {
            var r = new double[rowRequested - rowOff, colRequested - colOff];
            for (int i = rowOff; i < rowRequested; i++)
                for (int j = colOff; j < colRequested; j++)
                    r[i - rowOff, j - colOff] = m[i, j];
            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static double[,] Transpose(double[,] m)
        {
            int rows = m.GetLength(0); //rowSize();
            int columns = m.GetLength(1); //columnSize();
            var result = new double[columns, rows];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    result[col, row] = m[row, col]; //.setQuick(col, row, getQuick(row, col));
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="f"></param>
        public static void Assign(double[,] m, Func<double, double> f)
        {
            var rLen = m.GetLength(0);
            var cLen = m.GetLength(1);
            for (int i = 0; i < rLen; i++)
                for (int j = 0; j < cLen; j++)
                    m[i, j] = f(m[i, j]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="f"></param>
        public static void Assign(double[] v, Func<double, double> f)
        {
            for (int i = 0; i < v.Length; i++)
                v[i] = f(v[i]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static double[,] Times(double[,] m, double[,] other)
        {
            int columns = m.GetLength(1); //columnSize();
            if (columns != other.GetLength(0))
            { //.rowSize()
                throw new System.Exception();
            }
            int rows = m.GetLength(0); //rowSize();
            int otherColumns = other.GetLength(1);  //columnSize();
            double[,] result = new double[rows, otherColumns];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < otherColumns; col++)
                {
                    double sum = 0.0;
                    for (int k = 0; k < columns; k++)
                    {
                        sum += m[row, k] * other[k, col];
                    }
                    result[row, col] = sum;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double[] Times(double[] v, double x)
        {
            var vCopy = new double[v.Length];
            for (int i = 0; i < v.Length; i++)
                vCopy[i] = v[i] * x;
            return vCopy;
        }
    }
}