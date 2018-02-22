using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TaskTester.Tasking
{
    public abstract class BackgroundTask
    {
        protected object mLock = new object();
        protected Dispatcher mEventDispatcher;
        protected CancellationToken mCancellationToken;
        bool mStarted = false;

        public event EventHandler Finished;

        public Task ExecutingTask { get; private set; }

        protected void Start(Action action)
        {
            MarkAsStarted();
            ExecutingTask = Task.Run(action).ContinueWith((x) => Notify(Finished, EventArgs.Empty));
        }

        protected BackgroundTask(Dispatcher eventDispatcher, CancellationToken cancellationToken)
        {
            this.mEventDispatcher = eventDispatcher;
            this.mCancellationToken = cancellationToken;
        }

        protected void NotifyFinished() => Notify(Finished, EventArgs.Empty);

        protected void Notify(EventHandler eventReference, EventArgs e)
        {
            if (eventReference != null)
            {
                if (mEventDispatcher != null)
                {
                    mEventDispatcher.Invoke(eventReference, args: new object[] { this, e });
                }
                else
                {
                    eventReference.Invoke(this, e);
                }
            }
        }

        protected void Notify<TEventArgs>(EventHandler<TEventArgs> genericEventReference, TEventArgs e)
        {
            if (genericEventReference != null)
            {
                if (mEventDispatcher != null)
                {
                    mEventDispatcher.Invoke(genericEventReference, args: new object[] { this, e });
                }
                else
                {
                    genericEventReference.Invoke(this, e);
                }
            }
        }

        public abstract void Start();


        private void MarkAsStarted()
        {
            bool success = true;
            if (mStarted) success = false;
            else
            {
                lock (mLock)
                {
                    if (mStarted) success = false;
                    else mStarted = true;
                }
            }
            if (!success)
            {
                throw new InvalidOperationException("Task already started.");
            }
        }
    }
}
