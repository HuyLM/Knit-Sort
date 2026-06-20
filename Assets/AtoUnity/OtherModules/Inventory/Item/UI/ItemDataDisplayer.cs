using AtoGame.Base.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory.UI
{
    public class ItemDataDisplayer : Displayer<ItemData>
    {
        [SerializeField] private BaseItemDisplayer[] itemDisplayers;
        [SerializeField] private BaseItemDisplayer defaultDisplayer;

        public override void Show()
        {
            if (Model == null)
            {
                return;
            }
            bool isShowed = false;
            for (int i = 0; i < itemDisplayers.Length; ++i)
            {
                if (itemDisplayers[i].CheckConfigType(Model.ItemConfig) == true)
                {
                    itemDisplayers[i].gameObject.SetActive(true);
                    itemDisplayers[i].SetModel(Model);
                    itemDisplayers[i].Show();
                    isShowed = true;
                }
                else
                {
                    itemDisplayers[i].gameObject.SetActive(false);
                }
            }
            if (isShowed == false && defaultDisplayer != null)
            {
                defaultDisplayer.gameObject.SetActive(true);
                defaultDisplayer.SetModel(Model).Show();
            }
        }
    }
}
