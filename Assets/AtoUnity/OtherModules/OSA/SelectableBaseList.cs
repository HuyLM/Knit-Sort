#if OSA_ENABLE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.OptimizedScrollView
{
    public abstract class SelectableBaseList<TModel, TDisplayer> : BaseList<TModel> where TModel : IScrollViewItemModel where TDisplayer : SelectableViewHolderComponent<TModel>
    {
        protected Action<TDisplayer> onSelected;

        protected override ListItemViewHolder<TModel> CreateViewsHolder(int itemIndex)
        {
            var newHolder = base.CreateViewsHolder(itemIndex);
            var displayer = newHolder.GetViewComponent() as TDisplayer;
            displayer.SetOnSelected(OnSelectedDisplayer);
            return newHolder;
        }

        public SelectableBaseList<TModel, TDisplayer> SetOnSelected(Action<TDisplayer> onSelected)
        {
            this.onSelected = onSelected;
            return this;
        }

        protected void OnSelectedDisplayer(SelectableViewHolderComponent<TModel> displayer)
        {
            onSelected?.Invoke((TDisplayer)displayer);
        }

        public TDisplayer GetDisplayer(TModel model)
        {
            int index = 0;
            var visibleCellCount = Data;
            for(int i = 0; i < visibleCellCount.Count; ++i)
            {
                if(visibleCellCount[i].Equals(model))
                {
                    index = i;
                }
            }
            return this.GetItemViewsHolder(index).GetViewComponent() as TDisplayer;
        }
    }
}
#endif