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
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace TimingApp
{
	public class Boat 
	{
		public int StartNumber { get; set; } 
		public string Name { get; set; } 
	}

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
		CrewsDialogViewController _popover;
		Timer timer;

		public TimingDetailViewController() : base (UITableViewStyle.Plain, null)
		{
			Initialize ();
		}


		protected void Initialize()
		{
			var crews = Create ().ToDictionary(b => b.StartNumber, b => b.Name);
			if(crews.Count == 0)
				crews = Enumerable.Range (1, 220).ToDictionary (i => i, i => "crew " + i);

			_popover = new CrewsDialogViewController(crews);
			UIPopoverController myPopOver = new UIPopoverController(_popover); 
			_popover.Changed += () => 
			{
				PopulateTable();
			};


			Action action = 
				() => {
					if (Root != null)
				{
					// TODO - looks like this isn't actually updating the caption 
					Root.Caption = DateTime.Now.ToString ("HH:mm:ss"); // ReloadData();
				}
				};
			timer = new Timer (_ => action(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds (900));



			// TODO - this should be a setting dialog which appears and has a done button? ideally just a small one? 
			NavigationItem.RightBarButtonItem = new UIBarButtonItem("Crews", UIBarButtonItemStyle.Plain, null);
			NavigationItem.RightBarButtonItem.Clicked += (sender, e) => { myPopOver.PopoverContentSize = new SizeF(450f, 420f);
				myPopOver.PresentFromBarButtonItem (NavigationItem.RightBarButtonItem, UIPopoverArrowDirection.Up, true); };
		}

		// chris - euch. 
		public void Reset ()
		{
			var crews = Create ().ToDictionary(b => b.StartNumber, b => b.Name);
			if(crews.Count == 0)
				crews = Enumerable.Range (1, 220).ToDictionary (i => i, i => "crew " + i);

			_popover.Reset (crews);
		}

		public IList<Boat> Create()
		{
			try{
				// chris - this needs to be strengthened and whatnot 
				const string path = "Json/VetsHead2014.json";

				bool exists = File.Exists(path);
				if(exists)
				{
					string json = File.ReadAllText(path);
									
					var intermediate = JsonConvert.DeserializeObject<List<Boat>>(json) as IList<Boat>;

					return intermediate;
				}
			}
			catch(Exception ex)
			{

			}

			return new List<Boat>();
		}

		Section AddAnotherItem(string heading)
		{
			var notes = new EntryElement ("Notes", "identifying marks", string.Empty);
			var button = new StyledStringElement("Insert mystery finisher");
			button.TextColor = UIColor.LightTextColor;
			button.DetailColor = UIColor.LightTextColor;
			button.BackgroundColor = UIColor.DarkTextColor;

			var newRoot = new Section (heading) { notes, button };
			button.Tapped += () => {
				ItemAdded (new TimingItem ("Vets Head 2014", Location, Coordinates, OurLittleSecret, -1, DateTime.Now, notes.Value));
				notes.Value = string.Empty;
			};
			return newRoot;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
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
			Root = new RootElement ("<time>") { _sections };
			Root.Add (AddAnotherItem ("Unidentified finisher"));
		}

		public override void Selected (NSIndexPath indexPath)
		{
			if (indexPath.Section < _sections.Count) 
			{
				// TODO - shouldn't need to parse something we put in there 
				int crew = int.Parse (_sections [indexPath.Section].Caption);
				_popover.Remove(crew);
				ItemAdded (new TimingItem ("Vets Head 2014", Location, Coordinates, OurLittleSecret, crew, DateTime.Now, String.Empty));
				PopulateTable ();
			}
			else
				base.Selected (indexPath);
		}

		public string Location { get; set; }
		public string OurLittleSecret { get; set; }
		// TODO - get the GPS if available
		public string Coordinates { get { return "Where are we?"; } } 
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
