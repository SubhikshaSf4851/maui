#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls.Handlers.Items
{
	internal class ObservableItemsSource : IItemsViewSource, IObservableItemsViewSource
	{
		readonly IEnumerable _itemsSource;
		readonly BindableObject _container;
		readonly ICollectionChangedNotifier _notifier;
		readonly WeakNotifyCollectionChangedProxy _proxy = new();
		readonly NotifyCollectionChangedEventHandler _collectionChanged;
		readonly Dictionary<object, WeakNotifyPropertyChangedProxy> _itemPropertyProxies = new();
		readonly PropertyChangedEventHandler _itemPropertyChangedHandler;
		bool _disposed;

		~ObservableItemsSource()
		{
			_proxy.Unsubscribe();
			ClearItemSubscriptions();
		}

		public ObservableItemsSource(IEnumerable itemSource, BindableObject container, ICollectionChangedNotifier notifier)
		{
			_itemsSource = itemSource;
			_container = container;
			_notifier = notifier;
			_collectionChanged = CollectionChanged;
			_proxy.Subscribe((INotifyCollectionChanged)itemSource, _collectionChanged);

			_itemPropertyChangedHandler = OnItemPropertyChanged;
			SubscribeExistingItems();
		}


		internal event NotifyCollectionChangedEventHandler CollectionItemsSourceChanged;

		public int Count => ItemsCount() + (HasHeader ? 1 : 0) + (HasFooter ? 1 : 0);

		public bool HasHeader { get; set; }
		public bool HasFooter { get; set; }

		public bool ObserveChanges { get; set; } = true;

		public void Dispose()
		{
			Dispose(true);
		}

		public bool IsFooter(int index)
		{
			return HasFooter && index == Count - 1;
		}

		public bool IsHeader(int index)
		{
			return HasHeader && index == 0;
		}

		public int GetPosition(object item)
		{
			for (int n = 0; n < ItemsCount(); n++)
			{
				var elementByIndex = ElementAt(n);
				var isEqual = elementByIndex == item || (elementByIndex != null && item != null && elementByIndex.Equals(item));

				if (isEqual)
				{
					return AdjustPositionForHeader(n);
				}
			}

			return -1;
		}

		public object GetItem(int position)
		{
			return ElementAt(AdjustIndexForHeader(position));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				_proxy.Unsubscribe();
				ClearItemSubscriptions();
			}
		}

		int AdjustIndexForHeader(int index)
		{
			return index - (HasHeader ? 1 : 0);
		}

		int AdjustPositionForHeader(int position)
		{
			return position + (HasHeader ? 1 : 0);
		}

		void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (!ObserveChanges)
			{
				return;
			}

			_container.Dispatcher.DispatchIfRequired(() => CollectionChanged(args));
		}

		void CollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					Add(args);
					break;
				case NotifyCollectionChangedAction.Remove:
					Remove(args);
					break;
				case NotifyCollectionChangedAction.Replace:
					Replace(args);
					break;
				case NotifyCollectionChangedAction.Move:
					Move(args);
					break;
				case NotifyCollectionChangedAction.Reset:
					_notifier.NotifyDataSetChanged();
					ClearItemSubscriptions();
					SubscribeExistingItems();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			CollectionItemsSourceChanged?.Invoke(this, args);
		}

		void Move(NotifyCollectionChangedEventArgs args)
		{
			var count = args.NewItems.Count;

			if (count == 1)
			{
				// For a single item, we can use NotifyItemMoved and get the animation
				_notifier.NotifyItemMoved(this, AdjustPositionForHeader(args.OldStartingIndex), AdjustPositionForHeader(args.NewStartingIndex));
				return;
			}

			var start = AdjustPositionForHeader(Math.Min(args.OldStartingIndex, args.NewStartingIndex));
			var end = AdjustPositionForHeader(Math.Max(args.OldStartingIndex, args.NewStartingIndex) + count);
			_notifier.NotifyItemRangeChanged(this, start, end);
		}

		void Add(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : _itemsSource.IndexOf(args.NewItems[0]);
			startIndex = AdjustPositionForHeader(startIndex);
			var count = args.NewItems.Count;

			if (args.NewItems != null)
			{
				foreach (var item in args.NewItems)
				{
					SubscribeItem(item);
				}
			}

			if (count == 1)
			{
				_notifier.NotifyItemInserted(this, startIndex);
				return;
			}

			_notifier.NotifyItemRangeInserted(this, startIndex, count);
		}

		void Remove(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.OldStartingIndex;

			if (startIndex < 0)
			{
				// INCC implementation isn't giving us enough information to know where the removed items were in the
				// collection. So the best we can do is a NotifyDataSetChanged()
				_notifier.NotifyDataSetChanged();
				ClearItemSubscriptions();
				SubscribeExistingItems();
				return;
			}

			startIndex = AdjustPositionForHeader(startIndex);

			// If we have a start index, we can be more clever about removing the item(s) (and get the nifty animations)
			var count = args.OldItems.Count;

			if (args.OldItems != null)
			{
				foreach (var item in args.OldItems)
				{
					UnsubscribeItem(item);
				}
			}

			if (count == 1)
			{
				_notifier.NotifyItemRemoved(this, startIndex);
				return;
			}

			_notifier.NotifyItemRangeRemoved(this, startIndex, count);
		}

		void Replace(NotifyCollectionChangedEventArgs args)
		{
			var startIndex = args.NewStartingIndex > -1 ? args.NewStartingIndex : _itemsSource.IndexOf(args.NewItems[0]);
			startIndex = AdjustPositionForHeader(startIndex);
			var newCount = args.NewItems.Count;

			if (args.OldItems != null)
			{
				foreach (var item in args.OldItems)
				{
					UnsubscribeItem(item);
				}
			}

			if (args.NewItems != null)
			{
				foreach (var item in args.NewItems)
				{
					SubscribeItem(item);
				}
			}

			if (newCount == args.OldItems.Count)
			{
				// We are replacing one set of items with a set of equal size; we can do a simple item or range 
				// notification to the adapter
				if (newCount == 1)
				{
					_notifier.NotifyItemChanged(this, startIndex);
				}
				else
				{
					_notifier.NotifyItemRangeChanged(this, startIndex, newCount);
				}

				return;
			}

			// The original and replacement sets are of unequal size; this means that everything currently in view will 
			// have to be updated. So we just have to use NotifyDataSetChanged and let the RecyclerView update everything
			_notifier.NotifyDataSetChanged();
		}

		internal int ItemsCount()
		{
			if (_itemsSource is IList list)
				return list.Count;

			int count = 0;
			foreach (var item in _itemsSource)
				count++;
			return count;
		}

		internal object ElementAt(int index)
		{
			if (_itemsSource is IList list)
				return list[index];

			int count = 0;
			foreach (var item in _itemsSource)
			{
				if (count == index)
					return item;
				count++;
			}

			return -1;
		}

		void SubscribeExistingItems()
		{
			foreach (var item in EnumerateItems())
			{
				SubscribeItem(item);
			}
		}

		IEnumerable EnumerateItems()
		{
			if (_itemsSource is IList list)
			{
				for (int i = 0; i < list.Count; i++)
					yield return list[i];
			}
			else
			{
				foreach (var item in _itemsSource)
					yield return item;
			}
		}

		void SubscribeItem(object item)
		{
			if (item is INotifyPropertyChanged inpc && !_itemPropertyProxies.ContainsKey(item))
			{
				var proxy = new WeakNotifyPropertyChangedProxy(inpc, _itemPropertyChangedHandler);
				_itemPropertyProxies[item] = proxy;
			}
		}

		void UnsubscribeItem(object item)
		{
			if (item != null && _itemPropertyProxies.TryGetValue(item, out var proxy))
			{
				proxy.Unsubscribe();
				_itemPropertyProxies.Remove(item);
			}
		}

		void ClearItemSubscriptions()
		{
			foreach (var kvp in _itemPropertyProxies)
			{
				kvp.Value.Unsubscribe();
			}
			_itemPropertyProxies.Clear();
		}

		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!ObserveChanges)
				return;

			// Find the index of the changed item
			int index = -1;
			if (_itemsSource is IList list)
			{
				index = list.IndexOf(sender);
			}
			else
			{
				int i = 0;
				foreach (var item in _itemsSource)
				{
					if (ReferenceEquals(item, sender) || (item != null && sender != null && item.Equals(sender)))
					{
						index = i;
						break;
					}
					i++;
				}
			}

			// Notify adapter if item was found
			if (index >= 0)
			{
				_notifier.NotifyItemChanged(this, AdjustPositionForHeader(index));
			}
		}
	}
}
