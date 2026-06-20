using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    public partial class EventKey
    {
        public struct OnItemInventoryChanged : Base.IEventParams
        {
        }

        public struct OnAddItemInventory: Base.IEventParams
        {
            public int id;
            public string name;
            public long value;
            public long balance;
            public string source;
        }

        public struct OnRemoveItemInventory : Base.IEventParams
        {
            public int id;
            public string name;
            public long value;
            public long balance;
            public string source;
        }
    }
}
