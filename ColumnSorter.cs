using System;
using System.Collections;
using System.Text.RegularExpressions;	
using System.Windows.Forms;
using System.Globalization;

namespace Cameyo.RdpMon
{
	/// <summary>
	/// This class is an implementation of the 'IComparer' interface.
	/// </summary>
	public class ListViewColumnSorter : IComparer
	{
		public int ColumnToSort;
		public SortOrder OrderOfSort;
		private MainForm MainForm;
		//private NumberCaseInsensitiveComparer ObjectCompare;
		//private ImageTextComparer FirstObjectCompare;

		public ListViewColumnSorter(MainForm _mainForm)
		{
			ColumnToSort = 0;
			MainForm = _mainForm;
			//ObjectCompare = new NumberCaseInsensitiveComparer();
			//FirstObjectCompare = new ImageTextComparer();
		}

		/// <summary>
		/// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
		/// </summary>
		/// <param name="x">First object to be compared</param>
		/// <param name="y">Second object to be compared</param>
		/// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
		public int Compare(object x, object y)
		{
			int compareResult = 0;
			ListViewItem listviewX, listviewY;

			// Cast the objects to be compared to ListViewItem objects
			listviewX = (ListViewItem)x;
			listviewY = (ListViewItem)y;

			ListView listViewMain = listviewX.ListView;

			// Calculate correct return value based on object comparison
			if (listViewMain.Sorting != SortOrder.Ascending &&
				listViewMain.Sorting != SortOrder.Descending)
			{
				// Return '0' to indicate they are equal
				return compareResult;
			}

            // RdpMon.Addr
            if (listviewX.Tag is RdpMon.Addr)
            {
                var objX = (RdpMon.Addr)listviewX.Tag;
                var objY = (RdpMon.Addr)listviewY.Tag;
                if (ColumnToSort == MainForm.ColFailCount.DisplayIndex)
                    compareResult = objX.FailCount - objY.FailCount;
                else if (ColumnToSort == MainForm.ColSuccessCount.DisplayIndex)
                    compareResult = objX.SuccessCount - objY.SuccessCount;
                else if (ColumnToSort == MainForm.ColDuration.DisplayIndex)
                {
                    var durationX = objX.Last.Subtract(objX.First).TotalSeconds;
                    var durationY = objY.Last.Subtract(objY.First).TotalSeconds;
                    var ongoingX = objX.IsOngoing();
                    var ongoingY = objY.IsOngoing();
                    if (string.IsNullOrEmpty(listviewX.SubItems[ColumnToSort].Text))
                        compareResult = -1;
                    else if (string.IsNullOrEmpty(listviewY.SubItems[ColumnToSort].Text))
                        compareResult = 1;
                    else if ((ongoingX && ongoingY) || (!ongoingX && !ongoingY))   // Usual case: comparing apples to apples
                        compareResult = (int)durationX - (int)durationY;
                    else if (ongoingX)
                        compareResult = 1;
                    else // X is ongoing, but Y isn't
                        compareResult = -1;
                }
                else if (ColumnToSort == MainForm.ColIP.DisplayIndex)
                    compareResult = (int)(Utils.IP2Long(objX.AddrId) - Utils.IP2Long(objY.AddrId));
                else if (ColumnToSort == MainForm.ColFirstTime.DisplayIndex)
                    compareResult = DateTime.Compare(objX.First, objY.First);
                else if (ColumnToSort == MainForm.ColLastTime.DisplayIndex)
                    compareResult = DateTime.Compare(objX.Last, objY.Last);
                else
                    compareResult = StringComparer.InvariantCultureIgnoreCase.Compare(
                        listviewX.SubItems[ColumnToSort].Text.Trim(),
                        listviewY.SubItems[ColumnToSort].Text.Trim());
            }

            // RdpMon.Session
            else if (listviewX.Tag is RdpMon.Session)
            {
                var objX = (RdpMon.Session)listviewX.Tag;
                var objY = (RdpMon.Session)listviewY.Tag;
                if (ColumnToSort == MainForm.ColSessionStarted.DisplayIndex)
                    compareResult = DateTime.Compare(objX.Start, objY.Start);
                else if (ColumnToSort == MainForm.ColSessionEnded.DisplayIndex)
                    compareResult = DateTime.Compare(objX.End ?? DateTime.MinValue, objY.End ?? DateTime.MinValue);
                else if (ColumnToSort == MainForm.ColWtsSessionId.DisplayIndex)
                    compareResult = (int)objX.WtsSessionId - (int)objY.WtsSessionId;
                else
                    compareResult = StringComparer.InvariantCultureIgnoreCase.Compare(
                        listviewX.SubItems[ColumnToSort].Text.Trim(),
                        listviewY.SubItems[ColumnToSort].Text.Trim());
            }

            // Calculate correct return value based on object comparison
            if (OrderOfSort == SortOrder.Ascending)
			{
				// Ascending sort is selected, return normal result of compare operation
				return compareResult;
			}
			else if (OrderOfSort == SortOrder.Descending)
			{
				// Descending sort is selected, return negative result of compare operation
				return (-compareResult);
			}
			else
			{
				// Return '0' to indicate they are equal
				return 0;
			}
		}
	
		/// <summary>
		/// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
		/// </summary>
		public int SortColumn
		{
			set => ColumnToSort = value;
			get => ColumnToSort;
		}

		/// <summary>
		/// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
		/// </summary>
		public SortOrder Order
		{
			set => OrderOfSort = value;
			get => OrderOfSort;
		}
	}

}
