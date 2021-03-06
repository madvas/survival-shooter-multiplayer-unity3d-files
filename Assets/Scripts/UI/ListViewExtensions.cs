﻿using UnityEngine;
using System.Collections;
using Endgame;

static class ListViewExtensions
{
	public static void AddColumn (this ListView listView, string columnTitle, int columnWidth)
	{
		ColumnHeader column = new ColumnHeader ();
		column.Text = columnTitle;
		int colIndex = listView.Columns.Add (column);
		listView.Columns [colIndex].Width = columnWidth;
	}

	public static void AddItem (this ListView listView, params string[] rowItems)
	{
		ListViewItem a = new ListViewItem (rowItems);
		a.ForeColor = new Color (25, 60, 99);
		listView.Items.Add (new ListViewItem (rowItems));
	}

	public static void ClearAllItems (this ListView listView)
	{
		int itemsCount = listView.Items.Count;
		for (int i = 0; i < itemsCount; i++) {
			listView.Items.RemoveAt (0);
		}
	}


}