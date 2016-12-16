using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CQSS.Common.Infrastructure.EventPattern
{
    public class EventDispatcher<T>
    {
        private readonly IDictionary<int, List<IEventHandler<T>>> _eventHandlerBySequence = new Dictionary<int, List<IEventHandler<T>>>();
        private BlockingCollection<T> _events = new BlockingCollection<T>();
        private Atomic.Volatile.Boolean _running = new Atomic.Volatile.Boolean(false);
        private CancellationTokenSource _cancellationToken = null;
        private Task _worker;
        private EventDispatchMode _dispatchMode = EventDispatchMode.Parallelling;
        private bool _breakWhenAnyExceptionOccur = false;
        private int _timeoutMillseconds = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatchMode">指定 EventHandler 的执行模式，Parallelling = 并行，Sequencing = 串行</param>
        /// <param name="breakWhenAnyExceptionOccur">当有异常发生时，是否中断整个 EventHandler 链</param>
        /// <param name="timeoutMillseconds">指定 EventHandler 执行的超时时间，仅当 DispatchMode = Parallelling 时有效</param>
        public EventDispatcher(EventDispatchMode dispatchMode = EventDispatchMode.Parallelling, bool breakWhenAnyExceptionOccur = false, int timeoutMillseconds = 10000)
        {
            _dispatchMode = dispatchMode;
            _breakWhenAnyExceptionOccur = breakWhenAnyExceptionOccur;
            _timeoutMillseconds = timeoutMillseconds;
        }

        public EventDispatcher<T> HandleEventsWith(params IEventHandler<T>[] eventHandlers)
        {
            if (!_running.ReadFullFence())
            {
                var sequence = this.GetMaxSequence();
                _eventHandlerBySequence[sequence].AddRange(eventHandlers);
            }

            return this;
        }

        public EventDispatcher<T> And(params IEventHandler<T>[] eventHandlers)
        {
            if (!_running.ReadFullFence())
            {
                var sequence = this.GetMaxSequence();
                _eventHandlerBySequence[sequence].AddRange(eventHandlers);
            }

            return this;
        }

        public EventDispatcher<T> Then(params IEventHandler<T>[] eventHandlers)
        {
            if (!_running.ReadFullFence())
            {
                var sequence = this.GetMaxSequence() + 1;
                _eventHandlerBySequence.Add(sequence, new List<IEventHandler<T>>());
                _eventHandlerBySequence[sequence].AddRange(eventHandlers);
            }

            return this;
        }

        public EventDispatcher<T> Start()
        {
            if (!_running.AtomicCompareExchange(true, false))
                return this;

            _worker = Task.Factory.StartNew(() =>
            {
                try
                {
                    _cancellationToken = new CancellationTokenSource();

                    foreach (var e in _events.GetConsumingEnumerable(_cancellationToken.Token))
                    {
                        if (_cancellationToken.IsCancellationRequested)
                            return;

                        if (_dispatchMode == EventDispatchMode.Parallelling)
                            this.ParallellingDispatch(e);
                        else
                            this.SequencingDispatch(e);
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }, TaskCreationOptions.LongRunning);

            return this;
        }

        public void Shutdown()
        {
            if (_running.ReadFullFence())
            {
                _running.WriteFullFence(false);

                SpinWait spinWait = new SpinWait();
                while (_cancellationToken == null)
                    spinWait.SpinOnce();

                _cancellationToken.Cancel();

                if (_worker != null)
                    _worker.Wait();
            }
        }

        public void Dispatch(T e)
        {
            _events.TryAdd(e);
        }

        private int GetMaxSequence()
        {
            if (!_eventHandlerBySequence.Keys.Any())
                _eventHandlerBySequence.Add(1, new List<IEventHandler<T>>());

            return _eventHandlerBySequence.Keys.Max();
        }

        private void ParallellingDispatch(T e)
        {
            var sequences = _eventHandlerBySequence.Keys.OrderBy(t => t).ToList();
            foreach (var sequence in sequences)
            {
                var eventHandlers = _eventHandlerBySequence[sequence];
                var subWorkers = new List<Task>(eventHandlers.Count);
                foreach (var eventHandler in eventHandlers)
                {
                    var subWorker = Task.Factory.StartNew(() => eventHandler.Handle(e));
                    subWorkers.Add(subWorker);
                }

                try
                {
                    if (!Task.WaitAll(subWorkers.ToArray(), _timeoutMillseconds))
                        throw new Exception(string.Format("Subworker timeout after {0} millseconds", _timeoutMillseconds));
                }
                catch
                {
                    if (_breakWhenAnyExceptionOccur)
                        break;
                }
            }
        }

        private void SequencingDispatch(T e)
        {
            var sequences = _eventHandlerBySequence.Keys.OrderBy(t => t).ToList();
            foreach (var sequence in sequences)
            {
                var eventHandlers = _eventHandlerBySequence[sequence];
                foreach (var eventHandler in eventHandlers)
                {
                    try
                    {
                        eventHandler.Handle(e);
                    }
                    catch
                    {
                        if (_breakWhenAnyExceptionOccur)
                            return;
                    }
                }
            }
        }
    }
}