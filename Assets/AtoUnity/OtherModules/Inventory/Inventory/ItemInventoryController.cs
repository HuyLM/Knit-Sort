using AtoGame.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    public class ItemInventoryController : AtoGame.Base.Singleton<ItemInventoryController>
    {
        private ItemDatabase itemDatabase;
        private ItemInventory itemInventory;
        private IItemInventorySaver inventorySaver;

        private bool isInitialized;
        private Dictionary<string, ItemCart> carts = new Dictionary<string, ItemCart>();

        public ItemDatabase ItemDatabase { get => itemDatabase; }
        public ItemInventory ItemInventory { get => itemInventory; }
        public IItemInventorySaver Saver { get => inventorySaver; }

        protected override void Initialize()
        {
            base.Initialize();
            isInitialized = false;
        }

        public void Init(ItemDatabase itemDatabase, IItemInventorySaver inventorySaver)
        {
            isInitialized = false;
            this.itemDatabase = itemDatabase;
            this.inventorySaver = inventorySaver;
            this.itemInventory = new ItemInventory();

            itemDatabase.OnInitialize();
            if(inventorySaver != null)
            {
                inventorySaver.AddOnInit(() => {
                    inventorySaver.Load((result, items, infiniteIds) => {
                        if(result == true)
                        {
                            itemInventory.Load(items, infiniteIds);
                        }
                        isInitialized = true;
                    });
                });
            }
        }

        public void Add(int id, long amount, string source, params string[] tags)
        {
            if(isInitialized == false)
            {
                Log($"Can't Add Func, because it is not initialized");
                return;
            }
            if(id == ItemDatabase.NoneId)
            {
                return;
            }
            AddCart(id, amount, tags);
            ItemInventory.Add(id, amount, source);
        }

        public void Remove(string source, params ItemData[] items)
        {
            if(isInitialized == false)
            {
                Log($"Can't Remove(params) Func, because it is not initialized");
                return;
            }
            ItemInventory.Remove(source, items);
        }

        public void Remove(int id, long amount, string source)
        {
            if(isInitialized == false)
            {
                Log($"Can't Remove Func, because it is not initialized");
                return;
            }
            ItemInventory.Remove(id, amount, source);
        }

        public void Save(Action<bool> onSaveResult = null)
        {
            if(isInitialized == false || itemInventory == null)
            {
                onSaveResult?.Invoke(false);
                return;
            }
            itemInventory.Save((doSave, items, infiniteIds) => {
                if(doSave)
                {
                    Saver.PushSave(items, infiniteIds, (saveSuccess) => {
                        onSaveResult?.Invoke(saveSuccess);
                    });
                }
                else
                {
                    onSaveResult?.Invoke(true);
                }
            });
        }

        #region Cart

        public void StartUseCart(string tag)
        {
            if(isInitialized == false)
            {
                Log($"Can't StartUseCart Func, because it is not initialized");
                return;
            }
            if(carts.ContainsKey(tag))
            {
                ItemCart itemCart;
                carts.TryGetValue(tag, out itemCart);
                if(itemCart != null)
                {
                    if(itemCart.Items != null)
                    {
                        itemCart.Items.Clear();
                    }
                    else
                    {
                        itemCart.Items = new List<ItemData>();
                    }
                }
                else
                {
                    carts[tag] = new ItemCart() { Tag = tag };
                }
            }
            else
            {
                carts.Add(tag, new ItemCart() { Tag = tag });
            }
        }

        public List<ItemData> GetCart(string tag, bool isStackCart = true)
        {
            if(isInitialized == false)
            {
                Log($"Can't GetCart Func, because it is not initialized");
                return null;
            }
            if(carts.ContainsKey(tag))
            {
                ItemCart itemCart;
                carts.TryGetValue(tag, out itemCart);
                if(itemCart != null)
                {
                    List<ItemData> result = new List<ItemData>();
                    if(isStackCart)
                    {
                        for(int i = 0; i < itemCart.Items.Count; ++i)
                        {
                            ItemData itemInList = GetItemWithIdInList(itemCart.Items[i].Id, result);
                            if(itemInList.IsEmpty)
                            {
                                result.Add(new ItemData(itemCart.Items[i].Id, itemCart.Items[i].Amount));
                            }
                            else
                            {
                                itemInList.Stack(itemCart.Items[i].Amount);
                            }
                        }
                    }
                    else
                    {
                        result = itemCart.Items.ToList();
                    }
                    // defalt not stack
                    return result;
                }
                Log($"Can't GetCart Func, because cart with {tag} is not data");
                return null;
            }
            Log($"Can't GetCart Func, because Carts not contain {tag}");
            return null;
        }

        private ItemData GetItemWithIdInList(int id, List<ItemData> list)
        {
            for(int i = 0; i < list.Count; ++i)
            {
                if(list[i].Id == id)
                {
                    return list[i];
                }
            }
            return ItemData.Empty;
        }

        public void ClearCart(params string[] tags)
        {
            if(isInitialized == false)
            {
                Log($"Can't ClearCart Func, because it is not initialized");
                return;
            }
            for(int i = 0; i < tags.Length; ++i)
            {
                if(carts.ContainsKey(tags[i]))
                {
                    ItemCart itemCart;
                    carts.TryGetValue(tags[i], out itemCart);
                    if(itemCart != null)
                    {
                        itemCart.Items.Clear();
                    }
                }
            }
        }

        public void EndUseCart(params string[] tags)
        {
            if(isInitialized == false)
            {
                Log($"Can't EndUseCart Func, because it is not initialized");
                return;
            }
            for(int i = 0; i < tags.Length; ++i)
            {
                if(carts.ContainsKey(tags[i]))
                {
                    carts.Remove(tags[i]);
                }
            }
        }

        public void AddCart(string[] tags, params ItemData[] items)
        {
            if(isInitialized == false)
            {
                Log($"Can't AddCart(params) Func, because it is not initialized");
                return;
            }
            if(items != null && tags != null)
            {
                foreach(var tag in tags)
                {
                    if(carts.ContainsKey(tag))
                    {
                        ItemCart itemCart;
                        carts.TryGetValue(tag, out itemCart);
                        if(itemCart != null)
                        {
                            itemCart.Items.AddRange(items);
                        }
                    }
                }
            }
        }

        public void AddCart(int id, long amount, params string[] tags)
        {
            if(isInitialized == false)
            {
                Log($"Can't AddCart Func, because it is not initialized");
                return;
            }
            if(tags != null)
            {
                foreach(var tag in tags)
                {
                    if(carts.ContainsKey(tag))
                    {
                        ItemCart itemCart;
                        carts.TryGetValue(tag, out itemCart);
                        if(itemCart != null)
                        {
                            itemCart.Items.Add(new ItemData(id, amount));
                        }
                    }
                }
            }
        }

        public void RemoveCart(ItemData item, params string[] tags)
        {
            if(isInitialized == false)
            {
                Log($"Can't RemoveCart Func, because it is not initialized");
                return;
            }
            if(tags != null)
            {
                foreach(var tag in tags)
                {
                    if(carts.ContainsKey(tag))
                    {
                        ItemCart itemCart;
                        carts.TryGetValue(tag, out itemCart);
                        if(itemCart != null)
                        {
                            itemCart.Items.Remove(item);
                        }
                    }
                }
            }
        }

        #endregion

        public void Log(string message)
        {
            if(!isInitialized)
            {
                return;
            }
            if(ItemDatabase.EnableLog)
            {
                Debug.Log("[Inventory] " + message);
            }
        }
    }
}
