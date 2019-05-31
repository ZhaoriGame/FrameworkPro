using System;

namespace Framework
{
    public class ColliderMouseDownEventListener : AEventListener<ColliderMouseDownEventListener>
    {
        public event Action onEvent;

        private void OnMouseDown()
        {
            if (null != onEvent)
            {
                onEvent.Invoke();
            }
        }
    }
}