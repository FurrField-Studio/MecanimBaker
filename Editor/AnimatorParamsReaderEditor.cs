using System.Collections.Generic;
using System.Linq;
using FurrfieldStudio.MecanimBaker.Data;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

namespace FurrfieldStudio.MecanimBaker.Editor
{
    [CustomEditor(typeof(AnimatorParamsReader))]
    public class AnimatorParamsReaderEditor : UnityEditor.Editor
    {
        public VisualTreeAsset inspectorXML;
        
        public VisualTreeAsset animationParamXML;
        
        private AnimatorParamsReader _animatorParamsReader;
        
        private Dictionary<int, AnimationParam> _loadedAnimationParamsHash = new();
        private Dictionary<string, AnimationParam> _loadedAnimationParams = new();
        
        public override VisualElement CreateInspectorGUI()
        {
            _animatorParamsReader = target as AnimatorParamsReader;
            
            // Create a new VisualElement to be the root of our Inspector UI.
            VisualElement myInspector = new VisualElement();

            // Load from default reference.
            inspectorXML.CloneTree(myInspector);

            myInspector.Q<Button>("BakeParametersButton").clicked += BakeParameters;
            
            SetupAnimationParamListView(myInspector);
            SetupStateNamesListView(myInspector);
                
            // Return the finished Inspector UI.
            return myInspector;
        }
        
