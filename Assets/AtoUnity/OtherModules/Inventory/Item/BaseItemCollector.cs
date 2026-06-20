using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    public abstract class BaseItemCollector : ScriptableObject
    {
        [SerializeField] private string nameCollector = "other";
        [SerializeField] private List<ItemCollector> collectors;


        public virtual string NameCollector { get => nameCollector; }
        public virtual List<ItemCollector> Collectors => collectors;

        public abstract List<ItemConfig> GetItems();

#if UNITY_EDITOR
        public void LockAllItem()
        {
            foreach(var i in GetItems())
            {
                i.lockID = true;
                UnityEditor.EditorUtility.SetDirty(i);
            }
            foreach(var c in Collectors)
            {
                c.LockAllItem();
            }
        }
#endif

    }
}
