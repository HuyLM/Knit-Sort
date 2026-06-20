using UnityEngine.UI;

namespace AtoGame.OtherModules.DOTA
{
    public class HorizontalOrVerticalLayoutGroupSpacingGetSet : FloatGetSet {
        public HorizontalOrVerticalLayoutGroup target;

        public override float Get()
        {
            return target.spacing;
        }

        public override void Set(float value)
        {
            target.spacing = value;
        }
    }

   
}
