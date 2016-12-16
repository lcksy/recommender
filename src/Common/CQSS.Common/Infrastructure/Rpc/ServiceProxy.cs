using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CQSS.Common.Infrastructure.Rpc
{
    public class ServiceProxy<TChannel> : ClientBase<TChannel>, IDisposable
        where TChannel : class
    {
        #region Props

        public TChannel Client
        {
            get
            {
                return base.Channel;
            }
        }

        #endregion

        #region Ctors

        public ServiceProxy()
        {
        }

        public ServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        public ServiceProxy(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        public ServiceProxy(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        public ServiceProxy(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceProxy(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        #endregion

        #region Methods

        public new void Close()
        {
            ((IDisposable)this).Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (base.State != CommunicationState.Closed)
                    base.Close();
            }
            catch (CommunicationException)
            {
                base.Abort();
            }
            catch (TimeoutException)
            {
                base.Abort();
            }
            catch
            {
                base.Abort();
                throw;
            };
        }

        #endregion

        #region IDisposable members

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}