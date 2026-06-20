using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    [System.Serializable]
    public class ItemData
    {
        public static ItemData Empty = new ItemData(ItemDatabase.NoneId, 0);
        [SerializeField, ItemField, Label("ID")] protected int i;
        [SerializeField, Label("Amount")] protected long a;

        private ItemConfig item;
        public ItemConfig ItemConfig
        {
            get
            {
                if (item == null)
                {
                    ItemInventoryController.Instance.ItemDatabase.TryGetItem(Id, out item);
                }
                return item;
            }
        }

        public long Amount
        {
            set
            {
                a = value;
            }

            get
            {
                return a;
            }
        }

        public int Id => i;

#if UNITY_EDITOR
        public string DisplayName
        {
            get => ItemConfig?.DisplayName;
        }
#endif

        public string NameKey
        {
            get => ItemConfig?.NameKey;
        }

        public virtual Sprite Icon
        {
            get => ItemConfig?.Icon;
        }

        public virtual Sprite Sprite
        {
            get => ItemConfig?.Sprite;
        }

        public string DescriptionKey
        {
            get => ItemConfig?.DescriptionKey;
        }

        public bool IsEmpty => Id == ItemDatabase.NoneId || a <= 0;
        public ItemData(int itemId, long amount)
        {
            this.i = itemId;
            this.a = amount;
        }

        public override string ToString()
        {
#if UNITY_EDITOR
            return $"{DisplayName} - {Amount}";
#else
            return $"{NameKey} - {Amount}";
#endif
        }

        public void Stack(long amount)
        {
            this.a += amount;
        }

        public void Destack(long amount)
        {
            this.a -= amount;
            if (this.a < 0)
            {
                this.a = 0;
            }
        }

        public void Claim(string source, params string[] tags)
        {
            ItemConfig item = ItemConfig;
            if (item != null)
            {
                item.Claim(Amount, source, tags);
            }
        }

        public void Remove(string source)
        {
            ItemConfig item = ItemConfig;
            if (item != null)
            {
                item.Remove(Amount, source);
            }
        }

        public bool CanClaim()
        {
            ItemConfig item = ItemConfig;
            if (item != null)
            {
                return item.CanClaim(Amount);
            }
            return false;
        }
    }
}
