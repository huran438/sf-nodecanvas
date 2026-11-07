---
name: sf-nodecanvas
description: Use when creating SFramework-injected NodeCanvas actions/conditions or SFIdNodeCanvas task-inspector selectors.
---

# sf-nodecanvas

Read [the package guide](references/package-readme.md) and [installed source](SOURCE.md).

Require the installed NodeCanvas assemblies and NODECANVAS define. Derive actions from SFActionTask or SFActionTask<T>, and conditions from SFConditionTask or SFConditionTask<T>; typed agents use T : class.

Declare injectable tasks as partial and use mutable, nonserialized fields typed as ISFDependency contracts. SF Core generates their injection. Use the protected Init hook for setup after SF injection. If overriding NodeCanvas OnInit, call base.OnInit so the injection step still runs. Register dependencies before task initialization and separately await asynchronous service readiness.

Implement NodeCanvas OnExecute/OnCheck and completion behavior according to the installed NodeCanvas API. These bases do not supply ready-made game tasks or service registration.

SFIdNodeCanvas uses cached SF Configs IDs. Refresh that cache, configure paths and use the root config type when options are missing; the drawer does not populate caches itself or supply a runtime resolver.

Verify injection timing, graph execution, missing defines and selector output in the consuming project with NodeCanvas installed.
