using AtoGame.Base.UnityInspector.Editor;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Data/OtherModules/Inventory/ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        [Header("[Item]")]
        [SerializeField, SpriteField] private Sprite icon;
        [SerializeField, SpriteField] private Sprite sprite;
#if UNITY_EDITOR
        [SerializeField] public bool lockID;
        [SerializeField, Label("Name For Editor")] private string displayName;
#endif
        [DisableIf("lockID")]
        [SerializeField, Range(1, 999999)] private int id = 1;
        [SerializeField] private string nameKey;
        [SerializeField, TextArea(3, 5)] private string descriptionKey;
        [SerializeField] private ItemData sellPrice;

        #region Properties
        public int Id
        {
            get => id;
#if UNITY_EDITOR
            set
            {
                if (lockID)
                {
                    Debug.LogError("Can't change id. " + name + " is locked");
                    return;
                }
                id = value;
            }
#endif
        }
#if UNITY_EDITOR
        public virtual string DisplayName
        {
            get => displayName;
            set => displayName = value;
        }
#endif

        public virtual string NameKey
        {
            get => nameKey;
            set => nameKey = value;
        }

        public virtual Sprite Icon
        {
            get => icon;
            set => icon = value;
        }

        public virtual Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }

        public string DescriptionKey
        {
            get => descriptionKey;
            set => descriptionKey = value;
        }

        public ItemData SellPrice
        {
            get => sellPrice;
            set => sellPrice = value;
        }
        #endregion


        public virtual void Claim(long amount, string source, params string[] tags)
        {
            ItemInventoryController.Instance.Add(Id, amount, source, tags);
        }

        public virtual void Remove(long amount, string source)
        {
            ItemInventoryController.Instance.Remove(Id, amount, source);
        }

        public virtual long GetAvaliable()
        {
            return ItemInventoryController.Instance.ItemInventory.GetItem(Id).Amount;
        }

        public virtual bool CanClaim(long amount)
        {
            return true;
        }
    }
}
