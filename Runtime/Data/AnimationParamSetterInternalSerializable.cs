using System;
using UnityEngine;

namespace FurrfieldStudio.MecanimBaker.Data
{
    /// <summary>
    /// Use AnimationParamSetter if you want to change AnimationParam value.
    /// Do not use this class alone without a AnimationParamSetter unless you know what you are doing.
    /// AnimationParamSetterInternalSerializable is internal class used to store parameter values.
    /// </summary>
    [Serializable]
    public class AnimationParamSetterInternalSerializable
    {
        public virtual void SetExpectedValue(AnimationParam animationParam, Animator animator) { }

        public virtual void SetDefaultValue(AnimationParam animationParam, Animator animator) { }

#if UNITY_EDITOR

        public virtual float GetValueToSet() => 0f;

        public virtual AnimatorControllerParameterType GetAnimatorControllerParameterType() => AnimatorControllerParameterType.Trigger;

#endif
    }
}