using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Design;
using SFramework.Configs.Editor;
using SFramework.Configs.Runtime;
using SFramework.NodeCanvas.Runtime;
using UnityEditor;
using UnityEngine;

namespace SFramework.NodeCanvas.Editor
{
    public class SFIdNodeCanvasAttributeDrawer : AttributeDrawer<SFIdNodeCanvasAttribute>
    {
        private int hash;
        private HashSet<ISFConfig> _repositories = new();

        private bool CheckAndLoadDatabase(Type type, object instance)
        {
            if (instance == null) return false;
            if (instance.GetHashCode() == hash && _repositories.Count != 0) return true;
            _repositories = SFConfigsEditorExtensions.FindRepositories(type);
            hash = instance.GetHashCode();
            return _repositories.Count != 0;
        }


        public override object OnGUI(GUIContent content, object instance)
        {
            if (instance == null) return string.Empty;
            if (fieldInfo.FieldType != typeof(string)) return MoveNextDrawer();


            var value = (string)instance;

            var sfTypeAttribute =
                fieldInfo.GetCustomAttributes(typeof(SFIdNodeCanvasAttribute), false)[0] as SFIdNodeCanvasAttribute;
            if (!CheckAndLoadDatabase(sfTypeAttribute.Type, instance)) return MoveNextDrawer();

            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }

            var paths = new List<string> { "-" };

            foreach (var repository in _repositories)
            {
                repository.Nodes.FindAllPaths(out var ids, sfTypeAttribute.Indent);

                foreach (var id in ids)
                {
                    paths.Add($"{repository.Name}/{id}");
                }
            }

            if (!string.IsNullOrWhiteSpace(value) && !paths.Contains(value))
            {
                return EditorGUILayout.TextField(content, value);
            }

            var name = paths.Contains(value)
                ? value
                : paths[0];

            var _index = paths.IndexOf(name);

            if (_index == 0)
            {
                GUI.backgroundColor = Color.red;
            }

            _index = EditorGUILayout.Popup(content, _index, paths.ToArray());

            GUI.backgroundColor = Color.white;

            return _index == 0 ? string.Empty : paths.ElementAt(_index);
        }
    }
}