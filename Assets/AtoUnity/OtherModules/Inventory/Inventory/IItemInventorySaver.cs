using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    public interface IItemInventorySaver
    {
        void AddOnInit(Action onCompleted);
        void Load(Action<bool, ItemData[], int[]> onLoaded);
        void PushSave(ItemData[] items, List<int> infiniteIds, Action<bool> onResult);
    }
}
