using AtoGame.Base;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    [CreateAssetMenu(fileName = "ItemDatabaseEditor", menuName = "Data/OtherModules/Inventory/ItemDatabaseEditor")]
    public class ItemDatabaseEditor : SingletonScriptableObject<ItemDatabaseEditor>
    {
#if UNITY_EDITOR

        [SerializeField] private ItemDatabase itemDatabase;
        public override void OnAwake()
        {
            base.OnAwake();
            OnInitialize();
        }

        private Dictionary<int, ItemTypeName> itemTypeDictionary;

        [Button("Reload")]
        [ContextMenu("Reload")]
        public void OnInitialize()
        {
            itemTypeDictionary = new Dictionary<int, ItemTypeName>();
            foreach (ItemCollector collector in itemDatabase.Collectors)
            {
                AddCollectorToDictionary(collector, string.Empty);
            }
        }

        private void AddCollectorToDictionary(ItemCollector collector, string path)
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
            if (itemTypeDictionary.ContainsKey(item.Id))
            {
                return;
            }
            itemTypeDictionary.Add(item.Id, new ItemTypeName() { Item = item, NameType = path.ToString() });
        }

        public static int GetCount()
        {
            return Instance.itemTypeDictionary.Count;
        }

        public static IEnumerable<ItemTypeName> GetAllItem(string nameCollector)
        {
            foreach (ItemTypeName item in Instance.itemTypeDictionary.Values)
            {
                string path = item.NameType;
                string[] sub = path.Split('/');
                if (sub.Contains(nameCollector))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<ItemTypeName> GetAllItem()
        {
            foreach (ItemTypeName item in Instance.itemTypeDictionary.Values)
            {
                yield return item;
            }
        }

        [Button("Lock All Item")]
        private void LockAllItem()
        {
            foreach (var c in itemDatabase.Collectors)
            {
                c.LockAllItem();
            }
        }
#endif
    }

#if UNITY_EDITOR
    public class ItemTypeName
    {
        private ItemConfig item;
        private string nameType;

        public ItemConfig Item { get => item; set => item = value; }
        public string NameType { get => nameType; set => nameType = value; }
    }
#endif
}
