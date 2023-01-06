using System;
using ParadoxNotion.Design;

namespace SFramework.NodeCanvas.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SFTypeNCAttribute : DrawerAttribute
    {
        public SFTypeNCAttribute(Type databaseType, int targetLayer = -1)
        {
            DatabaseType = databaseType;
            TargetLayer = targetLayer;
        }

        public readonly Type DatabaseType;
        public readonly int TargetLayer;

    }
}