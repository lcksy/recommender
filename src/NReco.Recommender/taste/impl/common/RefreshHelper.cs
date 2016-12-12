using System;
using System.Collections.Generic;
using System.Threading;

using NReco.CF.Taste.Common;

namespace NReco.CF.Taste.Impl.Common
{
    /// <summary>
    /// A helper class for implementing <see cref="NReco.CF.Taste.Common.IRefreshable"/>. This object is typically included in an implementation
    /// <see cref="NReco.CF.Taste.Common.IRefreshable"/> to implement <see cref="NReco.CF.Taste.Common.IRefreshable.Refresh"/>.
    /// It execute the class's own supplied update logic, after updating all the object's dependencies. This also ensures that dependencies
    /// are not updated multiple times.
    /// </summary>
    public sealed class RefreshHelper : IRefreshable
    {
        private static Logger log = LoggerFactory.GetLogger(typeof(RefreshHelper));

        private List<IRefreshable> dependencies;
        private Action refreshRunnable;

        /// @param refreshRunnable
        ///          encapsulates the containing object's own refresh logic
        public RefreshHelper(Action refreshRunnable)
        {
            this.dependencies = new List<IRefreshable>(3);
            this.refreshRunnable = refreshRunnable;
        }

        /// <summary>Add a dependency to be refreshed first when the encapsulating object does.</summary>
        public void AddDependency(IRefreshable refreshable)
        {
            if (refreshable != null)
            {
                dependencies.Add(refreshable);
            }
        }

        public void RemoveDependency(IRefreshable refreshable)
        {
            if (refreshable != null)
            {
                dependencies.Remove(refreshable);
            }
        }

        /// <summary>
        /// Typically this is called in {@link Refreshable#refresh(java.util.Collection)} and is the entire body of  that method.
        /// </summary>
        public void Refresh(IList<IRefreshable> alreadyRefreshed)
        {
            if (Monitor.TryEnter(this))
            {
                try
                {
                    alreadyRefreshed = BuildRefreshed(alreadyRefreshed);
                    foreach (IRefreshable dependency in dependencies)
                    {
                        MaybeRefresh(alreadyRefreshed, dependency);
                    }
                    if (refreshRunnable != null)
                    {
                        try
                        {
                            refreshRunnable();
                        }
                        catch (Exception e)
                        {
                            log.Warn("Unexpected exception while refreshing", e);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        /// Creates a new and empty {@link Collection} if the method parameter is {@code null}.
        ///
        /// @param currentAlreadyRefreshed
        ///          {@link Refreshable}s to refresh later on
        /// @return an empty {@link Collection} if the method param was {@code null} or the unmodified method
        ///         param.
        public static IList<IRefreshable> BuildRefreshed(IList<IRefreshable> currentAlreadyRefreshed)
        {
            return currentAlreadyRefreshed == null ? new List<IRefreshable>(3) : currentAlreadyRefreshed;
        }

        /// <summary>
        /// Adds the specified {@link Refreshable} to the given collection of {@link Refreshable}s if it is not
        /// already there and immediately refreshes it.
        /// </summary>
        /// <param name="alreadyRefreshed">the collection of <see cref="IRefreshable"/>s</param>
        /// <param name="refreshable">the <see cref="IRefreshable"/> to potentially add and refresh</param>     
        public static void MaybeRefresh(IList<IRefreshable> alreadyRefreshed, IRefreshable refreshable)
        {
            if (!alreadyRefreshed.Contains(refreshable))
            {
                alreadyRefreshed.Add(refreshable);
                log.Info("Added refreshable: {}", refreshable);
                refreshable.Refresh(alreadyRefreshed);
                log.Info("Refreshed: {}", alreadyRefreshed);
            }
        }
    }
}