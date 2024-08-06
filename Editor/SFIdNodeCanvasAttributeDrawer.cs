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
        private int _hash;
        private readonly HashSet<ISFNodesConfig> _configs = new();
        private readonly List<string> _paths = new List<string>();

        private bool CheckAndLoadDatabase(Type type, object inst)
        {
            if (inst == null) return false;
            if (inst.GetHashCode() == _hash && _configs.Count != 0) return true;
            
            foreach (var config in SFConfigsEditorExtensions.FindConfigs<ISFNodesConfig>(type))
            {
                if (config is ISFNodesConfig nodesConfig)
                {
                    _configs.Add(nodesConfig);
                }
            }
            
            _hash = inst.GetHashCode();
            return _configs.Count != 0;
        }


        public override object OnGUI(GUIContent cont, object inst)
        {
            if (inst == null) return MoveNextDrawer();
            if (fieldInfo.FieldType != typeof(string)) return MoveNextDrawer();


            var value = (string)inst;

            var sfTypeAttribute = fieldInfo.GetCustomAttributes(typeof(SFIdNodeCanvasAttribute), false)[0] as SFIdNodeCanvasAttribute;

            if (sfTypeAttribute == null) return MoveNextDrawer();
            
            if (!CheckAndLoadDatabase(sfTypeAttribute.Type, inst)) return MoveNextDrawer();

            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }

            _paths.Clear();
            _paths.Add("-");

            foreach (var config in _configs)
            {
                config.Children.FindAllPaths(out var ids, sfTypeAttribute.Indent);
                
                if (sfTypeAttribute.Indent == 0)
                {
                    _paths.Add(config.Id);
                }
                else
                {
                    foreach (var id in ids)
                    {
                        _paths.Add(string.Join("/", config.Id, id));
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(value) && !_paths.Contains(value))
            {
                return EditorGUILayout.TextField(cont, value);
            }

            var name = _paths.Contains(value)
                ? value
                : _paths[0];

            var index = _paths.IndexOf(name);

            if (index == 0)
            {
                GUI.backgroundColor = Color.red;
            }

            index = EditorGUILayout.Popup(cont, index, _paths.ToArray());

            GUI.backgroundColor = Color.white;

            return index == 0 ? string.Empty : _paths.ElementAt(index);
        }
    }
}