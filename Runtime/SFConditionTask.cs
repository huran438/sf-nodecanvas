using NodeCanvas.Framework;
using SFramework.Core.Runtime;

namespace SFramework.NodeCanvas.Runtime
{
    public abstract class SFConditionTask<T> : ConditionTask<T>, ISFInjectable where T : class
    {
        protected override string OnInit()
        {
            SFContextRoot.Container.Inject(this);
            return base.OnInit();
        }

        [SFInject]
        protected virtual void Init(ISFContainer container)
        {
        }
    }

    public abstract class SFConditionTask : ConditionTask, ISFInjectable
    {
        protected override string OnInit()
        {
            SFContextRoot.Container.Inject(this);
            return base.OnInit();
        }

        [SFInject]
        protected virtual void Init(ISFContainer container)
        {
            
        }
    }
}