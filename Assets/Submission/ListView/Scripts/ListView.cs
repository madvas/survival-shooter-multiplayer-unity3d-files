//#define ENABLE_PROFILING

namespace Endgame
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEngine.EventSystems;
	using UnityEngine.Events;
	using System;

	public partial class ListView : MonoBehaviour
	{
		public GameObject ColumnPanelPrefab;
		public GameObject ItemButtonPrefab;
		private RectTransform columnsPanel;
		private RectTransform columns;
		private GameObject horizontalScrollBar;
		private GameObject verticalScrollBar;
		private int layoutSuspendedCount = 0;
		private float previousButtonClickTime = -1000;
		private ItemButton previousButtonClickItemButton = null;
		private const float doubleClickTimeOut = 0.5f;
		private bool suppressSelectionEvent = false;
		private float columnsLocalXPosition = 0;
		private float itemsLocalYPosition = 0;
		private float itemHoverStartTime = -1;
		private ItemButton hoverItemButton;
		private bool suspendItemChangedEvents = false;
		private static bool InstantiateItemsFromPools = false;
		private static bool InstantiateColumnsFromPools = false;
		public static bool SetDefaultValuesManually = false;
		private bool ignoreLayout = false;
		private RectTransform dummyScrollContents;
		private bool suppressDummyScrollRectEvents = false;
		private int indexOfFirstVisibleItem = 0;
		private int indexOfLastVisibleItem = 0;
		private int indexOfFirstPresentItem = 0;
		private int indexOfLastPresentItem = 0;
		private bool createMinimalItems = true;
		//private bool createMinimalItems = false;
		private bool destroyed = false;
		private bool ignoreNextScrollRectEvent = false;

		// Editable heading properties.
		public Color DefaultHeadingBackgroundColor = new Color(1, 1, 1, 0.0f);
		public Color DefaultHeadingTextColor = new Color(0, 0, 0, 1);
		public Font DefaultHeadingFont;
		public int DefaultHeadingFontSize = 14;
		public FontStyle DefaultHeadingFontStyle = FontStyle.Normal;
		public bool DefaultShowColumnHeaders = true;
		private bool showColumnHeaders;
		public bool ShowColumnHeaders
		{
			get
			{
				return this.showColumnHeaders;
			}
			set
			{
				this.showColumnHeaders = value;
				this.RebuildHierarchy();
			}
		}
		public int DefaultColumnHeaderHeight = 30;
		private int columnHeaderHeight = -1;
		public int ColumnHeaderHeight
		{
			get
			{
				int result = this.columnHeaderHeight;
				if (result == -1)
				{
					result = this.DefaultColumnHeaderHeight;
				}

				return result;
			}

			set
			{
				this.columnHeaderHeight = value;
				this.RebuildHierarchy();
			}
		}

		// Editable item properties.
		public Color DefaultItemBackgroundColor = new Color(1, 1, 1, 0.0f);
		public Color DefaultItemTextColor = new Color(0, 0, 0, 1);
		public Color DefaultSelectedItemColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
		public bool DefaultChangeTextColorOnSelection = false;
		public Color DefaultSelectedItemTextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		public Font DefaultItemFont;
		public int DefaultItemFontSize = 14;
		public FontStyle DefaultItemFontStyle = FontStyle.Normal;
		public int DefaultItemButtonHeight = 30;
		private int itemButtonHeight = -1;
		public int ItemButtonHeight
		{
			get
			{
				int result = this.itemButtonHeight;
				if ((this.smallImageList != null) && this.smallImageList.WasImageSizeSet)
				{
					int verticalMargin = 5;
					result = verticalMargin + (int)this.smallImageList.ImageSize.y + verticalMargin;
				}
				else if (result == -1)
				{
					result = this.DefaultItemButtonHeight;
				}

				return result;
			}

			set
			{
				this.itemButtonHeight = value;
				this.RebuildHierarchy();
			}
		}
		private float itemHoverTime = 0.5f;
		public float ItemHoverTime
		{
			get { return this.itemHoverTime; }
			set { this.itemHoverTime = value; }
		}

		// Other editable properties.
		public Color DefaultControlBackgroundColor = new Color(1, 1, 1, 0.5f);
		public Color DefaultScrollBarBackgroundColor = new Color(1, 1, 1, 0.5f);
		public Color DefaultScrollBarThumbColor = new Color(1, 1, 1, 0.5f);
		public int DefaultHorizontalGridLineSize = 0;
		public int DefaultVerticalGridLineSize = 0;
		private int horizontalGridLineSize;
		public int HorizontalGridLineSize
		{
			get
			{
				return this.horizontalGridLineSize;
			}

			set
			{
				this.horizontalGridLineSize = value;
				this.RebuildHierarchy();
			}
		}
		public Color DefaultGridLineColor = new Color(1, 1, 1, 0.1f);
		private Color gridLineColor;
		public Color GridLineColor
		{
			get
			{
				return this.gridLineColor;
			}

			set
			{
				this.gridLineColor = value;
				this.RebuildHierarchy();
			}
		}

		private int verticalGridLineSize;
		public int VerticalGridLineSize
		{
			get
			{
				return this.verticalGridLineSize;
			}

			set
			{
				this.verticalGridLineSize = value;
				this.RebuildHierarchy();
			}
		}

		public bool DefaultColumnHeaderGridLines = false;
		private bool columnHeaderGridLines;
		public bool ColumnHeaderGridLines
		{
			get
			{
				return this.columnHeaderGridLines;
			}
			set
			{
				this.columnHeaderGridLines = value;
				this.RebuildHierarchy();
			}
		}

		private Color selectedItemBackgroundColor;
		public Color SelectedItemBackgroundColor
		{
			get
			{
				return this.selectedItemBackgroundColor;
			}

			set
			{
				this.selectedItemBackgroundColor = value;
				this.RefreshSelection();
			}
		}

		private bool changeTextColorOnSelection = false;
		public bool ChangeTextColorOnSelection
		{
			get
			{
				return this.changeTextColorOnSelection;
			}

			set
			{
				this.changeTextColorOnSelection = value;
				this.RefreshSelection();
			}
		}

		private Color selectedItemTextColor;
		public Color SelectedItemTextColor
		{
			get
			{
				Color result = this.ForeColor;
				if (this.changeTextColorOnSelection)
				{
					result = this.selectedItemTextColor;
				}
				return result;
			}

			set
			{
				this.selectedItemTextColor = value;
				this.RefreshSelection();
			}
		}

		private Color scrollBarBackgroundColor;
		public Color ScrollBarBackgroundColor
		{
			get
			{
				return this.scrollBarBackgroundColor;
			}

			set
			{
				this.scrollBarBackgroundColor = value;
				this.RebuildHierarchy();
			}
		}

		private Color scrollBarThumbColor;
		public Color ScrollBarThumbColor
		{
			get
			{
				return this.scrollBarThumbColor;
			}

			set
			{
				this.scrollBarThumbColor = value;
				this.RebuildHierarchy();
			}
		}

		public bool AnyColumnCanStretchToFill
		{
			get;
			set;
		}

		public Vector2 Size
		{
			get
			{
				return (this.transform as RectTransform).sizeDelta;
			}

			set
			{
				(this.transform as RectTransform).sizeDelta = value;
			}
		}

		private string gamepadInputNameForActivate = "Submit";
		public string GamepadInputNameForActivate
		{
			get { return this.gamepadInputNameForActivate; }
			set { this.gamepadInputNameForActivate = value; }
		}
		private string gamepadInputNameForPageUp = "PageUp";
		public string GamepadInputNameForPageUp
		{
			get { return this.gamepadInputNameForPageUp; }
			set { this.gamepadInputNameForPageUp = value; }
		}
		private string gamepadInputNameForPageDown = "PageDown";
		public string GamepadInputNameForPageDown
		{
			get { return this.gamepadInputNameForPageDown; }
			set { this.gamepadInputNameForPageDown = value; }
		}

		private bool canScrollHorizontally = false;
		private bool canScrollVertically = false;
		private bool downButtonDown = false;
		private bool upButtonDown = false;
		private float downButtonDownTime = 0;
		private float upButtonDownTime = 0;
		private bool initialised = false;
		public bool Initialised { get { return this.initialised; } }
		private bool waitForKeyRelease = false;
		private float waitForKeyReleaseTime = 0;

		void Awake()
		{
			this.columnsPanel = this.transform.Find("ColumnsPanel") as RectTransform;
			this.columns = this.columnsPanel.Find("Columns") as RectTransform;
			this.horizontalScrollBar = this.transform.Find("HorizontalScrollBar").gameObject;
			this.verticalScrollBar = this.transform.Find("VerticalScrollBar").gameObject;

			columnHeaders = new ColumnHeaderCollection(this);
			items = new ListViewItemCollection(this);
			selectedIndices = new SelectedIndexCollection(this);
			selectedItems = new SelectedListViewItemCollection(this);

			Transform dummyScrollContentsTransform = this.transform.Find("ColumnsPanel/DummyScrollContents");
			if (dummyScrollContentsTransform != null)
			{
				this.dummyScrollContents = dummyScrollContentsTransform.GetComponent<RectTransform>();
			}

			// When the application is started, set the default colours from the
			// values set in the editor.
			this.SetDefaultValues();

			// Disable keyboard navigation on the scrollbars, because the listview
			// performs its own navigation.
			List<GameObject> scrollbars = new List<GameObject>() { this.horizontalScrollBar, this.verticalScrollBar };
			foreach (var scrollbar in scrollbars)
			{
				var navigation = scrollbar.GetComponent<Scrollbar>().navigation;
				navigation.mode = Navigation.Mode.None;
				scrollbar.GetComponent<Scrollbar>().navigation = navigation;
			}

			this.initialised = true;
		}

		public void Start()
		{
			CheckCanvasRenderMode();
		}

		void Update()
		{
			this.UpdateMouseWheelScrolling();
			this.UpdateItemHover();

			if (this.ContainsFocus())
			{
				this.HandleGamepadInput();
			}

			// If the scrollbar is clicked on, it will clear the focus, because
			// it is set to navigation mode none (which is necessary so that
			// it won't be part of the navigation cycle).
			// Thus, it is necessary to return the focus to the listview here
			// by detecting whether the click was over its bounds.
			if
			(
				Input.GetMouseButtonDown(0) && 
				this.ContainsPoint(Input.mousePosition) &&
				(EventSystem.current.currentSelectedGameObject == null)
			)
			{
				this.Select();
			}
		}

		public void SetVerticalScrollBarValue(float value)
		{
			// [NK] Invert the scroll value so that it works correctly with the
			// [NK] "Bottom to top" orientation (this is necessary for clicking
			// [NK] on the scroll background to work correctly).
			value = Mathf.Clamp01(value);
			value = 1.0f - value;

			float currentValue = this.verticalScrollBar.GetComponent<Scrollbar>().value;
			this.verticalScrollBar.GetComponent<Scrollbar>().value = value;

			// Unity won't call the changed callback function unless the value
			// has changed, but the listview needs it to be called in this case,
			// so it must be called manually here.
			if (currentValue == value)
			{
				OnVerticalScrollValueChanged(value);
			}
		}

		public void SetHorizontalScrollBarValue(float value)
		{
			float currentValue = this.horizontalScrollBar.GetComponent<Scrollbar>().value;

			this.horizontalScrollBar.GetComponent<Scrollbar>().value = value;

			if (currentValue == value)
			{
				OnHorizontalScrollValueChanged(value);
			}
		}

		private void UpdateScrollBars()
		{
			this.canScrollHorizontally = false;
			this.canScrollVertically = false;

			if (this.columnsPanel == null)
			{
				return;
			}

			RectTransform listView = this.transform as RectTransform;
			RectTransform horizontalScrollBar = this.horizontalScrollBar.transform as RectTransform;
			RectTransform verticalScrollBar = this.verticalScrollBar.transform as RectTransform;
			float verticalScrollBarWidth = verticalScrollBar.sizeDelta.x;
			float horizontalScrollBarHeight = horizontalScrollBar.sizeDelta.y;
			//Debug.Log("verticalScrollBarWidth " + verticalScrollBarWidth);
			//Debug.Log("horizontalScrollBarHeight " + horizontalScrollBarHeight);
			float horizontalProportionRevealed = 1;
			float verticalProportionRevealed = 1;

			// Get the width of the columns panel.
			float extendedColumnsPanelWidth = this.GetElementWidth(listView);
			float originalColumnsPanelWidth = extendedColumnsPanelWidth - verticalScrollBarWidth;
			//Debug.Log("originalColumnsPanelWidth " + originalColumnsPanelWidth);
			//Debug.Log("extendedColumnsPanelWidth " + extendedColumnsPanelWidth);

			// Get the height of the columns panel.
			float extendedColumnsPanelHeight = this.GetElementHeight(listView);
			float originalColumnsPanelHeight = extendedColumnsPanelHeight - horizontalScrollBarHeight;
			//Debug.Log("originalColumnsPanelHeight " + originalColumnsPanelHeight);
			//Debug.Log("extendedColumnsPanelHeight " + extendedColumnsPanelHeight);

			// If there is at least one column...
			if (this.activeColumnPanels.Count > 0)
			{
				// Get the width of the columns object.
				float columnsWidth = this.GetElementWidth(this.columns);

				// Get the height of the items object.
				float itemsHeight = 0;
				if (this.firstItemsObject != null)
				{
					itemsHeight = this.GetElementHeight(this.firstItemsObject);
				}
				float itemsPlusHeadingHeight = itemsHeight + headingButtonHeight;
				//Debug.Log("columnsWidth " + columnsWidth);
				//Debug.Log("itemsHeight " + itemsHeight);

				// Determine whether a horizontal scroll bar is required.
				bool lastColumnStretchesToFill = (this.columnHeaders[this.columnHeaders.Count - 1].Width == -2);

				if (columnsWidth > (extendedColumnsPanelWidth))
				{
					this.canScrollHorizontally = true;
				}
				else if ((columnsWidth > originalColumnsPanelWidth) && (itemsPlusHeadingHeight > originalColumnsPanelHeight))
				{
					if ((columnsWidth == extendedColumnsPanelWidth) && (lastColumnStretchesToFill))
					{
					}
					else
					{
						this.canScrollHorizontally = true;
					}
				}

				// Determine whether a vertical scroll bar is required.
				if (itemsPlusHeadingHeight > (extendedColumnsPanelHeight))
				{
					this.canScrollVertically = true;
				}
				else if ((itemsPlusHeadingHeight > originalColumnsPanelHeight) && (columnsWidth > originalColumnsPanelWidth))
				{
					if ((columnsWidth == extendedColumnsPanelWidth) && (lastColumnStretchesToFill))
					{
					}
					else
					{
						this.canScrollVertically = true;
					}
				}

				// Resize the columns panel depending on the status of the scroll bars,
				// and calculate the proportion revealed.
				float columnsPanelWidth = extendedColumnsPanelWidth;
				float columnsPanelRightOffset = 0;
				if (this.canScrollVertically)
				{
					columnsPanelRightOffset = verticalScrollBarWidth;
					columnsPanelWidth = originalColumnsPanelWidth;
				}
				horizontalProportionRevealed = columnsPanelWidth / columnsWidth;

				float columnsPanelHeight = extendedColumnsPanelHeight;
				float columnsPanelBottomOffset = 0;
				if (this.canScrollHorizontally)
				{
					columnsPanelBottomOffset = horizontalScrollBarHeight;
					columnsPanelHeight = originalColumnsPanelHeight;
				}
				verticalProportionRevealed = columnsPanelHeight / itemsPlusHeadingHeight;

				// Apply the columns panel offsets.
				var offsetMin = this.columnsPanel.offsetMin;
				var offsetMax = this.columnsPanel.offsetMax;
				offsetMin.y = columnsPanelBottomOffset;
				offsetMax.x = -columnsPanelRightOffset;
				this.columnsPanel.offsetMin = offsetMin;
				this.columnsPanel.offsetMax = offsetMax;

				// Apply the same offsets to the scroll bars.
				offsetMin = verticalScrollBar.offsetMin;
				offsetMax = horizontalScrollBar.offsetMax;
				offsetMin.y = columnsPanelBottomOffset;
				offsetMax.x = -columnsPanelRightOffset;
				verticalScrollBar.offsetMin = offsetMin;
				horizontalScrollBar.offsetMax = offsetMax;
			}

			this.horizontalScrollBar.SetActive(this.canScrollHorizontally);
			this.verticalScrollBar.SetActive(this.canScrollVertically);

			// Set the scroll bar thumb sizes according to the amount of scrolling allowed.
			// (There seems to be a bug where the scroll thumb won't move if its size
			// is too large, so limit it here.)
			Scrollbar horizontalScrollBarScript = this.horizontalScrollBar.GetComponent<Scrollbar>();
			Scrollbar verticalScrollBarScript = this.verticalScrollBar.GetComponent<Scrollbar>();
			float maximumProportionRevealed = 0.9f;
			horizontalScrollBarScript.size = Mathf.Min(horizontalProportionRevealed, maximumProportionRevealed);
			verticalScrollBarScript.size = Mathf.Min(verticalProportionRevealed, maximumProportionRevealed);
		}

		private void UpdateMouseWheelScrolling()
		{
			// Allow the mouse wheel to perform vertical scrolling.
			if (this.ContainsPoint(Input.mousePosition))
			{
				float verticalScroll = Input.GetAxis("Mouse ScrollWheel");

				// [NK] Workaround for the "bottom to top" scrollbar bug.
				//this.verticalScrollBar.GetComponent<Scrollbar>().value -= verticalScroll;
				this.verticalScrollBar.GetComponent<Scrollbar>().value += verticalScroll;
			}
		}

		private void SizeItemsObjectToContents(RectTransform itemsObject)
		{
			float width = itemsObject.sizeDelta.x;
			float height = this.CalculateItemsObjectHeight(itemsObject);

			itemsObject.sizeDelta = new Vector2(width, height);
		}

		private void AbbreviateButtonText(RectTransform buttonTransform, float textRegionWidth)
		{
			RectTransform textTransform = buttonTransform.Find("Text").transform as RectTransform;
			Text text = textTransform.GetComponent<Text>();
			int textWidth = Mathf.RoundToInt(text.preferredWidth);
			textRegionWidth = Mathf.RoundToInt(textRegionWidth);
			string baseText = text.text;

			while (textWidth > textRegionWidth)
			{
				if (baseText.Length > 0)
				{
					baseText = baseText.Substring(0, baseText.Length - 1);
					text.text = baseText + "...";
					textWidth = Mathf.RoundToInt(text.preferredWidth);
				}
				else
				{
					break;
				}
			}
		}

		private void AbbreviateItemButtonText(ItemButton itemButton, Image image, bool displayImage)
		{
			// If the button's text is too long to fit within the column panel, 
			// truncate it and add an ellipsis.

			Text text = itemButton.Text;

			// As the item button text may have been abbreviated previously,
			// reinstate the unabbreviated text before abbreviating it here.
			text.text = itemButton.ListViewSubItem.Text;

			// Get the owning column panel by navigating from the item button to
			// the items, to the item panel, and then the column panel.
			GameObject columnPanel = itemButton.transform.parent.parent.parent.gameObject;
			RectTransform columnPanelTransform = columnPanel.transform as RectTransform;

			float columnPanelWidth = columnPanelTransform.sizeDelta.x;
			int marginBetweenButtonEdgeAndTextRegion = itemButton.Margin;
			float textRegionWidth = columnPanelWidth - (marginBetweenButtonEdgeAndTextRegion * 2);

			if (displayImage)
			{
				int imageWidth = (int)image.rectTransform.sizeDelta.x;
				textRegionWidth =
					columnPanelWidth -
					(
						marginBetweenButtonEdgeAndTextRegion +
						imageWidth +
						marginBetweenButtonEdgeAndTextRegion +
						marginBetweenButtonEdgeAndTextRegion
					);
			}

			this.AbbreviateButtonText(itemButton.transform as RectTransform, textRegionWidth);
		}

		private void AbbreviateColumnHeaderText(RectTransform headingButtonTransform)
		{
			// If the button's text is too long to fit within the column panel, 
			// truncate it and add an ellipsis.

			RectTransform textTransform = headingButtonTransform.Find("Text").transform as RectTransform;
			Text text = textTransform.GetComponent<Text>();

			// Get the owning column panel by navigating from the heading button to
			// the column panel.
			GameObject columnPanel = headingButtonTransform.parent.gameObject;
			RectTransform columnPanelTransform = columnPanel.transform as RectTransform;

			// As the column header text may have been abbreviated previously,
			// reinstate the unabbreviated text before abbreviating it here.
			ColumnPanel columnPanelScript = columnPanel.GetComponent<ColumnPanel>();
			text.text = columnPanelScript.ColumnHeader.Text;

			float columnPanelWidth = columnPanelTransform.sizeDelta.x;

			float marginBetweenButtonEdgeAndTextRegion = textTransform.offsetMin.x;
			float textRegionWidth = columnPanelWidth - (marginBetweenButtonEdgeAndTextRegion * 2);

			this.AbbreviateButtonText(headingButtonTransform, textRegionWidth);
		}

		private void SizeColumnsObjectToContents(RectTransform columns)
		{
			float width = this.CalculateColumnsObjectWidth();
			float height = columns.sizeDelta.y;

			columns.sizeDelta = new Vector2(width, height);
		}

		private float GetElementWidth(RectTransform transform)
		{
			Vector3[] localCorners = new Vector3[4];
			transform.GetLocalCorners(localCorners);

			return Mathf.Abs(localCorners[0].x - localCorners[2].x);
		}

		private float GetElementHeight(RectTransform transform)
		{
			Vector3[] localCorners = new Vector3[4];
			transform.GetLocalCorners(localCorners);

			return Mathf.Abs(localCorners[0].y - localCorners[1].y);
		}

		private bool LayoutSuspended
		{
			get
			{
				return this.ignoreLayout || (this.layoutSuspendedCount > 0);
			}
		}

		public void RebuildHierarchy()
		{
			if (this.LayoutSuspended)
			{
				return;
			}

			// Recreate the Unity hierarchy to reflect the contents of the Windows Forms data
			// structures.
			this.RebuildHierarchyInternal();

			// Now that all the controls are the correct size, update the scroll bars,
			// and then the columns. (The columns are updated after all the control dimensions
			// have been set.)
			this.UpdateScrollBarsAndColumns();
		}

		public void UpdateScrollBarsAndColumns()
		{
#if ENABLE_PROFILING
			using (new Endgame.CodeBlockTimer("UpdateScrollBarsAndColumns: updatescroll "))
#endif
			{
				// Determine whether scroll bars are required, and display
				// them if they are.
				// (This will also resize the scrollable panel to accommodate 
				// the scroll bars.)
				this.UpdateScrollBars();
			}

#if ENABLE_PROFILING
			using (new Endgame.CodeBlockTimer("UpdateScrollBarsAndColumns: updatecol "))
#endif
			{
				// Since the scrollable panel may have been resized to accommodate
				// the scroll bars, update the columns here.
				// (This will recalculate the column widths. If the final column
				// has a width of -2, its actual width needs to be recalculated
				// here, now that the scrollable panel has been resized.)
				foreach (ColumnHeader columnHeader in this.Columns)
				{
					// Only recalculate the scroll value once all the column
					// widths have been applied.
					this.UpdateColumnInternal(columnHeader);
				}
			}

			this.UpdateDummyScrollRect();
		}

		private void RebuildHierarchyInternal()
		{
#if ENABLE_PROFILING
			using (new Endgame.CodeBlockTimer("RebuildHierarchy"))
#endif
			{
				// Set the back colour.
				this.GetComponent<Image>().color = this.BackColor;

				// Set the scrollbar colours.
				this.verticalScrollBar.GetComponent<Image>().color = this.scrollBarBackgroundColor;
				this.verticalScrollBar.transform.Find("Sliding Area/Handle").GetComponent<Image>().color = this.scrollBarThumbColor;
				this.horizontalScrollBar.GetComponent<Image>().color = this.scrollBarBackgroundColor;
				this.horizontalScrollBar.transform.Find("Sliding Area/Handle").GetComponent<Image>().color = this.scrollBarThumbColor;

#if ENABLE_PROFILING
				using (new Endgame.CodeBlockTimer("RebuildHierarchy: destroy"))
#endif
				{
					DestroyAllItemButtons();

					// Delete all the existing columns (and items).
					ReturnColumnPanelsToPool();
				}

				// Unlink the Windows Forms listview items from the hierarchy.
				foreach (ListViewItem item in this.Items)
				{
					item.ItemButtonInHierarchy = null;
				}

#if ENABLE_PROFILING
				using (new Endgame.CodeBlockTimer("RebuildHierarchy: add cols"))
#endif
				{
					// Add the new columns.
					foreach (ColumnHeader columnHeader in this.Columns)
					{
						this.AddColumnToHierarchy(columnHeader);
					}
				}

#if ENABLE_PROFILING
				using (new Endgame.CodeBlockTimer("RebuildHierarchy: add items"))
#endif
				{
					if (this.createMinimalItems)
					{
						this.HandleAddingWithMinimalItems();
					}
					else
					{
						// Add the new items.
						foreach (ListViewItem item in this.Items)
						{
							this.AddItemToHierarchy(item);
						}
					}
				}

#if ENABLE_PROFILING
				using (new Endgame.CodeBlockTimer("RebuildHierarchy: widths"))
#endif
				{
					// Reapply the column widths now that the items have been added.
					foreach (ColumnHeader columnHeader in this.Columns)
					{
						this.UpdateColumnInternal(columnHeader);
					}
				}

#if ENABLE_PROFILING
				using (new Endgame.CodeBlockTimer("RebuildHierarchy: scroll"))
#endif
				{
					this.RecalculateVerticalScrollValue(this.itemsLocalYPosition);
					this.RecalculateHorizontalScrollValue(this.columnsLocalXPosition);

					// Refresh the selection.
					this.RefreshSelection();
				}
			}
		}

		public void LayOutColumnPanels()
		{
			float xPosition = 0;

			// Lay out each column panel horizontally.
			foreach (Transform transform in this.activeColumnPanels)
			{
				RectTransform rectTransform = transform as RectTransform;
				if (rectTransform != null)
				{
					var localPosition = rectTransform.localPosition;
					localPosition.x = xPosition;
					rectTransform.localPosition = localPosition;
					float columnPanelWidth = rectTransform.sizeDelta.x;
					xPosition += columnPanelWidth;
				}
			}

			// Resize the columns object to fit its contents.
			this.SizeColumnsObjectToContents(this.columns);
		}

		public void AddColumnToHierarchy(ColumnHeader columnHeader)
		{
			GameObject columnPanel;
			if (InstantiateColumnsFromPools)
			{
				columnPanel = this.InstantiateColumnPanelFromPool();
			}
			else
			{
				columnPanel = GameObject.Instantiate(this.ColumnPanelPrefab) as GameObject;
			}

			columnPanel.transform.SetParent(this.columns, worldPositionStays: false);

			// Link the item in the hierarchy to its Windows Forms counterpart.
			ColumnPanel columnPanelScript = columnPanel.GetComponent<ColumnPanel>();
			columnPanelScript.ColumnHeader = columnHeader;
			columnHeader.ColumnPanelInHierarchy = columnPanelScript;

			// Hide the column header if requested.
			if (!this.showColumnHeaders)
			{
				// Set the column header button height to 0.
				RectTransform buttonTransform = columnPanelScript.Button.transform as RectTransform;
				var sizeDelta = buttonTransform.sizeDelta;
				sizeDelta.y = 0;
				buttonTransform.sizeDelta = sizeDelta;

				// Move the item panel up to cover the column button.
				RectTransform itemPanelTransform = columnPanelScript.transform.Find("ItemPanel").transform as RectTransform;

				var offsetMax = itemPanelTransform.offsetMax;
				offsetMax.y = 0;
				itemPanelTransform.offsetMax = offsetMax;
			}
			else
			{
				// Set the column header height.
				RectTransform buttonTransform = columnPanelScript.Button.GetComponent<RectTransform>();
				var sizeDelta = buttonTransform.sizeDelta;
				sizeDelta.y = this.ColumnHeaderHeight;
				buttonTransform.sizeDelta = sizeDelta;

				// Position the item panel below the column button.
				RectTransform itemPanelTransform = columnPanelScript.transform.Find("ItemPanel").transform as RectTransform;

				var offsetMax = itemPanelTransform.offsetMax;
				offsetMax.y = -this.ColumnHeaderHeight;
				itemPanelTransform.offsetMax = offsetMax;
			}

			// Update the column, but don't recalculate the scroll value yet
			// because not all columns have been added.
			// TODO: Is this unnecessary, as it's called later anyway?
			//this.UpdateColumnInternal(columnHeader);
		}

		public void AddItemToHierarchy(ListViewItem item)
		{
			int index = 0;

#if ENABLE_PROFILING
			using (new Endgame.CodeBlockTimer("AddItem: foreach col"))
#endif
			{
				foreach (Transform columnPanel in this.activeColumnPanels)
				{
					// Get the items object.
					ColumnPanel columnPanelScript = columnPanel.GetComponent<ColumnPanel>();
					RectTransform items = columnPanelScript.ItemPanel.Items;

					// Create an item button.
					GameObject itemButton;
					if (InstantiateItemsFromPools)
					{
						itemButton = this.InstantiateItemButtonFromPool();
					}
					else
					{
						itemButton = GameObject.Instantiate(this.ItemButtonPrefab) as GameObject;
					}
					RectTransform itemButtonTransform = itemButton.transform as RectTransform;

#if ENABLE_PROFILING
					using (new Endgame.CodeBlockTimer("AddItem: parent"))
#endif
					{
						// Add the item button as a child of the items object.
						itemButton.transform.SetParent(items, worldPositionStays: false);
					}

					// Link the item in the hierarchy to its Windows Forms counterpart.
					// (As there are multiple buttons that refer to the same ListViewItem,
					// just store the first button in the item.)
					ItemButton itemButtonScript = itemButton.GetComponent<ItemButton>();
					itemButtonScript.ListViewItem = item;
					itemButtonScript.ListViewSubItem = item.SubItems[index];
					if (index == 0)
					{
						item.ItemButtonInHierarchy = itemButtonScript;
					}

					// Set the item button's on click function.
					// (Instead of using OnClick, use OnPointerDown, as the control will 
					// respond more quickly this way.)
					CustomButton button = itemButtonTransform.GetComponent<CustomButton>();
					button.onPointerDown = new CustomButton.PointerDownEvent();
					button.onPointerDown.AddListener(new UnityAction(() => this.OnItemClicked(itemButtonScript)));
					index++;
				}
			}

#if ENABLE_PROFILING
			using (new Endgame.CodeBlockTimer("AddItem: updateitem"))
#endif
			{
				// Update the item, but don't refresh the selection yet because
				// all the items have not yet been added.
				this.UpdateItem(item, refreshSelection: false);
			}
		}

		private void UpdateColumnInternal(ColumnHeader columnHeader)
		{
			ColumnPanel columnPanel = columnHeader.ColumnPanelInHierarchy;

			if (columnPanel != null)
			{
				columnPanel.Button.GetComponent<Image>().color = columnHeader.BackColor;
				columnPanel.Text.color = columnHeader.ForeColor;
				columnPanel.Text.text = columnHeader.Text;
				columnPanel.Text.font = columnHeader.Font;
				columnPanel.Text.fontSize = columnHeader.FontSize;
				columnPanel.Text.fontStyle = columnHeader.FontStyle;
				columnPanel.Button.onClick = new Button.ButtonClickedEvent();
				columnPanel.Button.onClick.AddListener(new UnityAction(() => this.OnHeaderClicked(columnPanel)));

				// Set the grid line appearance.
				if (this.ColumnHeaderGridLines)
				{
					columnPanel.HorizontalGridLineSize = this.HorizontalGridLineSize;
					columnPanel.VerticalGridLineSize = this.VerticalGridLineSize;
					columnPanel.GridLineColor = this.GridLineColor;
				}

				this.SetColumnPanelWidth(columnPanel);

				this.AbbreviateColumnHeaderText(columnPanel.Button.transform as RectTransform);
			}
		}

		public void UpdateItem(ListViewItem listViewItem, bool refreshSelection = true)
		{
			// If the items have not been populated yet (such as when this
			// function is called from RebuildHierarchy), return.
			if (listViewItem.ItemButtonInHierarchy == null)
			{
				return;
			}

			// Find the index of the item in the items object.
			int itemIndexInHierarchy = listViewItem.Index;
			if (this.createMinimalItems && (itemIndexInHierarchy != -1))
			{
				itemIndexInHierarchy =
					listViewItem.ItemButtonInHierarchy.GetComponent<RectTransform>().GetSiblingIndex();
			}

			if (itemIndexInHierarchy != -1)
			{
				// Update each subitem's text.
				int index = 0;
				foreach (Transform columnPanel in this.activeColumnPanels)
				{
					// Get the items object.
					ColumnPanel columnPanelScript = columnPanel.GetComponent<ColumnPanel>();
					RectTransform items = columnPanelScript.ItemPanel.Items;

					// Get the item button at the index.
					ItemButton itemButton = items.GetChild(itemIndexInHierarchy).GetComponent<ItemButton>();
					RectTransform itemButtonTransform = itemButton.transform as RectTransform;

					// Set the item button's text.
					Text text = itemButton.Text;

					// Get the subitem to set the text properties from.
					// (This is the subitem for the current column, or the first subitem if
					// UseItemStyleForSubItems is set.)
					ListViewItem.ListViewSubItem subItem = listViewItem.SubItems[index];
					ListViewItem.ListViewSubItem subItemForStyle = listViewItem.SubItems[index];

					text.text = subItemForStyle.Text;

					if (listViewItem.UseItemStyleForSubItems)
					{
						subItemForStyle = listViewItem.SubItems[0];
					}

					text.font = subItemForStyle.Font;
					text.fontSize = subItemForStyle.FontSize;
					text.fontStyle = subItemForStyle.FontStyle;
					text.color = subItemForStyle.ForeColor;

					ItemButton itemButtonScript = itemButton.GetComponent<ItemButton>();
					Image buttonImage = itemButtonScript.BackgroundImage;
					buttonImage.color = subItemForStyle.BackColor;

					// Set the width of the item button to match the column width.
					var sizeDelta = itemButtonTransform.sizeDelta;
					RectTransform columnPanelTransform = columnPanel.transform as RectTransform;
					float columnPanelWidth = columnPanelTransform.sizeDelta.x;
					sizeDelta.x = columnPanelWidth;
					itemButtonTransform.sizeDelta = sizeDelta;

					// Set the grid line appearance.
					itemButtonScript.HorizontalGridLineSize = this.HorizontalGridLineSize;
					itemButtonScript.VerticalGridLineSize = this.VerticalGridLineSize;
					itemButtonScript.GridLineColor = this.GridLineColor;

					// Adjust the size and position of the text to accommodate the image,
					// if one has been specified.
					Sprite sprite = null;
					Image image = itemButton.Image;
					int itemButtonHeight = this.ItemButtonHeight;

					if (listViewItem.ImageList != null)
					{
						var images = listViewItem.ImageList.Images;

						if (!string.IsNullOrEmpty(subItem.ImageKey))
						{
							sprite = images[subItem.ImageKey];
						}
						else if (subItem.ImageIndex != -1)
						{
							sprite = images[subItem.ImageIndex];
						}

						// If the image list specifies an image size, set the image to
						// that size.
						// TODO: Should only the rows with images change size, rather than 
						// all rows? (Currently this matches the .NET ListView behaviour.)
						if (this.smallImageList.WasImageSizeSet)
						{
							// Resize the image horizontally.
							var imageSizeDelta = image.rectTransform.sizeDelta;
							imageSizeDelta.x = this.smallImageList.ImageSize.x;
							image.rectTransform.sizeDelta = imageSizeDelta;
						}
					}

					bool displayImage = (sprite != null);
					int horizontalMargin = itemButton.Margin;

					if (displayImage)
					{
						// Enable the image GameObject.
						if (!image.gameObject.activeSelf)
						{
							image.gameObject.SetActive(true);
						}

						// Set the sprite.
						image.sprite = sprite;

						// Reposition the text.
						RectTransform textTransform = text.rectTransform;
						var offsetMin = textTransform.offsetMin;
						int imageWidth = (int)image.rectTransform.sizeDelta.x;
						offsetMin.x = horizontalMargin + imageWidth + horizontalMargin;
						textTransform.offsetMin = offsetMin;
					}
					else
					{
						// Disable the image GameObject.
						if (image.gameObject.activeSelf)
						{
							image.gameObject.SetActive(false);
						}

						// Set the text position to default.
						RectTransform textTransform = text.rectTransform;
						var offsetMin = textTransform.offsetMin;
						offsetMin.x = horizontalMargin;
						textTransform.offsetMin = offsetMin;
					}

					// Adjust the item's text to display an ellipsis if it doesn't fit in the
					// text region.
					this.AbbreviateItemButtonText(itemButtonScript, image, displayImage);

					// Set the item button height.
					sizeDelta.y = itemButtonHeight;
					itemButtonTransform.sizeDelta = sizeDelta;

					// Set the item button's Y position.
					float yPosition = -(itemButtonHeight * listViewItem.Index);
					var localPosition = itemButtonTransform.localPosition;
					localPosition.y = yPosition;
					itemButtonTransform.localPosition = localPosition;

					// Resize the items object to fit its contents.
					this.SizeItemsObjectToContents(items);

					// Add the custom control, if one has been specified.
					Transform customControlParentTransform;

					customControlParentTransform =
						itemButtonTransform.Find("CustomControlParent");

					if (subItem.CustomControl != null)
					{
						if (subItem.CustomControl.transform.parent != customControlParentTransform)
						{
							subItem.CustomControl.SetParent(customControlParentTransform, worldPositionStays: false);
						}

						// Adjust the custom control's anchors so that it stretches to fit
						// the parent.
						var offsetMin = subItem.CustomControl.offsetMin;
						var offsetMax = subItem.CustomControl.offsetMax;
						var anchorMin = subItem.CustomControl.anchorMin;
						var anchorMax = subItem.CustomControl.anchorMax;
						offsetMin = Vector2.zero;
						offsetMax = Vector2.zero;
						anchorMin = Vector2.zero;
						anchorMax = Vector2.one;
						subItem.CustomControl.offsetMin = offsetMin;
						subItem.CustomControl.offsetMax = offsetMax;
						subItem.CustomControl.anchorMin = anchorMin;
						subItem.CustomControl.anchorMax = anchorMax;

						// Hide the text and image if there is a custom control being shown.
						if (text.gameObject.activeSelf)
						{
							text.gameObject.SetActive(false);
						}
						if (image.gameObject.activeSelf)
						{
							image.gameObject.SetActive(false);
						}
					}
					else
					{
						// Unparent the custom control.
						List<Transform> children = new List<Transform>();
						foreach (Transform child in customControlParentTransform)
						{
							children.Add(child);
						}
						foreach (Transform child in children)
						{
							child.SetParent(null);
						}

						if (!text.gameObject.activeSelf)
						{
							text.gameObject.SetActive(true);
						}
					}

					index++;
				}

				if (refreshSelection)
				{
					this.RefreshSelection();
				}

				// Send an ItemChanged event.
				if ((this.ItemChanged != null) && !this.suspendItemChangedEvents)
				{
					this.suspendItemChangedEvents = true;
					this.ItemChanged(listViewItem);
					this.suspendItemChangedEvents = false;
				}
			}
		}

		public void OnVerticalScrollValueChanged(float value)
		{
			// [NK] The value was inverted before it was set (to work with
			// [NK] the "Bottom to top" orientation) so recover the original
			// [NK] value by inverting it again here.
			value = Mathf.Clamp01(value);
			value = 1.0f - value;

			ColumnPanel firstColumnPanel = this.firstColumnPanel;

			if (firstColumnPanel != null)
			{
				// If there is at least one item...
				if (this.firstItemsObject.childCount > 0)
				{
					// Calculate the minimum and maximum values of the items object's
					// y position.
					float minimumY = 0;
					int itemsHeight = (int)this.GetElementHeight(this.firstItemsObject);
					float maximumY = Mathf.Max(itemsHeight - this.firstItemPanelHeight, 0);

					// Lerp between these values using the scroll value.
					float y = Mathf.Lerp(minimumY, maximumY, value);

					// For each column, set the items object's y position.
					foreach (Transform columnPanel in this.activeColumnPanels)
					{
						ColumnPanel columnPanelScript = columnPanel.GetComponent<ColumnPanel>();
						RectTransform items = columnPanelScript.ItemPanel.Items;
						var localPosition = items.localPosition;
						localPosition.y = y;
						items.localPosition = localPosition;
					}

					if (this.createMinimalItems)
					{
						// As HandleScrollingWithMinimalItems assumes that there are
						// enough items to require scrolling, only call it when this
						// is true.
						// Also, do not call the function if the layout is suspended,
						// as the hierarchy and items may not be in sync.
						// (For example, if the layout is suspended and Items.Clear is 
						// called, the items array will be cleared, but the hieararchy
						// won't be, meaning that HandleScrollingWithMinimalItems
						// can't be called safely.)
						if ((minimumY != maximumY) && !this.LayoutSuspended)
						{
							this.HandleScrollingWithMinimalItems();
						}
					}

					this.itemsLocalYPosition = y;
				}
			}

			// Update the dummy scroll rect.
			// [NK] Workaround for the "bottom to top" scrollbar bug.
			//this.SetDummyScrollRectVerticalScrollPosition(1.0f - value);
			this.SetDummyScrollRectVerticalScrollPosition(value);
		}

		public void OnHorizontalScrollValueChanged(float value)
		{
			if (this.columnsPanel == null)
			{
				return;
			}

			// If there is at least one column...
			if (this.columnsPanel.childCount > 0)
			{
				// Get the width of the columns panel.
				float columnsPanelWidth = this.GetElementWidth(this.columnsPanel);

				// Get the width of the columns object.
				float columnsWidth = this.GetElementWidth(this.columns);

				// Calculate the minimum and maximum values of the columns object's
				// x position.
				float minimumX = 0;
				float maximumX = Mathf.Min(-columnsWidth + columnsPanelWidth, 0);

				// Lerp between these values using the scroll value.
				float x = Mathf.Lerp(minimumX, maximumX, value);

				// Set the columns object's x position.
				var localPosition = this.columns.localPosition;
				localPosition.x = x;
				this.columns.localPosition = localPosition;

				this.columnsLocalXPosition = x;
			}

			// Update the dummy scroll rect.
			this.SetDummyScrollRectHorizontalScrollPosition(value);
		}

		public void OnItemClicked(ItemButton itemButton)
		{
			float timeSincePreviousClick = Time.time - this.previousButtonClickTime;
			this.previousButtonClickTime = Time.time;

			if (timeSincePreviousClick < doubleClickTimeOut &&
				this.previousButtonClickItemButton == itemButton)
			{
				// Double click.
				this.ActivateRowContainingButton(itemButton);

				this.previousButtonClickTime = -1000;
				this.previousButtonClickItemButton = null;
			}
			else
			{
				// Select the row that was clicked.			
				this.SelectedIndices.Add(itemButton.ListViewItem.Index);

				this.previousButtonClickItemButton = itemButton;
			}
		}

		public void OnSubItemClicked(PointerEventData pointerEventData, ItemButton itemButton)
		{
			if (this.SubItemClicked != null)
			{
				this.SubItemClicked(pointerEventData, itemButton.ListViewSubItem);
			}
		}

		private void OnHeaderClicked(ColumnPanel columnPanel)
		{
			if (this.ColumnClick != null)
			{
				int columnIndex = System.Array.IndexOf(this.columns.GetComponentsInChildren<ColumnPanel>(), columnPanel);

				ColumnClickEventArgs eventArgs = new ColumnClickEventArgs(columnIndex);

				this.ColumnClick(this, eventArgs);
			}
		}

		public void RefreshSelection()
		{
			// If a selection exists, reselect it.
			if (this.SelectedIndices.Count > 0)
			{
				this.suppressSelectionEvent = true;
				int selectedIndex = this.SelectedIndices[0];
				this.SelectedIndices.Clear();
				this.SelectedIndices.Add(selectedIndex);
				this.suppressSelectionEvent = false;
			}
		}

		public void OnSelectionClearedInternal()
		{
			if (this.SelectedIndices.Count > 0)
			{
				int selectedIndex = this.SelectedIndices[0];
				this.DeselectItemButton(selectedIndex);
				this.DeselectItem(selectedIndex);
				this.DeselectItemProperty(selectedIndex);
			}
		}

		public void OnSelectionRemovedInternal(int index)
		{
			this.DeselectItemButton(index);
			this.DeselectItem(index);
			this.DeselectItemProperty(index);
		}

		public void OnSelectionAddedInternal(int index, bool updateGameObjects)
		{
			int selectedIndex = this.SelectedIndices[0];
			if (updateGameObjects)
			{
				this.SelectItemButton(selectedIndex);
			}
			this.SelectItem(selectedIndex);
			this.SelectItemProperty(selectedIndex);
		}

		private void DeselectItemButton(int index)
		{
			ListViewItem itemToDeselect = this.Items[index];
			this.UpdateItemButtonColors(listViewItem: itemToDeselect, selected: false, focussed: ContainsFocus());
		}

		private void SelectItemButton(int index)
		{
			ListViewItem itemToSelect = this.Items[index];
			this.UpdateItemButtonColors(listViewItem: itemToSelect, selected: true, focussed: ContainsFocus());
		}

		private void SelectItem(int index)
		{
			// Update the SelectedItems collection using reflection
			// to call the private members that manipulate the data.
			// This is necessary because general code should not be
			// able to modify the SelectedItems collection, as it
			// must mirror the SelectedIndices collection.

			// Clear the selected items.
			HelperFunctions.CallPrivateMethod(this.SelectedItems, "ClearInternal");

			// Add the selected item.
			ListViewItem itemToAdd = this.Items[index];
			HelperFunctions.CallPrivateMethod(this.SelectedItems, "AddInternal", itemToAdd);
		}

		private void DeselectItem(int index)
		{
			// Clear the selected items.
			HelperFunctions.CallPrivateMethod(this.SelectedItems, "ClearInternal");
		}

		private void SelectItemProperty(int index)
		{
			ListViewItem itemToSelect = this.Items[index];
			HelperFunctions.CallPrivateMethod(itemToSelect, "SetSelectedFlagInternal", true);
		}

		private void DeselectItemProperty(int index)
		{
			ListViewItem itemToSelect = this.Items[index];
			HelperFunctions.CallPrivateMethod(itemToSelect, "SetSelectedFlagInternal", false);
		}

		private void ActivateRowContainingButton(ItemButton itemButton)
		{
			System.EventArgs eventArguments = new System.EventArgs();

			this.OnItemActivate(eventArguments);
		}

		protected virtual void OnItemActivate(System.EventArgs e)
		{
			if (this.ItemActivate != null)
			{
				this.ItemActivate(this, e);
			}
		}

		protected virtual void OnSelectedIndexChanged(System.EventArgs e)
		{
			if (this.suppressSelectionEvent)
			{
				return;
			}

			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged(this, e);
			}
		}

		public List<ItemButton> GetItemButtons(ListViewItem listViewItem)
		{
			List<ItemButton> result = new List<ItemButton>();
			int index = listViewItem.Index;

			if (this.createMinimalItems)
			{
				index = -1;
				if (listViewItem.ItemButtonInHierarchy != null)
				{
					index = listViewItem.ItemButtonInHierarchy.GetComponent<RectTransform>().GetSiblingIndex();
				}
			}

			if (index != -1)
			{
				foreach (Transform columnPanel in this.activeColumnPanels)
				{
					ColumnPanel columnPanelScript = columnPanel.GetComponent<ColumnPanel>();
					RectTransform items = columnPanelScript.ItemPanel.Items;
					result.Add(items.GetChild(index).GetComponent<ItemButton>());
				}
			}

			return result;
		}

		private void UpdateItemButtonColors(ListViewItem listViewItem, bool selected, bool focussed)
		{
			var colorWhenControlFocussed = this.SelectedItemBackgroundColor;
			var colorWhenControlNotFocussed = this.SelectedItemBackgroundColor;
			colorWhenControlNotFocussed.a *= 0.5f;
			var color = focussed ? colorWhenControlFocussed : colorWhenControlNotFocussed;

			List<ItemButton> itemButtons = this.GetItemButtons(listViewItem);
			foreach (ItemButton itemButton in itemButtons)
			{
				itemButton.BackgroundImage.color =
					selected ? color : itemButton.ListViewSubItem.BackColor;

				itemButton.Text.color =
					selected ? this.SelectedItemTextColor : itemButton.ListViewSubItem.ForeColor;
			}
		}

		public void SuspendLayout()
		{
			this.layoutSuspendedCount++;
		}

		public void ResumeLayout()
		{
			this.layoutSuspendedCount--;
			if (this.layoutSuspendedCount <= 0)
			{
				this.layoutSuspendedCount = 0;
				this.RebuildHierarchy();
			}
		}

		private void SortItems()
		{
			if (this.listViewItemSorter != null)
			{
				this.items.Sort(this.listViewItemSorter);
			}

			this.RebuildHierarchy();
		}

		private Transform GetFirstActiveChild(Transform parent)
		{
			Transform result = null;
			foreach (Transform child in parent)
			{
				if (child.gameObject.activeSelf)
				{
					result = child;
					break;
				}
			}
			return result;
		}

		private Transform GetLastActiveChild(Transform parent)
		{
			Transform result = null;
			int childCount = parent.childCount;

			for (int i = childCount - 1; i >= 0; i--)
			{
				Transform child = parent.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					result = child;
					break;
				}
			}
			return result;
		}

		private float GetLongestItemTextWidthPlusMargin(ColumnPanel columnPanel)
		{
			ItemPanel itemPanelScript = columnPanel.ItemPanel;
			RectTransform items = itemPanelScript.Items;

			float longestItemWidth = 0;
			//string longestString = "";

			var firstActiveChild = GetFirstActiveChild(items);
			if (firstActiveChild != null)
			{
				// Calculate the longest item text width, considering all the items in the listview,
				// be they visible or not.
				// TODO: Force a rebuild if an item's text is updated and it causes the maximum
				// width to change.
				RectTransform itemButtonTransform = firstActiveChild.GetComponent<RectTransform>();
				Text text = itemButtonTransform.GetComponentInChildren<Text>();

				int subItemIndex = columnPanel.ColumnHeader.Index;
				string currentText = text.text;
				foreach (ListViewItem listViewItem in this.Items)
				{
					text.text = listViewItem.SubItems[subItemIndex].Text;
					//if (text.preferredWidth > longestItemWidth) longestString = text.text;
					longestItemWidth = Mathf.Max(longestItemWidth, text.preferredWidth);
				}

				text.text = currentText;

				RectTransform textTransform = firstActiveChild.GetComponentInChildren<Text>().transform as RectTransform;
				float marginBetweenButtonEdgeAndTextRegion = textTransform.offsetMin.x;

				// Instead of multiplying the margin by 2, add the left and right margins
				// separately, as they may be different if an image is being displayed.
				ItemButton itemButton = itemButtonTransform.GetComponent<ItemButton>();
				float totalMargin = marginBetweenButtonEdgeAndTextRegion + itemButton.Margin;

				longestItemWidth += totalMargin;
				longestItemWidth = Mathf.Ceil(longestItemWidth);
			}
			else
			{
				longestItemWidth = ColumnHeader.DefaultWidth;
			}

			return longestItemWidth;
		}

		private float GetColumnHeaderTextWidthPlusMargin(ColumnPanel columnPanel)
		{
			float headingTextWidth = 0;
			Text text = columnPanel.Text;

			// As the item button text may have been abbreviated previously,
			// reinstate the unabbreviated text before reading the width.
			string originalText = columnPanel.ColumnHeader.Text;
			string currentText = text.text;
			text.text = originalText;
			headingTextWidth = text.preferredWidth;
			text.text = currentText;

			RectTransform textTransform = text.transform as RectTransform;
			float marginBetweenButtonEdgeAndTextRegion = textTransform.offsetMin.x;
			headingTextWidth += (marginBetweenButtonEdgeAndTextRegion * 2);

			return headingTextWidth;
		}

		private float GetRemainingWidthForColumnPanel(ColumnPanel columnPanel)
		{
			float listViewWidth = this.GetElementWidth(this.transform as RectTransform);
			float columnsPanelAdjustment = (this.columnsPanel.sizeDelta.x);
			float visibleListViewWidth = listViewWidth + columnsPanelAdjustment;
			float columnWidthsAggregate = 0;
			foreach (ColumnHeader columnHeader in this.columnHeaders)
			{
				if (columnHeader == columnPanel.ColumnHeader)
				{
					continue;
				}

				columnWidthsAggregate += this.GetElementWidth(columnHeader.ColumnPanelInHierarchy.transform as RectTransform);
			}

			float remainingWidth = (visibleListViewWidth - columnWidthsAggregate);
			remainingWidth = Mathf.Max(ColumnHeader.DefaultWidth, remainingWidth);

			return remainingWidth;
		}

		public void SetColumnPanelWidth(ColumnPanel columnPanel)
		{
			float width = columnPanel.ColumnHeader.Width;

			if (width == -1)
			{
				width = this.GetLongestItemTextWidthPlusMargin(columnPanel);
			}
			else if (width == -2)
			{
				if (this.AnyColumnCanStretchToFill || (columnPanel.ColumnHeader.Index == (this.Columns.Count - 1)))
				{
					width = this.GetRemainingWidthForColumnPanel(columnPanel);
				}
				else
				{
					width = this.GetColumnHeaderTextWidthPlusMargin(columnPanel);
				}
			}

			// Set the column panel width.
			RectTransform columnPanelTransform = columnPanel.transform as RectTransform;
			var sizeDelta = columnPanelTransform.sizeDelta;
			sizeDelta.x = width;
			columnPanelTransform.sizeDelta = sizeDelta;

			// Set the column heading button width.
			RectTransform headingButtonTransform =
				columnPanel.Button.transform as RectTransform;
			sizeDelta = headingButtonTransform.sizeDelta;
			sizeDelta.x = width;
			headingButtonTransform.sizeDelta = sizeDelta;

			// Reposition the column panels so that they are aligned properly.
			this.LayOutColumnPanels();

#if ENABLE_PROFILING
			using (new Endgame.CodeBlockTimer("AddItem: setwidth update items (" + this.Items.Count + ")"))
#endif
			{
				// Update every item's text now that the column width has changed.
				foreach (ListViewItem listViewItem in this.Items)
				{
					this.UpdateItem(listViewItem);
				}
			}
		}

		private void SetDefaultValues()
		{
			if (!SetDefaultValuesManually)
			{
				this.SuspendLayout();
			}
			else
			{
				this.ignoreLayout = true;
			}

			this.BackColor = this.DefaultControlBackgroundColor;
			this.ForeColor = this.DefaultItemTextColor;
			this.SelectedItemBackgroundColor = this.DefaultSelectedItemColor;
			this.SelectedItemTextColor = this.DefaultSelectedItemTextColor;
			this.ScrollBarBackgroundColor = this.DefaultScrollBarBackgroundColor;
			this.ScrollBarThumbColor = this.DefaultScrollBarThumbColor;
			this.HorizontalGridLineSize = this.DefaultHorizontalGridLineSize;
			this.VerticalGridLineSize = this.DefaultVerticalGridLineSize;
			this.GridLineColor = this.DefaultGridLineColor;
			this.ColumnHeaderGridLines = this.DefaultColumnHeaderGridLines;
			this.ChangeTextColorOnSelection = this.DefaultChangeTextColorOnSelection;
			this.ShowColumnHeaders = this.DefaultShowColumnHeaders;

			if (!SetDefaultValuesManually)
			{
				this.ResumeLayout();
			}
			else
			{
				this.ignoreLayout = false;
			}
		}

		private void RecalculateVerticalScrollValue(float itemsLocalYPosition)
		{
			if (this.firstColumnPanel != null)
			{
				// If there is at least one item...
				if (this.firstItemsObject.childCount > 0)
				{
					float itemsHeight = (int)this.GetElementHeight(this.firstItemsObject);

					//Debug.Log("itemsHeight " + itemsHeight);

					itemsHeight = Mathf.Max(itemsHeight - this.firstItemPanelHeight, 0);

					//Debug.Log("firstItemPanelHeight " + firstItemPanelHeight);
					//Debug.Log("itemslocaly " + itemsLocalYPosition);
					//Debug.Log("itemsHeight2 " + itemsHeight);

					float newScrollValue = 0;
					if (itemsHeight != 0)
					{
						newScrollValue = itemsLocalYPosition / itemsHeight;
					}

					//Debug.Log("newscrollvalue " + newScrollValue);

					this.SetVerticalScrollBarValue(newScrollValue);
				}
			}
		}

		private void RecalculateHorizontalScrollValue(float columnsLocalXPosition)
		{
			if (this.columnsPanel == null)
			{
				return;
			}

			// If there is at least one column...
			if (this.columnsPanel.childCount > 0)
			{
				// Get the width of the columns panel.
				float columnsPanelWidth = this.GetElementWidth(this.columnsPanel);

				// Get the width of the columns object.
				float columnsWidth = this.GetElementWidth(this.columns);

				float newScrollValue = 0;
				float denominator = (-columnsWidth + columnsPanelWidth);
				if (denominator != 0)
				{
					newScrollValue = (columnsLocalXPosition / denominator);
				}

				this.SetHorizontalScrollBarValue(newScrollValue);
			}
		}

		private float CalculateColumnsObjectWidth()
		{
			float totalWidth = 0;
			foreach (RectTransform child in this.activeColumnPanels)
			{
				totalWidth += child.sizeDelta.x;
			}
			return totalWidth;
		}

		private float CalculateItemsObjectHeight(RectTransform itemsObject)
		{
			float totalHeight = 0;

			if (this.createMinimalItems)
			{
				int itemCount = this.Items.Count;
				int itemButtonHeight = this.ItemButtonHeight;
				totalHeight = itemCount * itemButtonHeight;
			}
			else
			{
				if (itemsObject != null)
				{
					foreach (RectTransform child in itemsObject)
					{
						if (!child.gameObject.activeSelf)
						{
							continue;
						}
						totalHeight += child.sizeDelta.y;
					}
				}
			}

			return totalHeight;
		}

		private RectTransform GetItemsObject(int objectIndex)
		{
			RectTransform result = null;

			int index = 0;

			foreach (Transform columnPanel in this.activeColumnPanels)
			{
				if (index == objectIndex)
				{
					// Get the items object.
					ColumnPanel columnPanelScript = columnPanel.GetComponent<ColumnPanel>();
					result = columnPanelScript.ItemPanel.Items;
					break;
				}

				index++;
			}

			return result;
		}

		private RectTransform firstItemsObject
		{
			get
			{
				ColumnPanel firstColumnPanel = this.firstColumnPanel;
				RectTransform result = null;

				if (firstColumnPanel != null)
				{
					// Get the items object.
					ColumnPanel columnPanelScript = firstColumnPanel.GetComponent<ColumnPanel>();
					result = columnPanelScript.ItemPanel.Items;
				}

				return result;
			}
		}

		private float headingButtonHeight
		{
			get
			{
				float result = 0;

				ColumnPanel firstColumnPanel = this.firstColumnPanel;

				if (firstColumnPanel != null)
				{
					ColumnPanel firstColumnPanelScript = firstColumnPanel.GetComponent<ColumnPanel>();
					RectTransform headingButton = firstColumnPanelScript.Button.transform as RectTransform;
					result = this.GetElementHeight(headingButton);
				}

				return result;
			}
		}

		private ColumnPanel firstColumnPanel
		{
			get
			{
				ColumnPanel result = null;

				if (this.columnsPanel != null)
				{
					// If there is at least one column...
					if (this.columnsPanel.childCount > 0)
					{
						// Get the first column.
						// (Skip over the column panels that have been destroyed.)
						foreach (Transform columnPanel in this.activeColumnPanels)
						{
							result = columnPanel.GetComponent<ColumnPanel>();
							break;
						}
					}
				}

				return result;
			}
		}

		private List<Transform> activeColumnPanels
		{
			get
			{
				List<Transform> result = new List<Transform>();
				foreach (Transform columnPanel in this.columns)
				{
					if (!columnPanel.gameObject.activeSelf)
					{
						continue;
					}

					result.Add(columnPanel);
				}

				return result;
			}
		}

		private ItemPanel firstItemPanel
		{
			get
			{
				ItemPanel result = null;
				if (this.firstColumnPanel != null)
				{
					result = this.firstColumnPanel.ItemPanel;
				}
				return result;
			}
		}

		private float firstItemPanelHeight
		{
			get
			{
				float result = 0;
				if (this.firstItemPanel != null)
				{
					// Calculate the first item panel's height.
					result = this.GetElementHeight(this.firstItemPanel.GetComponent<RectTransform>());
				}
				return result;
			}
		}

		public void OnItemMouseEnter(ItemButton itemButton)
		{
			this.itemHoverStartTime = Time.realtimeSinceStartup;
			this.hoverItemButton = itemButton;
		}

		public void OnItemMouseExit(ItemButton itemButton)
		{
			this.hoverItemButton = null;
		}

		private void UpdateItemHover()
		{
			if (this.hoverItemButton != null)
			{
				if (this.itemHoverStartTime > this.ItemHoverTime)
				{
					if (this.ItemMouseHover != null)
					{
						this.ItemMouseHover(this, new ListViewItemMouseHoverEventArgs(this.hoverItemButton.ListViewItem));
					}
					this.itemHoverStartTime = -1;
				}
			}
		}

		private void AddTriggeredEvent(EventTrigger eventTrigger, UnityAction action, EventTriggerType triggerType)
		{
			EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
			triggerEvent.AddListener((eventData) => action());

			EventTrigger.Entry entry = new EventTrigger.Entry() { callback = triggerEvent, eventID = triggerType };
			eventTrigger.triggers.Add(entry);
		}

		private void AddTriggeredEvent<T0>(EventTrigger eventTrigger, UnityAction<T0> action, EventTriggerType triggerType)
			where T0 : BaseEventData
		{
			EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
			triggerEvent.AddListener((eventData) => action((T0)eventData));

			EventTrigger.Entry entry = new EventTrigger.Entry() { callback = triggerEvent, eventID = triggerType };
			eventTrigger.triggers.Add(entry);
		}

		private bool ContainsPoint(Vector2 point)
		{
			RectTransform rectTransform = this.transform as RectTransform;

			return RectTransformUtility.RectangleContainsScreenPoint
			(
				rectTransform,
				point,
				Camera.main
			);
		}

		// Pool functions.
		private static Stack<GameObject> itemButtonPool;
		private static GameObject itemButtonPoolParent;

		public static void CreateItemButtonPool(GameObject prefab, int capacity)
		{
#if ENABLE_PROFILING
			using (new Endgame.CodeBlockTimer("CreateItemButtonPool"))
#endif
			{
				InstantiateItemsFromPools = true;

				itemButtonPool = new Stack<GameObject>(capacity);

				itemButtonPoolParent = new GameObject("ListViewItemButtonPool");
				itemButtonPoolParent.transform.position = new Vector3(-10000, -10000, 0);
				for (int i = 0; i < capacity; i++)
				{
					var itemButton = GameObject.Instantiate(prefab) as GameObject;
					itemButton.transform.SetParent(itemButtonPoolParent.transform, worldPositionStays: false);
					itemButtonPool.Push(itemButton);
				}
			}
		}

		private GameObject InstantiateItemButtonFromPool()
		{
			GameObject result;
			if ((itemButtonPool != null) && (itemButtonPool.Count > 0))
			{
				result = itemButtonPool.Pop();
			}
			else
			{
				Debug.Log("[ListView.InstantiateItemButtonFromPool]: WARNING: Pool exhausted, calling Instantiate.");
				result = GameObject.Instantiate(this.ItemButtonPrefab) as GameObject;
			}
			return result;
		}

		private void DestroyAllItemButtons()
		{
			Action<ItemButton> action;

			if (InstantiateItemsFromPools)
			{
				// When instantiating from pools, manually release all the objects
				// originating from pools before deleting the existing hierarchy.
				action = (itemButton) => this.ReturnItemButtonToPool(itemButton.gameObject);
			}
			else
			{
				action = (itemButton) =>
				{
					itemButton.gameObject.SetActive(false);
					GameObject.Destroy(itemButton.gameObject);
				};
			}

			foreach (Transform column in this.activeColumnPanels)
			{
				ColumnPanel columnPanelScript = column.GetComponent<ColumnPanel>();
				RectTransform items = columnPanelScript.ItemPanel.Items;
				List<ItemButton> itemButtons = new List<ItemButton>();
				foreach (RectTransform item in items)
				{
					ItemButton itemButton = item.GetComponent<ItemButton>();
					if (itemButton.gameObject.activeSelf)
					{
						itemButtons.Add(itemButton);
					}
				}
				foreach (var itemButton in itemButtons)
				{
					if (this.createMinimalItems)
					{
						if (this.ItemBecameInvisible != null)
						{
							this.ItemBecameInvisible(itemButton.ListViewItem);
						}
					}

					action(itemButton);
					itemButton.ListViewItem.ItemButtonInHierarchy = null;
					itemButton.ListViewItem = null;
					itemButton.ListViewSubItem = null;
				}
			}
		}

		private void ReturnItemButtonToPool(GameObject itemButton)
		{
			itemButtonPool.Push(itemButton);
			itemButton.transform.SetParent(itemButtonPoolParent.transform, worldPositionStays: false);
		}

		private static Stack<GameObject> columnPanelPool;
		private static GameObject columnPanelPoolParent;

		public static void CreateColumnPanelPool(GameObject prefab, int capacity)
		{
#if ENABLE_PROFILING
			using (new Endgame.CodeBlockTimer("CreateColumnPanelPool"))
#endif
			{
				InstantiateColumnsFromPools = true;

				columnPanelPool = new Stack<GameObject>(capacity);

				columnPanelPoolParent = new GameObject("ListViewColumnPanelPool");
				columnPanelPoolParent.transform.position = new Vector3(-10000, -10000, 0);
				for (int i = 0; i < capacity; i++)
				{
					var columnPanel = GameObject.Instantiate(prefab) as GameObject;
					columnPanel.transform.SetParent(columnPanelPoolParent.transform, worldPositionStays: false);
					columnPanelPool.Push(columnPanel);
				}
			}
		}

		private GameObject InstantiateColumnPanelFromPool()
		{
			GameObject result;
			if ((columnPanelPool != null) && (columnPanelPool.Count > 0))
			{
				result = columnPanelPool.Pop();

				// Reset the items object height before using the column panel.
				ColumnPanel columnPanelScript = result.GetComponent<ColumnPanel>();
				columnPanelScript.Reset();
			}
			else
			{
				Debug.Log("[ListView.InstantiateColumnPanelFromPool]: WARNING: Pool exhausted, calling Instantiate.");
				result = GameObject.Instantiate(this.ColumnPanelPrefab) as GameObject;
			}
			return result;
		}

		private void ReturnColumnPanelToPool(GameObject columnPanel)
		{
			columnPanelPool.Push(columnPanel);
			columnPanel.transform.SetParent(columnPanelPoolParent.transform, worldPositionStays: false);
		}

		private void ReturnColumnPanelsToPool()
		{
			// When instantiating from pools, manually release all the objects
			// originating from pools before deleting the existing hierarchy.
			if (InstantiateColumnsFromPools)
			{
				List<GameObject> columnPanels = new List<GameObject>();
				foreach (Transform columnPanel in this.activeColumnPanels)
				{
					columnPanels.Add(columnPanel.gameObject);
				}
				foreach (var columnPanel in columnPanels)
				{
					ReturnColumnPanelToPool(columnPanel);
				}
			}
			else
			{
				foreach (Transform column in this.activeColumnPanels)
				{
					column.gameObject.SetActive(false);
					GameObject.Destroy(column.gameObject);
				}
			}
		}

		private void OnDestroy()
		{
			if (!this.destroyed)
			{
				DestroyAllItemButtons();
				ReturnColumnPanelsToPool();
				this.destroyed = true;
			}
		}

		private void SetDummyScrollRectHorizontalScrollPosition(float value)
		{
			if (this.suppressDummyScrollRectEvents)
			{
				return;
			}

			if (this.columnsPanel != null)
			{
				// [NK 12/12/2014] When the scroll bar is moved, the dummy scroll rect
				// [NK 12/12/2014] is adjusted. This will then cause the scroll bar
				// [NK 12/12/2014] to be updated again, unless this flag is set.
				this.ignoreNextScrollRectEvent = true;
				ScrollRect scrollRect = this.columnsPanel.GetComponent<ScrollRect>();
				if (scrollRect != null)
				{
					scrollRect.horizontalNormalizedPosition = value;
				}
			}
		}

		private void SetDummyScrollRectVerticalScrollPosition(float value)
		{
			// [NK] Workaround for the "bottom to top" bug.
			value = Mathf.Clamp01(value);
			value = 1.0f - value;

			if (this.suppressDummyScrollRectEvents)
			{
				return;
			}

			if (this.columnsPanel != null)
			{
				this.ignoreNextScrollRectEvent = true;
				ScrollRect scrollRect = this.columnsPanel.GetComponent<ScrollRect>();
				if (scrollRect != null)
				{
					this.columnsPanel.GetComponent<ScrollRect>().verticalNormalizedPosition = value;
				}
			}
		}

		private void OnDummyScrollRectScrolledHorizontally(float value)
		{
			if (this.horizontalScrollBar != null)
			{
				this.horizontalScrollBar.GetComponent<Scrollbar>().value = value;
			}
		}

		private void OnDummyScrollRectScrolledVertically(float value)
		{
			if (this.verticalScrollBar != null)
			{
				this.verticalScrollBar.GetComponent<Scrollbar>().value = value;
			}
		}

		public void OnDummyScrollRectScrolled(Vector2 position)
		{
			if (this.ignoreNextScrollRectEvent)
			{
				this.ignoreNextScrollRectEvent = false;
				return;
			}

			this.suppressDummyScrollRectEvents = true;
			{
				this.OnDummyScrollRectScrolledHorizontally(position.x);

				// [NK] Workaround for the "bottom to top" scrollbar bug.
				//this.OnDummyScrollRectScrolledVertically(1.0f - position.y);
				this.OnDummyScrollRectScrolledVertically(position.y);
			}
			this.suppressDummyScrollRectEvents = false;
		}

		private void UpdateDummyScrollRect()
		{
			// Resize the dummy scroll rect so that it encompasses all the
			// child controls in the listview.

			float columnsWidth = 0;
			float itemsHeight = 0;
			float itemsPlusHeadingHeight = 0;

			// If there is at least one column...
			if (this.activeColumnPanels.Count > 0)
			{
				// Get the width of the columns object.
				columnsWidth = this.GetElementWidth(this.columns);

				// Get the height of the items object.
				if (this.firstItemsObject != null)
				{
					itemsHeight = this.GetElementHeight(this.firstItemsObject);
				}
				itemsPlusHeadingHeight = itemsHeight + headingButtonHeight;
			}

			if (this.dummyScrollContents != null)
			{
				var sizeDelta = this.dummyScrollContents.sizeDelta;
				sizeDelta.x = columnsWidth;
				sizeDelta.y = itemsPlusHeadingHeight;
				this.dummyScrollContents.sizeDelta = sizeDelta;
			}
		}

		private int potentiallyVisibleItems
		{
			get
			{
				int result = 0;
				float visibleItemsHeight = this.firstItemPanelHeight;
				result = (int)Mathf.Ceil(visibleItemsHeight / this.ItemButtonHeight);

				// If, for example, 11.5 items can potentially be visible, the 0.5 item
				// counts as one (hence the Ceil call above), but if the items were then scrolled
				// down slightly, another item would be partially revealed. Thus, increment
				// the count here.
				// (Result can be zero in the case where no columns have been added.)
				if (result > 0)
				{
					result++;
				}

				return result;
			}
		}

		public int PotentiallyVisibleItems
		{
			get
			{
				// Return an estimate of the potentially visible items.
				// This is necessary because this value is used to 
				// determine how many pooled items to create, and as such
				// may be called at any time, even before the hierarchy
				// has been built.
				// It's only an estimate because it doesn't take into account
				// the horizontal scroll bar, and can't do so because
				// the hierarchy hasn't been built yet.
				float listViewHeight = Mathf.Round(this.GetElementHeight(this.GetComponent<RectTransform>()));
				float visibleItemsHeight =
					(listViewHeight - headingButtonHeight);
				int estimate = (int)Mathf.Ceil(visibleItemsHeight / this.ItemButtonHeight);
				estimate++;
				return estimate;
			}
		}

		private void HandleAddingWithMinimalItems()
		{
			// Determine the subset of items to add to the hierarchy,
			// according to the listview height and vertical scroll bar value.
			int visibleItemsRequired = Mathf.Min(this.potentiallyVisibleItems, this.Items.Count);

			if (this.firstItemsObject != null)
			{
				this.indexOfFirstVisibleItem =
					(int)(this.firstItemsObject.localPosition.y / this.ItemButtonHeight);
				this.indexOfFirstPresentItem = this.indexOfFirstVisibleItem;
				this.indexOfLastPresentItem = this.indexOfFirstVisibleItem + visibleItemsRequired - 1;

				// Initially, the additional items required for scrolling are positioned
				// after the items being displayed. However, if the scroll position
				// is set to the end of the list, then the additional items need to
				// appear before the items being displayed.
				int lastValidIndex = this.Items.Count - 1;
				int overflow = Mathf.Max(this.indexOfLastPresentItem - lastValidIndex, 0);
				this.indexOfFirstPresentItem -= overflow;
				this.indexOfLastPresentItem -= overflow;

				for (int index = 0; index < visibleItemsRequired; index++)
				{
					int offsetIndex = index + this.indexOfFirstPresentItem;
					ListViewItem item = this.Items[offsetIndex];
					this.AddItemToHierarchy(item);

					if (this.ItemBecameVisible != null)
					{
						this.ItemBecameVisible(item);
					}
				}
			}
		}

		private void HandleScrollingWithMinimalItems()
		{
			this.indexOfFirstVisibleItem =
				(int)(firstItemsObject.localPosition.y / this.ItemButtonHeight);
			this.indexOfLastVisibleItem =
				(int)((firstItemsObject.localPosition.y + this.firstItemPanelHeight - 1) / this.ItemButtonHeight);
			ListViewItem itemToUpdate = null;

			int itemsRequiredAbove = (this.indexOfFirstPresentItem - this.indexOfFirstVisibleItem);
			int itemsRequiredBelow = (this.indexOfLastVisibleItem - this.indexOfLastPresentItem);
			int threshold = this.potentiallyVisibleItems;

			if ((itemsRequiredAbove > threshold) || (itemsRequiredBelow > threshold))
			{
				this.DestroyAllItemButtons();
				this.HandleAddingWithMinimalItems();
				return;
			}

			bool wasUpdated = false;

			while (this.indexOfFirstVisibleItem < this.indexOfFirstPresentItem)
			{
				wasUpdated = true;

				// The user has scrolled up enough that a new item needs to be
				// created.
				if (this.ItemBecameInvisible != null)
				{
					this.ItemBecameInvisible(this.Items[this.indexOfLastPresentItem]);
				}

				// Another item is now present, so update the indices.
				this.Items[this.indexOfLastPresentItem].ItemButtonInHierarchy = null;
				this.indexOfLastPresentItem--;
				this.indexOfFirstPresentItem--;
				itemToUpdate = this.Items[this.indexOfFirstPresentItem];
				int index = 0;

				foreach (Transform columnPanel in this.activeColumnPanels)
				{
					// Get the items object.
					ColumnPanel columnPanelScript = columnPanel.GetComponent<ColumnPanel>();
					RectTransform items = columnPanelScript.ItemPanel.Items;

					// Get the last item in the hierarchy.
					RectTransform lastItemInHierarchy = GetLastActiveChild(items) as RectTransform;

					// Make it the first item.
					lastItemInHierarchy.SetAsFirstSibling();

					// Link the item in the hierarchy to its Windows Forms counterpart.
					ItemButton itemButton = lastItemInHierarchy.GetComponent<ItemButton>();
					itemButton.ListViewItem = itemToUpdate;
					itemButton.ListViewSubItem = itemToUpdate.SubItems[index];
					if (index == 0)
					{
						itemToUpdate.ItemButtonInHierarchy = itemButton;
					}

					index++;
				}

				if (itemToUpdate != null)
				{
					this.UpdateItem(itemToUpdate, refreshSelection: false);
				}

				if (this.ItemBecameVisible != null)
				{
					this.ItemBecameVisible(itemToUpdate);
				}
			}

			while (this.indexOfLastVisibleItem > this.indexOfLastPresentItem)
			{
				wasUpdated = true;

				// The user has scrolled down enough that a new item needs to be
				// created.
				if (this.ItemBecameInvisible != null)
				{
					this.ItemBecameInvisible(this.Items[this.indexOfFirstPresentItem]);
				}

				this.Items[this.indexOfFirstPresentItem].ItemButtonInHierarchy = null;
				this.indexOfFirstPresentItem++;
				this.indexOfLastPresentItem++;
				itemToUpdate = this.Items[this.indexOfLastPresentItem];
				int index = 0;

				foreach (Transform columnPanel in this.activeColumnPanels)
				{
					// Get the items object.
					ColumnPanel columnPanelScript = columnPanel.GetComponent<ColumnPanel>();
					RectTransform items = columnPanelScript.ItemPanel.Items;

					// Get the first item in the hierarchy.
					RectTransform firstItemInHierarchy = GetFirstActiveChild(items) as RectTransform;

					// Make it the last item.
					firstItemInHierarchy.SetAsLastSibling();

					// Link the item in the hierarchy to its Windows Forms counterpart.
					ItemButton itemButton = firstItemInHierarchy.GetComponent<ItemButton>();
					itemButton.ListViewItem = itemToUpdate;
					itemButton.ListViewSubItem = itemToUpdate.SubItems[index];
					if (index == 0)
					{
						itemToUpdate.ItemButtonInHierarchy = itemButton;
					}
					index++;
				}

				if (itemToUpdate != null)
				{
					this.UpdateItem(itemToUpdate, refreshSelection: false);
				}

				if (this.ItemBecameVisible != null)
				{
					this.ItemBecameVisible(itemToUpdate);
				}
			}

			if (wasUpdated)
			{
				this.RefreshSelection();
			}
		}

		private void OnApplicationQuit()
		{
			this.OnDestroy();
		}

		private bool IsButtonDown(string buttonName)
		{
			try
			{
				return Input.GetButtonDown(buttonName);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool IsButton(string buttonName)
		{
			try
			{
				return Input.GetButton(buttonName);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void HandleGamepadInput()
		{
			if (this.Items.Count == 0)
			{
				return;
			}

			// Update the selection in response to gamepad input.
			this.downButtonDown = false;
			this.upButtonDown = false;

			if (IsButton("Vertical"))
			{
				if (Input.GetAxis("Vertical") < 0)
				{
					this.downButtonDown = true;
				}
				else if (Input.GetAxis("Vertical") > 0)
				{
					this.upButtonDown = true;
				}
			}
			else
			{
				this.downButtonDownTime = 0;
				this.upButtonDownTime = 0;
			}

			// If the control was selected by keyboard, wait for the keys to be
			// released before processing further input.
			// This prevents the bug where the user navigates to the control via keyboard,
			// and the selection jumps immediately by one item.
			if (this.waitForKeyRelease)
			{
				this.waitForKeyReleaseTime = Mathf.MoveTowards(this.waitForKeyReleaseTime, 0, Time.deltaTime);

				if ((!this.downButtonDown && !this.upButtonDown) || (this.waitForKeyReleaseTime == 0))
				{
					waitForKeyRelease = false;
				}
				else
				{
					return;
				}
			}

			if (IsButtonDown("Vertical"))
			{
				if (Input.GetAxis("Vertical") < 0)
				{
					this.SelectNextItem();
				}
				else if (Input.GetAxis("Vertical") > 0)
				{
					this.SelectPreviousItem();
				}
			}
			else if (IsButtonDown(this.gamepadInputNameForPageUp))
			{
				this.SelectPreviousPage();
			}
			else if (IsButtonDown(this.gamepadInputNameForPageDown))
			{
				this.SelectNextPage();
			}

			float threshold = 0.25f;
			if (this.upButtonDown)
			{
				if (this.upButtonDownTime > threshold)
				{
					this.upButtonDownTime = threshold / 2;
					SelectPreviousItem();
				}
				else
				{
					this.upButtonDownTime += Time.deltaTime;
				}
			}
			else if (this.downButtonDown)
			{
				if (this.downButtonDownTime > threshold)
				{
					this.downButtonDownTime = threshold / 2;
					SelectNextItem();
				}
				else
				{
					this.downButtonDownTime += Time.deltaTime;
				}
			}

			// Activate the selected item if the appropriate key is pressed.
			if (IsButtonDown(this.gamepadInputNameForActivate))
			{
				if (this.selectedIndices.Count > 0)
				{
					ItemButton itemButton = this.selectedItems[0].ItemButtonInHierarchy;
					this.ActivateRowContainingButton(itemButton);
				}
			}
		}

		private void SelectPreviousItem()
		{
			if (this.selectedIndices.Count == 0)
			{
				this.selectedIndices.Add(0);
			}

			int oldIndex = this.selectedIndices[0];
			int newIndex = oldIndex - 1;

			if (newIndex >= 0)
			{
				this.ScrollToEnsureItemVisible(newIndex);
				this.selectedIndices.Remove(oldIndex);
				this.selectedIndices.Add(newIndex);
			}
		}

		private void SelectNextItem()
		{
			if (this.selectedIndices.Count == 0)
			{
				this.selectedIndices.Add(0);
			}

			int oldIndex = this.selectedIndices[0];
			int newIndex = oldIndex + 1;

			if (newIndex < this.Items.Count)
			{
				this.ScrollToEnsureItemVisible(newIndex);
				this.selectedIndices.Remove(oldIndex);
				this.selectedIndices.Add(newIndex);
			}
		}

		private void SelectPreviousPage()
		{
			if (this.selectedIndices.Count == 0)
			{
				this.selectedIndices.Add(0);
			}

			int indicesPerPage = (int)(this.firstItemPanelHeight / this.ItemButtonHeight);
			int oldIndex = this.selectedIndices[0];
			int newIndex = Mathf.Max(oldIndex - indicesPerPage, 0);

			this.ScrollToEnsureItemVisible(newIndex);
			this.selectedIndices.Remove(oldIndex);
			this.selectedIndices.Add(newIndex);
		}

		private void SelectNextPage()
		{
			if (this.selectedIndices.Count == 0)
			{
				this.selectedIndices.Add(0);
			}

			int indicesPerPage = (int)(this.firstItemPanelHeight / this.ItemButtonHeight);
			int oldIndex = this.selectedIndices[0];
			int newIndex = Mathf.Min(oldIndex + indicesPerPage, this.Items.Count - 1);

			this.ScrollToEnsureItemVisible(newIndex);
			this.selectedIndices.Remove(oldIndex);
			this.selectedIndices.Add(newIndex);
		}

		private int firstFullyVisibleRowIndex
		{
			get
			{
				float topEdgeRowPosition = firstItemsObject.localPosition.y / this.ItemButtonHeight;
				int firstFullyVisibleRowIndex = (int)Mathf.Ceil(topEdgeRowPosition);
				return firstFullyVisibleRowIndex;
			}
		}

		private int lastFullyVisibleRowIndex
		{
			get
			{
				float bottomEdgeRowPosition = (firstItemsObject.localPosition.y + this.firstItemPanelHeight - 1) / this.ItemButtonHeight;
				int lastFullyVisibleRowIndex = (int)((Mathf.Floor(bottomEdgeRowPosition) - 1));
				return lastFullyVisibleRowIndex;
			}
		}

		private bool IsItemVisible(int index)
		{
			bool result =
				(index >= this.firstFullyVisibleRowIndex) &&
				(index <= this.lastFullyVisibleRowIndex);
			return result;
		}

		private void ScrollToEnsureItemVisible(int index)
		{
			if (!this.canScrollVertically || this.IsItemVisible(index))
			{
				return;
			}

			float rowPositionForScrollPosition0;
			float rowPositionForScrollPosition1;
			float rowPositionForSelectedItem;

			if (index > this.lastFullyVisibleRowIndex)
			{
				// Scroll down.
				// Calculate the scroll position that will have the selected item
				// fully visible at the bottom row of the control.

				// Calculate the fractional position of the rows mapping to
				// scroll positions 0 and 1.
				rowPositionForScrollPosition0 =
					this.firstItemPanelHeight / this.ItemButtonHeight;
				rowPositionForScrollPosition1 = this.items.Count;

				// Calculate the fractional position of the selected item.
				// (One is added to calculate the position of the bottom edge.)
				rowPositionForSelectedItem = index + 1;
			}
			else
			{
				// Scroll up.
				// Calculate the scroll position that will have the selected item
				// fully visible at the top row of the control.

				// Calculate the fractional position of the rows mapping to
				// scroll positions 0 and 1.
				rowPositionForScrollPosition0 = 0;
				rowPositionForScrollPosition1 =
					this.items.Count - 
					(this.firstItemPanelHeight / this.ItemButtonHeight);

				// Calculate the fractional position of the selected item.
				rowPositionForSelectedItem = index;
			}

			// Use these values to calculate the scroll position of
			// the selected row.
			float scrollPositionForRowPosition =
				(rowPositionForSelectedItem - rowPositionForScrollPosition0) /
				(rowPositionForScrollPosition1 - rowPositionForScrollPosition0);

			// Set the vertical scroll position to that value.
			SetVerticalScrollBarValue(scrollPositionForRowPosition);
		}

		private Canvas canvas
		{
			get
			{
				Canvas canvas = null;
				Transform transform = this.transform;
				for (;;)
				{
					if (transform == null)
					{
						break;
					}

					var components = transform.GetComponents(typeof(Canvas));
					if (components.Count() > 0)
					{
						canvas = transform.GetComponent<Canvas>();
						break;
					}
					
					transform = transform.parent;
				}

				return canvas;
			}
		}

		private void CheckCanvasRenderMode()
		{
			if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				if (this.canvas.GetComponent<Camera>() == null)
				{
					Debug.LogWarning
					(
						"[ListView.CheckRenderMode]: WARNING: Either \"Screen space overlay\" render mode set, or " +
						"no camera has been set in the Canvas in the inspector. " +
						"The ListView requires a mode that uses a camera to work correctly."
					);
				}
				else
				{
					Debug.LogWarning
					(
						"[ListView.CheckRenderMode]: WARNING: \"Screen space overlay\" render mode set. " +
						"The ListView requires a mode that uses a camera to work correctly."
					);
				}
			}
		}

		public bool IsFirstItemSelected
		{
			get
			{
				int selectedIndex = 0;
				if (this.selectedIndices.Count > 0)
				{
					selectedIndex = this.selectedIndices[0];
				}

				return (selectedIndex == 0);
			}
		}

		public bool IsLastItemSelected
		{
			get
			{
				int selectedIndex = -1;
				if (this.selectedIndices.Count > 0)
				{
					selectedIndex = this.selectedIndices[0];
				}
				return (selectedIndex == this.Items.Count - 1);
			}
		}

		public void OnSelect(bool value)
		{
			if (this.Items.Count == 0)
			{
				return;
			}

			// If the control is being selected, but there's no selection, select an item.
			if (value)
			{
				if (this.selectedIndices.Count == 0)
				{
					this.selectedIndices.Add(0);
				}
			}

			// When the control gains or loses focus, refresh the selected item buttons
			// to update the colour of the highlight bar.
			ListViewItem itemToSelect = this.Items[this.selectedIndices[0]];
			UpdateItemButtonColors(itemToSelect, selected: true, focussed: value);

			// When the control is selected by keyboard, prevent further keyboard processing
			// until the keys are released.
			// This prevents the bug where the user navigates to the control via keyboard,
			// and the selection jumps immediately by one item.
			if (value)
			{
				if (IsButton("Vertical"))
				{
					if ((Input.GetAxis("Vertical") < 0) || (Input.GetAxis("Vertical") > 0))
					{
						this.waitForKeyRelease = true;
						waitForKeyReleaseTime = 0.25f;
					}
				}
			} 
		}

		private int editorVersion
		{
			get
			{
				// Parse the Unity version string.
				string versionText = Application.unityVersion;

				// Skip characters until the first digit.
				IEnumerable<char> enumerable;
				enumerable = versionText.SkipWhile(c => !Char.IsDigit(c));
				versionText = new string(enumerable.ToArray());

				// Take characters while they are digits or dots.
				enumerable = versionText.TakeWhile(c => Char.IsDigit(c) || c == '.');
				versionText = new string(enumerable.ToArray());

				// Remove the dots.
				versionText = new string(versionText.Where(c => Char.IsDigit(c)).ToArray());

				// Convert the string to a number.
				int version = int.Parse(versionText);
				return version;
			}
		}
	}
}
