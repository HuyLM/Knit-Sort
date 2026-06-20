/*
 * * * * This bare-bones script was auto-generated * * * *
 * The code commented with "/ * * /" demonstrates how data is retrieved and passed to the adapter, plus other common commands. You can remove/replace it once you've got the idea
 * Complete it according to your specific use-case
 * Consult the Example scripts if you get stuck, as they provide solutions to most common scenarios
 * 
 * Main terms to understand:
 *		Model = class that contains the data associated with an item (title, content, icon etc.)
 *		Views Holder = class that contains references to your views (Text, Image, MonoBehavior, etc.)
 * 
 * Default expected UI hiererchy:
 *	  ...
 *		-Canvas
 *		  ...
 *			-MyScrollViewAdapter
 *				-Viewport
 *					-Content
 *				-Scrollbar (Optional)
 *				-ItemPrefab (Optional)
 * 
 * Note: If using Visual Studio and opening generated scripts for the first time, sometimes Intellisense (autocompletion)
 * won't work. This is a well-known bug and the solution is here: https://developercommunity.visualstudio.com/content/problem/130597/unity-intellisense-not-working-after-creating-new-1.html (or google "unity intellisense not working new script")
 * 
 * 
 * Please read the manual under "Assets/OSA/Docs", as it contains everything you need to know in order to get started, including FAQ
 */
#if OSA_ENABLE

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using Com.ForbiddenByte.OSA.DataHelpers;

// The date was temporarily included in the namespace to prevent duplicate class names
// You should modify the namespace to your own or - if you're sure there will be no conflicts - remove it altogether
namespace AtoGame.OtherModules.OptimizedScrollView
{
    // There is 1 important callback you need to implement: UpdateCellViewsHolder()
    // See explanations below
    public class BaseGrid<T> : GridAdapter<GridParams, GridItemViewHolder<T>> where T : IScrollViewItemModel
    {
        // Helper that stores data and notifies the adapter when items count changes
        // Can be iterated and can also have its elements accessed by the [] operator
        public SimpleDataHelper<T> Data { get; private set; }


        #region OSA implementation
        protected override void Start()
        {
            if (Data == null)
            {
                Data = new SimpleDataHelper<T>(this);

                // Calling this initializes internal data and prepares the adapter to handle item count changes
                base.Start();
            }

            // Retrieve the models from your data source and set the items count
            /*
			RetrieveDataAndUpdate(1500);
			*/
        }

        // This is called anytime a previously invisible item become visible, or after it's created, 
        // or when anything that requires a refresh happens
        // Here you bind the data from the model to the item's views
        // *For the method's full description check the base implementation
        protected override void UpdateCellViewsHolder(GridItemViewHolder<T> newOrRecycled)
        {
            // In this callback, "newOrRecycled.ItemIndex" is guaranteed to always reflect the
            // index of item that should be represented by this views holder. You'll use this index
            // to retrieve the model from your data set
            T model = Data[newOrRecycled.ItemIndex];

            newOrRecycled.UpdateView(model);
        }

        // This is the best place to clear an item's views in order to prepare it from being recycled, but this is not always needed, 
        // especially if the views' values are being overwritten anyway. Instead, this can be used to, for example, cancel an image 
        // download request, if it's still in progress when the item goes out of the viewport.
        // <newItemIndex> will be non-negative if this item will be recycled as opposed to just being disabled
        // *For the method's full description check the base implementation
        /*
		protected override void OnBeforeRecycleOrDisableCellViewsHolder(MyGridItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			base.OnBeforeRecycleOrDisableCellViewsHolder(inRecycleBinOrVisible, newItemIndex);
		}
		*/
        #endregion

        // These are common data manipulation methods
        // The list containing the models is managed by you. The adapter only manages the items' sizes and the count
        // The adapter needs to be notified of any change that occurs in the data list. 
        // For GridAdapters, only Refresh and ResetItems work for now
        #region data manipulation
        public void AddItemsAt(int index, IList<T> items)
        {
            //Commented: this only works with Lists. ATM, Insert for Grids only works by manually changing the list and calling NotifyListChangedExternally() after
            //Data.InsertItems(index, items);
            Data.List.InsertRange(index, items);
            Data.NotifyListChangedExternally();
        }

        public void RemoveItemsFrom(int index, int count)
        {
            //Commented: this only works with Lists. ATM, Remove for Grids only works by manually changing the list and calling NotifyListChangedExternally() after
            //Data.RemoveRange(index, count);
            Data.List.RemoveRange(index, count);
            Data.NotifyListChangedExternally();
        }

        public void SetItems(IList<T> items)
        {
            if (Data == null)
            {
                Data = new SimpleDataHelper<T>(this);
                base.Start();
            }
            Data.ResetItems(items);
        }
        #endregion


        // Here, we're requesting <count> items from the data source
        void RetrieveDataAndUpdate(int count)
        {
            StartCoroutine(FetchMoreItemsFromDataSourceAndUpdate(count));
        }

        // Retrieving <count> models from the data source and calling OnDataRetrieved after.
        // In a real case scenario, you'd query your server, your database or whatever is your data source and call OnDataRetrieved after
        IEnumerator FetchMoreItemsFromDataSourceAndUpdate(int count)
        {
            // Simulating data retrieving delay
            yield return new WaitForSeconds(.5f);

            var newItems = new T[count];

            // Retrieve your data here
            /*
			for (int i = 0; i < count; ++i)
			{
				var model = new MyGridItemModel()
				{
					title = "Random item ",
					color = new Color(
								UnityEngine.Random.Range(0f, 1f),
								UnityEngine.Random.Range(0f, 1f),
								UnityEngine.Random.Range(0f, 1f),
								UnityEngine.Random.Range(0f, 1f)
							)
				};
				newItems[i] = model;
			}
			*/

            OnDataRetrieved(newItems);
        }

        void OnDataRetrieved(T[] newItems)
        {
            //Commented: this only works with Lists. ATM, Insert for Grids only works by manually changing the list and calling NotifyListChangedExternally() after
            // Data.InsertItemsAtEnd(newItems);

            Data.List.AddRange(newItems);
            Data.NotifyListChangedExternally();
        }

        protected override void OnDisable()
        {
            if (Data != null && Data.Count > 0)
            {
                ScrollTo(0, 0.5f, 0.5f);
                StopMovement();
            }
            base.OnDisable();
        }

        public void StopAllMovement(bool moveToTop = false)
        {
            StopMovement();
            if (moveToTop)
            {
                if (Data != null && Data.Count > 0)
                {
                    ScrollTo(0, 0.5f, 0.5f);
                }
            }
        }
    }

    // This class keeps references to an item's views.
    // Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
    // The cell views holder should have a single child (usually named "Views"), which contains the actual 
    // UI elements. A cell's root is never disabled - when a cell is removed, only its "views" GameObject will be disabled
    public class GridItemViewHolder<T> : CellViewsHolder where T : IScrollViewItemModel
    {
        ViewHolderComponent<T> viewComponent;
        public override void CollectViews()
        {
            base.CollectViews();
            viewComponent = root.GetComponent<ViewHolderComponent<T>>();

        }
        public virtual void UpdateView(T model)
        {
            viewComponent.UpdateView(ItemIndex, model);
        }

        public ViewHolderComponent<T> GetViewComponent()
        {
            return viewComponent;
        }
    }
}
#endif