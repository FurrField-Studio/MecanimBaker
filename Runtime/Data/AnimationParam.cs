using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FurrfieldStudio.MecanimBaker.Data
{
    [Serializable]
    public class AnimationParam
    {
        [SerializeField]
        private string _name;
        public string Name => _name;
        
        [SerializeField]
        private int _nameHash;
        public int NameHash => _nameHash;

        [SerializeField]
        private AnimatorControllerParameterType _animatorControllerParameterType;
        public AnimatorControllerParameterType AnimatorControllerParameterType => _animatorControllerParameterType;

        [SerializeField]
        private List<AnimationParamSetter> _animationParamSetters;
        public List<AnimationParamSetter> AnimationParamSetters => _animationParamSetters;

        public void Initialize(string name, int nameHash, AnimatorControllerParameterType animatorControllerParameterType)
        {
            _name = name;
            _nameHash = nameHash;
            _animatorControllerParameterType = animatorControllerParameterType;
            _animationParamSetters = new();
        }

        public override string ToString() => _name;

#if UNITY_EDITOR

        public void SortParamSetters()
        {
            _animationParamSetters = _animationParamSetters.OrderBy(aps => aps.Value.GetValueToSet()).ToList();
        }

#endif
        
        public void SetAnimatorValue(Animator animator, object value)
        {
            switch(_animatorControllerParameterType)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(_nameHash, (float) value);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(_nameHash, (int) value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(_nameHash, (bool) value);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(_nameHash);
                    break;
                default:
                    break;
            }
        }

        public object GetAnimatorValue(Animator animator)
        {
            switch(_animatorControllerParameterType)
            {
                case AnimatorControllerParameterType.Float:
                    return animator.GetFloat(_nameHash);
                    break;
                case AnimatorControllerParameterType.Int:
                    return animator.GetInteger(_nameHash);
                    break;
                case AnimatorControllerParameterType.Bool:
                    return animator.GetBool(_nameHash);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    return animator.GetBool(_nameHash);
                    break;
                default:
                    break;
            }

            return null;
        }
    }
}