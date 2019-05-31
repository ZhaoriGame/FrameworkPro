using System;

namespace Framework
{
    public class ColliderMouseOverEventListener : AEventListener<ColliderMouseOverEventListener>
    {

        public event Action onEvent;
        private void OnMouseOver()
        {
            if (null != onEvent)
            {
                onEvent.Invoke();
            }
        }
    }
}