        private void BakeParameters()
        {
            if(_animatorParamsReader.AnimatorController == null) return;

            _animatorParamsReader.AnimatorParams.Clear();
            _animatorParamsReader.StateNames.Clear();
            
            // LoadAllAnimationParams();

            GenerateAnimationParams();
            GenerateTransitionContainers();

            foreach(var animationParam in _animatorParamsReader.AnimatorParams)
            {
                animationParam.SortParamSetters();
            }

            _animatorParamsReader.SortStateNames();
            
            EditorUtility.SetDirty(_animatorParamsReader);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private void SetupStateNamesListView(VisualElement myInspector)
        {
            ScrollView animationParamListView = myInspector.Q<ScrollView>("StateNamesList");

            foreach (var animationParam in _animatorParamsReader.StateNames)
            {
                animationParamListView.Add(new Label(animationParam));
            }
        }

        private void SetupAnimationParamListView(VisualElement myInspector)
        {
            ScrollView animationParamScrollView = myInspector.Q<ScrollView>("AnimationsParamsList");

            foreach (var animationParam in _animatorParamsReader.AnimatorParams)
            {
                animationParamScrollView.contentContainer.Add(MakeAnimationParamVisualElement(animationParam));
            }
        }

        private VisualElement MakeAnimationParamVisualElement(AnimationParam animationParam)
        {
            VisualElement root = new VisualElement();
            animationParamXML.CloneTree(root);

            root.Q<Foldout>("ParamNameFoldout").text = animationParam.Name;
            root.Q<TextField>("ParamNameField").value = animationParam.Name;
            root.Q<IntegerField>("ParamNameHashField").value = animationParam.NameHash;
            root.Q<TextField>("ParamTypeField").value = animationParam.AnimatorControllerParameterType.ToString();
            
            ScrollView paramSettersScrollView = root.Q<ScrollView>("ParamSettersScrollView");

            paramSettersScrollView.Add(new Label("Values:"));
            foreach (string stringValueName in animationParam.AnimationParamSetters.Select(aps => aps.Value.GetValueToSet().ToString()).ToList())
            {
                paramSettersScrollView.Add(new Label(stringValueName));
            }

            return root;
        }

        #region DataBaking

        // private void LoadAllAnimationParams()
        // {
        //     _loadedAnimationParamsHash = _animatorParamsReader.AnimatorParams.ToDictionary(ap => ap.NameHash, ap => ap);
        //     _loadedAnimationParams = _animatorParamsReader.AnimatorParams.ToDictionary(ap => ap.Name, ap => ap);
        // }

        private void GenerateAnimationParams()
        {
            foreach(AnimatorControllerParameter param in _animatorParamsReader.AnimatorController.parameters)
            {
                AnimationParam animationParam;

                animationParam = new AnimationParam();

                _animatorParamsReader.AnimatorParams.Add(animationParam);

                animationParam.Initialize(param.name, param.nameHash, param.type);

                _loadedAnimationParamsHash.Add(animationParam.NameHash, animationParam);
                _loadedAnimationParams.Add(animationParam.Name, animationParam);

                if (param.type == AnimatorControllerParameterType.Bool)
                {
                    AnimationParamSetter trueParamSetter = new AnimationParamSetter();
                    trueParamSetter.Value = GenerateParamSetter(animationParam, -1);
                    animationParam.AnimationParamSetters.Add(trueParamSetter);

                    AnimationParamSetter falseParamSetter = new AnimationParamSetter();
                    falseParamSetter.Value = GenerateParamSetter(animationParam, 0);
                    animationParam.AnimationParamSetters.Add(falseParamSetter);
                }
                else
                {
                    AnimationParamSetterInternalSerializable animationParamSetterInternalSerializable = GenerateDefaultParamSetter(param);
                    AnimationParamSetter defaultParamSetter = GenerateStateTransition(param.name, animationParamSetterInternalSerializable.GetValueToSet(), animationParam);
                }
            }
        }

        #region GettingTransitions

        private class TransitionConditionContainer
        {
            public AnimatorCondition[] conditions;

            public TransitionConditionContainer(AnimatorCondition[] conditions)
            {
                this.conditions = conditions;
            }
        }

        private void Generate(TransitionConditionContainer transitionConditionContainer)
        {
            string parameterName;
            for(int index = 0; index < transitionConditionContainer.conditions.Length; index++)
            {
                parameterName = transitionConditionContainer.conditions[index].parameter;
                
                if(_loadedAnimationParams[parameterName].AnimatorControllerParameterType == AnimatorControllerParameterType.Bool) continue;
                
                GenerateStateTransition(parameterName, transitionConditionContainer.conditions[index].threshold);
            }
        }
        
        private AnimationParamSetter GenerateStateTransition(string parameter, float threshold)
        {
            AnimationParam animationParam = _animatorParamsReader.AnimatorParams.Find(param => param.Name == parameter);

            return GenerateStateTransition(parameter, threshold, animationParam);
        }
        
        private AnimationParamSetter GenerateStateTransition(string parameter, float threshold, AnimationParam animationParam)
        {
            AnimationParamSetter animationParamSetter = _loadedAnimationParams[parameter].AnimationParamSetters.Find(aps => aps.Value.GetValueToSet() == threshold);

            if(animationParamSetter != null)
            {
                if(!animationParam.AnimationParamSetters.Contains(animationParamSetter))
                {
                    animationParam.AnimationParamSetters.Add(animationParamSetter);
                }

                if(animationParam.AnimatorControllerParameterType != animationParamSetter.Value.GetAnimatorControllerParameterType())
                {
                    animationParamSetter.Value = GenerateParamSetter(animationParam, threshold);
                }

                return animationParamSetter;
            }

            animationParamSetter = new AnimationParamSetter();
            animationParamSetter.Value = GenerateParamSetter(animationParam, threshold);
            
            animationParam.AnimationParamSetters.Add(animationParamSetter);

            return animationParamSetter;
        }
        
        private void GenerateTransitionContainers()
        {
            foreach(TransitionConditionContainer transitionContainer in GetTransitions())
            {
                Generate(transitionContainer);
            }
        }

        private List<TransitionConditionContainer> GetTransitions()
        {
            List<TransitionConditionContainer> stateTransitions = new List<TransitionConditionContainer>();

            foreach(AnimatorControllerLayer animatorControllerLayer in _animatorParamsReader.AnimatorController.layers)
            {
                AddTransitions(stateTransitions, animatorControllerLayer.stateMachine.entryTransitions);

                foreach(ChildAnimatorState childAnimatorState in animatorControllerLayer.stateMachine.states)
                {
                    TryAddStateName(childAnimatorState.state.name);
                    AddTransitions(stateTransitions, childAnimatorState.state.transitions);
                }

                GetTransitionsForStateMachine(animatorControllerLayer.stateMachine.stateMachines, stateTransitions);
            }

            return stateTransitions;
        }

        private void GetTransitionsForStateMachine(ChildAnimatorStateMachine[] childAnimatorStateMachines, List<TransitionConditionContainer> stateTransitions)
        {
            foreach(ChildAnimatorStateMachine childAnimatorStateMachine in childAnimatorStateMachines)
            {
                AddTransitions(stateTransitions, childAnimatorStateMachine.stateMachine.entryTransitions);

                foreach(ChildAnimatorState childAnimatorState in childAnimatorStateMachine.stateMachine.states)
                {
                    TryAddStateName(childAnimatorState.state.name);
                    AddTransitions(stateTransitions, childAnimatorState.state.transitions);
                }

                if(childAnimatorStateMachine.stateMachine.stateMachines.Length > 0)
                {
                    GetTransitionsForStateMachine(childAnimatorStateMachine.stateMachine.stateMachines, stateTransitions);   
                }
            }
        }

        private void AddTransitions(List<TransitionConditionContainer> stateTransitions, AnimatorTransitionBase[] animatorTransitions)
        {
            foreach(AnimatorTransitionBase animatorTransition in animatorTransitions)
            {
                if(animatorTransition.conditions.Length > 0)
                {
                    stateTransitions.Add(new TransitionConditionContainer(animatorTransition.conditions));
                }
            }
        }

        private void TryAddStateName(string stateName)
        {
            if(_animatorParamsReader.StateNames.Contains(stateName)) return;
            
            _animatorParamsReader.StateNames.Add(stateName);
        }

        #endregion
        
        private AnimationParamSetterInternalSerializable GenerateDefaultParamSetter(AnimatorControllerParameter animatorControllerParameter)
        {
            switch (animatorControllerParameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    return new FloatAnimationParamSetterInternalSerializable(animatorControllerParameter.defaultFloat, animatorControllerParameter.defaultFloat);
                    break;
                case AnimatorControllerParameterType.Int:
                    return new IntAnimationParamSetterInternalSerializable(animatorControllerParameter.defaultInt, animatorControllerParameter.defaultInt);
                    break;
                case AnimatorControllerParameterType.Bool:
                    return new BoolAnimationParamSetterInternalSerializable(animatorControllerParameter.defaultBool, animatorControllerParameter.defaultBool);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    return new TriggerAnimationParamSetterInternalSerializable();
                    break;
            }

            return null;
        }

        private AnimationParamSetterInternalSerializable GenerateParamSetter(AnimationParam animationParam, float value)
        {
            AnimatorControllerParameter animatorControllerParameter = _animatorParamsReader.AnimatorController.parameters.First(animationControlerParameter => animationControlerParameter.nameHash == animationParam.NameHash);
            
            switch (animationParam.AnimatorControllerParameterType)
            {
                case AnimatorControllerParameterType.Float:
                    return new FloatAnimationParamSetterInternalSerializable(value, animatorControllerParameter.defaultFloat);
                    break;
                case AnimatorControllerParameterType.Int:
                    return new IntAnimationParamSetterInternalSerializable((int) value, animatorControllerParameter.defaultInt);
                    break;
                case AnimatorControllerParameterType.Bool:
                    return new BoolAnimationParamSetterInternalSerializable(value == -1, animatorControllerParameter.defaultBool);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    return new TriggerAnimationParamSetterInternalSerializable();
                    break;
            }

            return null;
        }
        #endregion
    }
}