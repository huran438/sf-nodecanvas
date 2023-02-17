using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Design;
using SFramework.Core.Editor;
using SFramework.Core.Runtime;
using SFramework.NodeCanvas.Runtime;
using SFramework.Repositories.Runtime;
using UnityEditor;
using UnityEngine;
using SFEditorExtensions = SFramework.Repositories.Editor.SFEditorExtensions;

namespace SFramework.NodeCanvas.Editor
{
    public class SFTypeNCAttributeDrawer : AttributeDrawer<SFTypeNCAttribute>
    {
        private HashSet<ISFRepository> _repositories;

        private bool CheckAndLoadDatabase(Type type)
        {
            if (_repositories.Count != 0) return true;
            _repositories = SFEditorExtensions.FindRepositories(type);
            return _repositories.Count != 0;
        }


        public override object OnGUI(GUIContent content, object instance)
        {
            if (fieldInfo.FieldType != typeof(string)) return MoveNextDrawer();

            var value = (string)instance;

            var sfTypeAttribute =
                fieldInfo.GetCustomAttributes(typeof(SFTypeNCAttribute), false)[0] as SFTypeNCAttribute;
            if (!CheckAndLoadDatabase(sfTypeAttribute.Type)) return MoveNextDrawer();

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
                    paths.Add($"{repository._Name}/{id}");
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