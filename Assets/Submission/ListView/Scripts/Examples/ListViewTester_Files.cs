namespace Examples
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Endgame;

	public class ListViewTester_Files : MonoBehaviour
	{
		public ListView ListView;

		private List<string> currentDirectoryChain = new List<string>();

		private string currentDirectory
		{
			get
			{
				string result = string.Empty;

				foreach (string directoryName in this.currentDirectoryChain)
				{
					result += directoryName + "/";
				}

				return result;
			}
		}

		public void Start()
		{
			this.currentDirectoryChain.Add("C:");

			this.AddTestData();

			this.ListView.ItemActivate += OnItemActivated;
			this.ListView.ColumnClick += OnColumnClick;

			// Initialise an array with some example column widths
			// that will be toggled between by clicking on the column header.
			// (-1 in Windows Forms means size to the longest item, and
			// -2 means size to the column header.)
			this.ListView.Columns[0].Width = 200;
			this.ListView.Columns[1].Width = 150;
			this.ListView.Columns[2].Width = 100;
		}

		private void AddTestData()
		{
			if (this.ListView != null)
			{
				// Add columns.
				AddColumns();

				// Add items.
				AddFiles();
			}
		}

		private void AddColumns()
		{
			this.ListView.SuspendLayout();
			{
				AddColumnHeader("Name");
				AddColumnHeader("Date modified");
				AddColumnHeader("Size");
			}
			this.ListView.ResumeLayout();
		}

		private abstract class DirectoryEntry
		{
			public string Name = null;
			public System.DateTime LastModifiedTime = System.DateTime.Now;
			public long Size = 0;

			public DirectoryEntry(string name, System.DateTime lastModifiedTime, long size)
			{
				this.Name = name;
				this.LastModifiedTime = lastModifiedTime;
				this.Size = size;
			}
		}

		private class Directory : DirectoryEntry
		{
			public Directory(string name, System.DateTime lastModifiedTime)
				: base(name, lastModifiedTime, 0)
			{
			}
		}

		private class File : DirectoryEntry
		{
			public File(string name, System.DateTime lastModifiedTime, long size)
				: base(name, lastModifiedTime, size)
			{
			}
		}

		private IEnumerable<Directory> GetSubDirectories(System.IO.DirectoryInfo parentDirectoryInfo)
		{
			List<Directory> subDirectories = new List<Directory>();

#if !UNITY_WEBPLAYER

			System.IO.DirectoryInfo[] subDirectoriesInfos = parentDirectoryInfo.GetDirectories();
			foreach (System.IO.DirectoryInfo subDirectoryInfo in subDirectoriesInfos)
			{
				subDirectories.Add(new Directory(subDirectoryInfo.Name, subDirectoryInfo.LastWriteTime));
			}

#else //!UNITY_WEBPLAYER

		for (int i = 0; i < 10; i++)
		{
			subDirectories.Add(new Directory(string.Format("Placeholder folder {0}", i + 1), System.DateTime.Now));
		}

#endif //!UNITY_WEBPLAYER

			return subDirectories;
		}

		private IEnumerable<File> GetFiles(System.IO.DirectoryInfo parentDirectoryInfo)
		{
			List<File> files = new List<File>();

#if !UNITY_WEBPLAYER

			System.IO.FileInfo[] fileInfos = parentDirectoryInfo.GetFiles();
			foreach (System.IO.FileInfo fileInfo in fileInfos)
			{
				files.Add(new File(fileInfo.Name, fileInfo.LastWriteTime, fileInfo.Length));
			}

#else //!UNITY_WEBPLAYER

		for (int i = 0; i < 10; i++)
		{
			files.Add(new File(string.Format("Placeholder file {0}", i + 1), System.DateTime.Now, Random.Range(2048, 50000000)));
		}

#endif //!UNITY_WEBPLAYER

			return files;
		}

		private void AddFiles()
		{
			System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(this.currentDirectory);
			IEnumerable<Directory> subDirectories = GetSubDirectories(directoryInfo);
			IEnumerable<File> files = GetFiles(directoryInfo);

			this.ListView.SuspendLayout();
			{
				this.ListView.Items.Clear();

				// Add the previous folder.
				if (this.currentDirectoryChain.Count > 1)
				{
					AddListViewItem("..", string.Empty, string.Empty, null);
				}

				foreach (Directory subDirectory in subDirectories)
				{
					AddListViewItem(subDirectory.Name, subDirectory.LastModifiedTime.ToString(), string.Empty, subDirectory);
				}

				foreach (File file in files)
				{
					AddListViewItem(file.Name, file.LastModifiedTime.ToString(), GetPrettyFileSizeTextFromInteger(file.Size), file);
				}
			}
			this.ListView.ResumeLayout();
		}

		private void AddListViewItem(string name, string lastModified, string fileSize, object tag)
		{
			ListViewItem listViewItem = new ListViewItem(name);
			listViewItem.Tag = tag;
			listViewItem.SubItems.Add(lastModified);
			listViewItem.SubItems.Add(fileSize);
			listViewItem.UseItemStyleForSubItems = false;
			listViewItem.SubItems[1].ForeColor = new Color(0.43f, 0.43f, 0.43f);
			listViewItem.SubItems[2].ForeColor = new Color(0.43f, 0.43f, 0.43f);
			this.ListView.Items.Add(listViewItem);
		}

		private void OnItemActivated(object sender, System.EventArgs e)
		{
			ListView listView = (ListView)sender;
			ListViewItem item = listView.SelectedItems[0];
			if (item.Tag is File)
			{
				//File file = item.Tag as File;
			}
			else if (item.Tag is Directory)
			{
				Directory directory = item.Tag as Directory;
				this.currentDirectoryChain.Add(directory.Name);
				AddFiles();
			}
			else
			{
				// Up one level.
				this.currentDirectoryChain.RemoveAt(this.currentDirectoryChain.Count - 1);
				AddFiles();
			}
		}

		private void OnColumnClick(object sender, ListView.ColumnClickEventArgs e)
		{
			ListView listView = (ListView)sender;
			listView.ListViewItemSorter = new ListViewItemComparer(e.Column);
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

		private void AddColumnHeader(string title)
		{
			ColumnHeader columnHeader = new ColumnHeader();
			columnHeader.Text = title;
			this.ListView.Columns.Add(columnHeader);
		}
	}
}
