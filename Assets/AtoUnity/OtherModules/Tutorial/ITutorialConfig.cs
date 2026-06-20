using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Tutorial
{
    public interface ITutorialConfig
    {
        public void Init(Action onCompleted);
        TutorialData[] GetTutorialDatas();
        int[] GetEndTutorialKeys();
        TutorialData[] GetExtraTutorialDatas();
        bool EnableLog();
        bool EnableTutorial();
        bool EnableSkipAll();
    }
}
