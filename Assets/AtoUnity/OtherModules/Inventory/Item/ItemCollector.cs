using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    [CreateAssetMenu(fileName = "ItemCollector", menuName = "Data/OtherModules/Inventory/ItemCollector")]
    public class ItemCollector : BaseItemCollector
    {
        [SerializeField] private List<ItemConfig> items;

        public override List<ItemConfig> GetItems()
        {
            return items;
        }
    }
}
