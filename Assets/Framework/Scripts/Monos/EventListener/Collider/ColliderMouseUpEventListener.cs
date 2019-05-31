using System;

namespace Framework
{

    public class ColliderMouseUpEventListener : AEventListener<ColliderMouseUpEventListener>
    {
        public event Action onEvent;

        private void OnMouseUp()
        {
            if (null != onEvent)
            {
                onEvent.Invoke();
            }
        }

    }
}
