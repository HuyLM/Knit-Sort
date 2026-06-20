using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.LocalSaveLoad
{
    public interface LocalSaveLoadable 
    {
        /// <summary>
        /// Call when create new data
        /// </summary>
        void CreateData();
        /// <summary>
        /// Call when load saved data
        /// </summary>
        void LoadData();
        /// <summary>
        /// Call after create or load data
        /// </summary>
        void InitData();
        /// <summary>
        /// Call when need save data
        /// </summary>
        void SaveData();
        /// <summary>
        /// Call when need ease data
        /// </summary>
        void EaseData();
    }
}
