using NodeCanvas.Framework;
using SFramework.Core.Runtime;

namespace SFramework.NodeCanvas.Runtime
{
    public abstract class SFActionTask<T> : ActionTask<T>, ISFInjectable where T : class
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
    
    public abstract class SFActionTask : ActionTask, ISFInjectable
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