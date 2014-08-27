using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using TimingApp.ApplicationLayer;
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
		}

		public void Go(string location, string secret)
		{
			_details.Location = location;
			_details.OurLittleSecret = secret;
			Initialize ();
		}

		void Initialize()
		{
			_timingItemManager = null;//  new TimingItemManager () { Race = new Race() { Name = _details.Race, Code = _details.Race}, Location =  _details.Location, Token = _details.OurLittleSecret}; 

			_popover = new SettingsDialogViewController (_details.Location, _details.OurLittleSecret);
			UIPopoverController myPopOver = new UIPopoverController(_popover); 
			_popover.Changed += () => 
			{
				_details.Location = _popover.Location;
				_details.OurLittleSecret = _popover.Secret;
//				_timingItemManager = new TimingItemManager () { Race = new Race() { Name = _details.Race, Code = _details.Race}, Location =  _details.Location, Token = _details.OurLittleSecret}; 
				PopulateTable(true);
			};
			_popover.Clear += () => 
			{
				// _timingItemManager.Reset();
				_details.Reset();
				PopulateTable(false);
			};
			_popover.Save += () => {
//				_timingItemManager.SaveItem (null);
				_popover.UpdateStatus(_timingItemManager.Status);
			};

			NavigationItem.RightBarButtonItem = new UIBarButtonItem("Settings", UIBarButtonItemStyle.Plain, null);
			NavigationItem.RightBarButtonItem.Clicked += (sender, e) => { myPopOver.PopoverContentSize = new SizeF(450f, 800f);
				myPopOver.PresentFromBarButtonItem (NavigationItem.RightBarButtonItem, UIPopoverArrowDirection.Left, true); };
			PopulateTable (true);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// reload/refresh
			PopulateTable(false);			
		}


		protected void PopulateTable(bool remove)
		{
			_items = new List<TimingItem> (); // _timingItemManager.Items.ToList ();
			if (remove)
				foreach (var item in _items)
					_details.Remove (item.StartNumber);
			var rows = from t in _items 
				orderby t.Time descending
				select (Element)new StringElement (String.Format("{0} : {1}", t.StartNumber, t.Time.ToString("HH:mm:ss.fff")), t.Notes);
			var s = new Section ();
			s.AddAll(rows);
			Root = new RootElement (string.Format ("{0}/{1}: {2} crews", _popover.Location, _popover.Secret, _items.Count)) { s }; 
		}

		public void AddItem(TimingItem item)
		{
			_timingItemManager.SaveBoat (null); // (item);
			_popover.UpdateStatus(_timingItemManager.Status);
			PopulateTable (false);
		}

		// TODO - click on the master list to bring it up for notes in the detail view 
	}
}
	