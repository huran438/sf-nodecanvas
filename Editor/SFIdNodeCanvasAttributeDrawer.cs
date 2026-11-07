using System;
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
            if (fieldInfo?.FieldType != typeof(string) || attribute.Type == null)
                return MoveNextDrawer();

            var paths = SFConfigsEditorUtility.GetNodePaths(attribute.Type.Name, attribute.Indent);
            if (paths == null || paths.Length == 0)
                return MoveNextDrawer();

            var value = (string)inst;
            var index = string.IsNullOrWhiteSpace(value) ? 0 : Array.IndexOf(paths, value);
            if (index < 0)
                return EditorGUILayout.TextField(cont, value);

            var backgroundColor = GUI.backgroundColor;
            try
            {
                if (index == 0)
                    GUI.backgroundColor = Color.red;

                index = EditorGUILayout.Popup(cont, index, paths);
                return index == 0 ? string.Empty : paths[index];
            }
            finally
            {
                GUI.backgroundColor = backgroundColor;
            }
        }
    }
}