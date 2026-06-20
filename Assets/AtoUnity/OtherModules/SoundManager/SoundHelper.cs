using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.SoundManager
{
    public static class SoundHelper 
    {
        /// <summary>
        /// Converts a percentage fraction to decibels,
        /// with a lower clamp of 0.0001 for a minimum of -80dB, same as Unity's Mixers.
        /// </summary>
        public static float ConvertToDecibel(float _value)
        {
            return Mathf.Log10(Mathf.Max(_value, 0.0001f)) * 20f;
        }
    }
}
