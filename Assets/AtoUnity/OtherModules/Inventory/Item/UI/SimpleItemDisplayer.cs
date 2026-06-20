using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtoGame.OtherModules.Inventory.UI
{
    public class SimpleItemDisplayer : BaseItemDisplayer
    {
        [SerializeField] private Image imgIcon;
        [SerializeField] private TextMeshProUGUI txtAmount;
        public override bool CheckConfigType(ItemConfig itemConfig)
        {
            return true;
        }

        public override void Show()
        {
            if(Model == null)
            {
                return;
            }
            if(imgIcon != null)
            {
                imgIcon.sprite = Model.Icon;
            }
            if(txtAmount != null)
            {
                txtAmount.text = Model.Amount.ToString();
            }
        }
    }
}
