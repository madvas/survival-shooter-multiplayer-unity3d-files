namespace Endgame
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.Design.Serialization;
	using System.Reflection;
	using UnityEngine;

	public sealed class ImageList
	{
		private ImageList.ImageCollection images;

		//public ImageList();
		//public ImageList(IContainer container);
		//public ColorDepth ColorDepth { get; set; }
		//public IntPtr Handle { get; }
		//public bool HandleCreated { get; }

		public delegate void ImageListChanged(ImageList sender);
		public event ImageListChanged OnChanged;

		public ImageList()
		{
			this.images = new ImageCollection();
			this.images.Owner = this;
			this.WasImageSizeSet = false;
		}

		private void RaiseOnChangedEvent()
		{
			if (this.OnChanged != null)
			{
				this.OnChanged(this);
			}
		}

		public bool WasImageSizeSet { get; set; }
		public ImageList.ImageCollection Images { get { return this.images; } }
		private Vector2 imageSize;
		public Vector2 ImageSize
		{
			get
			{
				return this.imageSize;
			}

			set
			{
				this.imageSize = value;
				this.WasImageSizeSet = true;
				RaiseOnChangedEvent();
			}
		}
		//public ImageListStreamer ImageStream { get; set; }
		public object Tag { get; set; }
		//public Color TransparentColor { get; set; }
		//public event EventHandler RecreateHandle;

		//protected override void Dispose(bool disposing);
		//public void Draw(Graphics g, Point pt, int index);
		//public void Draw(Graphics g, int x, int y, int index);
		//public void Draw(Graphics g, int x, int y, int width, int height, int index);
		//public override string ToString();

		public sealed class ImageCollection : IList, ICollection, IEnumerable
		{
			private class Entry
			{
				public Entry(string key, Sprite sprite)
				{
					this.Key = key;
					this.Sprite = sprite;
				}

				public Entry(Sprite sprite)
					: this(null, sprite)
				{
				}

				public string Key;
				public Sprite Sprite;
			}
			public ImageList Owner { get; set; }

			private List<Entry> imageList = new List<Entry>();

			public int Count { get { return this.imageList.Count; } }
			public bool Empty { get { return this.Count == 0; } }
			//public bool IsReadOnly { get; }
			//public StringCollection Keys { get { return images.Keys; } }

			public Sprite this[int index]
			{
				get
				{
					Sprite result = null;
					if (index < this.imageList.Count)
					{
						result = this.imageList[index].Sprite;
					}
					return result;
				}

				set
				{
					this.imageList[index] = new Entry(value);
					this.Owner.RaiseOnChangedEvent();
				}
			}

			object IList.this[int index]
			{
				get
				{
					object result = null;
					if (index < this.imageList.Count)
					{
						result = this.imageList[index].Sprite;
					}
					return result;
				}

				set
				{
					if (index < this.imageList.Count)
					{
						Sprite sprite = value as Sprite;
						if (sprite != null)
						{
							this.imageList[index].Sprite = sprite;
							this.Owner.RaiseOnChangedEvent();
						}
					}
				}
			}

			public Sprite this[string key]
			{
				get
				{
					Sprite result = null;
					var findResult = this.imageList.Find(entry => entry.Key == key);
					if (findResult != null)
					{
						result = findResult.Sprite;
					}
					return result;
				}
			}

			//public void Add(Icon value);
			public void Add(Sprite value)
			{
				this.imageList.Add(new Entry(value));
				this.Owner.RaiseOnChangedEvent();
			}

			public int Add(object value)
			{
				Sprite sprite = value as Sprite;
				int result = -1;

				if (sprite != null)
				{
					Add(sprite);
					result = this.Count - 1;
				}
				return result;
			}

			public void Insert(int index, object value)
			{
				Sprite sprite = value as Sprite;
				if (sprite != null)
				{
					this.imageList.Insert(index, new Entry(sprite));
					this.Owner.RaiseOnChangedEvent();
				}
			}

			//public int Add(Image value, Color transparentColor);
			//public void Add(string key, Icon icon);
			public void Add(string key, Sprite image)
			{
				Entry result = this.imageList.Find(entry => entry.Key == key);
				if (result != null)
				{
					result.Sprite = image;
				}
				else
				{
					this.imageList.Add(new Entry(key, image));
				}
				this.Owner.RaiseOnChangedEvent();
			}

			public void AddRange(Sprite[] images)
			{
				foreach (var image in images)
				{
					this.imageList.Add(new Entry(image));
				}
				this.Owner.RaiseOnChangedEvent();
			}

			//public int AddStrip(Image value);
			public void Clear()
			{
				this.imageList.Clear();
				this.Owner.RaiseOnChangedEvent();
			}

			public bool Contains(Sprite image)
			{
				var result = this.imageList.Find(entry => entry.Sprite == image);
				return result != null;
			}

			public bool Contains(object image)
			{
				Sprite sprite = image as Sprite;

				if (sprite != null)
				{
					return Contains(sprite);
				}
				return false;
			}

			public bool ContainsKey(string key)
			{
				var result = this.imageList.Find(entry => entry.Key == key);
				return result != null;
			}

			public IEnumerator GetEnumerator()
			{
				return this.imageList.GetEnumerator();
			}

			public int IndexOf(Sprite image)
			{
				int result = 0;
				bool found = false;
				foreach (var entry in this.imageList)
				{
					if (entry.Sprite == image)
					{
						found = true;
						break;
					}
					result++;
				}
				if (!found)
				{
					result = -1;
				}
				return result;
			}

			public int IndexOf(object image)
			{
				Sprite sprite = image as Sprite;

				if (sprite != null)
				{
					return IndexOf(sprite);
				}
				return -1;
			}

			public int IndexOfKey(string key)
			{
				int result = 0;
				bool found = false;
				foreach (var entry in this.imageList)
				{
					if (entry.Key == key)
					{
						found = true;
						break;
					}
					result++;
				}
				if (!found)
				{
					result = -1;
				}
				return result;
			}

			public void Remove(Sprite image)
			{
				int index = IndexOf(image);
				if (index != -1)
				{
					RemoveAt(index);
				}
			}

			public void Remove(object value)
			{
				Sprite sprite = value as Sprite;
				if (sprite != null)
				{
					Remove(sprite);
				}
			}

			public void RemoveAt(int index)
			{
				this.imageList.RemoveAt(index);
				this.Owner.RaiseOnChangedEvent();
			}

			public void RemoveByKey(string key)
			{
				int index = IndexOfKey(key);
				if (index != -1)
				{
					RemoveAt(index);
				}
			}

			public void SetKeyName(int index, string name)
			{
				this.imageList[index].Key = name;
				this.Owner.RaiseOnChangedEvent();
			}

			public bool IsReadOnly { get { return false; } }
			public bool IsFixedSize { get { return false; } }
			public object SyncRoot { get { return null; } }
			public bool IsSynchronized { get { return false; } }

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
