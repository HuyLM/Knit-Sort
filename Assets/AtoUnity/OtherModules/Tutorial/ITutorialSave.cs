using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Tutorial
{
    public interface ITutorialSave
    {
        void Init(Action onCompleted);
        void Load(Action<bool> onLoaded);
        void Save(int[] keys, Action<bool> onResult);
        void PushSave(Action<bool> onResult);
        bool IsTutorialCompleted();
        void SetTutorialCompleted();
        bool CheckHasKey(int key);
    }

}
