namespace Examples
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using Endgame;

	public class ListViewTester_GamepadInput : MonoBehaviour
	{
		public ListView ListView1;
		public ListView ListView2;
		private const int columnWidthCount = 3;
		private int columnCount
		{
			get
			{
				return this.ListView1.Columns.Count;
			}
		}
		private int[] columnWidths = new int[columnWidthCount];
		private int[] columnWidthStates = null;
		private bool clickingAColumnSorts = true;

		public void Start()
		{
			// Add some test data (columns and items).
			this.AddTestData();

			// Add some events.
			// (Clicking on the first column header will sort by that column, and
			// clicking on any other column header will change that column's width
			// between default, sized to the header or sized to the longest item.)
			this.ListView1.ColumnClick += OnColumnClick;
			this.ListView2.ColumnClick += OnColumnClick;

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

			this.ListView1.Columns[0].Width = 250;
			this.ListView1.Columns[1].Width = 300;
			this.ListView1.Columns[2].Width = 100;
			this.ListView1.Columns[3].Width = 100;

			this.ListView2.Columns[0].Width = 250;
			this.ListView2.Columns[1].Width = 300;
			this.ListView2.Columns[2].Width = 100;
			this.ListView2.Columns[3].Width = 100;

			// Configure the gamepad inputs used by the listviews.
			this.ListView1.GamepadInputNameForActivate = "Submit";
			this.ListView2.GamepadInputNameForActivate = "Submit";
			this.ListView1.GamepadInputNameForPageUp = "PageUp";
			this.ListView2.GamepadInputNameForPageUp = "PageUp";
			this.ListView1.GamepadInputNameForPageDown = "PageDown";
			this.ListView2.GamepadInputNameForPageDown = "PageDown";

			// Select the first listview initially.
			this.ListView1.Select();
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
			this.ListView1.Columns[columnIndex].Width = columnWidth;
		}

		private void AddTestData()
		{
			ListView[] listViews = new ListView[] { this.ListView1, this.ListView2 };

			foreach (ListView listView in listViews)
			{
				if (listView != null)
				{
					listView.SuspendLayout();
					{
						ColumnHeader NameColumn = new ColumnHeader();
						NameColumn.Text = "From";
						listView.Columns.Add(NameColumn);

						ColumnHeader DescriptionColumn = new ColumnHeader();
						DescriptionColumn.Text = "Subject";
						listView.Columns.Add(DescriptionColumn);

						ColumnHeader EffectsColumn = new ColumnHeader();
						EffectsColumn.Text = "Date";
						listView.Columns.Add(EffectsColumn);

						ColumnHeader PriceColumn = new ColumnHeader();
						PriceColumn.Text = "Size";
						listView.Columns.Add(PriceColumn);

						this.columnWidthStates = new int[this.columnCount];

						AddListViewItem(listView, "Olympia Sykes", "gravida.sit.amet@id.org", "lectus ante dictum mi, ac mattis velit");
						AddListViewItem(listView, "Neil Patton", "et@ipsumac.net", "vel sapien imperdiet ornare. In");
						AddListViewItem(listView, "Melvin Dickerson", "egestas@estMauris.ca", "elementum purus, accumsan interdum libero dui");
						AddListViewItem(listView, "Kevin Lyons", "erat.Etiam.vestibulum@nuncinterdum.net", "quis diam luctus lobortis.");
						AddListViewItem(listView, "Macy Frederick", "nunc.nulla@magna.co.uk", "ut, molestie in, tempus");
						AddListViewItem(listView, "Nash Rios", "et.libero@uteratSed.com", "dictum eu, placerat");
						AddListViewItem(listView, "Erasmus Moran", "sit.amet.ante@ipsum.net", "lorem, eget mollis lectus pede et");
						AddListViewItem(listView, "Sydnee Vargas", "scelerisque.dui@pharetra.net", "id, mollis nec, cursus a, enim. Suspendisse aliquet, sem");
						AddListViewItem(listView, "Zahir Knight", "mus.Proin@odio.edu", "malesuada. Integer id magna et ipsum cursus vestibulum. Mauris magna.");
						AddListViewItem(listView, "Rhonda Powers", "Duis.dignissim.tempor@fringilla.edu", "elementum purus, accumsan interdum");
						AddListViewItem(listView, "Whoopi Harrington", "dui@Sedeu.edu", "pulvinar arcu et pede. Nunc sed");
						AddListViewItem(listView, "Kiona Cooley", "felis.Donec@lectussit.co.uk", "Donec fringilla. Donec feugiat metus sit amet ante.");
						AddListViewItem(listView, "Josephine Bean", "ultricies@sapiencursusin.com", "arcu. Vestibulum ut eros non enim");
						AddListViewItem(listView, "Meredith Hooper", "sit@orciinconsequat.co.uk", "non ante bibendum ullamcorper.");
						AddListViewItem(listView, "Malcolm Richards", "mus.Aenean@nisl.edu", "vitae odio sagittis semper. Nam");
						AddListViewItem(listView, "Mary Alvarado", "odio@Morbiaccumsanlaoreet.com", "mollis. Duis sit amet diam eu dolor egestas rhoncus.");
						AddListViewItem(listView, "Yoshi Day", "ligula.tortor@ataugue.co.uk", "quis accumsan convallis, ante");
						AddListViewItem(listView, "Harper Mejia", "Fusce.mollis@convallis.com", "et, eros. Proin ultrices. Duis volutpat nunc sit");
						AddListViewItem(listView, "Sophia Bradford", "sociis.natoque@dictumeueleifend.com", "ipsum porta elit, a feugiat tellus lorem eu");
						AddListViewItem(listView, "Nasim Bernard", "eget.volutpat@liberoettristique.edu", "scelerisque mollis. Phasellus libero mauris, aliquam eu, accumsan sed, facilisis");
						AddListViewItem(listView, "Cameron Camacho", "mollis.non@ornare.com", "sem semper erat, in consectetuer");
						AddListViewItem(listView, "Erin Coffey", "Cras@arcu.ca", "libero. Proin mi.");
						AddListViewItem(listView, "Forrest Wells", "a.feugiat.tellus@Nullamscelerisqueneque.com", "aliquet. Proin velit. Sed malesuada");
						AddListViewItem(listView, "Kadeem Phillips", "ornare@habitantmorbitristique.ca", "Proin velit. Sed");
						AddListViewItem(listView, "Walter Lane", "Morbi.accumsan@placeratvelit.edu", "Quisque tincidunt pede ac urna. Ut tincidunt vehicula risus. Nulla");
						AddListViewItem(listView, "Hannah Strickland", "nec@ornareIn.org", "tellus, imperdiet non, vestibulum nec, euismod in, dolor.");
						AddListViewItem(listView, "Beck Farmer", "lacinia.Sed@mienimcondimentum.com", "Nullam enim. Sed nulla ante, iaculis nec,");
						AddListViewItem(listView, "Renee Alvarez", "Nunc.ac@tincidunt.edu", "Nam ligula elit, pretium et,");
						AddListViewItem(listView, "Ulric Harvey", "amet@tinciduntorci.edu", "ac urna. Ut tincidunt vehicula risus. Nulla");
						AddListViewItem(listView, "Nelle Ewing", "non@quam.org", "nibh. Quisque nonummy ipsum non");
						AddListViewItem(listView, "Kessie Vega", "interdum@elementumdui.net", "tempor arcu. Vestibulum ut eros non enim commodo");
						AddListViewItem(listView, "Hyacinth Tucker", "convallis.est@Suspendisseseddolor.com", "convallis est, vitae sodales nisi magna");
						AddListViewItem(listView, "Thomas Ashley", "Nunc.mauris@sit.net", "eu dolor egestas rhoncus. Proin nisl sem, consequat nec,");
						AddListViewItem(listView, "Erasmus Alexander", "amet@diam.org", "ullamcorper magna. Sed eu eros. Nam");
						AddListViewItem(listView, "Melinda Dillon", "Donec@ullamcorperDuis.com", "elementum sem, vitae");
						AddListViewItem(listView, "Leilani Richardson", "dolor@velmaurisInteger.co.uk", "ante ipsum primis");
						AddListViewItem(listView, "Armand Hood", "Aliquam.adipiscing@nec.net", "sed tortor. Integer aliquam adipiscing lacus. Ut nec urna");
						AddListViewItem(listView, "Adrienne Oliver", "ut.odio@Pellentesqueultriciesdignissim.edu", "Phasellus dolor elit, pellentesque a, facilisis non, bibendum sed, est.");
						AddListViewItem(listView, "May Willis", "lorem.auctor@vitaeeratvel.edu", "ultricies ligula. Nullam enim. Sed nulla ante, iaculis nec, eleifend");
						AddListViewItem(listView, "Marsden Buck", "lorem@necurnaet.co.uk", "Quisque porttitor eros nec tellus. Nunc lectus pede, ultrices");
						AddListViewItem(listView, "Lee Buckley", "eu@liberoest.net", "ac mattis velit justo nec");
						AddListViewItem(listView, "Linda Santana", "at@eratSednunc.co.uk", "non nisi. Aenean eget metus. In");
						AddListViewItem(listView, "Britanni Higgins", "ridiculus.mus.Aenean@vitae.co.uk", "nulla. Donec non justo. Proin non massa non");
						AddListViewItem(listView, "Ocean Powers", "ipsum@Praesenteunulla.edu", "ridiculus mus. Proin vel");
						AddListViewItem(listView, "Sara Moran", "sit.amet@nuncestmollis.net", "Curabitur consequat, lectus sit");
						AddListViewItem(listView, "Dean Singleton", "Aliquam.auctor@urna.edu", "Duis cursus, diam at pretium aliquet, metus urna convallis");
						AddListViewItem(listView, "Miranda Bolton", "lorem@nuncacmattis.com", "justo. Praesent luctus. Curabitur");
						AddListViewItem(listView, "Louis Ross", "semper.rutrum.Fusce@rutrum.ca", "orci. Donec nibh. Quisque nonummy ipsum");
						AddListViewItem(listView, "Cedric Harrison", "nisl@loremipsum.org", "lectus rutrum urna,");
						AddListViewItem(listView, "Remedios Tyler", "Pellentesque@dolor.org", "Fusce mollis. Duis");
						AddListViewItem(listView, "Ori Bird", "montes@liberoProinmi.org", "enim. Etiam imperdiet dictum magna. Ut tincidunt orci quis lectus.");
					}
					listView.ResumeLayout();
				}
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

		private void AddListViewItem(ListView listView, string sender, string emailAddress, string subject)
		{
			listView.Items.Add(GetListViewItemFromStrings(sender, emailAddress, subject));
		}
	}
}
