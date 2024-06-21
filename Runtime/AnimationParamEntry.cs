using System;
using FurrfieldStudio.MecanimBaker.Data;
using UnityEngine;

namespace FurrfieldStudio.MecanimBaker
{
    [Serializable]
    public class AnimationParamEntry
    {
        [SerializeField]
        private AnimatorParamsReader _animatorParamsReader;

        [SerializeField]
        private string _paramName;

        [NonSerialized]
        private AnimationParam _animationParam;

        public void SetValue(Animator animator, object value)
        {
            LoadAnimationParam();
            
            _animationParam.SetAnimatorValue(animator, value);
        }

        public T GetValue<T>(Animator animator)
        {
            return (T) _animationParam.GetAnimatorValue(animator);
        }

        private void LoadAnimationParam()
        {
            if(_animationParam != null) return;

            _animationParam = _animatorParamsReader.AnimatorParams.Find(ap => ap.Name == _paramName);
        }
    }
}