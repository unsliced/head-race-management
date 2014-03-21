using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using TimingApp.ApplicationLayer;
using TimingApp.DataLayer;
using TimingApp.Model;
using System.Drawing;

namespace TimingApp
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	public class TimingMasterViewController : DialogViewController
	{
		IList<TimingItem> _items;
		TimingItemManager _timingItemManager;
		SettingsDialogViewController _popover;

		TimingDetailViewController _details;

		public TimingMasterViewController(TimingDetailViewController details) : base (UITableViewStyle.Plain, null)
		{
			_details = details;
			Initialize ();
		}

		protected void Initialize()
		{
			_timingItemManager = new TimingItemManager (); 

			_popover = new SettingsDialogViewController ();
			UIPopoverController myPopOver = new UIPopoverController(_popover); 
			_popover.Changed += () => 
			{
				_details.Location = _popover.Location;
				_details.OurLittleSecret = _popover.Secret;
				PopulateTable();
			};
			_popover.Clear += () => 
			{
				_timingItemManager.Reset();
				_details.Reset();
				PopulateTable();
			};

			NavigationItem.RightBarButtonItem = new UIBarButtonItem("Settings", UIBarButtonItemStyle.Plain, null);
			NavigationItem.RightBarButtonItem.Clicked += (sender, e) => { myPopOver.PopoverContentSize = new SizeF(450f, 420f);
				myPopOver.PresentFromBarButtonItem (NavigationItem.RightBarButtonItem, UIPopoverArrowDirection.Left, true); };

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// reload/refresh
			PopulateTable();			
		}


		protected void PopulateTable()
		{
			_items = _timingItemManager.GetItems().ToList ();
			var rows = from t in _items
				select (Element)new StringElement (String.Format("{0} : {1}", t.StartNumber, t.Time.ToString("HH:mm:ss.fff")), t.Notes);
			var s = new Section ();
			s.AddAll(rows);
			Root = new RootElement (string.Format ("{0}/{1}: {2} crews", _popover.Location, _popover.Secret, _items.Count)) { s }; 
		}

		public void AddItem(TimingItem item)
		{
			_timingItemManager.SaveItem (item);
			PopulateTable ();
		}

		// TODO - click on the master list to bring it up for notes in the detail view 

		// TODO - by default focus the list of the bottom/most recently added item 

	}
}
	