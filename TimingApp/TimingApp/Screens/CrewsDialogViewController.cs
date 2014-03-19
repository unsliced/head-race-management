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
using MonoTouch.CoreGraphics;

namespace TimingApp
{
	public class CrewsDialogViewController : DialogViewController
	{
		public event Action Changed;
		readonly IDictionary<int, CrewSelectionElement> _elements;

		// TODO - consider a crew object here, not a string 
		public CrewsDialogViewController(IDictionary<int, string> crews) : base (UITableViewStyle.Plain, null)
		{
			_elements = new Dictionary<int, CrewSelectionElement> ();
			Initialise (crews); 
		}

		void Initialise(IDictionary<int, string> crews)
		{
			foreach (var kvp in crews) 
			{
				var ce = new CrewSelectionElement (kvp.Key, kvp.Value, false);
				ce.Tapped += () => {
					Changed (); 	
				};
				_elements.Add (kvp.Key, ce);
			}
		}

		void PopulateTable()
		{
			var s = new Section();
			s.AddAll (_elements.Values);
			Root = new RootElement("Select crews") { s };
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// reload/refresh
			PopulateTable();			
		}

		// TODO - consider a delete action to remove known DNS or scratched crews 

		public void Remove (int crew)
		{
			_elements.Remove (crew);
		}
	}
	
}
