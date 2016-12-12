using System.Collections.Generic;

namespace NReco.CF.Taste.Common
{
    /// <summary>
    /// Implementations of this interface have state that can be periodically refreshed. For example, an
    /// implementation instance might contain some pre-computed information that should be periodically refreshed.
    /// The <see cref="IRefreshable.Refresh"/> method triggers such a refresh.
    /// <para>
    /// All Taste components implement this. In particular, <see cref="NReco.CF.Taste.Recommender.IRecommender"/>s do. Callers may want to call
    /// <see cref="IRefreshable.Refresh"/> periodically to re-compute information throughout the system and bring it up
    /// to date, though this operation may be expensive.
    /// </para>
    /// </summary>
    public interface IRefreshable
    {
        /// <summery>
        /// Triggers "refresh" -- whatever that means -- of the implementation. The general contract is that any
        /// {@link Refreshable} should always leave itself in a consistent, operational state, and that the refresh
        /// atomically updates internal state from old to new.
        /// </summery>
        /// <param name="alreadyRefreshed">
        /// <see cref="NReco.CF.Taste.Common.IRefreshable"/>s that are known to have already been
        /// refreshed as a result of an initial call to a <see cref="NReco.CF.Taste.Common.IRefreshable.Refresh"/> method on some
        /// object. This ensure that objects in a refresh dependency graph aren't refreshed twice
        /// needlessly.
        /// </param>
        void Refresh(IList<IRefreshable> alreadyRefreshed);
    }
}