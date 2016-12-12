using NReco.CF.Taste.Model;

namespace NReco.CF.Taste.Impl.Recommender
{
    /// <summary>
    /// Simple class which encapsulates restricting a preference value
    /// to a predefined range. The simple logic is wrapped up here for
    /// performance reasons.
    /// </summary>
    public sealed class EstimatedPreferenceCapper
    {
        private float min;
        private float max;

        public EstimatedPreferenceCapper(IDataModel model)
        {
            min = model.GetMinPreference();
            max = model.GetMaxPreference();
        }

        public float CapEstimate(float estimate)
        {
            if (estimate > max)
            {
                estimate = max;
            }
            else if (estimate < min)
            {
                estimate = min;
            }
            return estimate;
        }
    }
}