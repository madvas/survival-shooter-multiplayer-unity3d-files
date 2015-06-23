namespace Examples
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using UnityEngine.EventSystems;
	using Endgame;

	public class ListViewTester_ImagesAndSliders : MonoBehaviour
	{
		class ItemData
		{
			public float SliderValue;
			public string ImageKey;
		}

		public ListView ListView;
		public GameObject ItemButtonPrefab;
		public Sprite AndrosynthGuardianIcon;
		public Sprite ArilouSkiffIcon;
		public Sprite ChenjesuBroodhomeIcon;
		public Sprite ChmmrAvatarIcon;
		public Sprite UrQuanDreadnoughtIcon;
		public Sprite DruugeMaulerIcon;
		public Sprite EarthlingCruiserIcon;
		public Sprite KorAhMarauderIcon;
		public Sprite MelnormeTraderIcon;
		public Sprite MmrnmhrmXForm;
		public Sprite MyconPodshipIcon;
		public Sprite OrzNemesisIcon;
		public Sprite PkunkFuryIcon;
		public Sprite ShofixtiScoutIcon;
		public Sprite SlylandroProbeIcon;
		public Sprite SpathiEluderIcon;
		public Sprite SupoxBladeIcon;
		public Sprite SyreenPenetratorIcon;
		public Sprite ThraddashTorchIcon;
		public Sprite UmgahDroneIcon;
		public Sprite UtwigJuggerIcon;
		public Sprite VUXIntruderIcon;
		public Sprite YehatTerminatorIcon;
		public Sprite ZoqFotPikStingerIcon;
		private ImageList imageList;
		public GameObject SliderPrefab;
		private Button insertItemAtCurrentPositionButton;
		private Button removeItemAtCurrentPositionButton;
		private Button toggleColumnClickModeButton;
		private int itemAddedCount = 0;
		private int itemInsertedCount = 0;
		private bool clickingAColumnSorts = true;
		private bool columnSortAscending = true;
		private const int columnWidthCount = 3;
		private int columnCount
		{
			get
			{
				return this.ListView.Columns.Count;
			}
		}
		private int[] columnWidths = new int[columnWidthCount];
		private int[] columnWidthStates = null;

		public void Start()
		{
			// Get references to the buttons.
			this.insertItemAtCurrentPositionButton =
				GameObject.Find("/Canvas/Buttons/InsertItemAtCurrentPositionButton").GetComponent<Button>();
			this.removeItemAtCurrentPositionButton =
				GameObject.Find("/Canvas/Buttons/RemoveItemAtCurrentPositionButton").GetComponent<Button>();
			this.toggleColumnClickModeButton =
				GameObject.Find("/Canvas/Buttons/ToggleColumnClickModeButton").GetComponent<Button>();

			// Add some test data (columns and items).
			this.AddTestData();

			// Add some events.
			// (Clicking on the first column header will sort by that column, and
			// clicking on any other column header will change that column's width
			// between default, sized to the header or sized to the longest item.)
			this.ListView.ColumnClick += OnColumnClick;

			// Initialise an array with some example column widths
			// that will be toggled between by clicking on the column header.
			// (-1 in Windows Forms means size to the longest item, and
			// -2 means size to the column header.)
			this.columnWidths[0] = 100;
			this.columnWidths[1] = 150;
			this.columnWidths[2] = 200;

			this.columnWidthStates = new int[this.columnCount];
			for (int index = 0; index < columnCount; index++)
			{
				this.columnWidthStates[index] = 0;
			}

			this.ListView.Columns[0].Width = 150;
			this.ListView.Columns[1].Width = 120;
			this.ListView.Columns[2].Width = 160;

			// Create an image list.
			imageList = new ImageList();

			// Add some images.
			imageList.Images.Add("AndrosynthGuardianIcon", AndrosynthGuardianIcon);
			imageList.Images.Add("ArilouSkiffIcon", ArilouSkiffIcon);
			imageList.Images.Add("ChenjesuBroodhomeIcon", ChenjesuBroodhomeIcon);
			imageList.Images.Add("ChmmrAvatarIcon", ChmmrAvatarIcon);
			imageList.Images.Add("UrQuanDreadnoughtIcon", UrQuanDreadnoughtIcon);
			imageList.Images.Add("DruugeMaulerIcon", DruugeMaulerIcon);
			imageList.Images.Add("EarthlingCruiserIcon", EarthlingCruiserIcon);
			imageList.Images.Add("KorAhMarauderIcon", KorAhMarauderIcon);
			imageList.Images.Add("MelnormeTraderIcon", MelnormeTraderIcon);
			imageList.Images.Add("MmrnmhrmXForm", MmrnmhrmXForm);
			imageList.Images.Add("MyconPodshipIcon", MyconPodshipIcon);
			imageList.Images.Add("OrzNemesisIcon", OrzNemesisIcon);
			imageList.Images.Add("PkunkFuryIcon", PkunkFuryIcon);
			imageList.Images.Add("ShofixtiScoutIcon", ShofixtiScoutIcon);
			imageList.Images.Add("SlylandroProbeIcon", SlylandroProbeIcon);
			imageList.Images.Add("SpathiEluderIcon", SpathiEluderIcon);
			imageList.Images.Add("SupoxBladeIcon", SupoxBladeIcon);
			imageList.Images.Add("SyreenPenetratorIcon", SyreenPenetratorIcon);
			imageList.Images.Add("ThraddashTorchIcon", ThraddashTorchIcon);
			imageList.Images.Add("UmgahDroneIcon", UmgahDroneIcon);
			imageList.Images.Add("UtwigJuggerIcon", UtwigJuggerIcon);
			imageList.Images.Add("VUXIntruderIcon", VUXIntruderIcon);
			imageList.Images.Add("YehatTerminatorIcon", YehatTerminatorIcon);
			imageList.Images.Add("ZoqFotPikStingerIcon", ZoqFotPikStingerIcon);

			// Set the listview's image list.
			this.ListView.SmallImageList = imageList;

			this.ListView.SubItemClicked += this.OnSubItemClicked;
			this.ListView.ItemChanged += this.OnItemChanged;


			RefreshToggleColumnClickButtonText();
		}

		public void OnSubItemClicked(PointerEventData pointerEventData, ListViewItem.ListViewSubItem subItem)
		{
			//Debug.Log(pointerEventData.button);
		}

		public void OnItemChanged(ListViewItem listViewItem)
		{
			//Debug.Log("Item text changed to: " + listViewItem.Text);
		}

		public void OnToggleSubItem0ImageButtonClicked()
		{
			OnToggleImageButtonClicked(0);
		}

		public void OnToggleImageButtonClicked(int index)
		{
			if (this.ListView.SelectedItems.Count > 0)
			{
				ListViewItem item = this.ListView.SelectedItems[0];
				var subitem = item.SubItems[index];

				if (subitem.ImageKey == "")
				{
					ItemData itemData = item.Tag as ItemData;
					subitem.ImageKey = itemData.ImageKey;
				}
				else
				{
					subitem.ImageKey = "";
				}
			}
		}

		private int imageSizeState = 0;
		public void OnToggleImageSizeButtonClicked()
		{
			if (imageSizeState == 0)
			{
				imageList.ImageSize = new Vector2(60, 60);
			}
			else
			{
				imageList.ImageSize = new Vector2(20, 20);
			}
			imageSizeState = imageSizeState ^ 1;
		}

		private ListViewItem CreateListViewItem(string imageKey, string shipSpecies, string shipName)
		{
			string[] subItemTexts = new string[]
		{
			shipSpecies,
			shipName,
			"slider"
		};

			ListViewItem item = new ListViewItem(subItemTexts);

			// Add an image to the first subitem.
			item.ImageKey = imageKey;
			ItemData itemData = new ItemData();
			itemData.ImageKey = imageKey;
			itemData.SliderValue = 0;
			item.Tag = itemData;

			// NOTE: Any custom controls to be added to the list view item 
			// should be created in OnItemBecameVisible, and destroyed in
			// OnItemBecameInvisible. This is because the list view only
			// creates GameObjects to display the items that are visible,
			// rather than for every item in ListView.Items.

			return item;
		}

		private void AddListViewItem(string imageKey, string shipSpecies, string shipName)
		{
			ListViewItem item = CreateListViewItem(imageKey, shipSpecies, shipName);
			this.ListView.Items.Add(item);
		}

		private static float GetItemSliderValue(ListViewItem item)
		{
			RectTransform customControl = item.SubItems[2].CustomControl;
			if (customControl != null)
			{
				return customControl.gameObject.GetComponentInChildren<Scrollbar>().value;
			}
			else
			{
				return (item.Tag as ItemData).SliderValue;
			}
		}

		private void OnItemBecameVisible(ListViewItem item)
		{
			// Create a slider custom control and add it to the third subitem.
			var subItem = item.SubItems[2];
			GameObject slider = GameObject.Instantiate(this.SliderPrefab) as GameObject;
			subItem.CustomControl = slider.transform as RectTransform;

			ItemData itemData = item.Tag as ItemData;
			slider.GetComponentInChildren<Scrollbar>().value = itemData.SliderValue;
		}

		private void OnItemBecameInvisible(ListViewItem item)
		{
			var subItem = item.SubItems[2];
			GameObject slider = subItem.CustomControl.gameObject;

			// Save the value of the slider so that it can be restored
			// when the item becomes visible again.
			ItemData itemData = item.Tag as ItemData;
			itemData.SliderValue = slider.GetComponentInChildren<Scrollbar>().value;

			// Destroy the slider custom control.
			GameObject.Destroy(subItem.CustomControl.gameObject);
		}

		private void AddTestData()
		{
			if (this.ListView != null)
			{
				this.ListView.SuspendLayout();
				{
					this.ListView.ItemBecameVisible += this.OnItemBecameVisible;
					this.ListView.ItemBecameInvisible += this.OnItemBecameInvisible;

					ColumnHeader ShipSpeciesColumn = new ColumnHeader();
					ShipSpeciesColumn.Text = "Species";
					this.ListView.Columns.Add(ShipSpeciesColumn);

					ColumnHeader ShipNameColumn = new ColumnHeader();
					ShipNameColumn.Text = "Ship Name";
					this.ListView.Columns.Add(ShipNameColumn);

					ColumnHeader AmountInFleetColumn = new ColumnHeader();
					AmountInFleetColumn.Text = "Amount In Fleet";
					this.ListView.Columns.Add(AmountInFleetColumn);

					for (int i = 0; i < 200; i++)
					{
						AddListViewItem("AndrosynthGuardianIcon", "Androsynth", "Guardian");
						AddListViewItem("ArilouSkiffIcon", "Arilou", "Skiff");
						AddListViewItem("ChenjesuBroodhomeIcon", "Chenjesu", "Broodhome");
						AddListViewItem("ChmmrAvatarIcon", "Chmmr", "Avatar");
						AddListViewItem("UrQuanDreadnoughtIcon", "Ur-Quan", "Dreadnought");
						AddListViewItem("DruugeMaulerIcon", "Druuge", "Mauler");
						AddListViewItem("EarthlingCruiserIcon", "Earthling", "Cruiser");
						AddListViewItem("KorAhMarauderIcon", "Kor-Ah", "Marauder");
						AddListViewItem("MelnormeTraderIcon", "Melnorme", "Trader");
						AddListViewItem("MmrnmhrmXForm", "Mmrnmhrm", "X-Form");
						AddListViewItem("MyconPodshipIcon", "Mycon", "Podship");
						AddListViewItem("OrzNemesisIcon", "Orz", "Nemesis");
						AddListViewItem("PkunkFuryIcon", "Pkunk", "Fury");
						AddListViewItem("ShofixtiScoutIcon", "Shofixti", "Scout");
						AddListViewItem("SlylandroProbeIcon", "Slylandro", "Probe");
						AddListViewItem("SpathiEluderIcon", "Spathi", "Eluder");
						AddListViewItem("SupoxBladeIcon", "Supox", "Blade");
						AddListViewItem("SyreenPenetratorIcon", "Syreen", "Penetrator");
						AddListViewItem("ThraddashTorchIcon", "Thraddash", "Torch");
						AddListViewItem("UmgahDroneIcon", "Umgah", "Drone");
						AddListViewItem("UtwigJuggerIcon", "Utwig", "Jugger");
						AddListViewItem("VUXIntruderIcon", "VUX", "Intruder");
						AddListViewItem("YehatTerminatorIcon", "Yehat", "Terminator");
						AddListViewItem("ZoqFotPikStingerIcon", "ZoqFotPik", "Stinger");
					}
				}
				this.ListView.ResumeLayout();
			}
		}

		public void OnAddNewItemButtonClicked()
		{
			this.itemAddedCount++;
			AddListViewItem("ZoqFotPikStingerIcon", "NEW SPECIES", "SHIP ADDED (" + this.itemAddedCount + ")");

			// Select the new item and scroll to it.
			this.ListView.SelectedIndices.Add(this.ListView.Items.Count - 1);
			this.ListView.SetVerticalScrollBarValue(1);
		}

		public void OnInsertItemAtCurrentPositionButtonClicked()
		{
			this.itemInsertedCount++;

			int selectedIndex = this.ListView.SelectedIndices[0];
			ListViewItem item = CreateListViewItem("ZoqFotPikStingerIcon", "NEW SPECIES", "SHIP INSERTED (" + this.itemInsertedCount + ")");
			this.ListView.Items.Insert(selectedIndex, item);
		}

		public void OnRemoveItemAtCurrentPositionButtonClicked()
		{
			int selectedIndex = this.ListView.SelectedIndices[0];
			this.ListView.Items.RemoveAt(selectedIndex);
		}

		public void Update()
		{
			// Some buttons require a selection, so disable them if there is no 
			// selection.
			bool isItemSelected = false;
			if (this.ListView != null)
			{
				if (this.ListView.SelectedIndices.Count > 0)
				{
					isItemSelected = true;
				}
			}

			this.insertItemAtCurrentPositionButton.interactable = isItemSelected;
			this.removeItemAtCurrentPositionButton.interactable = isItemSelected;
		}

		private void RefreshToggleColumnClickButtonText()
		{
			this.toggleColumnClickModeButton.GetComponentInChildren<Text>().text = 
				string.Format(
					"Clicking a column header will {0} (click here to change)",
					this.clickingAColumnSorts ? "sort" : "change its width"
				);
		}

		public void OnToggleColumnClickModeButtonClicked()
		{
			this.clickingAColumnSorts = !this.clickingAColumnSorts;

			RefreshToggleColumnClickButtonText();
		}

		private class ListViewItemComparer : IComparer
		{
			private int columnIndex = 0;
			private bool sortAscending = true;

			public ListViewItemComparer(int columnIndex, bool sortAscending)
			{
				this.columnIndex = columnIndex;
				this.sortAscending = sortAscending;
			}

			public int Compare(object object1, object object2)
			{
				int comparisonValue = CompareAscending(object1, object2);
				return this.sortAscending ? comparisonValue : -comparisonValue;
			}

			private int CompareAscending(object object1, object object2)
			{
				ListViewItem listViewItem1 = object1 as ListViewItem;
				ListViewItem listViewItem2 = object2 as ListViewItem;
				ListViewItem.ListViewSubItem subItem1 = listViewItem1.SubItems[this.columnIndex];
				ListViewItem.ListViewSubItem subItem2 = listViewItem2.SubItems[this.columnIndex];

				switch (this.columnIndex)
				{
					// Column 2 contains a slider, so compare by slider value rather than text.
					case 2:
						{
							float diff = GetItemSliderValue(listViewItem1) - GetItemSliderValue(listViewItem2);
							return (int)(diff * 1000);
						}

					// All other columns, we can simply compare by text/string.
					default: return CompareSubItemsByText(subItem1, subItem2);
				}
			}

			private int CompareSubItemsByText(ListViewItem.ListViewSubItem a, ListViewItem.ListViewSubItem b)
			{
				return string.Compare(a.Text, b.Text);
			}
		}

		private void OnColumnClick(object sender, ListView.ColumnClickEventArgs e)
		{
			if (this.clickingAColumnSorts)
			{
				ListView listView = (ListView)sender;
				listView.ListViewItemSorter = new ListViewItemComparer(e.Column, this.columnSortAscending);
				this.columnSortAscending = !this.columnSortAscending;
			}
			else
			{
				this.IncrementColumnWidthState(e.Column);
			}
		}

		private void IncrementColumnWidthState(int columnIndex)
		{
			this.columnWidthStates[columnIndex]++;
			if (this.columnWidthStates[columnIndex] >= columnWidthCount)
			{
				this.columnWidthStates[columnIndex] = 0;
			}

			int columnWidth = this.columnWidths[this.columnWidthStates[columnIndex]];
			this.ListView.Columns[columnIndex].Width = columnWidth;
		}
	}
}
