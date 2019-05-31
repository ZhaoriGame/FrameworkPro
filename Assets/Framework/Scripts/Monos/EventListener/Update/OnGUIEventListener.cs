﻿using System;

namespace Framework
{
    public class OnGUIEventListener : AEventListener<OnGUIEventListener>
    {
        public event Action onGUI;

        private void OnGUI()
        {
            if (null != onGUI)
            {
                onGUI.Invoke();
            }
        }
    }
}