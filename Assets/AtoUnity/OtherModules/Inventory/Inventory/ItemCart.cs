using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    public class ItemCart
    {
        //public const string DEFAULT_TAG = "default_tag";
        public string Tag;
        public List<ItemData> Items = new List<ItemData>();
    }
}
