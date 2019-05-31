namespace IL.Framework
{
    /// <summary>
    /// 单例模式基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T:new()
    {
        private static object _singletonLock = new object(); //锁同步

        private static T _ins;

        /// <summary>
        /// 单例
        /// </summary>
        public static T Ins
        {
            get {
                lock (_singletonLock)
                {
                    if (null == _ins)
                    {
                        _ins = new T();
                    }
                }
                return _ins;
            }
        }

        protected Singleton()
        {

        }
    }
}
