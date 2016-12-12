namespace NReco.CF.Taste.Recommender
{
    /// <summary>
    /// Implementations encapsulate items that are recommended, and include the item recommended and a value
    /// expressing the strength of the preference.
    /// </summary>
    public interface IRecommendedItem
    {
        /// <summary>
        /// Item ID
        /// </summary>
        /// <returns>the recommended item ID </returns>
        long GetItemID();

        /// <summary>
        /// A value expressing the strength of the preference for the recommended item. The range of the values
        /// depends on the implementation. Implementations must use larger values to express stronger preference.
        /// </summary>
        /// <returns>strength of the preference</returns>
        float GetValue();
    }
}