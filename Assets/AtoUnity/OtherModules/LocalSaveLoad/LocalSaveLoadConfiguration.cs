using AtoGame.Base;
using AtoGame.Base.UnityInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.LocalSaveLoad
{
    [CreateAssetMenu(fileName = "LocalSaveLoadConfiguration", menuName = "Data/OtherModules/LocalSaveLoad/LocalSaveLoadConfiguration")]
    public class LocalSaveLoadConfiguration : ScriptableObject
    {
        [SerializeField] private PipelineLocalStepConfig[] localStepConfigs;

        public int SaveVersion()
        {
            string curVersionString = Application.version;
            string[] splitNumbers = curVersionString.Split('.');
            int[] numbers = new int[splitNumbers.Length];
            for(int i = 0; i < splitNumbers.Length; ++i)
            {
                numbers[i] = int.Parse(splitNumbers[i]);
            }
            int saveVersion = 0;
            for(int i = 0; i < numbers.Length; ++i)
            {
                saveVersion += numbers[i] * ((int)Mathf.Pow(1000, numbers.Length - i - 1));
            }
            return saveVersion;
        }

        private void OnValidate()
        {
            Array.Sort(localStepConfigs, new Comparison<PipelineLocalStepConfig>((i1, i2) => i1.Version.CompareTo(i2.Version)));
        }

        public List<PipelineLocalStepConfig> GetNextLocalSaveSteps(int preVersion)
        {
            List<PipelineLocalStepConfig> nextSaveSteps = new List<PipelineLocalStepConfig>();
            for(int i = 0; i < localStepConfigs.Length; ++i)
            {
                if(localStepConfigs[i].Version > preVersion)
                {
                    nextSaveSteps.Add(localStepConfigs[i]);
                }
            }
            return nextSaveSteps;
        }

    }
}
