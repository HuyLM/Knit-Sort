#if OSA_ENABLE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AtoGame.OtherModules.OptimizedScrollView
{
    public abstract class ViewHolderComponent<T> : MonoBehaviour where T : IScrollViewItemModel
    {
        public abstract void UpdateView(int index, T model);
    }
}
#endif