using System.Collections.Generic;

namespace NReco.Math3.Als
{
    /// See
    /// <a href="http://www.hpl.hp.com/personal/Robert_Schreiber/papers/2008%20AAIM%20Netflix/netflix_aaim08(submitted).pdf">
    /// this paper.</a>
    public sealed class AlternatingLeastSquaresSolver
    {
        private AlternatingLeastSquaresSolver() { }

        //TODO make feature vectors a simple array
        public static double[] Solve(IList<double[]> featureVectors, double[] ratingVector, double lambda, int numFeatures)
        {
            //Preconditions.checkNotNull(featureVectors, "Feature Vectors cannot be null");
            //Preconditions.checkArgument(!Iterables.isEmpty(featureVectors));
            //Preconditions.checkNotNull(ratingVector, "Rating Vector cannot be null");
            //Preconditions.checkArgument(ratingVector.getNumNondefaultElements() > 0, "Rating Vector cannot be empty");
            //Preconditions.checkArgument(Iterables.size(featureVectors) == ratingVector.getNumNondefaultElements());

            int nui = ratingVector.Length; //.getNumNondefaultElements();

            var MiIi = CreateMiIi(featureVectors, numFeatures);
            var RiIiMaybeTransposed = CreateRiIiMaybeTransposed(ratingVector);

            /// compute Ai = MiIi * t(MiIi) + lambda * nui * E 
            var Ai = MiTimesMiTransposePlusLambdaTimesNuiTimesE(MiIi, lambda, nui);
            /// compute Vi = MiIi * t(R(i,Ii)) 
            var Vi = MatrixUtil.Times(MiIi, RiIiMaybeTransposed);
            /// compute Ai * ui = Vi 
            return Solve(Ai, Vi);
        }

        private static double[] Solve(double[,] Ai, double[,] Vi)
        {
            return MatrixUtil.ViewColumn(new QRDecomposition(Ai).Solve(Vi), 0);
        }

        public static double[,] AddLambdaTimesNuiTimesE(double[,] matrix, double lambda, int nui)
        {
            //Preconditions.checkArgument(matrix.numCols() == matrix.numRows(), "Must be a Square Matrix");
            double lambdaTimesNui = lambda * nui;
            int numCols = matrix.GetLength(1); //numCols();
            for (int n = 0; n < numCols; n++)
            {
                matrix[n, n] += matrix[n, n] + lambdaTimesNui;
            }
            return matrix;
        }

        private static double[,] MiTimesMiTransposePlusLambdaTimesNuiTimesE(double[,] MiIi, double lambda, int nui)
        {
            double lambdaTimesNui = lambda * nui;
            int rows = MiIi.GetLength(0); //.numRows();

            double[,] result = new double[rows, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = i; j < rows; j++)
                {
                    double dot = MatrixUtil.VectorDot(MatrixUtil.ViewRow(MiIi, i), MatrixUtil.ViewRow(MiIi, j));
                    if (i != j)
                    {
                        result[i, j] = dot;
                        result[j, i] = dot;
                    }
                    else
                    {
                        result[i, i] = dot + lambdaTimesNui;
                    }
                }
            }
            return result;
        }

        public static double[,] CreateMiIi(IList<double[]> featureVectors, int numFeatures)
        {
            double[,] MiIi = new double[numFeatures, featureVectors.Count];
            int n = 0;
            foreach (var featureVector in featureVectors)
            {
                for (int m = 0; m < numFeatures; m++)
                {
                    MiIi[m, n] = featureVector[m];
                }
                n++;
            }
            return MiIi;
        }

        public static double[,] CreateRiIiMaybeTransposed(double[] ratingVector)
        {
            //Preconditions.checkArgument(ratingVector.isSequentialAccess(), "Ratings should be iterable in Index or Sequential Order");

            double[,] RiIiMaybeTransposed = new double[ratingVector.Length, 1];  //getNumNondefaultElements()
            int index = 0;
            foreach (var elem in MatrixUtil.NonZeroes(ratingVector))
            {
                RiIiMaybeTransposed[index++, 0] = elem;
            }
            return RiIiMaybeTransposed;
        }
    }
}