using System;
using System.Linq;
using FurrfieldStudio.MecanimBaker.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FurrfieldStudio.MecanimBaker.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AnimationParamSetterEntry))]
    public class AnimationParamSetterEntryPropertyDrawer : PropertyDrawer
    {
        private ParamEntryRenderer _paramEntryRenderer;

        private SerializedProperty _paramValueProperty => _property.FindPropertyRelative("_animationParamValue");
        
        private SerializedProperty _property;

        //Fix for weird property spacing
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            
            _property = property;

            _paramEntryRenderer = new ParamEntryRenderer(_property);
            _paramEntryRenderer.RenderParamEntry();

            if(_paramEntryRenderer.paramsReaderNull) return;
            
            RenderValueProperty(_paramEntryRenderer.ParamNameProperty.stringValue);

            EditorGUILayout.EndHorizontal();
        }

        private void RenderValueProperty(string paramNameValue)
        {
            if(string.IsNullOrEmpty(paramNameValue) || _paramEntryRenderer.AnimatorParamsReaderProperty.objectReferenceValue == null) return;

            AnimatorParamsReader paramsReader = (_paramEntryRenderer.AnimatorParamsReaderProperty.objectReferenceValue as AnimatorParamsReader);
            AnimationParam animationParam = paramsReader.AnimatorParams.Find(ap => ap.Name == paramNameValue);

            UpdateNumericDropdown(animationParam, paramNameValue);
            UpdateBoolToggle(animationParam);
            UpdateTriggerLabel(animationParam);
        }

        private void UpdateNumericDropdown(AnimationParam animationParam, string paramNameValue)
        {
            if(animationParam.AnimatorControllerParameterType != AnimatorControllerParameterType.Float && animationParam.AnimatorControllerParameterType != AnimatorControllerParameterType.Int) return;

            string[] values = (_paramEntryRenderer.AnimatorParamsReaderProperty.objectReferenceValue as AnimatorParamsReader)
                .AnimatorParams.Find(ap => ap.Name == paramNameValue)
                .AnimationParamSetters.Select(aps => aps.Value.GetValueToSet().ToString()).ToArray();
            int index = Array.IndexOf(values, _paramValueProperty.stringValue);
            if(index == -1) index = 0;
                    
            _paramValueProperty.serializedObject.Update();
                
            index = EditorGUILayout.Popup(index, values);
            _paramValueProperty.stringValue = values[index];
                
            _paramValueProperty.serializedObject.ApplyModifiedProperties();
        }

        private void UpdateBoolToggle(AnimationParam animationParam)
        {
            if(animationParam.AnimatorControllerParameterType != AnimatorControllerParameterType.Bool) return;

            _paramValueProperty.serializedObject.Update();
                
            bool value = string.IsNullOrEmpty(_paramValueProperty.stringValue) ? false : int.Parse(_paramValueProperty.stringValue) == -1;
            value = EditorGUILayout.Toggle(value);
            _paramValueProperty.stringValue = value ? "-1" : "0";
                
            _paramValueProperty.serializedObject.ApplyModifiedProperties();
        }

        private void UpdateTriggerLabel(AnimationParam animationParam)
        {
            if(animationParam.AnimatorControllerParameterType != AnimatorControllerParameterType.Trigger) return;
            
            GUILayout.Label("Trigger");
        }
    }
}