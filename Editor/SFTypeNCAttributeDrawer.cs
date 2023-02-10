using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion.Design;
using SFramework.Core.Runtime;
using SFramework.NodeCanvas.Runtime;
using UnityEditor;
using UnityEngine;

namespace SFramework.NodeCanvas.Editor
{
    public class SFTypeNCAttributeDrawer : AttributeDrawer<SFTypeNCAttribute>
    {
        private ISFDatabase _database;

        private bool CheckAndLoadDatabase(Type databaseType)
        {
            if (_database != null && _database.GetType() == databaseType) return true;

            var typeName = databaseType.Name;

            var assetsGuids = AssetDatabase.FindAssets($"t:{typeName}");

            if (assetsGuids == null || assetsGuids.Length == 0)
            {
                Debug.LogWarning($"Missing Database: {typeName}");
                return false;
            }

            var path = AssetDatabase.GUIDToAssetPath(assetsGuids.First());
            _database = AssetDatabase.LoadAssetAtPath(path, databaseType) as ISFDatabase;

            return _database != null;
        }

        public override object OnGUI(GUIContent content, object instance)
        {
            if (fieldInfo.FieldType != typeof(string)) return MoveNextDrawer();

            var value = (string)instance;

            var sfTypeAttribute =
                fieldInfo.GetCustomAttributes(typeof(SFTypeNCAttribute), false)[0] as SFTypeNCAttribute;
            if (!CheckAndLoadDatabase(sfTypeAttribute.DatabaseType)) return MoveNextDrawer();

            if (string.IsNullOrWhiteSpace(value))
            {
                value = string.Empty;
            }
            
            var paths = new List<string> { "-" };
            _database.Nodes.FindAllPaths(out var ids, sfTypeAttribute.TargetLayer);
            paths.AddRange(ids);

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