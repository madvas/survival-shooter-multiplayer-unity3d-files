namespace Examples
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using Endgame;

	public class ListViewTester_Leaderboard2 : MonoBehaviour
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

			this.ListView.Columns[0].Width = 80;
			this.ListView.Columns[1].Width = 65;
			this.ListView.Columns[2].Width = 200;
			this.ListView.Columns[3].Width = 100;
			this.ListView.Columns[4].Width = 125;
			this.ListView.Columns[5].Width = 100;
			this.ListView.Columns[6].Width = 150;

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

		public void OnAddNewItemButtonClicked()
		{
			this.itemAddedCount++;
			AddListViewItem(string.Format("aDDed {0}", this.itemAddedCount));

			// Select the new item and scroll to it.
			this.ListView.SelectedIndices.Add(this.ListView.Items.Count - 1);
			this.ListView.SetVerticalScrollBarValue(1);
		}

		public void OnInsertItemAtCurrentPositionButtonClicked()
		{
			this.itemInsertedCount++;

			int selectedIndex = this.ListView.SelectedIndices[0];
			this.ListView.Items.Insert(selectedIndex, CreateListViewItemFromPlayerName(string.Format("iNSeRTed {0}", this.itemInsertedCount)));
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
					AddColumn("Rank");
					AddColumn("Level");
					AddColumn("Name");
					AddColumn("Score");
					AddColumn("Time");
					AddColumn("Games");
					AddColumn("Average");

					this.columnWidthStates = new int[this.columnCount];

					AddListViewItem("Griddler");
					AddListViewItem("Kovsa");
					AddListViewItem("GrantTheAnt");
					AddListViewItem("TheKlaus");
					AddListViewItem("RUMble");
					AddListViewItem("Denzel");
					AddListViewItem("sVinX");
					AddListViewItem("tEd");
					AddListViewItem("ErkTic");
					AddListViewItem("RageBurns");
					AddListViewItem("IncredibleOrb");
					AddListViewItem("Skyline Spirit");
					AddListViewItem("eSTeeM");
					AddListViewItem("-=[A!M]bob");
					AddListViewItem("Shells");
					AddListViewItem("[sTyLER]");
					AddListViewItem("Piet");
					AddListViewItem("Innkvart");
					AddListViewItem("nilde");
					AddListViewItem("Boba Fett");
					AddListViewItem("Timaloy");
					AddListViewItem("eatbrain");
					AddListViewItem("TOMMA:-D");
				}
				this.ListView.ResumeLayout();
			}
		}

		private void AddColumn(string title)
		{
			ColumnHeader column = new ColumnHeader();
			column.Text = title;
			this.ListView.Columns.Add(column);
		}

		private int previousScore = 301540;
		private int previousRank = 1;

		private ListViewItem CreateListViewItemFromPlayerName(string playerName)
		{
			int level = Random.Range(30, 40);
			int score = this.previousScore;
			int timePlayedInMins = Random.Range(400, 800);
			int games = Random.Range(85, 120);
			int averageScore = score / games;
			int rank = this.previousRank;

			this.previousScore -= Random.Range(1, 1000);
			this.previousRank++;

			int timePlayedInHours = timePlayedInMins / 60;
			int timePlayedInMinsMod = timePlayedInMins % 60;

			string timePlayedString = string.Format("{0:00}h {1:00}m", timePlayedInHours, timePlayedInMinsMod);

			string[] subItemTexts = new string[]
		{
			rank.ToString(),
			level.ToString(),
			playerName,
			score.ToString(),
			timePlayedString,
			games.ToString(),
			averageScore.ToString()
		};

			ListViewItem item = new ListViewItem(subItemTexts);
			return item;
		}

		private void AddListViewItem(string playerName)
		{
			this.ListView.Items.Add(CreateListViewItemFromPlayerName(playerName));
		}
	}
}
