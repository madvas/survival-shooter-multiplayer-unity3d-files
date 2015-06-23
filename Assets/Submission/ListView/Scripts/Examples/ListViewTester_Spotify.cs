namespace Examples
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using Endgame;

	public class ListViewTester_Spotify : MonoBehaviour
	{
		public ListView ListView;
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
		private Button insertItemAtCurrentPositionButton;
		private Button removeItemAtCurrentPositionButton;
		private Button toggleColumnClickModeButton;
		private Button changeItemBackgroundColorButton;
		private Button changeItemTextColorButton;
		private Button changeControlBackgroundColorButton;
		private int itemAddedCount = 0;
		private int itemInsertedCount = 0;
		private bool clickingAColumnSorts = true;

		private Color defaultControlBackgroundColor;
		private Color defaultItemBackgroundColor = new Color(0, 0, 0, 0);
		private Color defaultItemTextColor;
		private Color spotifyGreen = new Color(0.50f, 0.72f, 0.01f);
		private Color spotifyRed = new Color(0.68f, 0.27f, 0.27f);

		public void Start()
		{
			// Get references to the buttons.
			this.insertItemAtCurrentPositionButton =
				GameObject.Find("/Canvas/Buttons/InsertItemAtCurrentPositionButton").GetComponent<Button>();
			this.removeItemAtCurrentPositionButton =
				GameObject.Find("/Canvas/Buttons/RemoveItemAtCurrentPositionButton").GetComponent<Button>();
			this.toggleColumnClickModeButton =
				GameObject.Find("/Canvas/Buttons/ToggleColumnClickModeButton").GetComponent<Button>();
			this.changeItemBackgroundColorButton =
				GameObject.Find("/Canvas/Buttons/ChangeItemBackgroundColorButton").GetComponent<Button>();
			this.changeItemTextColorButton =
				GameObject.Find("/Canvas/Buttons/ChangeItemTextColorButton").GetComponent<Button>();
			this.changeControlBackgroundColorButton =
				GameObject.Find("/Canvas/Buttons/ChangeControlBackgroundColorButton").GetComponent<Button>();

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
			this.columnWidths[1] = -1;
			this.columnWidths[2] = -2;

			for (int index = 0; index < columnCount; index++)
			{
				this.columnWidthStates[index] = 0;
			}

			this.ListView.Columns[0].Width = 175;
			this.ListView.Columns[1].Width = 175;
			this.ListView.Columns[2].Width = 50;
			this.ListView.Columns[3].Width = 175;

			this.defaultControlBackgroundColor = this.ListView.BackColor;
			this.defaultItemTextColor = this.ListView.ForeColor;
		}

		private class ListViewItemComparer : IComparer
		{
			private int columnIndex = 0;

			public ListViewItemComparer()
			{
			}

			public ListViewItemComparer(int columnIndex)
			{
				this.columnIndex = columnIndex;
			}

			public int Compare(object object1, object object2)
			{
				ListViewItem listViewItem1 = object1 as ListViewItem;
				ListViewItem listViewItem2 = object2 as ListViewItem;
				string text1 = listViewItem1.SubItems[this.columnIndex].Text;
				string text2 = listViewItem2.SubItems[this.columnIndex].Text;
				return string.Compare(text1, text2);
			}
		}

		private void OnColumnClick(object sender, ListView.ColumnClickEventArgs e)
		{
			if (this.clickingAColumnSorts)
			{
				ListView listView = (ListView)sender;
				listView.ListViewItemSorter = new ListViewItemComparer(e.Column);
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
			this.changeItemBackgroundColorButton.interactable = isItemSelected;
			this.changeItemTextColorButton.interactable = isItemSelected;
			this.changeControlBackgroundColorButton.interactable = isItemSelected;
		}

		private string[] CreateAddedInsertedText(string addedInserted, int count)
		{
			string name = addedInserted.ToUpper() + " ITEM (" + count + ")";
			return new string[] { name, "Item " + addedInserted, "0:00", "" };
		}

		public void OnAddNewItemButtonClicked()
		{
			this.itemAddedCount++;
			AddListViewItem(CreateAddedInsertedText("added", this.itemAddedCount));

			// Select the new item and scroll to it.
			this.ListView.SelectedIndices.Add(this.ListView.Items.Count - 1);
			this.ListView.SetVerticalScrollBarValue(1);
		}

		public void OnInsertItemAtCurrentPositionButtonClicked()
		{
			this.itemInsertedCount++;

			int selectedIndex = this.ListView.SelectedIndices[0];
			this.ListView.Items.Insert(selectedIndex, GetListViewItemFromStrings(CreateAddedInsertedText("inserted", this.itemInsertedCount)));
		}

		public void OnRemoveItemAtCurrentPositionButtonClicked()
		{
			int selectedIndex = this.ListView.SelectedIndices[0];
			this.ListView.Items.RemoveAt(selectedIndex);
		}

		public void OnToggleColumnClickModeButtonClicked()
		{
			string text = "";

			if (this.clickingAColumnSorts)
			{
				this.clickingAColumnSorts = false;
				text = "Clicking a column header will change its width (click here to change)";
			}
			else
			{
				this.clickingAColumnSorts = true;
				text = "Clicking a column header will sort (click here to change)";
			}

			this.toggleColumnClickModeButton.GetComponentInChildren<Text>().text = text;
		}

		public void OnChangeItemBackgroundColorButtonClicked()
		{
			ListViewItem selectedItem = this.ListView.SelectedItems[0];

			if (selectedItem.BackColor == this.defaultItemBackgroundColor)
			{
				SetAllSubItemsBackgroundColor(selectedItem, this.spotifyRed);
			}
			else
			{
				SetAllSubItemsBackgroundColor(selectedItem, this.defaultItemBackgroundColor);
			}
		}

		public void OnChangeItemTextColorButtonClicked()
		{
			ListViewItem selectedItem = this.ListView.SelectedItems[0];
			if (selectedItem.ForeColor == this.defaultItemTextColor)
			{
				SetAllSubItemsTextColor(selectedItem, Color.cyan);
			}
			else
			{
				SetAllSubItemsTextColor(selectedItem, this.defaultItemTextColor);
			}
		}

		private void SetAllSubItemsBackgroundColor(ListViewItem item, Color color)
		{
			item.BackColor = color;

			foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
			{
				subItem.BackColor = color;
			}
		}

		private void SetAllSubItemsTextColor(ListViewItem item, Color color)
		{
			item.ForeColor = color;

			foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
			{
				subItem.ForeColor = color;
			}
		}

		public void OnChangeControlBackgroundColorButtonClicked()
		{
			if (this.ListView.BackColor == this.defaultControlBackgroundColor)
			{
				this.ListView.BackColor = this.spotifyGreen;
			}
			else
			{
				this.ListView.BackColor = this.defaultControlBackgroundColor;
			}
		}

		private void AddTestData()
		{
			if (this.ListView != null)
			{
				this.ListView.SuspendLayout();
				{
					ColumnHeader NameColumn = new ColumnHeader();
					NameColumn.Text = "TRACK";
					this.ListView.Columns.Add(NameColumn);

					ColumnHeader DescriptionColumn = new ColumnHeader();
					DescriptionColumn.Text = "ARTIST";
					this.ListView.Columns.Add(DescriptionColumn);

					ColumnHeader EffectsColumn = new ColumnHeader();
					EffectsColumn.Text = "TIME";
					this.ListView.Columns.Add(EffectsColumn);

					ColumnHeader PriceColumn = new ColumnHeader();
					PriceColumn.Text = "ALBUM";
					this.ListView.Columns.Add(PriceColumn);

					this.columnWidthStates = new int[this.columnCount];

					AddListViewItem(new string[] { "Can't Get Better than this (Original Version)", "Mathew Gil, John Courtidis, Sam Littlemore, Parachute Youth", "5:02", "Can't Get Better Than This" });
					AddListViewItem(new string[] { "Forever", "Haim", "4:05", "Forever EP" });
					AddListViewItem(new string[] { "City Boy", "Donkeyboy", "3:25", "City Boy" });
					AddListViewItem(new string[] { "Dance All Night (feat. Matisyahu)", "The Dirty Heads", "3:26", "Cabin By the Sea" });
					AddListViewItem(new string[] { "Call it Off", "Washed Out", "3:33", "Within and Without" });
					AddListViewItem(new string[] { "Because Of You", "C2C, Pigeon John", "3:42", "Tetra" });
					AddListViewItem(new string[] { "Flutes", "Hot Chip", "7:05", "In Our Heads" });
					AddListViewItem(new string[] { "Get Lucky", "Daft Punk, Pharrell Williams, Nile Rodgers", "6:10", "Random Access Memories" });
					AddListViewItem(new string[] { "Candy", "Robbie Williams", "3:21", "Candy" });
					AddListViewItem(new string[] { "Body Work", "Morgan Page, Tegan And Sara", "3:53", "In The Air" });
					AddListViewItem(new string[] { "Free to Flee", "OHO", "4:00", "Land of the Happy" });
					AddListViewItem(new string[] { "Bassline", "Reverend And The Makers", "3:09", "@Reverend_Makers" });
					AddListViewItem(new string[] { "Freewheel", "Josh Osho", "3:36", "L.i.f.e (Deluxe)" });
					AddListViewItem(new string[] { "Fine China", "Chris Brown", "3:34", "Fine China" });
					AddListViewItem(new string[] { "50 Ways to Say Goodbye", "Train", "4:08", "California 37" });
					AddListViewItem(new string[] { "Bright Lights Bigger City", "Cee Lo Green", "3:38", "The Lady Killer" });
					AddListViewItem(new string[] { "Domino", "Jessie J", "3:52", "Domino" });
					AddListViewItem(new string[] { "Blackout", "Breathe Carolina", "3:30", "Hell Is What You Make It" });
					AddListViewItem(new string[] { "Get Free - Bonde Do Role Remix", "Major Lazer", "3:44", "Get Free [feat. Amber of Dirty Projectors]" });
					AddListViewItem(new string[] { "Don't Leave Me [Ne Me Quitte Pas]", "Regina Spektor", "3:37", "What We Saw From The Cheap Seats" });
					AddListViewItem(new string[] { "4th Of July (Fireworks)", "Kellis", "5:28", "Flesh Tone" });
					AddListViewItem(new string[] { "Chase Us Around (feat. Madi Diaz)", "Viceroy", "4:56", "Chase Us Around (feat. Madi Diaz)" });
					AddListViewItem(new string[] { "Don't Stop", "Gigamesh", "4:08", "Kitsune: All My Life - EP" });
					AddListViewItem(new string[] { "Battle Scars - featuring Lupe Fiasco", "Guy Sebastian", "4:10", "Battle Scars" });
					AddListViewItem(new string[] { "Anything For Love", "Cobra Starship", "4:11", "Night Shades" });
					AddListViewItem(new string[] { "Chandelier - feat. Lauriana Mae", "B.o.B", "4:00", "Strange Clouds" });
					AddListViewItem(new string[] { "Fifteen", "Goldroom", "5:00", "Fifteen - Single" });
					AddListViewItem(new string[] { "Classic", "MKTO", "2:55", "Classic" });
					AddListViewItem(new string[] { "D.A.N.C.E. - Radio Edit", "Justice", "3:30", "D.A.N.C.E." });
					AddListViewItem(new string[] { "Faster Horses", "MNDR", "3:40", "Feed Me Diamonds" });
				}
				this.ListView.ResumeLayout();
			}
		}

		private ListViewItem GetListViewItemFromStrings(string[] subItemTexts)
		{
			ListViewItem item = new ListViewItem(subItemTexts);
			// Allow the sub items to have their own individual foreground/text colours.
			item.UseItemStyleForSubItems = false;
			item.SubItems[2].ForeColor = new Color(0.58f, 0.58f, 0.60f);
			return item;
		}

		private void AddListViewItem(string[] subItemTexts)
		{
			this.ListView.Items.Add(GetListViewItemFromStrings(subItemTexts));
		}
	}
}
