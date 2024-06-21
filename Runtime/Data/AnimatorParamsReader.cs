using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace FurrfieldStudio.MecanimBaker.Data
{
    [CreateAssetMenu(fileName = "AnimatorParamsReader", menuName = "MecanimBaker/AnimatorParamsReader")]
    public class AnimatorParamsReader : ScriptableObject
    {
        [SerializeField]
        private AnimatorController _animatorController;
        public AnimatorController AnimatorController => _animatorController;
        
        [SerializeField]
        private List<AnimationParam> _animatorParams = new List<AnimationParam>();
        public List<AnimationParam> AnimatorParams => _animatorParams;
        
        [SerializeReference]
        private List<string> _stateNames = new List<string>();
        public List<string> StateNames => _stateNames;

#if UNITY_EDITOR

        public void SortStateNames()
        {
            _stateNames.Sort();
        }
        
#endif
    }
}