using System;
using FurrfieldStudio.MecanimBaker.Data;
using UnityEngine;

namespace FurrfieldStudio.MecanimBaker
{
    [Serializable]
    public class AnimationParamSetterEntry
    {
        [SerializeField]
        private AnimatorParamsReader _animatorParamsReader;
        
        [SerializeField]
        private string _paramName;
        
        [SerializeField]
        private string _animationParamValue;
        
        [NonSerialized]
        private AnimationParam _animationParam;
        [NonSerialized]
        private AnimationParamSetter _animationParamSetter;
        
        public void SetExpectedValue(Animator animator)
        {
            LoadAnimationParam();
            
            _animationParamSetter.SetExpectedValue(_animationParam, animator);
        }

        public void SetDefaultValue(Animator animator)
        {
            LoadAnimationParam();
            
            _animationParamSetter.SetDefaultValue(_animationParam, animator);
        }
        
        public T GetValue<T>(Animator animator)
        {
            return (T) _animationParam.GetAnimatorValue(animator);
        }
        
        private void LoadAnimationParam()
        {
            if(_animationParamSetter != null) return;
            
            _animationParam = _animatorParamsReader.AnimatorParams.Find(ap => ap.Name == _paramName);
            _animationParamSetter = _animationParam.AnimationParamSetters.Find(aps => aps.Value.GetValueToSet() == float.Parse(_animationParamValue));
        }
    }
}