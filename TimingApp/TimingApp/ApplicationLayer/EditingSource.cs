using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace TimingApp.ApplicationLayer
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.

	/// <summary>
	/// This is our subclass of the fixed-size Source that allows editing
	/// </summary>
	public class EditingSource : DialogViewController.Source {
		public EditingSource (DialogViewController dvc) : base (dvc) {}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			// Trivial implementation: we let all rows be editable, regardless of section or row
			return false;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			// trivial implementation: show a delete button always
			return UITableViewCellEditingStyle.None;
		}
	}
	
}
