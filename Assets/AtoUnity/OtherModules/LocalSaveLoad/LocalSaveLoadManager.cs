using AtoGame.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.LocalSaveLoad
{
    public class LocalSaveLoadManager : Singleton<LocalSaveLoadManager>
    {
        private const string HAS_LOCAL_DATA_KEY = "HAS_LOCAL_DATA_KEY";
        private Dictionary<Type, LocalSaveLoadable> registeredModules = new Dictionary<Type, LocalSaveLoadable>();
        private PipelineLocalSaveData pipelineLocalSaveData;
        private LocalSaveLoadConfiguration configuration;
        private bool isInitialized;

        public bool IsInitialized => isInitialized;

        public void RegisterModule(Type moduleType)
        {
            if (isInitialized == false)
            {
                return;
            }
            if (typeof(LocalSaveLoadable).IsAssignableFrom(moduleType) == false)
            {
                return;
            }
            if (registeredModules.ContainsKey(moduleType))
            {
                return;
            }
            registeredModules.Add(moduleType, Activator.CreateInstance(moduleType) as LocalSaveLoadable);
        }

        public void RegisterModule(Type moduleType, LocalSaveLoadable _object)
        {
            if(isInitialized == false) 
            {
                return;
            }
            if (typeof(LocalSaveLoadable).IsAssignableFrom(moduleType) == false)
            {
                return;
            }
            if (registeredModules.ContainsKey(moduleType))
            {
                return;
            }
            registeredModules.Add(moduleType, _object);
        }

        public static T Get<T>() where T : LocalSaveLoadable
        {
            if (Instance.isInitialized == false)
            {
                return default(T);
            }
            Type tType = typeof(T);
            if (Instance.registeredModules.ContainsKey(tType) == false)
            {
                return default(T);
            }
            LocalSaveLoadable val = null;
            Instance.registeredModules.TryGetValue(tType, out val);
            return (T)val;
        }

        private bool HasLocalData()
        {
            return PlayerPrefExtension.GetBool(HAS_LOCAL_DATA_KEY, false);
        }

        private void SetLocalDataSaved()
        {
            PlayerPrefExtension.SetBool(HAS_LOCAL_DATA_KEY, true);
        }

        public void Setup(LocalSaveLoadConfiguration config)
        {
            this.configuration = config;
            pipelineLocalSaveData = new PipelineLocalSaveData();
            pipelineLocalSaveData.Setup(config);
            isInitialized = true;
        }

        /// <summary>
        /// Call when already Setup and all modoles registered
        /// </summary>
        public void Install()
        {
            if (isInitialized == false)
            {
                return;
            }
            if (HasLocalData())
            {
                LoadData();
            }
            else // is first time open game
            {
                CreateData();
            }
            ConvertData();
            InitData();
            SaveData();
        }


        private void CreateData()
        {
            SetLocalDataSaved();
            foreach(var m in registeredModules)
            {
                m.Value.CreateData();
            }
            pipelineLocalSaveData.CreateData();
        }

        private void LoadData()
        {
            foreach (var m in registeredModules)
            {
                m.Value.LoadData();
            }
            pipelineLocalSaveData.LoadData();
        }

        
        private void ConvertData()
        {
            pipelineLocalSaveData.ConvertData();
        }

        private void InitData()
        {
            foreach (var m in registeredModules)
            {
                m.Value.InitData();
            }
            pipelineLocalSaveData.InitData();
        }

        private void SaveData()
        {
            foreach (var m in registeredModules)
            {
                m.Value.SaveData();
            }
            pipelineLocalSaveData.SaveData();
        }

        private void EaseData()
        {
            foreach (var m in registeredModules)
            {
                m.Value.EaseData();
            }
            pipelineLocalSaveData.EaseData();
        }

        public void EaseAllData()
        {
            EaseData();
            InitData();
            SaveData();
        }
    }
}
