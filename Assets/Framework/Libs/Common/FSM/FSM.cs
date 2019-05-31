using Framework;
using System;
using System.Collections.Generic;


#region 示例代码
//public class TestFSM
//{
//    public enum ET
//    {
//        A,
//        B,
//        C,
//    }

//    FiniteStateMachine<ET> fsm = new FiniteStateMachine<ET>();
    
//    public TestFSM()
//    {
//        fsm.RegistState(ET.A, OnEnter, OnExit, OnUpdate, SwitchEnable);
//        fsm.RegistState(ET.B, OnEnter, OnExit, OnUpdate, SwitchEnable);
//        fsm.SwitchState(ET.A);
//        fsm.Update();
//        fsm.SwitchState(ET.B);
//        fsm.Update();
//        fsm.SwitchState(ET.C);
//        fsm.Update();
//    }

//    private void OnEnter(ET arg1, object arg2)
//    {
//        throw new NotImplementedException();
//    }

//    private void OnUpdate(ET obj)
//    {
       
//    }

   
//    public void OnExit(ET to)
//    {

//    }

   

//    public bool SwitchEnable(ET toState)
//    {
//        return true;
//    }
//}
#endregion

namespace Framework
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class FSM<T>
    {
        /// <summary>
        /// 状态控制器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class StateController<T>
        {
            /// <summary>
            /// 进入状态的委托
            /// </summary>
            /// <param name="fromState"></param>
            public Action<T, object> onEnter;

            /// <summary>
            /// 退出状态的委托
            /// </summary>
            /// <param name="toState"></param>
            public Action<T> onExit;

            /// <summary>
            /// 更新状态的委托
            /// <param name="curState"></param>
            /// </summary>
            public Action<T> onUpdate;

            /// <summary>
            /// 切换状态检查的委托
            /// </summary>
            /// <param name="toState"></param>
            /// <returns></returns>
            public Func<T, bool> checkSwitchEnable;

            /// <summary>
            /// 状态
            /// </summary>
            public T state;

            /// <summary>
            /// 配置的能切换到的状态，null表示不限制
            /// </summary>
            public HashSet<T> canTransitionState = null;

            public StateController(T state)
            {
                this.state = state;
            }
        }

        /// <summary>
        /// 在当前状态下经过的时间(根据Update传入的dt值累计）
        /// </summary>
        public float StateStayTime { get; private set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public T CurState { get; private set; }

        /// <summary>
        /// 状态字典
        /// </summary>
        Dictionary<T, StateController<T>> _stateDic = new Dictionary<T, StateController<T>>();

        /// <summary>
        /// 状态机名称
        /// </summary>
        public string Name { get; private set; }

        public FSM(string name = null)
        {
            this.Name = name;
        }

        /// <summary>
        /// 注册一个状态，不适用的方法可以传递Null
        /// </summary>
        public void RegistState(T state, Action<T, object> onEnter = null, Action<T> onExit = null, Action<T> onUpdate = null, Func<T, bool> checkSwitchEnable = null)
        {
            StateController<T> sc = new StateController<T>(state);
            sc.onEnter = onEnter;
            sc.onExit = onExit;
            sc.onUpdate = onUpdate;
            sc.checkSwitchEnable = checkSwitchEnable;

            if (null == CurState)
            {
                //设置为第一个状态
                CurState = state;
            }

            _stateDic[state] = sc;
        }

        /// <summary>
        /// 注销一个状态
        /// </summary>
        public void UnregistState(T state)
        {
            if (_stateDic.ContainsKey(state))
            {
                _stateDic.Remove(state);
            }

            if (CurState.Equals(state))
            {
                CurState = default(T);
            }
        }

        /// <summary>
        /// 添加一个合法的状态转换
        /// </summary>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        public void AddTransitionState(T fromState, T toState)
        {
            if (false == _stateDic.ContainsKey(fromState))
            {
                return;
            }


            if (null == _stateDic[fromState].canTransitionState)
            {
                _stateDic[fromState].canTransitionState = new HashSet<T>();
            }

            _stateDic[fromState].canTransitionState.Add(toState);
        }

        /// <summary>
        /// 移除一个合法的状态转换
        /// </summary>
        /// <param name="fromState"></param>
        public void RemoveTransitionState(T fromState, T toState)
        {
            if (false == _stateDic.ContainsKey(fromState) || null == _stateDic[fromState].canTransitionState)
            {
                return;
            }

            _stateDic[fromState].canTransitionState.Remove(toState);
        }

        /// <summary>
        /// 切换一个状态
        /// </summary>
        public bool SwitchState(T toState, object data = null)
        {
            if (false == _stateDic.ContainsKey(toState))
            {
                return false;
            }

            var oldSC = _stateDic[CurState];
            //变换条件等于null时可以任意转换
            if (oldSC.canTransitionState != null && !oldSC.canTransitionState.Contains(toState))
            {
                return false;
            }

            var newSC = _stateDic[toState];

            if (oldSC.checkSwitchEnable !=null && !oldSC.checkSwitchEnable.Invoke(toState))
            {
                return false;
            }

            if (oldSC.onExit!=null)
            {
                oldSC.onExit.Invoke(toState);
            }
            CurState = toState;
            StateStayTime = 0;
            if (null != newSC.onEnter)
            {
                newSC.onEnter.Invoke(oldSC.state, data);
            }
            return true;
        }

        /// <summary>
        /// 状态更新
        /// </summary>
        /// <param name="dt">距离上次状态更新的间隔，如果传入，可以统计状态持续的时间</param>
        public void Update(float dt = 0f)
        {
            StateStayTime += dt;
            var nowSC = _stateDic[CurState];
            if (null != nowSC.onUpdate)
            {
                nowSC.onUpdate.Invoke(CurState);
            }
        }
    }
}
