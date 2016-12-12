namespace NReco.CF.Taste.Recommender
{
    /// <p>
    /// A {@link Rescorer} which operates on {@code long} primitive IDs, rather than arbitrary {@link Object}s.
    /// This is provided since most uses of this interface in the framework take IDs (as {@code long}) as an
    /// argument, and so this can be used to avoid unnecessary boxing/unboxing.
    /// </p>
    public interface IDRescorer
    {
        /// @param id
        ///          ID of thing (user, item, etc.) to rescore
        /// @param originalScore
        ///          original score
        /// @return modified score, or {@link Double#NaN} to indicate that this should be excluded entirely
        double Rescore(long id, double originalScore);

        /// Returns {@code true} to exclude the given thing.
        ///
        /// @param id
        ///          ID of thing (user, item, etc.) to rescore
        /// @return {@code true} to exclude, {@code false} otherwise
        bool IsFiltered(long id);
    }
}