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
	// TODO - crews pop over as a DialogViewController? 
	public class CrewsPopOver : UINavigationController 
	{
		public event Action Changed;
		readonly IDictionary<int, CrewSelectionElement> _elements;
		RootElement _root;

		public CrewsPopOver(IDictionary<int, string> crews)
		{
			_elements = new Dictionary<int, CrewSelectionElement> ();
			CreateRoot (crews);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad();
			// TODO - ideally we wouldn't have the back button on the nav controller - how performant is reconstructing this? 
			var dv = new DialogViewController(_root, true);
			this.PushViewController(dv, true);
		}

		void CreateRoot (IDictionary<int, string> crews)
		{
			foreach (var kvp in crews) {
				var ce = new CrewSelectionElement (kvp.Key, kvp.Value, false);
				ce.Tapped += () => {
					Changed (); 
				};
				_elements.Add (kvp.Key, ce);
			}
			PopulateRoot ();
		}
		void PopulateRoot()
		{
			var s = new Section();
			s.AddAll (_elements.Values);
			_root = new RootElement("Select crews") { s };
		}

		public void Remove (int crew)
		{
			_elements.Remove (crew);
			PopulateRoot ();
			var dv = new DialogViewController(_root, true);
			this.PushViewController(dv, true);
		}

		public IDictionary<int, string> Selected { get { return _elements.Where(ce => ce.Value.Value).ToDictionary(ce => ce.Key, ce => ce.Value.Name); } } 
	}
	
}
