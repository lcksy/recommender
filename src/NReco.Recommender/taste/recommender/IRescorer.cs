
namespace NReco.CF.Taste.Recommender
{
    /// <summary>
    /// A IRescorer simply assigns a new "score" to a thing like an ID of an item or user which a
    /// {@link Recommender} is considering returning as a top recommendation. It may be used to arbitrarily re-rank
    /// the results according to application-specific logic before returning recommendations. For example, an
    /// application may want to boost the score of items in a certain category just for one request.
    /// <para>
    /// A <see cref="IRescorer"/> can also exclude a thing from consideration entirely by returning <code>true</code> from
    /// <see cref="IRescorer.isFiltered"/>.
    /// </para>
    /// </summary>
    public interface IRescorer<T>
    {
        /// <summary>
        /// Calculate new score for given thing and its original score
        /// </summary>
        /// <param name="thing">thing to rescore</param>
        /// <param name="originalScore">original score</param>
        /// <returns>modified score, or {@link Double#NaN} to indicate that this should be excluded entirely</returns>
        double Rescore(T thing, double originalScore);


        /// <summary>
        /// Returns <code>true</code> to exclude the given thing.
        /// </summary>
        /// <param name="thing">the thing to filter</param>
        /// <returns><code>true</code> to exclude, <code>false</code> otherwise</returns>
        bool IsFiltered(T thing);
    }
}