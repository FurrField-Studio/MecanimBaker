using System;
using UnityEngine;

namespace FurrfieldStudio.MecanimBaker.Data
{
    [Serializable]
    public abstract class GenericAnimationParamSetterInternalSerializable<T> : AnimationParamSetterInternalSerializable
    {
        public T ValueToSet;

        public T DefaultValue;

        public GenericAnimationParamSetterInternalSerializable(T valueToSet, T defaultValue)
        {
            ValueToSet = valueToSet;
            DefaultValue = defaultValue;
        }
    }

    [Serializable]
    public class TriggerAnimationParamSetterInternalSerializable : AnimationParamSetterInternalSerializable
    {
        public override void SetExpectedValue(AnimationParam animationParam, Animator animator)
        {
            animator.SetTrigger(animationParam.Name);
        }
    }

    [Serializable]
    public class FloatAnimationParamSetterInternalSerializable : GenericAnimationParamSetterInternalSerializable<float>
    {
        public FloatAnimationParamSetterInternalSerializable(float valueToSet, float defaultValue) : base(valueToSet, defaultValue) { }
        
        public override void SetExpectedValue(AnimationParam animationParam, Animator animator)
        {
            animator.SetFloat(animationParam.Name, ValueToSet);
        }

        public override void SetDefaultValue(AnimationParam animationParam, Animator animator)
        {
            animator.SetFloat(animationParam.Name, DefaultValue);
        }
        
#if UNITY_EDITOR
        public override float GetValueToSet() => ValueToSet;
        
        public override AnimatorControllerParameterType GetAnimatorControllerParameterType() => AnimatorControllerParameterType.Float;
#endif
    }
    
    [Serializable]
    public class IntAnimationParamSetterInternalSerializable : GenericAnimationParamSetterInternalSerializable<int>
    {
        public IntAnimationParamSetterInternalSerializable(int valueToSet, int defaultValue) : base(valueToSet, defaultValue) { }
        
        public override void SetExpectedValue(AnimationParam animationParam, Animator animator)
        {
            animator.SetInteger(animationParam.Name, ValueToSet);
        }

        public override void SetDefaultValue(AnimationParam animationParam, Animator animator)
        {
            animator.SetInteger(animationParam.Name, DefaultValue);
        }
        
#if UNITY_EDITOR
        public override float GetValueToSet() => (float) ValueToSet;

        public override AnimatorControllerParameterType GetAnimatorControllerParameterType() => AnimatorControllerParameterType.Int;
#endif
    }
    
    [Serializable]
    public class BoolAnimationParamSetterInternalSerializable : GenericAnimationParamSetterInternalSerializable<bool>
    {
        public BoolAnimationParamSetterInternalSerializable(bool valueToSet, bool defaultValue) : base(valueToSet, defaultValue) { }
        
        public override void SetExpectedValue(AnimationParam animationParam, Animator animator)
        {
            animator.SetBool(animationParam.Name, ValueToSet);
        }

        public override void SetDefaultValue(AnimationParam animationParam, Animator animator)
        {
            animator.SetBool(animationParam.Name, DefaultValue);
        }
        
#if UNITY_EDITOR
        public override float GetValueToSet() => ValueToSet ? -1f : 0f;

        public override AnimatorControllerParameterType GetAnimatorControllerParameterType() => AnimatorControllerParameterType.Bool;
#endif
    }
}