namespace Examples
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using Endgame;

	public class ListViewTester_Email : MonoBehaviour
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

		public void Awake()
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
		}

		public void Start()
		{
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

			this.ListView.Columns[0].Width = 250;
			this.ListView.Columns[1].Width = 300;
			this.ListView.Columns[2].Width = 100;
			this.ListView.Columns[3].Width = 100;

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
			AddListViewItem("John Doe", "johndoe@hotmail.com", "Added this email right now (" + this.itemAddedCount + ")");

			// Select the new item and scroll to it.
			this.ListView.SelectedIndices.Add(this.ListView.Items.Count - 1);
			this.ListView.SetVerticalScrollBarValue(1);
		}

		public void OnInsertItemAtCurrentPositionButtonClicked()
		{
			this.itemInsertedCount++;

			int selectedIndex = this.ListView.SelectedIndices[0];
			this.ListView.Items.Insert(selectedIndex, GetListViewItemFromStrings("John Doe", "johndoe@hotmail.com", "Inserted this email right now (" + this.itemInsertedCount + ")"));
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
					NameColumn.Text = "From";
					this.ListView.Columns.Add(NameColumn);

					ColumnHeader DescriptionColumn = new ColumnHeader();
					DescriptionColumn.Text = "Subject";
					this.ListView.Columns.Add(DescriptionColumn);

					ColumnHeader EffectsColumn = new ColumnHeader();
					EffectsColumn.Text = "Date";
					this.ListView.Columns.Add(EffectsColumn);

					ColumnHeader PriceColumn = new ColumnHeader();
					PriceColumn.Text = "Size";
					this.ListView.Columns.Add(PriceColumn);

					this.columnWidthStates = new int[this.columnCount];

					AddListViewItem("Olympia Sykes", "gravida.sit.amet@id.org", "lectus ante dictum mi, ac mattis velit");
					AddListViewItem("Neil Patton", "et@ipsumac.net", "vel sapien imperdiet ornare. In");
					AddListViewItem("Melvin Dickerson", "egestas@estMauris.ca", "elementum purus, accumsan interdum libero dui");
					AddListViewItem("Kevin Lyons", "erat.Etiam.vestibulum@nuncinterdum.net", "quis diam luctus lobortis.");
					AddListViewItem("Macy Frederick", "nunc.nulla@magna.co.uk", "ut, molestie in, tempus");
					AddListViewItem("Nash Rios", "et.libero@uteratSed.com", "dictum eu, placerat");
					AddListViewItem("Erasmus Moran", "sit.amet.ante@ipsum.net", "lorem, eget mollis lectus pede et");
					AddListViewItem("Sydnee Vargas", "scelerisque.dui@pharetra.net", "id, mollis nec, cursus a, enim. Suspendisse aliquet, sem");
					AddListViewItem("Zahir Knight", "mus.Proin@odio.edu", "malesuada. Integer id magna et ipsum cursus vestibulum. Mauris magna.");
					AddListViewItem("Rhonda Powers", "Duis.dignissim.tempor@fringilla.edu", "elementum purus, accumsan interdum");
					AddListViewItem("Whoopi Harrington", "dui@Sedeu.edu", "pulvinar arcu et pede. Nunc sed");
					AddListViewItem("Kiona Cooley", "felis.Donec@lectussit.co.uk", "Donec fringilla. Donec feugiat metus sit amet ante.");
					AddListViewItem("Josephine Bean", "ultricies@sapiencursusin.com", "arcu. Vestibulum ut eros non enim");
					AddListViewItem("Meredith Hooper", "sit@orciinconsequat.co.uk", "non ante bibendum ullamcorper.");
					AddListViewItem("Malcolm Richards", "mus.Aenean@nisl.edu", "vitae odio sagittis semper. Nam");
					AddListViewItem("Mary Alvarado", "odio@Morbiaccumsanlaoreet.com", "mollis. Duis sit amet diam eu dolor egestas rhoncus.");
					AddListViewItem("Yoshi Day", "ligula.tortor@ataugue.co.uk", "quis accumsan convallis, ante");
					AddListViewItem("Harper Mejia", "Fusce.mollis@convallis.com", "et, eros. Proin ultrices. Duis volutpat nunc sit");
					AddListViewItem("Sophia Bradford", "sociis.natoque@dictumeueleifend.com", "ipsum porta elit, a feugiat tellus lorem eu");
					AddListViewItem("Nasim Bernard", "eget.volutpat@liberoettristique.edu", "scelerisque mollis. Phasellus libero mauris, aliquam eu, accumsan sed, facilisis");
					AddListViewItem("Cameron Camacho", "mollis.non@ornare.com", "sem semper erat, in consectetuer");
					AddListViewItem("Erin Coffey", "Cras@arcu.ca", "libero. Proin mi.");
					AddListViewItem("Forrest Wells", "a.feugiat.tellus@Nullamscelerisqueneque.com", "aliquet. Proin velit. Sed malesuada");
					AddListViewItem("Kadeem Phillips", "ornare@habitantmorbitristique.ca", "Proin velit. Sed");
					AddListViewItem("Walter Lane", "Morbi.accumsan@placeratvelit.edu", "Quisque tincidunt pede ac urna. Ut tincidunt vehicula risus. Nulla");
					AddListViewItem("Hannah Strickland", "nec@ornareIn.org", "tellus, imperdiet non, vestibulum nec, euismod in, dolor.");
					AddListViewItem("Beck Farmer", "lacinia.Sed@mienimcondimentum.com", "Nullam enim. Sed nulla ante, iaculis nec,");
					AddListViewItem("Renee Alvarez", "Nunc.ac@tincidunt.edu", "Nam ligula elit, pretium et,");
					AddListViewItem("Ulric Harvey", "amet@tinciduntorci.edu", "ac urna. Ut tincidunt vehicula risus. Nulla");
					AddListViewItem("Nelle Ewing", "non@quam.org", "nibh. Quisque nonummy ipsum non");
					AddListViewItem("Kessie Vega", "interdum@elementumdui.net", "tempor arcu. Vestibulum ut eros non enim commodo");
					AddListViewItem("Hyacinth Tucker", "convallis.est@Suspendisseseddolor.com", "convallis est, vitae sodales nisi magna");
					AddListViewItem("Thomas Ashley", "Nunc.mauris@sit.net", "eu dolor egestas rhoncus. Proin nisl sem, consequat nec,");
					AddListViewItem("Erasmus Alexander", "amet@diam.org", "ullamcorper magna. Sed eu eros. Nam");
					AddListViewItem("Melinda Dillon", "Donec@ullamcorperDuis.com", "elementum sem, vitae");
					AddListViewItem("Leilani Richardson", "dolor@velmaurisInteger.co.uk", "ante ipsum primis");
					AddListViewItem("Armand Hood", "Aliquam.adipiscing@nec.net", "sed tortor. Integer aliquam adipiscing lacus. Ut nec urna");
					AddListViewItem("Adrienne Oliver", "ut.odio@Pellentesqueultriciesdignissim.edu", "Phasellus dolor elit, pellentesque a, facilisis non, bibendum sed, est.");
					AddListViewItem("May Willis", "lorem.auctor@vitaeeratvel.edu", "ultricies ligula. Nullam enim. Sed nulla ante, iaculis nec, eleifend");
					AddListViewItem("Marsden Buck", "lorem@necurnaet.co.uk", "Quisque porttitor eros nec tellus. Nunc lectus pede, ultrices");
					AddListViewItem("Lee Buckley", "eu@liberoest.net", "ac mattis velit justo nec");
					AddListViewItem("Linda Santana", "at@eratSednunc.co.uk", "non nisi. Aenean eget metus. In");
					AddListViewItem("Britanni Higgins", "ridiculus.mus.Aenean@vitae.co.uk", "nulla. Donec non justo. Proin non massa non");
					AddListViewItem("Ocean Powers", "ipsum@Praesenteunulla.edu", "ridiculus mus. Proin vel");
					AddListViewItem("Sara Moran", "sit.amet@nuncestmollis.net", "Curabitur consequat, lectus sit");
					AddListViewItem("Dean Singleton", "Aliquam.auctor@urna.edu", "Duis cursus, diam at pretium aliquet, metus urna convallis");
					AddListViewItem("Miranda Bolton", "lorem@nuncacmattis.com", "justo. Praesent luctus. Curabitur");
					AddListViewItem("Louis Ross", "semper.rutrum.Fusce@rutrum.ca", "orci. Donec nibh. Quisque nonummy ipsum");
					AddListViewItem("Cedric Harrison", "nisl@loremipsum.org", "lectus rutrum urna,");
					AddListViewItem("Remedios Tyler", "Pellentesque@dolor.org", "Fusce mollis. Duis");
					AddListViewItem("Ori Bird", "montes@liberoProinmi.org", "enim. Etiam imperdiet dictum magna. Ut tincidunt orci quis lectus.");
				}
				this.ListView.ResumeLayout();
			}
		}

		private string GetPrettyFileSizeTextFromInteger(long fileSizeInBytes)
		{
			long kilobytes = (fileSizeInBytes / 1024);
			long megabytes = (kilobytes / 1024);
			long gigabytes = (megabytes / 1024);

			if (gigabytes > 0)
			{
				return string.Format("{0} GB", gigabytes);
			}
			else if (megabytes > 0)
			{
				return string.Format("{0} MB", megabytes);
			}
			else if (kilobytes > 0)
			{
				return string.Format("{0} KB", kilobytes);
			}
			else
			{
				return string.Format("{0} bytes", fileSizeInBytes);
			}
		}

		private int previousMinsAgo = 0;

		private ListViewItem GetListViewItemFromStrings(string sender, string emailAddress, string subject)
		{
			int minsAgo = this.previousMinsAgo;

			this.previousMinsAgo += Random.Range(15, 30);

			string timeAgo = "";
			if (minsAgo < 1)
			{
				timeAgo = "< 1 min ago";
			}
			else if (minsAgo < 60)
			{
				timeAgo = string.Format("{0} min{1} ago", minsAgo, minsAgo == 1 ? "" : "s");
			}
			else
			{
				int hoursAgo = minsAgo / 60;
				timeAgo = string.Format("{0} hour{1} ago", hoursAgo, hoursAgo == 1 ? "" : "s");
			}

			int size = Random.Range(0f, 1f) < 0.75f ? Random.Range(2500, 100000) : Random.Range(1000000, 3000000);

			string[] subItemTexts = new string[]
		{
			string.Format("\"{0}\" <{1}>", sender, emailAddress),
			subject,
			timeAgo,
			GetPrettyFileSizeTextFromInteger(size)
		};

			ListViewItem item = new ListViewItem(subItemTexts);

			// Randomly make it unread.
			if (Random.Range(0f, 1f) < 0.20f)
			{
				item.FontStyle = FontStyle.Bold;
			}

			return item;
		}

		private void AddListViewItem(string sender, string emailAddress, string subject)
		{
			this.ListView.Items.Add(GetListViewItemFromStrings(sender, emailAddress, subject));
		}
	}
}
