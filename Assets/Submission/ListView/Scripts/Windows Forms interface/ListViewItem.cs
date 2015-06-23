namespace Endgame
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime;
	using System.Runtime.Serialization;
	using System.Linq;
	using UnityEngine;

	public class ListViewItem : ICloneable/*, ISerializable*/
	{
		private ListView owner;
		private bool selected;
		private ListViewSubItemCollection subItems;
		public ListView Owner
		{
			get
			{
				return this.owner;
			}
		}

		public ListViewItem()
		{
			this.subItems = new ListViewSubItemCollection(this);
			this.subItems.Add("");
		}

		public ListViewItem(string text)
		{
			this.subItems = new ListViewSubItemCollection(this);
			this.subItems.Add("");
			this.Text = text;
		}

		public ListViewItem(string[] items)
		{
			this.subItems = new ListViewSubItemCollection(this);
			if (items.Length > 0)
			{
				subItems.AddRange(items);
			}
		}

		//public Font Font { get; set; }
		public int Index
		{
			get
			{
				int result = -1;
				if (this.ListView != null)
				{
					result = this.ListView.Items.IndexOf(this);
				}
				return result;
			}
		}

		public ListView ListView
		{
			get
			{
				return this.owner;
			}

			set
			{
				this.owner = value;
			}
		}

		public bool Selected
		{
			get
			{
				return this.selected;
			}

			set
			{
				this.selected = value;
				if (this.owner != null)
				{
					if (value)
					{
						this.owner.SelectedIndices.Add(this.Index);
					}
					else
					{
						this.owner.SelectedIndices.Remove(this.Index);
					}
				}
			}
		}

		public ListViewItem.ListViewSubItemCollection SubItems
		{
			get
			{
				return this.subItems;
			}
		}

		private string name;
		private object tag;

		public string Name
		{
			get
			{
				return this.name;
			}

			set
			{
				this.name = value;
				this.OnModified();
			}
		}

		public object Tag
		{
			get
			{
				return this.tag;
			}

			set
			{
				this.tag = value;
				this.OnModified();
			}
		}

		public string Text
		{
			get
			{
				return this.subItems[0].Text;
			}

			set
			{
				this.subItems[0].Text = value;
			}
		}

		public Color ForeColor
		{
			get
			{
				return this.subItems[0].ForeColor;
			}

			set
			{
				this.subItems[0].ForeColor = value;
			}
		}

		public Color BackColor
		{
			get
			{
				return this.subItems[0].BackColor;
			}

			set
			{
				this.subItems[0].BackColor = value;
			}
		}

		public Font Font
		{
			get
			{
				return this.subItems[0].Font;
			}

			set
			{
				this.subItems[0].Font = value;
			}
		}

		public int FontSize
		{
			get
			{
				return this.subItems[0].FontSize;
			}

			set
			{
				this.subItems[0].FontSize = value;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return this.subItems[0].FontStyle;
			}

			set
			{
				this.subItems[0].FontStyle = value;
			}
		}

		private bool useItemStyleForSubItems = true;
		public bool UseItemStyleForSubItems
		{
			get
			{
				return this.useItemStyleForSubItems;
			}

			set
			{
				this.useItemStyleForSubItems = value;
				if (this.owner != null)
				{
					this.owner.RebuildHierarchy();
				}
			}
		}

		public string ImageKey
		{
			get
			{
				return this.subItems[0].ImageKey;
			}

			set
			{
				this.subItems[0].ImageKey = value;
			}
		}

		public int ImageIndex
		{
			get
			{
				return this.subItems[0].ImageIndex;
			}

			set
			{
				this.subItems[0].ImageIndex = value;
			}
		}

		public ImageList ImageList
		{
			get
			{
				return this.owner.SmallImageList;
			}
		}

		public virtual void Remove()
		{
			if (this.ListView != null)
			{
				this.ListView.Items.Remove(this);
			}
		}

		public void OnModified()
		{
			if (this.ListView != null)
			{
				this.ListView.UpdateItem(this);
			}
		}

		public object Clone()
		{
			ListViewItem clone = new ListViewItem();

			clone.owner = this.owner;
			clone.selected = this.selected;

			foreach (ListViewSubItem subItem in this.subItems)
			{
				ListViewSubItem newItem = new ListViewSubItem();
				newItem.ListViewItem = subItem.ListViewItem;
				newItem.Name = subItem.Name;
				newItem.Tag = subItem.Tag;
				newItem.Text = subItem.Text;
				clone.subItems.Add(newItem);
			}

			return clone;
		}

		public ItemButton ItemButtonInHierarchy
		{
			get;
			set;
		}

		private void SetSelectedFlagInternal(bool value)
		{
			this.selected = value;
		}

		public RectTransform CustomControl
		{
			get
			{
				return this.SubItems[0].CustomControl;
			}

			set
			{
				this.SubItems[0].CustomControl = value;
			}
		}

		public class ListViewSubItem
		{
			private ListViewItem owner;

			public ListViewSubItem()
			{
			}
			public ListViewSubItem(ListViewItem owner, string text)
			{
				this.owner = owner;
				this.text = text;
			}

			private Font font;
			public Font Font
			{
				get
				{
					Font font = this.font;
					if (font == null)
					{
						if (this.owner != null)
						{
							if (this.owner.owner != null)
							{
								font = this.owner.owner.DefaultItemFont;
							}
						}
					}

					return font;
				}
				set
				{
					this.font = value;
					this.OnModified();
				}
			}
			private string name;
			private object tag;
			private string text;

			private void OnModified()
			{
				if (this.owner != null)
				{
					this.owner.OnModified();
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}

				set
				{
					this.name = value;
					this.OnModified();
				}
			}

			public object Tag
			{
				get
				{
					return this.tag;
				}

				set
				{
					this.tag = value;
					this.OnModified();
				}
			}

			public string Text
			{
				get
				{
					return this.text;
				}

				set
				{
					this.text = value;
					this.OnModified();
				}
			}

			private Color foreColor = Color.black;
			internal bool hasForeColorBeenSet = false;
			public Color ForeColor
			{
				get
				{
					// Only use the subitem's fore colour if the user has set it 
					// explicitly. If it is still at the default value, use the 
					// control-wide fore colour instead. (This is how Windows Forms behaves.)
					ListViewSubItem subItem = this;
					if (this.owner.UseItemStyleForSubItems)
					{
						subItem = this.owner.subItems[0];
					}

					Color foreColor = subItem.foreColor;
					if (!subItem.hasForeColorBeenSet)
					{
						foreColor = this.owner.owner.ForeColor;
					}

					return foreColor;
				}

				set
				{
					this.foreColor = value;
					this.hasForeColorBeenSet = true;
					this.OnModified();
				}
			}

			private Color backColor = Color.white;
			internal bool hasBackColorBeenSet = false;
			public Color BackColor
			{
				get
				{
					// For subitem back colours, Windows Forms uses the
					// subitem's colour if it has been set, or failing that,
					// uses the ListView's back colour.
					// However Windows Forms does not support colours with alpha.
					// Since this control supports alpha colours, it makes
					// more sense to return a colour with alpha 0 in the
					// case where no subitem back colour has been set.
					// This will mean that for subitems with no back colour
					// set, the ListView's back colour will be seen.
					ListViewSubItem subItem = this;
					if (this.owner.UseItemStyleForSubItems)
					{
						subItem = this.owner.subItems[0];
					}

					Color backColor = subItem.backColor;
					if (!subItem.hasBackColorBeenSet)
					{
						//backColor = new Color(0, 0, 0, 0);
						backColor = this.owner.owner.DefaultItemBackgroundColor;
					}

					return backColor;
				}

				set
				{
					this.backColor = value;
					this.hasBackColorBeenSet = true;
					this.OnModified();
				}
			}

			private int fontSize = -1;
			public int FontSize
			{
				get
				{
					int fontSize = this.fontSize;
					if (fontSize == -1)
					{
						if (this.owner != null)
						{
							if (this.owner.owner != null)
							{
								fontSize = this.owner.owner.DefaultItemFontSize;
							}
						}
					}

					return fontSize;
				}

				set
				{
					this.fontSize = value;
					this.OnModified();
				}
			}

			private string imageKey = "";
			public string ImageKey
			{
				get
				{
					return this.imageKey;
				}

				set
				{
					this.imageKey = value;
					this.imageIndex = -1;
					this.OnModified();
				}
			}

			private int imageIndex = -1;
			public int ImageIndex
			{
				get
				{
					return this.imageIndex;
				}

				set
				{
					this.imageIndex = value;
					this.imageKey = "";
					this.OnModified();
				}
			}

			private bool hasFontStyleBeenSet = false;
			private FontStyle fontStyle;
			public FontStyle FontStyle
			{
				get
				{
					FontStyle fontStyle = this.fontStyle;
					if (!this.hasFontStyleBeenSet)
					{
						if (this.owner != null)
						{
							if (this.owner.owner != null)
							{
								fontStyle = this.owner.owner.DefaultItemFontStyle;
							}
						}
					}

					return fontStyle;
				}
				set
				{
					this.fontStyle = value;
					this.hasFontStyleBeenSet = true;
					this.OnModified();
				}
			}

			public ListViewItem ListViewItem
			{
				get
				{
					return this.owner;
				}

				set
				{
					this.owner = value;
				}
			}

			private RectTransform customControl;
			public RectTransform CustomControl
			{
				get
				{
					return this.customControl;
				}

				set
				{
					this.customControl = value;
					this.OnModified();
				}
			}
		}

		public class ListViewSubItemCollection : IList, ICollection, IEnumerable
		{
			private ListViewItem owner;
			private List<ListViewSubItem> subItems;

			public ListViewSubItemCollection(ListViewItem owner)
			{
				this.owner = owner;
				this.subItems = new List<ListViewSubItem>();
			}

			public int Count
			{
				get
				{
					return this.subItems.Count;
				}
			}

			public bool IsReadOnly { get { return false; } }

			public ListViewSubItem this[int index]
			{
				get
				{
					if (index < this.subItems.Count)
					{
						return this.subItems[index];
					}
					else
					{
						return new ListViewSubItem(this.owner, string.Empty);
					}
				}

				set
				{
					if (index < this.subItems.Count)
					{
						this.subItems[index] = value;
						ListView owner = this.owner.owner;
						if (owner != null)
						{
							owner.RebuildHierarchy();
						}
					}
				}
			}

			public virtual ListViewSubItem this[string key]
			{
				get
				{
					return this.subItems.Find(x => x.Name == key);
				}
			}

			private void AddInternal(ListViewSubItem subItem)
			{
				this.subItems.Add(subItem);
				ListView owner = this.owner.owner;
				if (owner != null)
				{
					owner.RebuildHierarchy();
				}
			}

			public ListViewSubItem Add(ListViewSubItem item)
			{
				this.AddInternal(item);
				return item;
			}

			public ListViewSubItem Add(string text)
			{
				ListViewSubItem item = new ListViewSubItem(owner, text);
				this.AddInternal(item);
				return item;
			}

			public void AddRange(ListViewSubItem[] items)
			{
				foreach (var item in items)
				{
					this.AddInternal(item);
				}
			}

			public void AddRange(string[] items)
			{
				foreach (var item in items)
				{
					this.Add(item);
				}
			}

			public void Clear()
			{
				this.subItems.Clear();
				this.subItems.Add(new ListViewSubItem(this.owner, ""));

				ListView owner = this.owner.owner;
				if (owner != null)
				{
					owner.RebuildHierarchy();
				}
			}

			public bool Contains(ListViewItem.ListViewSubItem subItem)
			{
				return this.subItems.Contains(subItem);
			}

			public virtual bool ContainsKey(string key)
			{
				return this.subItems.Any(x => x.Name == key);
			}

			public IEnumerator GetEnumerator()
			{
				return this.subItems.GetEnumerator();
			}

			public int IndexOf(ListViewItem.ListViewSubItem subItem)
			{
				return this.subItems.IndexOf(subItem);
			}

			public virtual int IndexOfKey(string key)
			{
				int result = -1;

				foreach (var listViewSubItem in this.subItems)
				{
					if (listViewSubItem.Name == key)
					{
						result = this.subItems.IndexOf(listViewSubItem);
					}
				}

				return result;
			}

			public void Insert(int index, ListViewItem.ListViewSubItem item)
			{
				if (index < this.subItems.Count)
				{
					this.subItems.Insert(index, item);
					item.ListViewItem = this.owner;

					ListView owner = this.owner.owner;
					if (owner != null)
					{
						owner.RebuildHierarchy();
					}
				}
			}

			private void RemoveInternal(ListViewSubItem subItem)
			{
				this.subItems.Remove(subItem);
				if (this.subItems.Count == 0)
				{
					this.subItems.Add(new ListViewSubItem(this.owner, ""));
				}

				ListView owner = this.owner.owner;
				if (owner != null)
				{
					owner.RebuildHierarchy();
				}
			}

			public void Remove(ListViewItem.ListViewSubItem item)
			{
				this.RemoveInternal(item);
			}

			public void RemoveAt(int index)
			{
				ListViewSubItem subItem = this.subItems[index];
				this.RemoveInternal(subItem);
			}

			public virtual void RemoveByKey(string key)
			{
				ListViewSubItem value = this[key];
				if (value != null)
				{
					this.RemoveInternal(value);
				}
			}

			public bool IsFixedSize { get { return false; } }

			object IList.this[int index]
			{
				get
				{
					return this.subItems[index];
				}

				set
				{
					throw new System.NotImplementedException();
				}
			}

			public int Add(object value)
			{
				if (value is ListViewSubItem)
				{
					ListViewSubItem listViewSubItem = (ListViewSubItem)value;
					this.AddInternal(listViewSubItem);
				}
				return this.subItems.Count;
			}

			public bool Contains(object value)
			{
				bool result = false;
				if (value is ListViewSubItem)
				{
					ListViewSubItem listViewSubItem = (ListViewSubItem)value;
					result = this.subItems.Contains(listViewSubItem);
				}
				return result;
			}

			public int IndexOf(object value)
			{
				int result = -1;
				if (value is ListViewSubItem)
				{
					ListViewSubItem listViewSubItem = (ListViewSubItem)value;
					result = this.subItems.IndexOf(listViewSubItem);
				}
				return result;
			}

			public void Insert(int index, object value)
			{
				if (value is ListViewSubItem)
				{
					ListViewSubItem listViewSubItem = (ListViewSubItem)value;
					this.Insert(index, listViewSubItem);
				}
			}

			public void Remove(object value)
			{
				if (value is ListViewSubItem)
				{
					ListViewSubItem listViewSubItem = (ListViewSubItem)value;
					this.RemoveInternal(listViewSubItem);
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
	}
}
