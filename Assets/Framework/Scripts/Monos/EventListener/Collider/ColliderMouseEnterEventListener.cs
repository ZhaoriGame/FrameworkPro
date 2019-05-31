using System;
using Framework;

namespace Framework
{
    public class ColliderMouseEnterEventListener : AEventListener<ColliderMouseEnterEventListener>
    {
        public event Action onEvent;

        private void OnMouseEnter()
        {
            if (null != onEvent)
            {
                onEvent.Invoke();
            }
        }
    }
}