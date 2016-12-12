using System;
using System.Collections.Generic;
using System.Linq;

namespace NReco.Math3.Als
{
    /// see <a href="http://research.yahoo.com/pub/2433">Collaborative Filtering for Implicit Feedback Datasets</a> 
    // warining: no unit tests for this class
    public class ImplicitFeedbackAlternatingLeastSquaresSolver
    {
        private int numFeatures;
        private double alpha;
        private double lambda;

        private IDictionary<int, double[]> Y;
        private double[,] YtransposeY;

        public ImplicitFeedbackAlternatingLeastSquaresSolver(int numFeatures, double lambda, double alpha, IDictionary<int, double[]> Y)
        {
            this.numFeatures = numFeatures;
            this.lambda = lambda;
            this.alpha = alpha;
            this.Y = Y;
            YtransposeY = GetYtransposeY(Y);
        }

        public double[] Solve(IList<Tuple<int, double>> ratings)
        {
            var otherM = GetYtransponseCuMinusIYPlusLambdaI(ratings);
            var sumM = new double[YtransposeY.GetLength(0), YtransposeY.GetLength(1)];
            for (int i = 0; i < sumM.GetLength(0); i++)
                for (int j = 0; j < sumM.GetLength(1); j++)
                    sumM[i, j] = YtransposeY[i, j] + otherM[i, j];

            return Solve(sumM, GetYtransponseCuPu(ratings));
        }

        private static double[] Solve(double[,] A, double[,] y)
        {
            return MatrixUtil.ViewColumn(new QRDecomposition(A).Solve(y), 0);
        }

        double Confidence(double rating)
        {
            return 1 + alpha * rating;
        }

        /// Y' Y 
        private double[,] GetYtransposeY(IDictionary<int, double[]> Y)
        {

            var indexes = Y.Keys.ToList();
            indexes.Sort(); //.quickSort();
            int numIndexes = indexes.Count;

            double[,] YtY = new double[numFeatures, numFeatures];

            // Compute Y'Y by dot products between the 'columns' of Y
            for (int i = 0; i < numFeatures; i++)
            {
                for (int j = i; j < numFeatures; j++)
                {
                    double dot = 0;
                    for (int k = 0; k < numIndexes; k++)
                    {
                        double[] row = Y[indexes[k]];
                        dot += row[i] * row[j];
                    }
                    YtY[i, j] = dot;
                    if (i != j)
                    {
                        YtY[j, i] = dot;
                    }
                }
            }
            return YtY;
        }

        /// Y' (Cu - I) Y + λ I 
        private double[,] GetYtransponseCuMinusIYPlusLambdaI(IList<Tuple<int, double>> userRatings)
        {
            //Preconditions.checkArgument(userRatings.isSequentialAccess(), "need sequential access to ratings!");

            /// (Cu -I) Y 
            var CuMinusIY = new Dictionary<int, double[]>(userRatings.Count);
            foreach (var e in userRatings)
            {
                CuMinusIY[e.Item1] = MatrixUtil.Times(Y[e.Item1], Confidence(e.Item2) - 1);
            }

            var YtransponseCuMinusIY = new double[numFeatures, numFeatures];

            /// Y' (Cu -I) Y by outer products 
            foreach (var e in userRatings)
            {
                var currentEIdx = e.Item1;
                var featureIdx = 0;
                foreach (var feature in Y[currentEIdx])
                {
                    var currentFeatIdx = featureIdx++;
                    var partial = MatrixUtil.Times(CuMinusIY[currentEIdx], feature);

                    for (int i = 0; i < YtransponseCuMinusIY.GetLength(1); i++)
                    {
                        //YtransponseCuMinusIY.viewRow(feature.index()).assign(partial, Functions.PLUS);
                        YtransponseCuMinusIY[currentFeatIdx, i] += partial[i];
                    }
                }
            }

            /// Y' (Cu - I) Y + λ I  add lambda on the diagonal 
            for (int feature = 0; feature < numFeatures; feature++)
            {
                YtransponseCuMinusIY[feature, feature] = YtransponseCuMinusIY[feature, feature] + lambda;
            }

            return YtransponseCuMinusIY;
        }

        /// Y' Cu p(u) 
        private double[,] GetYtransponseCuPu(IList<Tuple<int, double>> userRatings)
        {
            //Preconditions.checkArgument(userRatings.isSequentialAccess(), "need sequential access to ratings!");

            double[] YtransponseCuPu = new double[numFeatures];

            foreach (var e in userRatings)
            {
                //YtransponseCuPu.assign(Y.get(e.index()).times(confidence(e.get())), Functions.PLUS);
                //	Y.get(e.index()).times(confidence(e.get()))
                var other = MatrixUtil.Times(Y[e.Item1], Confidence(e.Item2));

                for (int i = 0; i < YtransponseCuPu.Length; i++)
                    YtransponseCuPu[i] += other[i];
            }

            return ColumnVectorAsMatrix(YtransponseCuPu);
        }

        private double[,] ColumnVectorAsMatrix(double[] v)
        {
            double[,] matrix = new double[numFeatures, 1];
            for (int i = 0; i < v.Length; i++)
            {
                matrix[i, 0] = v[i];
            }
            return matrix;
        }
    }
}