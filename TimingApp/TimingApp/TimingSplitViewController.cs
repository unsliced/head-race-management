using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using TimingApp.ApplicationLayer;
using Dropins.Chooser.iOS;

namespace TimingApp
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.

	public class TimingSplitViewController : UISplitViewController
	{
		public TimingSplitViewController () : base()
		{
			// create our master and detail views
			var masterViewController = new TimingMasterViewController ();
			var masterNavigationController = new UINavigationController (masterViewController);
			var detailViewController = new TimingDetailViewController ();
			var detailNavigationController = new UINavigationController (detailViewController);

			WeakDelegate = detailViewController;

			detailViewController.ItemAdded += (item) => masterViewController.AddItem(item);

			this.ShouldHideViewController = (svc, vc, orientation) => {
				return false;
			};

			//			Li stFiles ("https://www.dropbox.com/sh/o0h70ccg9t3ssql/itnWVUH6K6");

			// ALWAYS SET THIS LAST (since iOS5.1)
			// https://bugzilla.xamarin.com/show_bug.cgi?id=3803
			// http://spouliot.wordpress.com/2012/03/26/events-vs-objective-c-delegates/
			// create an array of controllers from them and then assign it to the 
			// controllers property
			ViewControllers = new UIViewController[] { masterNavigationController, detailNavigationController };
		}
	

//		void ListFiles (string link)
//		{
//			var dbc = new DBChooser ("").
//
//			DBError error;
//			DBPath path = new DBPath ();
//			var contents = DBFilesystem.SharedFilesystem.ListFolder (path, out error);
//			foreach (DBFileInfo info in contents) {
//				Console.WriteLine (info.Path);
//			}    
//		}
	}
}
