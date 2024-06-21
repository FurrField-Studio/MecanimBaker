using System.Linq;
using FurrfieldStudio.MecanimBaker.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FurrfieldStudio.MecanimBaker.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AnimationParamEntry))]
    public class AnimationParamEntryPropertyDrawer : PropertyDrawer
    {
        //Fix for weird property spacing
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            
            ParamEntryRenderer paramEntryRenderer = new ParamEntryRenderer(property);
            paramEntryRenderer.RenderParamEntry();

            EditorGUILayout.EndHorizontal();
        }
    }
}