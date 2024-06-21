using System.Collections.Generic;
using FurrfieldStudio.MecanimBaker.Data;
using UnityEngine;

namespace FurrfieldStudio.MecanimBaker.Utils
{
    public static class AnimationParamSetterUtils
    {
        public static void SetExpectedValue(this IEnumerable<AnimationParamSetterEntry> animationParamSetterEntries, Animator animator)
        {
            foreach(AnimationParamSetterEntry animationParamSetterEntry in animationParamSetterEntries)
            {
                animationParamSetterEntry.SetExpectedValue(animator);
            }
        }
        
        public static void SetDefaultValue(this IEnumerable<AnimationParamSetterEntry> animationParamSetterEntries, Animator animator)
        {
            foreach(AnimationParamSetterEntry animationParamSetterEntry in animationParamSetterEntries)
            {
                animationParamSetterEntry.SetDefaultValue(animator);
            }
        }
    }
}