using System;
using System.Linq;
using ParadoxNotion.Design;
using SFramework.Configs.Editor;
using SFramework.NodeCanvas.Runtime;
using UnityEditor;
using UnityEngine;

namespace SFramework.NodeCanvas.Editor
{
    public class SFIdNodeCanvasAttributeDrawer : AttributeDrawer<SFIdNodeCanvasAttribute>
    {
        public override object OnGUI(GUIContent cont, object inst)
        {
            if (inst == null) return string.Empty;
            if (fieldInfo.FieldType != typeof(string)) return MoveNextDrawer();

            var value = (string)inst;

            var sfTypeAttribute = fieldInfo.GetCustomAttributes(typeof(SFIdNodeCanvasAttribute), false)[0] as SFIdNodeCanvasAttribute;

            if (sfTypeAttribute == null) return MoveNextDrawer();

            var _paths = SFConfigsEditorUtility.GetNodePaths(sfTypeAttribute.Type.Name, sfTypeAttribute.Indent);
            if (_paths == null || _paths.Length == 0) return MoveNextDrawer();

            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(value) && !_paths.Contains(value))
            {
                return EditorGUILayout.TextField(cont, value);
            }

            var name = _paths.Contains(value)
                ? value
                : _paths[0];

            var index = Array.IndexOf(_paths, name);

            if (index == 0)
            {
                GUI.backgroundColor = Color.red;
            }

            index = EditorGUILayout.Popup(cont, index, _paths);

            GUI.backgroundColor = Color.white;

            return index == 0 ? string.Empty : _paths[index];
        }
    }
}