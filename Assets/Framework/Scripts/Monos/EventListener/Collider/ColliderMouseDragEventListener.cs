using System;

namespace Framework
{
    public class ColliderMouseDragEventListener : AEventListener<ColliderMouseDragEventListener>
    {
        public event Action onEvent;

        private void OnMouseDrag()
        {
            if(null != onEvent)
            {
                onEvent.Invoke();
            }            
        }
    }
}