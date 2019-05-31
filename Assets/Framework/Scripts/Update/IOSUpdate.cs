using UnityEngine;

namespace Framework
{
    public class IOSUpdate : AClientUpdate
    {
        public override void OnNeedUpdate()
        {
            //打开IOS的APP STORE页面
            Application.OpenURL(Runtime.Ins.setting.client.url);
        }
    }
}
