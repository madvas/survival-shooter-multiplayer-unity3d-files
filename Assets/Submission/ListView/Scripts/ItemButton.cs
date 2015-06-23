namespace Endgame
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class ItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		public Image BackgroundImage;
		public Image HorizontalBorderImage;
		public Image VerticalBorderImage;
		public Text Text;
		public Image Image;
		public RectTransform CustomControlParent;
		private int margin;
		public int Margin { get { return this.margin; } }

		public void Awake()
		{
			this.margin = (int)this.Text.rectTransform.offsetMin.x;
		}

		public ListViewItem ListViewItem
		{
			get;
			set;
		}

		public ListViewItem.ListViewSubItem ListViewSubItem
		{
			get;
			set;
		}

		private int horizontalGridLineSize = 0;
		public int HorizontalGridLineSize
		{
			get
			{
				return this.horizontalGridLineSize;
			}

			set
			{
				this.horizontalGridLineSize = value;

				if (this.horizontalGridLineSize > 0)
				{
					this.HorizontalBorderImage.gameObject.SetActive(true);
					this.HorizontalBorderImage.transform.localScale = new Vector3(this.HorizontalBorderImage.transform.localScale.x, this.horizontalGridLineSize, this.HorizontalBorderImage.transform.localScale.z);
				}
				else
				{
					this.HorizontalBorderImage.gameObject.SetActive(false);
				}
			}
		}

		private int verticalGridLineSize = 0;
		public int VerticalGridLineSize
		{
			get
			{
				return this.verticalGridLineSize;
			}

			set
			{
				this.verticalGridLineSize = value;

				if (this.verticalGridLineSize > 0)
				{
					this.VerticalBorderImage.gameObject.SetActive(true);

					this.VerticalBorderImage.transform.localScale = new Vector3(-this.verticalGridLineSize, this.VerticalBorderImage.transform.localScale.y, this.VerticalBorderImage.transform.localScale.z);
				}
				else
				{
					this.VerticalBorderImage.gameObject.SetActive(false);
				}
			}
		}

		public Color GridLineColor
		{
			get
			{
				return this.VerticalBorderImage.color;
			}

			set
			{
				this.VerticalBorderImage.color = value;
				this.HorizontalBorderImage.color = value;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.ListViewItem != null)
			{
				this.ListViewItem.Owner.OnItemMouseEnter(this);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.ListViewItem != null)
			{
				this.ListViewItem.Owner.OnItemMouseExit(this);
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.ListViewItem != null)
			{
				this.ListViewItem.Owner.OnSubItemClicked(eventData, this);
			}
		}
	}
}
