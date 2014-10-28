// File TableViewDataSource.cs
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Head.Common.Generate.Validators;
using Head.Common.Domain;
using Head.Common.Generate;


namespace Mac
{
	/// <summary>
	/// due credit is due for the basis for this class from http://www.netneurotic.net/Mono/MonoMac-NSTableView.html 
	/// </summary>
	[Register ("TableViewDataSource")]
	public class TableViewDataSource : NSTableViewDataSource
	{
		public TableViewDataSource ()
		{
		}

		// This method will be called by the NSTableView control to learn the number of rows to display.
		[Export ("numberOfRowsInTableView:")]
		public int NumberOfRowsInTableView(NSTableView table)
		{
			Console.WriteLine("numberOfRowsInTableView:");
			// We just return a static 2. We will have two rows.
			return 2;
		}

		// This method will be called by the control for each column and each row.
		[Export ("tableView:objectValueForTableColumn:row:")]
		public NSObject ObjectValueForTableColumn(NSTableView table, NSTableColumn col, int row)
		{
			Console.WriteLine("tableView:objectValueForTableColumn:row:");
			switch (row)
			{
			case 0:
				// We will write "Hello" in the first row...
				return new NSString("Hello");
			case 1:
				// ...and "World" in the second.
				return new NSString("Competitors");
				// Note that NSTableView requires an NSString, which we create with new NSString("bla").
			default:
				// We need a default value.
				return null;
			}
		}
	}

	/// <summary>
	/// due credit is due for the basis for this class from http://www.netneurotic.net/Mono/MonoMac-NSTableView.html 
	/// </summary>
	[Register ("CategoryDataSource")]
	public class CategoryDataSource : NSTableViewDataSource
	{
		readonly IList<ICategory> _categories;

		public CategoryDataSource (string path)
		{
			_categories = new CategoryCreator ().SetRawPath (path).Create ();
		}

		// This method will be called by the NSTableView control to learn the number of rows to display.
		[Export ("numberOfRowsInTableView:")]
		public int NumberOfRowsInTableView(NSTableView table)
		{
			Console.WriteLine("numberOfRowsInTableView:");
			return _categories.Count;
		}

		// This method will be called by the control for each column and each row.
		[Export ("tableView:objectValueForTableColumn:row:")]
		public NSObject ObjectValueForTableColumn(NSTableView table, NSTableColumn col, int row)
		{
			Console.WriteLine("tableView:objectValueForTableColumn:row:");
			if (row > _categories.Count - 1)
				return null;

			return new NSString(_categories [row].Name);
		}
	}

	/// <summary>
	/// due credit is due for the basis for this class from http://www.netneurotic.net/Mono/MonoMac-NSTableView.html 
	/// </summary>
	[Register ("CrewsDataSource")]
	public class CrewsDataSource : NSTableViewDataSource
	{
		public CrewsDataSource (string path)
		{
		}

		// This method will be called by the NSTableView control to learn the number of rows to display.
		[Export ("numberOfRowsInTableView:")]
		public int NumberOfRowsInTableView(NSTableView table)
		{
			Console.WriteLine("numberOfRowsInTableView:");
			// We just return a static 2. We will have two rows.
			return 2;
		}

		// This method will be called by the control for each column and each row.
		[Export ("tableView:objectValueForTableColumn:row:")]
		public NSObject ObjectValueForTableColumn(NSTableView table, NSTableColumn col, int row)
		{
			Console.WriteLine("tableView:objectValueForTableColumn:row:");
			switch (row)
			{
			case 0:
				// We will write "Hello" in the first row...
				return new NSString("Hello");
			case 1:
				// ...and "World" in the second.
				return new NSString("Crews");
				// Note that NSTableView requires an NSString, which we create with new NSString("bla").
			default:
				// We need a default value.
				return null;
			}
		}
	}
}
