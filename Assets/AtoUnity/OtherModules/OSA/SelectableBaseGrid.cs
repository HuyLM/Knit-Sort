#if OSA_ENABLE
using System;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.OptimizedScrollView
{
    public abstract class SelectableBaseGrid<TModel, TDisplayer> : BaseGrid<TModel> where TModel : IScrollViewItemModel where TDisplayer : SelectableViewHolderComponent<TModel>
    {
        protected Action<TDisplayer> onSelected;

        protected override void OnCellViewsHolderCreated(GridItemViewHolder<TModel> cellVH, CellGroupViewsHolder<GridItemViewHolder<TModel>> cellGroup)
        {
            TDisplayer displayer = cellVH.GetViewComponent() as TDisplayer;
            displayer.SetOnSelected(OnSelectedDisplayer);
        }

        public SelectableBaseGrid<TModel, TDisplayer> SetOnSelected(Action<TDisplayer> onSelected)
        {
            this.onSelected = onSelected;
            return this;
        }

        protected void OnSelectedDisplayer(SelectableViewHolderComponent<TModel> displayer)
        {
            onSelected?.Invoke((TDisplayer)displayer);
        }
    }
}
#endif