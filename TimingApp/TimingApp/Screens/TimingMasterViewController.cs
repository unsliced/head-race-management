using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using TimingApp.ApplicationLayer;
using TimingApp.DataLayer;
using TimingApp.Model;

namespace TimingApp
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	public class TimingMasterViewController : DialogViewController
	{
		IList<TimingItem> _items;
		TimingItemManager _timingItemManager;

		public TimingMasterViewController() : base (UITableViewStyle.Plain, null)
		{
			Initialize ();
		}

		protected void Initialize()
		{
			// TODO - set this in settings - perhaps in an opening splash screen? 
			_timingItemManager = new TimingItemManager (); 

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
			Root = new RootElement(_items.Count + " finishers") {s}; 
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
	