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

	public class CrewSelectionElement : CheckboxElement
	{
		readonly int _index;
		readonly string _name;

		public CrewSelectionElement(int index, string name, bool value) : base(string.Format("{0}: {1}", index, name), value)
		{
			_index = index;
			_name = name;
		}

		public int Index { get { return _index; } } 
		public string Name { get { return _name; } } 
	}

	public class TimingDetailViewController : DialogViewController
	{
		public Action<TimingItem> ItemAdded;

		readonly IList<Section> _sections = new List<Section>();
		CrewsPopOver _popover;

		public TimingDetailViewController() : base (UITableViewStyle.Plain, null)
		{
			Initialize ();
		}

		protected void Initialize()
		{
//			NavigationItem.SetRightBarButtonItem (new UIBarButtonItem (UIBarButtonSystemItem.Add), false);
//			NavigationItem.RightBarButtonItem.Clicked += (sender, e) => { _roots.Add(AddAnotherItem("Finisher")); PopulateTable(); };

			_popover = new CrewsPopOver(Enumerable.Range(1, 212).ToDictionary(i => i, i => "crew " + i));
			UIPopoverController myPopOver = new UIPopoverController(_popover); 
			//			myPopOver.DidDismiss += (sender, e) => 
			//			{
			//				_questionFilter.Text = popover.Text;
			//				PopulateTable();
			//			};;
			_popover.Changed += () => 
			{
				PopulateTable();
			};

			// TODO - this should be a setting dialog which appears and has a done button? ideally just a small one? 
			NavigationItem.RightBarButtonItem = new UIBarButtonItem("Filter", UIBarButtonItemStyle.Plain, null);
			NavigationItem.RightBarButtonItem.Clicked += (sender, e) => { myPopOver.PopoverContentSize = new SizeF(450f, 420f);
				myPopOver.PresentFromBarButtonItem (NavigationItem.RightBarButtonItem, UIPopoverArrowDirection.Up, true); };
		}

		Section AddAnotherItem(string heading)
		{
			var num = new EntryElement ("Crew Number", "crew number", string.Empty);
			num.KeyboardType = UIKeyboardType.DecimalPad;
			// TODO - consider replacing the element with a CrewElement 
			num.Changed += (sender, e) => {} ;

			var notes = new EntryElement ("Notes", "identifying marks", string.Empty);
			var button = new StringElement ("Press me");

			var newRoot = new Section (heading) { num, notes, button };
			button.Tapped += () => {
				int sn;
				if (!int.TryParse (num.Value, out sn))
					sn = 0;
				ItemAdded (new TimingItem (sn, DateTime.Now, notes.Value));
				// TODO - the notes and num should reset after pressing 
				//PopulateTable ();
			};
			return newRoot;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// reload/refresh
			PopulateTable();			
		}

		void PopulateTable()
		{
			_sections.Clear ();
			// TODO - have a ticking clock in the title bar 
			foreach (var kvp in _popover.Selected) 
			{
				var ce = new CrewElement (kvp.Key, kvp.Value);
				var s = new Section(kvp.Key.ToString ()) { ce };
				_sections.Add (s);
			}
			Root = new RootElement ("Time ...") { _sections };
			Root.Add (AddAnotherItem ("Unidentified finisher"));
		}

		public override void Selected (NSIndexPath indexPath)
		{
			if (indexPath.Section < _sections.Count) 
			{
				// TODO - shouldn't need to parse something we put in there 
				int crew = int.Parse (_sections [indexPath.Section].Caption);
				_popover.Remove(crew);
				ItemAdded (new TimingItem (crew, DateTime.Now, String.Empty));
				PopulateTable ();
			}
			else
				base.Selected (indexPath);
		}
	}

	public class CrewElement : OwnerDrawnElement
	{
		readonly string _text; 

		// TODO - consider club specific colours 
		// TODO - add a notes box 
		public CrewElement (int number, string name) : base(UITableViewCellStyle.Default, "crewElement")
		{
			_text  = String.Format("Crew {0}: {1}", number, name);
		}

		public override void Draw (RectangleF bounds, CGContext context, UIView view)
		{
			UIColor.DarkGray.SetFill();
			context.FillRect(bounds);

			UIColor.Yellow.SetColor();   
			view.DrawString(_text, new RectangleF(10, 15, bounds.Width - 20, bounds.Height - 30), UIFont.BoldSystemFontOfSize(14.0f), UILineBreakMode.TailTruncation);
		}

		public override float Height (RectangleF bounds)
		{
			return 44.0f;
		}
	}
}
