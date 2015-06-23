namespace Endgame
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;

	public class ColumnPanel : MonoBehaviour
	{
		public ColumnHeader ColumnHeader
		{
			get;
			set;
		}

		public Button Button;
		public Text Text
		{
			get
			{
				return this.Button.GetComponentInChildren<Text>();
			}
		}
		public Image HorizontalBorderImage;
		public Image VerticalBorderImage;

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

		public ItemPanel ItemPanel;

		public void Reset()
		{
			RectTransform items = this.ItemPanel.Items;
			items.sizeDelta = new Vector2(items.sizeDelta.x, 0);
		}
	}
}
