using System;

namespace Framework
{

    public class ColliderMouseUpAsButtonEventListener : AEventListener<ColliderMouseUpAsButtonEventListener>
    {
        public event Action onEvent;

        private void OnMouseUpAsButton()
        {
            if (null != onEvent)
            {
                onEvent.Invoke();
            }
        }
    }
}
