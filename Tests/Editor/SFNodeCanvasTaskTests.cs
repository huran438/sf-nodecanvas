using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NodeCanvas.Framework;
using NUnit.Framework;
using SFramework.Core.Runtime;
using SFramework.NodeCanvas.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SFramework.NodeCanvas.Editor.Tests
{
    public sealed partial class SFNodeCanvasTaskTests
    {
        private readonly List<GameObject> _objects = new();
        private SFServiceContainer _previousContainer;
        private SFServiceContainer _testContainer;
        private Dependency _dependency;
        private TaskSystem _owner;

        [SetUp]
        public void SetUp()
        {
            _testContainer = new SFServiceContainer();
            _previousContainer = SFContainer.SetCurrent(_testContainer);
            _dependency = new Dependency();
            SFContainer.Register<Dependency>(_dependency);
            _owner = new TaskSystem { agent = CreateAgent() };
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var gameObject in _objects)
                if (gameObject != null) Object.DestroyImmediate(gameObject);
            _objects.Clear();
            SFContainer.SetCurrent(_previousContainer);
            _testContainer.Dispose();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Action_InjectsDependencyAndCallsInitHookBeforeExecution(bool success)
        {
            _dependency.Result = success;
            var action = new ActionProbe();
            action.SetOwnerSystem(_owner);

            var status = action.Execute(_owner.agent, null);

            Assert.That(status, Is.EqualTo(success ? Status.Success : Status.Failure));
            CollectionAssert.AreEqual(new[] { "init", "execute" }, _dependency.Events);
            Assert.That(action.InitCount, Is.EqualTo(1));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GenericAction_ResolvesTypedAgentAndInjectsBeforeExecution(bool success)
        {
            _dependency.Result = success;
            var action = new GenericActionProbe();
            action.SetOwnerSystem(_owner);

            var status = action.Execute(_owner.agent, null);

            Assert.That(status, Is.EqualTo(success ? Status.Success : Status.Failure));
            Assert.That(action.AgentSeenByExecute, Is.SameAs(_owner.agent));
            CollectionAssert.AreEqual(new[] { "generic-init", "generic-execute" }, _dependency.Events);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Condition_InjectsBeforeEnableAndUsesDependencyWhenChecking(bool result)
        {
            _dependency.Result = result;
            var condition = new ConditionProbe();
            condition.SetOwnerSystem(_owner);

            Assert.That(condition.CheckOnce(_owner.agent, null), Is.EqualTo(result));

            CollectionAssert.AreEqual(new[] { "init", "enable", "check", "disable" }, _dependency.Events);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GenericCondition_ResolvesTypedAgentAndUsesInjectedDependency(bool result)
        {
            _dependency.Result = result;
            var condition = new GenericConditionProbe();
            condition.SetOwnerSystem(_owner);

            Assert.That(condition.CheckOnce(_owner.agent, null), Is.EqualTo(result));

            Assert.That(condition.AgentSeenByCheck, Is.SameAs(_owner.agent));
            CollectionAssert.AreEqual(new[] { "generic-init", "generic-check" }, _dependency.Events);
        }

        [Test]
        public void Action_SameAgentKeepsInjectionButNewAgentReinitializesAgainstCurrentContainer()
        {
            var action = new ActionProbe();
            action.SetOwnerSystem(_owner);
            action.Execute(_owner.agent, null);
            var replacement = new Dependency { Result = true };
            SFContainer.Register<Dependency>(replacement);

            Assert.That(action.Execute(_owner.agent, null), Is.EqualTo(Status.Failure));
            Assert.That(replacement.Events, Is.Empty, "NodeCanvas caches initialization for the same agent.");
            var otherAgent = CreateAgent();
            Assert.That(action.Execute(otherAgent, null), Is.EqualTo(Status.Success));

            Assert.That(action.InitCount, Is.EqualTo(2));
            CollectionAssert.AreEqual(new[] { "init", "execute", "execute" }, _dependency.Events);
            CollectionAssert.AreEqual(new[] { "init", "execute" }, replacement.Events);
        }

        [Test]
        public void DisabledAction_DoesNotInitializeOrResolveInjectedDependencies()
        {
            var action = new ActionProbe();
            typeof(global::NodeCanvas.Framework.Task).GetField("_isUserDisabled", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(action, true);
            action.SetOwnerSystem(_owner);

            Assert.That(action.Execute(_owner.agent, null), Is.EqualTo(Status.Optional));

            Assert.That(action.InitCount, Is.Zero);
            Assert.That(_dependency.Events, Is.Empty);
        }

        private Transform CreateAgent()
        {
            var gameObject = new GameObject("NodeCanvas task agent");
            _objects.Add(gameObject);
            return gameObject.transform;
        }

        public sealed class Dependency : ISFDependency
        {
            public bool Result;
            public readonly List<string> Events = new();
        }

        public sealed partial class ActionProbe : SFActionTask
        {
            private Dependency _dependency;
            public int InitCount;
            protected override void Init()
            {
                InitCount++;
                _dependency.Events.Add("init");
            }
            protected override void OnExecute()
            {
                _dependency.Events.Add("execute");
                EndAction(_dependency.Result);
            }
        }

        public sealed partial class GenericActionProbe : SFActionTask<Transform>
        {
            private Dependency _dependency;
            public Transform AgentSeenByExecute;
            protected override void Init() => _dependency.Events.Add("generic-init");
            protected override void OnExecute()
            {
                AgentSeenByExecute = agent;
                _dependency.Events.Add("generic-execute");
                EndAction(_dependency.Result);
            }
        }

        public sealed partial class ConditionProbe : SFConditionTask
        {
            private Dependency _dependency;
            protected override void Init() => _dependency.Events.Add("init");
            protected override void OnEnable() => _dependency.Events.Add("enable");
            protected override void OnDisable() => _dependency.Events.Add("disable");
            protected override bool OnCheck()
            {
                _dependency.Events.Add("check");
                return _dependency.Result;
            }
        }

        public sealed partial class GenericConditionProbe : SFConditionTask<Transform>
        {
            private Dependency _dependency;
            public Transform AgentSeenByCheck;
            protected override void Init() => _dependency.Events.Add("generic-init");
            protected override bool OnCheck()
            {
                AgentSeenByCheck = agent;
                _dependency.Events.Add("generic-check");
                return _dependency.Result;
            }
        }

        private sealed class TaskSystem : ITaskSystem
        {
            public Component agent { get; set; }
            public IBlackboard blackboard => null;
            public Object contextObject => agent;
            public float elapsedTime => 0;
            public float deltaTime => 0;
            public void UpdateTasksOwner() { }
            public void SendEvent(string name, object value, object sender) { }
            public void SendEvent<T>(string name, T value, object sender) { }
        }
    }
}
