namespace Endgame
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime;
	using System.Runtime.Serialization;
	using UnityEngine;

	public class ColumnHeader : ICloneable
	{
		public const int DefaultWidth = 60;

		public ColumnHeader()
		{
			// Windows Forms seems to set this to 60 by default.
			this.Width = DefaultWidth;
		}

		public int Index
		{
			get
			{
				int result = -1;
				if (this.ListView != null)
				{
					result = this.ListView.Columns.IndexOf(this);
				}
				return result;
			}
		}

		private int width;

		public ListView ListView { get; set; }

		public string Name { get; set; }

		public object Tag { get; set; }

		private string text;
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

		private Font font;
		public Font Font
		{
			get
			{
				Font font = this.font;
				if (font == null)
				{
					if (this.ListView != null)
					{
						font = this.ListView.DefaultHeadingFont;
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

		private int fontSize = -1;
		public int FontSize
		{
			get
			{
				int fontSize = this.fontSize;
				if (fontSize == -1)
				{
					if (this.ListView != null)
					{
						if (this.ListView != null)
						{
							fontSize = this.ListView.DefaultHeadingFontSize;
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

		private bool hasFontStyleBeenSet = false;
		private FontStyle fontStyle;
		public FontStyle FontStyle
		{
			get
			{
				FontStyle fontStyle = this.fontStyle;
				if (!this.hasFontStyleBeenSet)
				{
					if (this.ListView != null)
					{
						if (this.ListView != null)
						{
							fontStyle = this.ListView.DefaultHeadingFontStyle;
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

		public int Width
		{
			get
			{
				return this.width;
			}

			set
			{
				this.width = value;

				this.OnModified();
			}
		}

		public object Clone()
		{
			ColumnHeader clone = new ColumnHeader();

			clone.ListView = this.ListView;
			clone.Name = this.Name;
			clone.Tag = this.Tag;
			clone.Text = this.Text;
			clone.Width = this.Width;

			return clone;
		}

		public ColumnPanel ColumnPanelInHierarchy
		{
			get;
			set;
		}

		private void OnModified()
		{
			if (this.ListView != null)
			{
				this.ListView.RebuildHierarchy();
			}
		}

		private bool hasForeColorBeenSet = false;
		private Color foreColor;
		public Color ForeColor
		{
			get
			{
				Color foreColor = this.foreColor;
				if (!hasForeColorBeenSet)
				{
					if (this.ListView != null)
					{
						foreColor = this.ListView.DefaultHeadingTextColor;
					}
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

		private bool hasBackColorBeenSet = false;
		private Color backColor;
		public Color BackColor
		{
			get
			{
				Color backColor = this.backColor;
				if (!hasBackColorBeenSet)
				{
					if (this.ListView != null)
					{
						backColor = this.ListView.DefaultHeadingBackgroundColor;
					}
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
	}
}
