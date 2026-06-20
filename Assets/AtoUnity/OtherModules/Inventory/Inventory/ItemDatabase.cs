using AtoGame.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Data/OtherModules/Inventory/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public const int NoneId = 0;
        [SerializeField] private bool enableLog;
        [SerializeField] private BaseItemCollector[] collectors;
        private Dictionary<int, ItemConfig> itemDictionary;

        public bool EnableLog { get => enableLog; }
        public BaseItemCollector[] Collectors { get => collectors; }

        public void OnInitialize()
        {
            itemDictionary = new Dictionary<int, ItemConfig>();
            foreach (BaseItemCollector collector in collectors)
            {
                AddCollectorToDictionary(collector, string.Empty);
            }
        }

        private void AddCollectorToDictionary(BaseItemCollector collector, string path)
        {
            path += collector.NameCollector + "/";
            foreach (var c in collector.Collectors)
            {
                AddCollectorToDictionary(c, path);
            }
            var items = collector.GetItems();
            foreach (var i in items)
            {
                AddItemToDictionary(i, path);
            }
        }

        private void AddItemToDictionary(ItemConfig item, string path)
        {
            if (itemDictionary.ContainsKey(item.Id))
            {
                return;
            }
            itemDictionary.Add(item.Id, item);
        }

        public bool TryGetItem(int id, out ItemConfig item)
        {
            if (itemDictionary.TryGetValue(id, out ItemConfig i))
            {
                item = i;
                return true;
            }
            item = null;
            return false;
        }

        public bool Constains(int id)
        {
            return itemDictionary.ContainsKey(id);
        }
    }
}
