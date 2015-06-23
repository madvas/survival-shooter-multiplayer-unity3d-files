namespace Examples
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using Endgame;

	public class ListViewTester_Store : MonoBehaviour
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

			this.ListView.Columns[0].Width = 150;
			this.ListView.Columns[1].Width = 250;
			this.ListView.Columns[2].Width = 125;
			this.ListView.Columns[3].Width = 75;
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

		public void OnAddNewItemButtonClicked()
		{
			this.itemAddedCount++;
			string name = "ADDED ITEM (" + this.itemAddedCount + ")";

			ListViewItem listViewItem = new ListViewItem(new string[] { name, "Item added", "ATK +1" });
			this.ListView.Items.Add(listViewItem);

			// Select the new item and scroll to it.
			this.ListView.SelectedIndices.Add(this.ListView.Items.Count - 1);
			this.ListView.SetVerticalScrollBarValue(1);
		}

		public void OnInsertItemAtCurrentPositionButtonClicked()
		{
			this.itemInsertedCount++;
			string name = "INSERTED ITEM (" + this.itemInsertedCount + ")";

			ListViewItem listViewItem = new ListViewItem(new string[] { name, "Item inserted", "ATK +1" });
			int selectedIndex = this.ListView.SelectedIndices[0];
			this.ListView.Items.Insert(selectedIndex, listViewItem);
		}

		public void OnRemoveItemAtCurrentPositionButtonClicked()
		{
			int selectedIndex = this.ListView.SelectedIndices[0];
			//Debug.Log("removing " + selectedIndex);
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

			if (selectedItem.BackColor == this.ListView.DefaultItemBackgroundColor)
			{
				selectedItem.BackColor = Color.blue;
			}
			else
			{
				selectedItem.BackColor = this.ListView.DefaultItemBackgroundColor;
			}
		}

		public void OnChangeItemTextColorButtonClicked()
		{
			ListViewItem selectedItem = this.ListView.SelectedItems[0];
			if (selectedItem.ForeColor == Color.red)
			{
				selectedItem.ForeColor = this.ListView.DefaultItemTextColor;
			}
			else
			{
				selectedItem.ForeColor = Color.red;
			}
		}

		public void OnChangeControlBackgroundColorButtonClicked()
		{
			if (this.ListView.BackColor == this.ListView.DefaultControlBackgroundColor)
			{
				this.ListView.BackColor = Color.green;
			}
			else
			{
				this.ListView.BackColor = this.ListView.DefaultControlBackgroundColor;
			}
		}

		private void AddTestData()
		{
			if (this.ListView != null)
			{
				this.ListView.SuspendLayout();
				{
					ColumnHeader NameColumn = new ColumnHeader();
					NameColumn.Text = "Name";
					this.ListView.Columns.Add(NameColumn);

					ColumnHeader DescriptionColumn = new ColumnHeader();
					DescriptionColumn.Text = "Description";
					this.ListView.Columns.Add(DescriptionColumn);

					ColumnHeader EffectsColumn = new ColumnHeader();
					EffectsColumn.Text = "Effects";
					this.ListView.Columns.Add(EffectsColumn);

					ColumnHeader PriceColumn = new ColumnHeader();
					PriceColumn.Text = "Price";
					this.ListView.Columns.Add(PriceColumn);

					this.columnWidthStates = new int[this.columnCount];

					this.ListView.Items.Add(new ListViewItem(new string[] { "Gladius", "Sword of ancient Rome.", "ATK +3" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Scimitar", "Single edged curved sword", "ATK +5" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Cutlass", "Sword of the English Navy", "ATK +7" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Saber", "Light cavalry sword", "ATK +9" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Falchion", "Norman curved sword", "ATK +10" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Broadsword (Katzbalger)", "Simple mercenary's sword", "ATK +12" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Bekatowa (Bakatowa)", "Plain, easy-to-use war sword", "ATK +14" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Damascus Sword", "Fine Sword Honed To Razor-Edge", "ATK +17" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Hunter Sword (Jagdplaute)", "Ivory handled hunting sword	Sword", "ATK +20, DEF -1" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Bastard Sword", "Standard sword", "ATK +20" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Talwar", "Curved Indian sword", "ATK +22" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Sword of Hador (Sword of Helge)", "House of Hador heirloom", "ATK +24" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Luminus (Orcrist)", "Sword forged by elves", "ATK +26" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Harper", "The sword named Harper", "ATK +28" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Gram (Famous Sword Gram)", "The sword named Gram", "ATK +30" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Dark Blade (Glamdring)", "Sword forged by elves", "ATK +35, DEF +2" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Alucart Sword", "Resembles family sword", "ATK +2" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Tyrfing (Tyrfingr)", "Cursed dark sword	Sword", "ATK -30" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Mormegil (Holy Buster)", "Black sword - strong vs. holy", "ATK +25" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Thunderbrand (Thunder Sword of Indra)", "Lightning sword of Indra", "ATK +25" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Icebrand (Ice Sword of Rahab)", "Ice sword of Mim", "ATK +25" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Firebrand (Fire Sword of Agni)", "Fire sword of Oberon", "ATK +25" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Holy Sword", "Cross hilt - strong vs. undead", "ATK +26" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Terminus Est (Hrunting)", "Poisoned executioner's sword", "ATK +32, DEF +2" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Gurthang (Dainslef)", "Gets stronger when bloodied", "ATK +25, DEF +1" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Marsil (Leavatain)", "Powerful sword of flame", "ATK +33, DEF +1" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Mourneblade", "Feeds upon enemy's souls", "ATK +36, DEF +1" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Mablung Sword (Lemuria Sword)", "Spirit sword - improves DEF", "ATK +39, DEF +2" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Alucard Sword", "Mother's family heirloom", "ATK +42" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Badelaire (Scimitar of Barzai)", "Power increases with game play", "ATK + Game Time (in hours)" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Sword Familiar", "Sentient sword familiar", "ATK + Level of Sword (50 through 99)" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Vorpal Blade", "Slices cleanly through enemies", "ATK +25" }));
					this.ListView.Items.Add(new ListViewItem(new string[] { "Crissaegrim (Valmanway)", "Countless blades dice enemy", "ATK +36" }));

					// Add some random prices.
					foreach (ListViewItem item in this.ListView.Items)
					{
						float price = Random.Range(1, 1000);
						price *= 100;
						string text = "$" + price;
						item.SubItems.Add(text);
					}
				}
				this.ListView.ResumeLayout();
			}
		}
	}
}
