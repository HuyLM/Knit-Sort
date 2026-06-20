using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base.UI
{
    public abstract class SelectableDisplayerCollector<TModel, TDisplayer> : DisplayerCollector<TModel, TDisplayer> where TDisplayer : SelectableDisplayer<TModel>
    {
        protected Action<TDisplayer> onSelected;

        public override void SetupDisplayer(TDisplayer displayer, TModel item)
        {
            if (displayer == null)
            {
                return;
            }
            displayer.SetOnSelected(OnSelectedDisplayer);
            base.SetupDisplayer(displayer, item);
        }

        protected void OnSelectedDisplayer(SelectableDisplayer<TModel> displayer)
        {
            onSelected?.Invoke((TDisplayer)displayer);
        }

        public SelectableDisplayerCollector<TModel, TDisplayer> SetOnSelected(Action<TDisplayer> onSelected)
        {
            this.onSelected = onSelected;
            return this;
        }
    }
}
