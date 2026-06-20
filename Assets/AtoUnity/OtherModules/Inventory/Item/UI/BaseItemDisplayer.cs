using AtoGame.Base.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory.UI
{
    public abstract class BaseItemDisplayer : Displayer<ItemData>
    {
        public abstract bool CheckConfigType(ItemConfig itemConfig);
    }

    public abstract class BaseItemDisplayer<T> : BaseItemDisplayer where T : ItemConfig
    {
        public override bool CheckConfigType(ItemConfig itemConfig)
        {
            return typeof(T) == itemConfig.GetType();
        }
    }
}
