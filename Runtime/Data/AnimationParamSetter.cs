//using Sirenix.OdinInspector;

using System;
using UnityEngine;

namespace FurrfieldStudio.MecanimBaker.Data
{
    /// <summary>
    /// This is a wrapper for a single AnimationParamSetterInternalSerializable, required for proper serialization.
    /// If you want to change value of the parameter you should use this container, not AnimationParamSetterInternalSerializable alone.
    /// </summary>
    [Serializable]
    public class AnimationParamSetter
    {
        [SerializeReference]
        public AnimationParamSetterInternalSerializable Value;
        
        public void SetExpectedValue(AnimationParam animationParam, Animator animator) => Value.SetExpectedValue(animationParam, animator);
        
        public void SetDefaultValue(AnimationParam animationParam, Animator animator) => Value.SetDefaultValue(animationParam, animator);
    }
}