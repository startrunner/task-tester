using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskTester.CheckerCore.Tasking
{
    public abstract class TaskTesterJob
    {
        protected object mLock = new object();
        //protected Dispatcher mEventDispatcher;
        private Action<Delegate, object[]> mEventInvokeAction;
        protected CancellationToken mCancellationToken;
        bool mStarted = false;

        public event EventHandler Finished;

        public Task ExecutingTask { get; private set; }

        public abstract void Start();
        protected void Start(Action action)
        {
            MarkAsStarted();
            ExecutingTask = Task.Run(action).ContinueWith((x) => Notify(Finished, EventArgs.Empty));
        }

        protected TaskTesterJob(Action<Delegate, object[]> eventInvokeAction, CancellationToken cancellationToken)
        {
            mEventInvokeAction = eventInvokeAction;
            mCancellationToken = cancellationToken;
        }

        protected void NotifyFinished() => Notify(Finished, EventArgs.Empty);

        protected void Notify(EventHandler eventReference, EventArgs e)
        {
            if (eventReference != null)
            {
                if (mEventInvokeAction != null)
                {
                    mEventInvokeAction.Invoke(eventReference, new object[] { this, e });
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
                if (mEventInvokeAction != null)
                {
                    mEventInvokeAction.Invoke(genericEventReference, new object[] { this, e });
                }
                else
                {
                    genericEventReference.Invoke(this, e);
                }
            }
        }

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
