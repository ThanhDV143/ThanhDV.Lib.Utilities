using System;

namespace ThanhDV.Utilities
{
    public interface IReleaseEvent
    {
        event Action Dispatched;
    }
}
