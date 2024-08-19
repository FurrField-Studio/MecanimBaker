using System;
using System.Collections.Generic;
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

        private static Dictionary<AnimatorControllerParameterType, Action<AnimationParamSetterEntryPropertyDrawer>> _parameterTypeRenderers = new Dictionary<AnimatorControllerParameterType, Action<AnimationParamSetterEntryPropertyDrawer>>()
        {
            {AnimatorControllerParameterType.Float, UpdateNumericDropdown},
            {AnimatorControllerParameterType.Int, UpdateNumericDropdown},
            {AnimatorControllerParameterType.Bool, UpdateBoolToggle},
            {AnimatorControllerParameterType.Trigger, UpdateTriggerLabel},
        };

        //Fix for weird property spacing
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            
            _property = property;

            if(_paramEntryRenderer == null) _paramEntryRenderer = new ParamEntryRenderer(_property);
                
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

            _parameterTypeRenderers[animationParam.AnimatorControllerParameterType].Invoke(this);
        }

        private static void UpdateNumericDropdown(AnimationParamSetterEntryPropertyDrawer propertyDrawer)
        {
            var paramValueProperty = propertyDrawer._paramValueProperty;
            var paramEntryRenderer = propertyDrawer._paramEntryRenderer;
            
            string[] values = (paramEntryRenderer.AnimatorParamsReaderProperty.objectReferenceValue as AnimatorParamsReader)
                .AnimatorParams.Find(ap => ap.Name == paramEntryRenderer.ParamNameProperty.stringValue)
                .AnimationParamSetters.Select(aps => aps.Value.GetValueToSet().ToString()).ToArray();
            int index = Array.IndexOf(values, paramValueProperty.stringValue);
            if(index == -1) index = 0;
                    
            paramValueProperty.serializedObject.Update();
                
            index = EditorGUILayout.Popup(index, values);
            paramValueProperty.stringValue = values[index];
                
            paramValueProperty.serializedObject.ApplyModifiedProperties();
        }

        private static void UpdateBoolToggle(AnimationParamSetterEntryPropertyDrawer propertyDrawer)
        {
            var paramValueProperty = propertyDrawer._paramValueProperty;
            
            paramValueProperty.serializedObject.Update();
                
            bool value = string.IsNullOrEmpty(paramValueProperty.stringValue) ? false : int.Parse(paramValueProperty.stringValue) == -1;
            value = EditorGUILayout.Toggle(value);
            paramValueProperty.stringValue = value ? "-1" : "0";
                
            paramValueProperty.serializedObject.ApplyModifiedProperties();
        }

        private static void UpdateTriggerLabel(AnimationParamSetterEntryPropertyDrawer propertyDrawer)
        {
            GUILayout.Label("Trigger");
        }
    }
}