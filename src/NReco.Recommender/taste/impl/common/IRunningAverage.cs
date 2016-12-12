namespace NReco.CF.Taste.Impl.Common
{
    /// <summary>
    /// Interface for classes that can keep track of a running average of a series of numbers. One can add to or
    /// remove from the series, as well as update a datum in the series. The class does not actually keep track of
    /// the series of values, just its running average, so it doesn't even matter if you remove/change a value that
    /// wasn't added.
    /// </summary>
    public interface IRunningAverage
    {
        /// @param datum
        ///          new item to add to the running average
        /// @throws IllegalArgumentException
        ///           if datum is {@link Double#NaN}
        void AddDatum(double datum);

        /// @param datum
        ///          item to remove to the running average
        /// @throws IllegalArgumentException
        ///           if datum is {@link Double#NaN}
        /// @throws InvalidOperationException
        ///           if count is 0
        void RemoveDatum(double datum);

        /// @param delta
        ///          amount by which to change a datum in the running average
        /// @throws IllegalArgumentException
        ///           if delta is {@link Double#NaN}
        /// @throws InvalidOperationException
        ///           if count is 0
        void ChangeDatum(double delta);

        int GetCount();

        double GetAverage();

        /// @return a (possibly immutable) object whose average is the negative of this object's
        IRunningAverage Inverse();
    }
}