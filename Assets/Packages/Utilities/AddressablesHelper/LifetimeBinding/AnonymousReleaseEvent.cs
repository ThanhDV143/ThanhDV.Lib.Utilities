using System;

namespace ThanhDV.Utilities
{
    public sealed class AnonymousReleaseEvent : IReleaseEvent
    {
        event Action IReleaseEvent.Dispatched
        {
            add => ReleasedInternal += value;
            remove => ReleasedInternal -= value;
        }

        private event Action ReleasedInternal;

        public void Release()
        {
            ReleasedInternal?.Invoke();
        }
    }
}
