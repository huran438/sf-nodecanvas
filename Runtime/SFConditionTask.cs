using NodeCanvas.Framework;
using SFramework.Core.Runtime;

namespace SFramework.NodeCanvas.Runtime
{
    public abstract partial class SFConditionTask<T> : ConditionTask<T>, ISFInjectable, ISFInjectionCallback where T : class
    {
        protected override string OnInit()
        {
            this.Inject();
            return base.OnInit();
        }

        void ISFInjectionCallback.OnDependenciesInjected() => Init();

        protected virtual void Init()
        {
        }
    }

    public abstract partial class SFConditionTask : ConditionTask, ISFInjectable, ISFInjectionCallback
    {
        protected override string OnInit()
        {
            this.Inject();
            return base.OnInit();
        }

        void ISFInjectionCallback.OnDependenciesInjected() => Init();

        protected virtual void Init()
        {
            
        }
    }
}