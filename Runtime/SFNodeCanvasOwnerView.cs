using NodeCanvas.BehaviourTrees;
using NodeCanvas.StateMachines;
using SFramework.Core.Runtime;
using UnityEngine;

namespace SFramework.NodeCanvas.Runtime
{
    [DisallowMultipleComponent]
    public class SFNodeCanvasOwnerView : SFView
    {
        protected override void Init()
        {
            foreach (var fsmOwner in GetComponents<FSMOwner>())
            {
                fsmOwner.StartBehaviour();
            }
            
            foreach (var behaviourTreeOwner in GetComponents<BehaviourTreeOwner>())
            {
                behaviourTreeOwner.StartBehaviour();
            }
        }

        private void OnDestroy()
        {
            foreach (var fsmOwner in GetComponents<FSMOwner>())
            {
                fsmOwner.StopBehaviour();
            }
            
            foreach (var behaviourTreeOwner in GetComponents<BehaviourTreeOwner>())
            {
                behaviourTreeOwner.StopBehaviour();
            }
        }
    }
}