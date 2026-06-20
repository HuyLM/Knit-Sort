using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base.UI
{
    public class DisplayerCollector<TModel, TDisplayer> : Collector<TModel> where TDisplayer : Displayer<TModel>
    {
        [SerializeField] private TDisplayer prefab;
        [SerializeField] private Transform layout;

        protected readonly List<TDisplayer> displayers = new List<TDisplayer>();

        public override int DisplayerCount => displayers.Count;

        public TDisplayer GetDisplayer(int index)
        {
            if (index < 0 || index >= DisplayerCount)
            {
                return null;
            }
            return displayers[index];
        }

        public TDisplayer GetDisplayer(TModel data)
        {
            if (displayers == null)
            {
                return null;
            }
            for (int i = 0; i < DisplayerCount; ++i)
            {
                if (displayers[i].Model.Equals(data))
                {
                    return displayers[i];
                }
            }
            return null;
        }

        public IEnumerable<TDisplayer> GetDisplayers()
        {
            if (Items != null)
            {
                for (int i = 0; i < DisplayerCount; ++i)
                {
                    yield return displayers[i];
                }
            }
        }

        public override void Show()
        {
            for (int i = 0; i < Capacity; i++)
            {
                if (DisplayerCount == i)
                {
                    AddDisplayer(CreateDisplayer());
                }

                TDisplayer displayer = GetDisplayer(i);
                if (displayer)
                {
                    displayer.gameObject.SetActive(true);
                    SetupDisplayer(displayer, GetItem(i));
                }
            }

            for (int i = Capacity; i < DisplayerCount; i++)
            {
                TDisplayer displayer = GetDisplayer(i);
                if (displayer)
                {
                    displayer.gameObject.SetActive(false);
                }
            }
        }

        public virtual void AddDisplayer(TDisplayer displayer)
        {
            displayers.Add(displayer);
        }

        public virtual void SetupDisplayer(TDisplayer displayer, TModel item)
        {
            if (displayer == null)
            {
                return;
            }
            displayer.SetModel(item).Show();
        }

        protected TDisplayer CreateDisplayer()
        {
            TDisplayer viewItem = Instantiate(prefab, layout);
            return viewItem;
        }
    }
}
