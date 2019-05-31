﻿using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 客户端更新基类
    /// </summary>
    public abstract class AClientUpdate
    {
        public static AClientUpdate CreateNowPlatformUpdate()
        {            
            AClientUpdate update;

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    update = new AndroidUpdate();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    update = new IOSUpdate();
                    break;
                default:
                    update = new PCUpdate();
                    break;
            }
            return update;
        }

        Action<bool> _onOver;
        protected Action<float, long> _onProgress;
        protected Action<string> _onError;

        /// <summary>
        /// 开始检查更新
        /// </summary>
        /// <param name="onOver">不用更新的回调</param>
        public void Start(Action<bool> onOver, Action<float, long> onProgress, Action<string> onError)
        {
            Log.CI(Log.COLOR_BLUE, "「{0}」客户端版本号检查...", this.GetType().Name);
            if (!Runtime.Ins.IsLoadAssetsFromNet)
            {
                onOver(false);
                return;
            }

            _onOver = onOver;
            _onProgress = onProgress;
            _onError = onError;
            int result = CheckVersionCode(Application.version, Runtime.Ins.setting.client.version);

            Log.CI(Log.COLOR_BLUE, "客户端版本号 本地: {0}   网络: {1}  更新地址：{2}", Application.version, Runtime.Ins.setting.client.version, Runtime.Ins.setting.client.url);

            if (result == -1)
            {
                switch(Runtime.Ins.setting.client.type)
                {
                    case 0:
                        OnNeedUpdate();
                        break;
                    case 1:
                        Application.OpenURL(Runtime.Ins.setting.client.url);
                        break;
                }                
            }
            else
            {
                _onOver.Invoke(result == 1 ? true : false);
            }            
        }



        /// <summary>
        /// 检查版本编码，如果本地号大于网络，则返回1，等于返回0，小于返回-1
        /// </summary>
        /// <param name="local"></param>
        /// <param name="net"></param>
        /// <returns></returns>
        protected int CheckVersionCode(string local, string net)
        {
            string[] locals = local.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            string[] nets = net.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (locals.Length != nets.Length)
            {
                return -1;
            }

            for (int i = 0; i < locals.Length; i++)
            {
                int lc = int.Parse(locals[i]);
                int nc = int.Parse(nets[i]);

                if (lc > nc)
                {
                    return 1;
                }
                else if (lc < nc)
                {
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// 开始更新
        /// </summary>
        /// <param name="state"></param>
        public abstract void OnNeedUpdate();
    }
}