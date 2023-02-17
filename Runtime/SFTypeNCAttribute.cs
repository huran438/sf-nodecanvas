using System;
using ParadoxNotion.Design;

namespace SFramework.NodeCanvas.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SFTypeNCAttribute : DrawerAttribute
    {
        public SFTypeNCAttribute(Type type, int indent = -1)
        {
            Type = type;
            Indent = indent;
        }

        public readonly Type Type;
        public readonly int Indent;

    }
}