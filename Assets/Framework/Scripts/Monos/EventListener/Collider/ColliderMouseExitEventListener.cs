using System;

namespace Framework
{
    public class ColliderMouseExitEventListener : AEventListener<ColliderMouseExitEventListener>
    {
        public event Action onEvent;

        private void OnMouseExit()
        {
            if (null != onEvent)
            {
                onEvent.Invoke();
            }
        }
    }
}