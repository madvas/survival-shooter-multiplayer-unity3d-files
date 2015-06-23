namespace Endgame
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using UnityEngine.Events;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public partial class ListView : MonoBehaviour
	{
		private ColumnHeaderCollection columnHeaders;
		private ListViewItemCollection items;
		private SelectedIndexCollection selectedIndices;
		private SelectedListViewItemCollection selectedItems;
		private IComparer listViewItemSorter = null;

		// FIXME: These are in the global namespace in WinForms, 
		// so should they really be here?
		public class ColumnClickEventArgs : System.EventArgs
		{
			public ColumnClickEventArgs(int column)
			{
				this.column = column;
			}

			private int column = -1;

			public int Column
			{
				get
				{
					return this.column;
				}
			}
		}

		public delegate void ColumnClickEventHandler(object sender, ColumnClickEventArgs e);

		public ColumnHeaderCollection Columns
		{
			get
			{
				if (this.columnHeaders == null)
				{
					Debug.LogError("ListView: Columns has not yet been initialised and is null. Columns is initialised in Awake. If you're calling this from Start, you'll need to move it to Awake instead.");
				}

				return this.columnHeaders;
			}
		}

		//public ListViewItem FocusedItem { get; set; }

		//public bool FullRowSelect { get; set; }

		public ListViewItemCollection Items
		{
			get
			{
				if (this.items == null)
				{
					Debug.LogError("ListView: Items has not yet been initialised and is null. Items is initialised in Awake. If you're calling this from Start, you'll need to move it to Awake instead.");
				}

				return this.items;
			}
		}

		public IComparer ListViewItemSorter
		{
			get
			{
				return this.listViewItemSorter;
			}
			set
			{
				this.listViewItemSorter = value;
				SortItems();
			}
		}

		//public bool MultiSelect { get; set; }

		public SelectedIndexCollection SelectedIndices { get { return this.selectedIndices; } }

		public SelectedListViewItemCollection SelectedItems { get { return this.selectedItems; } }

		//public SortOrder Sorting { get; set; }

		//public ListViewItem TopItem { get; set; }

		public event System.EventHandler ItemActivate = null;

		public event System.EventHandler SelectedIndexChanged = null;

		public event ColumnClickEventHandler ColumnClick = null;

		//public event ListViewItemSelectionChangedEventHandler ItemSelectionChanged;

		public event ListViewItemMouseHoverEventHandler ItemMouseHover = null;

		// New event handlers (additional to those provided by Windows Forms).
		public delegate void SubItemClickedEventHandler(UnityEngine.EventSystems.PointerEventData pointerEventData, ListViewItem.ListViewSubItem subItem);
		public event SubItemClickedEventHandler SubItemClicked = null;
		public delegate void ItemChangedEventHandler(ListViewItem listViewItem);
		public event ItemChangedEventHandler ItemChanged = null;
		public delegate void ItemBecameVisibleEventHandler(ListViewItem item);
		public event ItemBecameVisibleEventHandler ItemBecameVisible = null;
		public delegate void ItemBecameInvisibleEventHandler(ListViewItem item);
		public event ItemBecameInvisibleEventHandler ItemBecameInvisible = null;

		private Color foreColor;
		public Color ForeColor
		{
			get
			{
				return this.foreColor;
			}

			set
			{
				this.foreColor = value;
				this.RebuildHierarchy();
			}
		}

		private Color backColor;
		public Color BackColor
		{
			get
			{
				return this.backColor;
			}

			set
			{
				this.backColor = value;
				this.RebuildHierarchy();
			}
		}

		public ListViewItem FindItemWithText(string text)
		{
			ListViewItem result = null;
			foreach (ListViewItem item in this.items)
			{
				if (item.Text == text)
				{
					result = item;
				}
			}
			return result;
		}

		//public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex)

		//public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch);

		public class ColumnHeaderCollection : IList, ICollection, IEnumerable
		{
			private ListView owner;
			private List<ColumnHeader> columnHeaders = new List<ColumnHeader>();

			public ColumnHeaderCollection(ListView owner)
			{
				this.owner = owner;
			}

			public int Count { get { return this.columnHeaders.Count; } }
			public bool IsReadOnly { get { return false; } }
			public bool IsFixedSize { get { return false; } }
			object IList.this[int index]
			{
				get
				{
					return this.columnHeaders[index];
				}

				set
				{
					ColumnHeader columnHeader = value as ColumnHeader;
					if ((index < this.columnHeaders.Count) && (columnHeader != null))
					{
						this.columnHeaders[index] = columnHeader;
						this.owner.RebuildHierarchy();
					}
				}
			}

			public virtual ColumnHeader this[int index] { get { return this.columnHeaders[index]; } }

			public virtual ColumnHeader this[string key] { get { return this.columnHeaders.Find(x => x.Name == key); } }

			private void AddInternal(ColumnHeader columnHeader)
			{
				// Add the column header to the collection.
				this.columnHeaders.Add(columnHeader);
				columnHeader.ListView = this.owner;

				this.owner.RebuildHierarchy();
			}

			public virtual int Add(object value)
			{
				int result = -1;
				ColumnHeader columnHeader = value as ColumnHeader;
				if (columnHeader != null)
				{
					this.AddInternal(columnHeader);
					result = this.columnHeaders.Count - 1;
				}
				return result;
			}

			public virtual int Add(ColumnHeader value)
			{
				this.AddInternal(value);
				return this.columnHeaders.Count - 1;
			}

			public virtual ColumnHeader Add(string text)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.Text = text;
				this.AddInternal(columnHeader);
				return columnHeader;
			}

			public virtual ColumnHeader Add(string text, int width)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.Text = text;
				columnHeader.Width = width;
				this.AddInternal(columnHeader);
				return columnHeader;
			}

			public virtual ColumnHeader Add(string key, string text)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.Name = key;
				columnHeader.Text = text;
				this.AddInternal(columnHeader);
				return columnHeader;
			}

			public virtual ColumnHeader Add(string key, string text, int width)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.Name = key;
				columnHeader.Text = text;
				columnHeader.Width = width;
				this.AddInternal(columnHeader);
				return columnHeader;
			}

			public virtual void AddRange(ColumnHeader[] values)
			{
				foreach (ColumnHeader columnHeader in values)
				{
					this.AddInternal(columnHeader);
				}
			}

			public virtual void Clear()
			{
				this.columnHeaders.Clear();
				this.owner.RebuildHierarchy();
			}

			public bool Contains(object value)
			{
				bool result = false;
				ColumnHeader columnHeader = value as ColumnHeader;
				if (columnHeader != null)
				{
					result = this.Contains(columnHeader);
				}
				return result;
			}

			public bool Contains(ColumnHeader value)
			{
				return this.columnHeaders.Contains(value);
			}

			public virtual bool ContainsKey(string key)
			{
				return this.columnHeaders.Any(x => x.Name == key);
			}

			public IEnumerator GetEnumerator()
			{
				return this.columnHeaders.GetEnumerator();
			}

			public int IndexOf(object value)
			{
				int result = -1;
				ColumnHeader columnHeader = value as ColumnHeader;
				if (columnHeader != null)
				{
					result = this.IndexOf(columnHeader);
				}
				return result;
			}

			public int IndexOf(ColumnHeader value)
			{
				return this.columnHeaders.IndexOf(value);
			}

			public virtual int IndexOfKey(string key)
			{
				int result = -1;

				foreach (ColumnHeader columnHeader in this.columnHeaders)
				{
					if (columnHeader.Name == key)
					{
						result = this.columnHeaders.IndexOf(columnHeader);
					}
				}

				return result;
			}

			public void Insert(int index, object value)
			{
				if (value is ColumnHeader)
				{
					this.InsertInternal(index, value as ColumnHeader);
				}
			}

			private void InsertInternal(int index, ColumnHeader value)
			{
				this.columnHeaders.Insert(index, value);
				value.ListView = this.owner;
				this.owner.RebuildHierarchy();
			}

			public void Insert(int index, ColumnHeader value)
			{
				this.InsertInternal(index, value);
			}

			public void Insert(int index, string text)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.Text = text;
				this.InsertInternal(index, columnHeader);
			}

			public void Insert(int index, string text, int width)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.Text = text;
				columnHeader.Width = width;
				this.InsertInternal(index, columnHeader);
			}

			public void Insert(int index, string key, string text)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.Name = key;
				columnHeader.Text = text;
				this.InsertInternal(index, columnHeader);
			}

			public void Insert(int index, string key, string text, int width)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.Name = key;
				columnHeader.Text = text;
				columnHeader.Width = width;
				this.InsertInternal(index, columnHeader);
			}

			private void RemoveInternal(ColumnHeader columnHeader)
			{
				this.columnHeaders.Remove(columnHeader);
				this.owner.RebuildHierarchy();
			}

			public void Remove(object value)
			{
				ColumnHeader columnHeader = value as ColumnHeader;
				if (columnHeader != null)
				{
					this.RemoveInternal(columnHeader);
				}
			}

			public virtual void Remove(ColumnHeader column)
			{
				this.RemoveInternal(column);
			}

			public virtual void RemoveAt(int index)
			{
				ColumnHeader columnHeader = this.columnHeaders[index];
				this.RemoveInternal(columnHeader);
			}

			public virtual void RemoveByKey(string key)
			{
				ColumnHeader value = this[key];
				if (value != null)
				{
					this.RemoveInternal(value);
				}
			}

			public bool IsSynchronized { get { return false; } }

			public object SyncRoot { get { return null; } }

			public void CopyTo(System.Array array, int index)
			{
				int d = 0;
				for (int s = index; s < this.Count; s++)
				{
					array.SetValue(this[s], d);
					d++;
				}
			}
		}

		private ImageList smallImageList;
		public ImageList SmallImageList
		{
			get
			{
				return this.smallImageList;
			}

			set
			{
				ImageList imageList = value;
				this.smallImageList = imageList;
				imageList.OnChanged += OnImageListChanged;
				this.RebuildHierarchy();
			}
		}

		public class ListViewItemCollection : IList, ICollection, IEnumerable
		{
			private ListView owner;
			private List<ListViewItem> items;

			public ListViewItemCollection(ListView owner)
			{
				this.owner = owner;
				this.items = new List<ListViewItem>();
			}

			public int Count { get { return this.items.Count; } }

			public bool IsFixedSize { get { return false; } }

			public bool IsReadOnly { get { return false; } }

			object IList.this[int index]
			{
				get
				{
					return this.items[index];
				}

				set
				{
					ListViewItem listViewItem = value as ListViewItem;
					if (listViewItem != null)
					{
						if (index < this.items.Count)
						{
							// Add the item to the collection.
							this.items[index] = listViewItem;
							listViewItem.ListView = this.owner;

							this.owner.RebuildHierarchy();

							if (listViewItem.Selected)
							{
								this.owner.SelectedIndices.Add(listViewItem.Index);
							}
						}
					}
				}
			}

			public virtual ListViewItem this[int index]
			{
				get
				{
					return this.items[index];
				}
				set
				{
					if (index < this.items.Count)
					{
						// Add the item to the collection.
						this.items[index] = value;
						value.ListView = this.owner;

						this.owner.RebuildHierarchy();

						if (value.Selected)
						{
							this.owner.SelectedIndices.Add(value.Index);
						}
					}
				}
			}

			public virtual ListViewItem this[string key]
			{
				get
				{
					return this.items.Find(x => x.Name == key);
				}
			}

			private void AddInternal(ListViewItem listViewItem)
			{
				if (this.owner.Columns.Count == 0)
				{
					Debug.LogWarning("ListView: Adding an item without any columns - nothing will appear. (Add a column first using Listview.Columns.Add).");
				}

				// Add the item to the collection.
				this.items.Add(listViewItem);
				listViewItem.ListView = this.owner;

				this.owner.RebuildHierarchy();

				if (listViewItem.Selected)
				{
					this.owner.SelectedIndices.Add(listViewItem.Index);
				}
			}

			public int Add(object value)
			{
				int result = -1;
				ListViewItem item = value as ListViewItem;
				if (item != null)
				{
					this.Add(item);
					result = this.items.Count - 1;
				}
				return result;
			}

			public virtual ListViewItem Add(ListViewItem value)
			{
				this.AddInternal(value);
				return value;
			}

			public virtual ListViewItem Add(string text)
			{
				ListViewItem listViewItem = new ListViewItem(text);
				this.AddInternal(listViewItem);
				return listViewItem;
			}

			public void AddRange(ListView.ListViewItemCollection items)
			{
				foreach (ListViewItem item in items)
				{
					this.AddInternal(item);
				}
			}

			public void AddRange(ListViewItem[] items)
			{
				foreach (ListViewItem item in items)
				{
					this.AddInternal(item);
				}
			}

			public virtual void Clear()
			{
				this.owner.SuspendLayout();
				{
					this.owner.SelectedIndices.Clear();
					this.items.Clear();
					this.owner.SetHorizontalScrollBarValue(0);
					this.owner.SetVerticalScrollBarValue(0);
				}
				this.owner.ResumeLayout();
			}

			public bool Contains(object value)
			{
				bool result = false;
				ListViewItem item = value as ListViewItem;
				if (item != null)
				{
					result = this.Contains(item);
				}
				return result;
			}

			public bool Contains(ListViewItem item)
			{
				return this.items.Contains(item);
			}

			public virtual bool ContainsKey(string key)
			{
				return this.items.Any(x => x.Name == key);
			}

			public ListViewItem[] Find(string key, bool searchAllSubItems)
			{
				return this.items.FindAll(x => x.Name == key).ToArray();
			}

			public IEnumerator GetEnumerator()
			{
				return this.items.GetEnumerator();
			}

			public int IndexOf(object value)
			{
				int result = -1;
				ListViewItem item = value as ListViewItem;
				if (item != null)
				{
					result = this.IndexOf(value);
				}
				return result;
			}

			public int IndexOf(ListViewItem item)
			{
				return this.items.IndexOf(item);
			}

			public virtual int IndexOfKey(string key)
			{
				int result = -1;

				foreach (ListViewItem listViewItem in this.items)
				{
					if (listViewItem.Name == key)
					{
						result = this.items.IndexOf(listViewItem);
					}
				}

				return result;
			}

			public void Insert(int index, object value)
			{
				if (value is ListViewItem)
				{
					this.InsertInternal(index, value as ListViewItem);
				}
			}

			private void InsertInternal(int index, ListViewItem item)
			{
				ListViewItem selectedItem = null;
				if (this.owner.SelectedIndices.Count > 0)
				{
					selectedItem = this.owner.SelectedItems[0];
				}

				// If an item is already selected, remove the selection temporarily.
				if (selectedItem != null)
				{
					this.owner.SelectedIndices.RemoveAndUpdateSelection(selectedItem.Index, raiseEvent: false);
				}

				// Insert the new item.
				this.items.Insert(index, item);
				item.ListView = this.owner;

				// If the new item should be selected, select it.
				if (item.Selected)
				{
					this.owner.SelectedIndices.AddAndUpdateSelection(item.Index, raiseEvent: false);
				}
				// Otherwise, reselect the previously selected item (by using its updated index).
				else if (selectedItem != null)
				{
					this.owner.SelectedIndices.AddAndUpdateSelection(selectedItem.Index, raiseEvent: false);
				}

				this.owner.RebuildHierarchy();
			}

			public ListViewItem Insert(int index, ListViewItem item)
			{
				this.InsertInternal(index, item);
				return item;
			}

			public ListViewItem Insert(int index, string text)
			{
				ListViewItem listViewItem = new ListViewItem(text);
				this.InsertInternal(index, listViewItem);
				return listViewItem;
			}

			private void RemoveInternal(ListViewItem item)
			{
				ListViewItem selectedItem = null;
				int selectedItemIndex = -1;
				if (this.owner.SelectedIndices.Count > 0)
				{
					selectedItem = this.owner.SelectedItems[0];
					selectedItemIndex = selectedItem.Index;
				}

				// If an item is already selected, remove the selection temporarily.
				if (selectedItem != null)
				{
					this.owner.SelectedIndices.RemoveAndUpdateSelection(selectedItem.Index, raiseEvent: false);
				}

				// Remove the item.
				this.items.Remove(item);

				// If the selected item was removed, select the item now at its index.
				if (selectedItemIndex != -1)
				{
					if (this.items.Count > 0)
					{
						selectedItemIndex = Mathf.Min(Mathf.Max(selectedItemIndex, 0), this.items.Count - 1);
						this.owner.SelectedIndices.AddAndUpdateSelection(selectedItemIndex, raiseEvent: false);
					}
				}
				// Otherwise, reselect the previously selected item (by using its updated index).
				else if (selectedItem != null)
				{
					this.owner.SelectedIndices.AddAndUpdateSelection(selectedItem.Index, raiseEvent: false);
				}

				this.owner.RebuildHierarchy();
			}

			public void Remove(object value)
			{
				ListViewItem item = value as ListViewItem;
				if (item != null)
				{
					this.RemoveInternal(item);
				}
			}

			public virtual void Remove(ListViewItem item)
			{
				this.RemoveInternal(item);
			}

			public virtual void RemoveAt(int index)
			{
				ListViewItem listViewItem = this.items[index];
				this.RemoveInternal(listViewItem);
			}

			public virtual void RemoveByKey(string key)
			{
				ListViewItem value = this[key];
				if (value != null)
				{
					this.RemoveInternal(value);
				}
			}

			public bool IsSynchronized { get { return false; } }

			public object SyncRoot { get { return null; } }

			public void CopyTo(System.Array array, int index)
			{
				int d = 0;
				for (int s = index; s < this.Count; s++)
				{
					array.SetValue(this[s], d);
					d++;
				}
			}

			private class Comparer : IComparer<ListViewItem>
			{
				private IComparer comparer;
				public Comparer(IComparer comparer)
				{
					this.comparer = comparer;
				}

				public int Compare(ListViewItem x, ListViewItem y)
				{
					return this.comparer.Compare(x, y);
				}
			}

			public void Sort(IComparer listViewItemSorter)
			{
				this.items.Sort(new Comparer(listViewItemSorter));
			}
		}

		public class SelectedIndexCollection : IList, ICollection, IEnumerable
		{
			private ListView owner;
			private List<int> selectedIndices;

			public SelectedIndexCollection(ListView owner)
			{
				this.owner = owner;
				this.selectedIndices = new List<int>();
			}

			public int Count { get { return this.selectedIndices.Count; } }

			public int this[int index] { get { return this.selectedIndices[index]; } }

			private void AddInternal(int itemIndex)
			{
				this.selectedIndices.Add(itemIndex);
			}

			public void AddAndUpdateSelection(int itemIndex, bool raiseEvent)
			{
				// TODO: If MultiSelect is true, don't clear here.
				int indexToDeselect = -1;

				if (this.selectedIndices.Count > 0)
				{
					indexToDeselect = this.selectedIndices[0];
				}

				if (indexToDeselect != itemIndex)
				{
					this.ClearAndUpdateSelection(raiseEvent: false);

					this.AddInternal(itemIndex);

					bool updateGameObjects = !this.owner.LayoutSuspended;

					this.owner.OnSelectionAddedInternal(itemIndex, updateGameObjects: updateGameObjects);

					if (raiseEvent)
					{
						this.owner.OnSelectedIndexChanged(new System.EventArgs());
					}
				}
			}

			public int Add(int itemIndex)
			{
				this.AddAndUpdateSelection(itemIndex, raiseEvent: true);
				return this.selectedIndices.Count;
			}

			private void ClearInternal()
			{
				this.selectedIndices.Clear();
			}

			private void ClearAndUpdateSelection(bool raiseEvent)
			{
				if (this.owner != null)
				{
					this.owner.OnSelectionClearedInternal();
				}
				this.ClearInternal();

				if (raiseEvent)
				{
					this.owner.OnSelectedIndexChanged(new System.EventArgs());
				}
			}

			public void Clear()
			{
				this.ClearAndUpdateSelection(raiseEvent: true);
			}

			public bool Contains(int selectedIndex)
			{
				return this.selectedIndices.Contains(selectedIndex);
			}

			public IEnumerator GetEnumerator()
			{
				return this.selectedIndices.GetEnumerator();
			}

			public int IndexOf(int selectedIndex)
			{
				return this.selectedIndices.IndexOf(selectedIndex);
			}

			public void RemoveAndUpdateSelection(int index, bool raiseEvent)
			{
				if (!this.selectedIndices.Contains(index))
				{
					return;
				}

				if (this.owner != null)
				{
					this.owner.OnSelectionRemovedInternal(index);
				}
				this.selectedIndices.Remove(index);

				if (raiseEvent)
				{
					this.owner.OnSelectedIndexChanged(new System.EventArgs());
				}
			}

			public void Remove(int itemIndex)
			{
				this.RemoveAndUpdateSelection(itemIndex, raiseEvent: true);
			}

			public bool IsFixedSize { get { return false; } }

			public bool IsReadOnly { get { return false; } }

			object IList.this[int index]
			{
				get
				{
					return this.selectedIndices[index];
				}

				set
				{
					throw new System.NotImplementedException();
				}
			}

			public int Add(object value)
			{
				if (value is int)
				{
					int index = (int)value;
					this.AddAndUpdateSelection(index, raiseEvent: true);
				}
				return this.selectedIndices.Count;
			}

			public bool Contains(object value)
			{
				bool result = false;
				if (value is int)
				{
					int index = (int)value;
					result = this.selectedIndices.Contains(index);
				}
				return result;
			}

			public int IndexOf(object value)
			{
				int result = -1;
				if (value is int)
				{
					int index = (int)value;
					result = this.selectedIndices.IndexOf(index);
				}
				return result;
			}

			public void Insert(int index, object value)
			{
				if (value is int)
				{
					// TODO: If MultiSelect is true, perform an insert.
					// (When it is false, the operation is the same as Add.)
					this.Clear();

					int itemIndex = (int)value;
					this.AddAndUpdateSelection(itemIndex, raiseEvent: true);
				}
			}

			public void Remove(object value)
			{
				if (value is int)
				{
					int index = (int)value;
					this.RemoveAndUpdateSelection(index, raiseEvent: true);
				}
			}

			public void RemoveAt(int itemIndex)
			{
				int index = this.selectedIndices[itemIndex];
				this.RemoveAndUpdateSelection(index, raiseEvent: true);
			}

			public bool IsSynchronized { get { return false; } }

			public object SyncRoot { get { return null; } }

			public void CopyTo(System.Array array, int index)
			{
				int d = 0;
				for (int s = index; s < this.Count; s++)
				{
					array.SetValue(this[s], d);
					d++;
				}
			}
		}

		public class SelectedListViewItemCollection : IList, ICollection, IEnumerable
		{
			//private ListView owner;
			List<ListViewItem> selectedItems;

			public SelectedListViewItemCollection(ListView owner)
			{
				//this.owner = owner;
				this.selectedItems = new List<ListViewItem>();
			}

			public int Count { get { return this.selectedItems.Count; } }

			public ListViewItem this[int index] { get { return this.selectedItems[index]; } }

			public virtual ListViewItem this[string key] { get { return this.selectedItems.Find(x => x.Name == key); } }

			void IList.Clear()
			{
				this.selectedItems.Clear();
			}

			public bool Contains(ListViewItem item)
			{
				return this.selectedItems.Contains(item);
			}

			public virtual bool ContainsKey(string key)
			{
				return this.selectedItems.Any(x => x.Name == key);
			}

			public IEnumerator GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public int IndexOf(ListViewItem item)
			{
				return this.selectedItems.IndexOf(item);
			}

			public virtual int IndexOfKey(string key)
			{
				int result = -1;

				foreach (ListViewItem listViewItem in this.selectedItems)
				{
					if (listViewItem.Name == key)
					{
						result = this.selectedItems.IndexOf(listViewItem);
					}
				}

				return result;
			}

			public bool IsFixedSize { get { return false; } }

			public bool IsReadOnly { get { return false; } }

			object IList.this[int index]
			{
				get
				{
					return this.selectedItems[index];
				}

				set
				{
					throw new System.NotImplementedException();
				}
			}

			// AddInternal, RemoveInternal and ClearInternal are not be accessible to 
			// general code, because this collection needs to mirror the
			// SelectedIndices collection.
			// (I think this is achieved in Windows Forms via the internal
			// modifier, but that requires the code to be in a different assembly.)
			// Since this is not possible, the ListView class calls these private members
			// using reflection.
			private void ClearInternal()
			{
				this.selectedItems.Clear();
			}

			private void AddInternal(ListViewItem listViewItem)
			{
				this.selectedItems.Add(listViewItem);
			}

			private void RemoveInternal(ListViewItem listViewItem)
			{
				this.selectedItems.Remove(listViewItem);
			}

			int IList.Add(object value)
			{
				if (value is ListViewItem)
				{
					ListViewItem listViewItem = (ListViewItem)value;
					this.AddInternal(listViewItem);
				}
				return this.selectedItems.Count;
			}

			public bool Contains(object value)
			{
				bool result = false;
				if (value is ListViewItem)
				{
					ListViewItem listViewItem = (ListViewItem)value;
					result = this.selectedItems.Contains(listViewItem);
				}
				return result;
			}

			public int IndexOf(object value)
			{
				int result = -1;
				if (value is ListViewItem)
				{
					ListViewItem listViewItem = (ListViewItem)value;
					result = this.selectedItems.IndexOf(listViewItem);
				}
				return result;
			}

			void IList.Insert(int index, object value)
			{
				throw new System.NotImplementedException();
			}

			void IList.Remove(object value)
			{
				if (value is ListViewItem)
				{
					ListViewItem listViewItem = (ListViewItem)value;
					this.RemoveInternal(listViewItem);
				}
			}

			void IList.RemoveAt(int index)
			{
				ListViewItem listViewItem = this.selectedItems[index];
				this.RemoveInternal(listViewItem);
			}

			public bool IsSynchronized { get { return false; } }

			public object SyncRoot { get { return null; } }

			public void CopyTo(System.Array array, int index)
			{
				int d = 0;
				for (int s = index; s < this.Count; s++)
				{
					array.SetValue(this[s], d);
					d++;
				}
			}
		}

		private void OnImageListChanged(ImageList sender)
		{
			this.RebuildHierarchy();
		}

		public void Select()
		{
			EventSystem.current.SetSelectedGameObject(this.gameObject);
		}

		public bool ContainsFocus()
		{
			return EventSystem.current.currentSelectedGameObject == this.gameObject;
		}
	}
}
