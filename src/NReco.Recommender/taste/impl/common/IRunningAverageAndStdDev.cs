
namespace NReco.CF.Taste.Impl.Common
{
    /// <p>
    /// Extends {@link RunningAverage} by adding standard deviation too.
    /// </p>
    public interface IRunningAverageAndStdDev : IRunningAverage
    {
        /// @return standard deviation of data 
        double GetStandardDeviation();

        /// @return a (possibly immutable) object whose average is the negative of this object's
        new IRunningAverageAndStdDev Inverse();
    }
}