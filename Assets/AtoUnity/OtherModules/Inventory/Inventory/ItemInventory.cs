using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    public class ItemInventory
    {
        public const long INFINITE_AMOUNT = long.MaxValue;
        private readonly Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();
        private List<int> iii = new List<int>();

        private bool isDirty;

        public List<int> InfiniteItemIds { get => iii; }

        public virtual void Add(int id, long amount, string source)
        {
            if (ItemInventoryController.Instance.ItemDatabase.Constains(id))
            {
                if (itemDictionary.ContainsKey(id))
                {
                    itemDictionary[id].Stack(amount);
                    isDirty = true;
                }
                else
                {
                    itemDictionary.Add(id, new ItemData(id, amount));
                    isDirty = true;
                }
                AtoGame.Base.EventDispatcher.Instance.Dispatch(new EventKey.OnAddItemInventory() {
                    id = id,
                    name = itemDictionary[id].NameKey.ToLower(),
                    value = amount,
                    balance = itemDictionary[id].Amount,
                    source = source,
                });
            }

        }

        public virtual void Remove(string source, params ItemData[] items)
        {
            foreach (ItemData item in items)
            {
                Remove(item.Id, item.Amount, source);
            }
        }

        public virtual void Remove(int id, long amount, string source)
        {
            if (itemDictionary.ContainsKey(id))
            {
                itemDictionary[id].Destack(amount);
                isDirty = true;
            }

            AtoGame.Base.EventDispatcher.Instance.Dispatch(new EventKey.OnRemoveItemInventory()
            {
                id = id,
                name = itemDictionary[id].NameKey.ToLower(),
                value = amount,
                balance = itemDictionary[id].Amount,
                source = source,
            });
        }

        private bool IsInfinite(int id)
        {
            if (InfiniteItemIds != null)
            {
                return InfiniteItemIds.Contains(id);
            }
            return false;
        }

        public virtual ItemData GetItem(int id)
        {
            if (IsInfinite(id))
            {
                return new ItemData(id, INFINITE_AMOUNT);
            }
            if (itemDictionary.TryGetValue(id, out ItemData item))
            {
                return item;
            }
            return new ItemData(id, 0);
        }

        public void AddInfiniteItem(int id, string source)
        {
            if (itemDictionary.ContainsKey(id))
            {
                itemDictionary.Remove(id);
                isDirty = true;
            }
            if (!InfiniteItemIds.Contains(id))
            {
                InfiniteItemIds.Add(id);
                isDirty = true;
            }
            if(isDirty)
            {
                AtoGame.Base.EventDispatcher.Instance.Dispatch(new EventKey.OnAddItemInventory()
                {
                    id = id,
                    name = itemDictionary[id].NameKey.ToLower(),
                    value = INFINITE_AMOUNT,
                    balance = INFINITE_AMOUNT,
                    source = source,
                });
            }
        }

        public void RemoveInfiniteItem(int id, string source)
        {
            if (InfiniteItemIds != null)
            {
                if (InfiniteItemIds.Contains(id))
                {
                    InfiniteItemIds.Remove(id);
                    isDirty = true;

                    AtoGame.Base.EventDispatcher.Instance.Dispatch(new EventKey.OnRemoveItemInventory()
                    {
                        id = id,
                        name = itemDictionary[id].NameKey.ToLower(),
                        value = INFINITE_AMOUNT,
                        balance = 0,
                        source = source,
                    });
                }
            }
        }

        public void PushEvent(NotifyType notifyType = NotifyType.InventoryChanged)
        {
            if (notifyType == NotifyType.InventoryChanged)
            {
                AtoGame.Base.EventDispatcher.Instance.Dispatch(new EventKey.OnItemInventoryChanged());
            }
        }

        public void Load(ItemData[] items, int[] infiniteItemIds)
        {
            // Items
            if (items != null && items.Length > 0)
            {
                foreach (ItemData item in items)
                {
                    long amount = item.Amount;
                    if (itemDictionary.ContainsKey(item.Id))
                    {
                        itemDictionary[item.Id].Stack(amount);
                    }
                    else
                    {
                        itemDictionary.Add(item.Id, new ItemData(item.Id, amount));
                    }
                }
            }
            else
            {
                if(itemDictionary != null)
                {
                    itemDictionary.Clear();
                }
            }

            // Infinite Items
            if (infiniteItemIds != null && infiniteItemIds.Length > 0)
            {
                this.iii = new List<int>();
                for (int i = 0; i < infiniteItemIds.Length; ++i)
                {
                    iii.Add(infiniteItemIds[i]);
                }
            }
            else
            {
                this.iii = null;
            }
            isDirty = false;
        }

        public void Save(Action<bool, ItemData[], List<int>> onSave)
        {
            if (isDirty == false)
            {
                onSave?.Invoke(false, null, null);
                return;
            }
            isDirty = false;
            ItemData[] items = null;
            if (itemDictionary.Count > 0)
            {
                items = new ItemData[itemDictionary.Count];
                int index = 0;
                foreach (var i in itemDictionary.Values)
                {
                    items[index] = i;
                    index++;
                }
            }
            onSave?.Invoke(true, items, InfiniteItemIds);
        }
    }

    public enum NotifyType
    {
        Nothing, InventoryChanged
    }
}
