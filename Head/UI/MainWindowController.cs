
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace UI
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{

		protected int _numberOfTimesClicked = 0;

		internal static NSString FIRST_NAME = new NSString("firstname");
		internal static NSString LAST_NAME = new NSString("lastname");
		internal static NSString PHONE = new NSString("phone");

		EditController myEditController = null;

		internal static List<NSString> Keys = new List<NSString> { FIRST_NAME, LAST_NAME, PHONE};			
		internal const int FIRST_NAME_IDX = 0;
		internal const int LAST_NAME_IDX = 1;
		internal const int PHONE_IDX = 2;


		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}


		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			btnClickMe.Activated += (object sender, EventArgs e) => {
				_numberOfTimesClicked++;
				lblOutput.StringValue = "Clicked " + _numberOfTimesClicked + " times.";
			};

			// in the example this was based on a compiler directive - not sure why ... 
			UseBindingsByCode ();
		}

		void UseBindingsByCode()
		{
			NSTableColumn firstNameColumn = myTableView.FindTableColumn(FIRST_NAME);
			firstNameColumn.Bind("value",myContentArray,"arrangedObjects.firstname",null);

			NSTableColumn lastNameColumn = myTableView.FindTableColumn(LAST_NAME);
			lastNameColumn.Bind("value",myContentArray,"arrangedObjects.lastname",null);

			NSTableColumn phoneColumn = myTableView.FindTableColumn(PHONE);
			phoneColumn.Bind("value",myContentArray,"arrangedObjects.phone",null);


			List<NSObject> doubleClickObjects = new List<NSObject> {new NSString("inspect:"),
			NSNumber.FromBoolean(true),
			NSNumber.FromBoolean(true)};

			List<NSString> doubleClickKeys = new List<NSString> {new NSString("NSSelectorName"),
			new NSString("NSConditionallySetsHidden"),
			new NSString("NSRaisesForNotApplicableKeys")};

			NSDictionary doubleClickOptionsDict = NSDictionary.FromObjectsAndKeys(doubleClickObjects.ToArray(),doubleClickKeys.ToArray());

			myTableView.Bind("doubleClickArgument",myContentArray,"selectedObjects",doubleClickOptionsDict);
			myTableView.Bind("doubleClickTarget",this,"self",doubleClickOptionsDict);

			List<NSObject> enableOptionsObjects = new List<NSObject> {NSNumber.FromBoolean(true)};

			List<NSString> enableOptionsKeys = new List<NSString> {new NSString("NSRaisesForNotApplicableKeys")};

			NSDictionary enableOptionsDict = NSDictionary.FromObjectsAndKeys(enableOptionsObjects.ToArray(),enableOptionsKeys.ToArray());
			addButton.Bind("enabled",myContentArray,"canAdd",enableOptionsDict);
			removeButton.Bind("enabled",myContentArray,"canRemove",enableOptionsDict);

			List<NSObject> valueOptionsObjects = new List<NSObject> {NSNumber.FromBoolean(true),
				NSNumber.FromBoolean(true),
				NSNumber.FromBoolean(true)};

			List<NSString> valueOptionsKeys = new List<NSString> {new NSString("NSAllowsEditingMultipleValuesSelection"),
				new NSString("NSConditionallySetsEditable"),
				new NSString("NSRaisesForNotApplicableKeys")};

			NSDictionary valueOptionsDict = NSDictionary.FromObjectsAndKeys(valueOptionsObjects.ToArray(),valueOptionsKeys.ToArray());

			myFormFields.CellAtIndex(FIRST_NAME_IDX).Bind("value",myContentArray,"selection.firstname",valueOptionsDict);
			myFormFields.CellAtIndex(LAST_NAME_IDX).Bind("value",myContentArray,"selection.lastname",valueOptionsDict);
			myFormFields.CellAtIndex(PHONE_IDX).Bind("value",myContentArray,"selection.phone",valueOptionsDict);

			myContentArray.AddObserver(this,new NSString("selectionIndexes"),NSKeyValueObservingOptions.New,IntPtr.Zero);

			// finally, add the first record in the table as a default value.
			//
			// note: to allow the external NSForm fields to alter the table view selection through the "value" bindings,
			// added objects to the content array needs to be an "NSMutableDictionary" -
			//
			List<NSString> objects = new List<NSString> {new NSString("John"),
				new NSString("Doe"),
				new NSString("(333) 333-3333)")};

			var dict = NSMutableDictionary.FromObjectsAndKeys(objects.ToArray(), Keys.ToArray());
			myContentArray.AddObject(dict);

		}

		// 
		// Inspect our selected objects (user double-clicked them).
		// 
		// Note: this method will not get called until you make all columns in the table
		// as "non-editable".  So long as they are editable, double clicking a row will
		// cause the current cell to be editied.
		// 
		partial void Inspect (NSArray sender)
		{
			NSArray selectedObjects = sender;
			Console.WriteLine("inspect");

			int index;
			uint numItems = selectedObjects.Count;
			for (index = 0; index < numItems; index++)
			{
				NSDictionary objectDict =  new NSDictionary(selectedObjects.ValueAt(0));

				if (objectDict != null)
				{
					Console.WriteLine(string.Format("inspector item: [ {0} {1}, {2} ]",
						(NSString)objectDict[FIRST_NAME].ToString(),
						(NSString)objectDict[LAST_NAME].ToString(),
						(NSString)objectDict[PHONE].ToString()));
				}

				// setup the edit sheet controller if one hasn't been setup already
				if (myEditController == null)
					myEditController = new EditController();

				// remember which selection index we are changing
				int savedSelectionIndex = myContentArray.SelectionIndex;

				NSDictionary editItem =  new NSDictionary(selectedObjects.ValueAt(0));

				// get the current selected object and start the edit sheet
				NSMutableDictionary newValues = myEditController.edit(editItem, this);

				if (!myEditController.Cancelled)
				{
					// remove the current selection and replace it with the newly edited one
					var currentObjects = myContentArray.SelectedObjects;
					myContentArray.Remove(currentObjects);

					// make sure to add the new entry at the same selection location as before
					myContentArray.Insert(newValues,savedSelectionIndex);
				}
			}
		}

		/// <summary>
		/// This method demonstrates how to observe selection changes in our NSTableView's array controller
		/// </summary>
		/// <param name="keyPath">
		/// A <see cref="NSString"/>
		/// </param>
		/// <param name="ofObject">
		/// A <see cref="NSArrayController"/>
		/// </param>
		/// <param name="change">
		/// A <see cref="NSDictionary"/>
		/// </param>
		/// <param name="context">
		/// A <see cref="IntPtr"/>
		/// </param>
		[Export("observeValueForKeyPath:ofObject:change:context:")]
		private void observeValueForKeyPath(NSString keyPath, NSArrayController ofObject, NSDictionary change, IntPtr context)
		{
			Console.Write(String.Format("Table selection changed: keyPath = {0} : ",
				keyPath.ToString()));
			for(uint idx = 0; idx < ofObject.SelectionIndexes.Count; idx++)
			{
				Console.Write(ofObject.SelectionIndexes.IndexGreaterThanOrEqual(idx) + " ");
			}
			Console.WriteLine();
		}

		/// <summary>
		/// Ask the edit form to display itself to enter new record values
		/// </summary>
		/// <param name="sender">
		/// A <see cref="NSButton"/>
		/// </param>
		partial void Add (NSButton sender)
		{
			// setup the edit sheet controller if one hasn't been setup already
			if (myEditController == null)
				myEditController = new EditController();

			// ask our edit sheet for information on the record we want to add
			NSMutableDictionary newValues = myEditController.edit(null, this);

			if (!myEditController.Cancelled)
			{
				// add the new entry
				myContentArray.AddObject(newValues);
			}			
		}

		/// <summary>
		/// Remove an entry.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="NSButton"/>
		/// </param>
		partial void Remove (NSButton sender)
		{
			myContentArray.RemoveAt(myContentArray.SelectionIndex);
		}
	}
}

