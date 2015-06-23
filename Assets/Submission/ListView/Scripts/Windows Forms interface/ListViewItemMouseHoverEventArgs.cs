namespace Endgame
{
	using System;
	using System.Runtime;
	using System.Runtime.InteropServices;

	public class ListViewItemMouseHoverEventArgs : EventArgs
	{
		private ListViewItem item;
		public ListViewItem Item { get { return this.item; } }

		public ListViewItemMouseHoverEventArgs(ListViewItem item)
		{
			this.item = item;
		}
	}
}