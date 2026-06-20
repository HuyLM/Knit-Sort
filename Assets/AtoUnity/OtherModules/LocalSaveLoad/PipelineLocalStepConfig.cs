using AtoGame.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.LocalSaveLoad
{
    [CreateAssetMenu(fileName = "PipelineLocalStepConfig", menuName = "Data/OtherModules/LocalSaveLoad/PipelineLocalStepConfig")]
    public class PipelineLocalStepConfig : ScriptableObject
    {
        [SerializeField] private int version;
        [SerializeField] private ActionObject action;

        public int Version { get => version; }

        public void ApplyChange()
        {
            if(action == null)
            {
                return;
            }
            action.Execute();
        }
    }
}
