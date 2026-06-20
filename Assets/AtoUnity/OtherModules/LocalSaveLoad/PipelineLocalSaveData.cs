using AtoGame.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.LocalSaveLoad
{
    public class PipelineLocalSaveData : LocalSaveLoadable
    {
        private const string PIPELINE_LOCAL_SAVE_VERSION_KEY = "PIPELINE_LOCAL_SAVE_VERSION_KEY";
        private int savedSaveVersion;
        private int configSaveVersion;
        private LocalSaveLoadConfiguration config;

        public int SaveVersion
        {
            get => savedSaveVersion;
            set => savedSaveVersion = value;

        }

        public void Setup(LocalSaveLoadConfiguration config)
        {
            this.config = config;
        }

        public int ConfigSaveVersion()
        {
            return config.SaveVersion();
        }

        public void ConvertData()
        {
            if (SaveVersion < configSaveVersion)
            {
                List<PipelineLocalStepConfig> nextSteps = config.GetNextLocalSaveSteps(SaveVersion);
                for (int i = 0; i < nextSteps.Count; ++i)
                {
                    nextSteps[i].ApplyChange();
                }
                SaveVersion = configSaveVersion;
            }
        }

        public void CreateData()
        {
            savedSaveVersion = configSaveVersion = ConfigSaveVersion();
        }
        public void InitData()
        {
        }
        public void LoadData()
        {
            configSaveVersion = ConfigSaveVersion();
            savedSaveVersion = PlayerPrefExtension.GetInt(PIPELINE_LOCAL_SAVE_VERSION_KEY, configSaveVersion);
        }
        public void SaveData()
        {
            PlayerPrefExtension.SetInt(PIPELINE_LOCAL_SAVE_VERSION_KEY, savedSaveVersion);
        }

        public void EaseData()
        {
            savedSaveVersion = configSaveVersion = ConfigSaveVersion();
        }
    }
}
