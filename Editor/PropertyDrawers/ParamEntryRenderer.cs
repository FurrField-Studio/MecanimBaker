using System.Linq;
using FurrfieldStudio.MecanimBaker.Data;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FurrfieldStudio.MecanimBaker.Editor.PropertyDrawers
{
    internal class ParamEntryRenderer
    {
        public SerializedProperty AnimatorParamsReaderProperty => _property.FindPropertyRelative("_animatorParamsReader");
        public SerializedProperty ParamNameProperty => _property.FindPropertyRelative("_paramName");
        
        public bool paramsReaderNull;

        private SerializedProperty _property;
        private string[] _animatorParamNames;
        private int _selectedAnimatorParamName;

        public ParamEntryRenderer(SerializedProperty property)
        {
            _property = property;
        }

        public void RenderParamEntry()
        {
            if(!_property.displayName.Contains("Element"))
            {
                GUILayout.Label(_property.displayName);
            }

            _property.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(AnimatorParamsReaderProperty, GUIContent.none);
            if(EditorGUI.EndChangeCheck())
            {
                paramsReaderNull = AnimatorParamsReaderProperty.objectReferenceValue == null;
            }
            _property.serializedObject.ApplyModifiedProperties();

            if(AnimatorParamsReaderProperty.objectReferenceValue == null) return;
            
            _animatorParamNames = (AnimatorParamsReaderProperty.objectReferenceValue as AnimatorParamsReader).AnimatorParams.Select(ap => ap.Name).ToArray();
            _selectedAnimatorParamName = Array.IndexOf(_animatorParamNames, ParamNameProperty.stringValue);
            if(_selectedAnimatorParamName == -1) _selectedAnimatorParamName = 0;
            
            _property.serializedObject.Update();
            _selectedAnimatorParamName = EditorGUILayout.Popup(_selectedAnimatorParamName, _animatorParamNames);
            ParamNameProperty.stringValue = _animatorParamNames[_selectedAnimatorParamName];
            _property.serializedObject.ApplyModifiedProperties();
        }
    }
}