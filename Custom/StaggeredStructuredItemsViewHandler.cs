using Microsoft.Maui.Controls.Handlers.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Custom
{
    public class StaggeredStructuredItemsViewHandler : StructuredItemsViewHandler<CollectionView>
    {
        public StaggeredStructuredItemsViewHandler() : base(StaggeredStructuredItemsViewMapper)
        {

        }
        public StaggeredStructuredItemsViewHandler(PropertyMapper? mapper = null) : base(mapper ?? StaggeredStructuredItemsViewMapper)
        {

        }

        public static PropertyMapper<CollectionView, StructuredItemsViewHandler<CollectionView>> StaggeredStructuredItemsViewMapper = new(StructuredItemsViewMapper)
        {
            [StructuredItemsView.ItemsLayoutProperty.PropertyName] = MapItemsLayout
        };

#if ANDROID
        private static void MapItemsLayout(StructuredItemsViewHandler<CollectionView> handler, CollectionView view)
        {
            var platformView = handler.PlatformView as MauiRecyclerView<CollectionView, ItemsViewAdapter<CollectionView, IItemsViewSource>, IItemsViewSource>;
            switch (view.ItemsLayout)
            {
                case StaggeredItemsLayout staggeredItemsLayout:
                    platformView?.UpdateAdapter();
                    platformView?.SetLayoutManager(
                        new AndroidX.RecyclerView.Widget.StaggeredGridLayoutManager(
                            staggeredItemsLayout.Span,
                            staggeredItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal ? AndroidX.RecyclerView.Widget.StaggeredGridLayoutManager.Horizontal : AndroidX.RecyclerView.Widget.StaggeredGridLayoutManager.Vertical));
                    break;
                default:
                    platformView?.UpdateLayoutManager();
                    break;
            }
        }
#endif

#if IOS 
		protected override ItemsViewLayout SelectLayout()
		{
			var itemsLayout = ItemsView.ItemsLayout;

			if (itemsLayout is StaggeredItemsLayout staggeredItemsLayout)
			{
				return new StaggeredItemsViewLayout(staggeredItemsLayout, ItemsView.ItemSizingStrategy);
			}

			return base.SelectLayout();
		}
#endif

#if WINDOWS
		protected override Microsoft.UI.Xaml.Controls.ListViewBase SelectListViewBase()
		{
			return VirtualView.ItemsLayout switch
			{
				StaggeredItemsLayout staggeredItemsLayout => new Microsoft.UI.Xaml.Controls.GridView()
				{
					ItemsPanel = (Microsoft.UI.Xaml.Controls.ItemsPanelTemplate)Microsoft.UI.Xaml.Application.Current.Resources["StaggeredItemsPanel"]
				},
				_ => base.SelectListViewBase()
			};
		}
#endif
    }

    public class StaggeredItemsLayout(ItemsLayoutOrientation orientation) : ItemsLayout(orientation)
    {
        public static readonly BindableProperty SpanProperty = BindableProperty.Create(nameof(Span), typeof(int), typeof(StaggeredItemsLayout), default(int));

        public StaggeredItemsLayout() : this(ItemsLayoutOrientation.Vertical)
        {

        }

        public int Span
        {
            get => (int)GetValue(SpanProperty);
            set => SetValue(SpanProperty, value);
        }
    }
}
