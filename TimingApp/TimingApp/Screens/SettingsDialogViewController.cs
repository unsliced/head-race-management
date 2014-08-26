using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using TimingApp.ApplicationLayer;
using TimingApp.Portable.DataLayer;
using TimingApp.Portable.Model;
using System.Drawing;
using MonoTouch.CoreGraphics;
using System.Threading.Tasks;

namespace TimingApp
{
	public class SettingsDialogViewController : DialogViewController 
	{
		public event Action Changed;
		public event Action Clear;
		public event Action Save;

		IEnumerable<SaveStatus> _saveStatuses;

		public string Location { get; set; } 
		public string Secret { get; set; } 

		public SettingsDialogViewController(string location, string secret) : base (UITableViewStyle.Plain, null)
		{
			Location = location;
			Secret = secret;
			_saveStatuses = new List<SaveStatus> ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			PopulateTable();			
		}

		void PopulateTable()
		{
			var location = new EntryElement ("Location", "where are you?", Location);
			var code = new EntryElement ("Code", "your secret code", Secret);
			location.Changed += (object sender, EventArgs e) => {
				Location = location.Value;
				Changed ();
			};
			code.Changed += (object sender, EventArgs e) => 
			{
				Secret = code.Value;
				Changed ();
			};
			var clear = new StyledStringElement ("Clear the crews") { TextColor = UIColor.White, BackgroundColor = UIColor.Red };
			clear.Tapped += async () => {
				int button = await ShowAlert ("Really?", "This will remove all the crews so far. Are you *absolutely* sure?", "Cancel", "Yes, I'm sure");
				if (button == 1 && Clear != null)
					Clear ();
			};
			var save = new StyledStringElement("Force Save"){ TextColor = UIColor.DarkGray, BackgroundColor = UIColor.Green };
			save.Tapped += () => Save();
			var status = new Section ("Save status:");
			foreach (var stat in _saveStatuses) 
			{
				status.Add (new StyledStringElement (string.Format ("{0}: {1}", stat.Repo, stat.Success), stat.Success ? stat.WriteTime.ToShortTimeString () : "never written"));
			}
			Root = new RootElement("Settings") { new Section("Where are you?") { location, code }, new Section("Save") { save}, new Section("Clear") { clear}, status } ;
		}

		public void UpdateStatus (IEnumerable<SaveStatus> status)
		{
			_saveStatuses = status;
		}

		// Displays a UIAlertView and returns the index of the button pressed.
		static Task<int> ShowAlert (string title, string message, params string [] buttons)
		{
			var tcs = new TaskCompletionSource<int> ();
			var alert = new UIAlertView {
				Title = title,
				Message = message
			};
			foreach (var button in buttons)
				alert.AddButton (button);
			alert.Clicked += (s, e) => 
			{
				tcs.TrySetResult (e.ButtonIndex);
			};
			alert.Show ();
			return tcs.Task;
		}
	}

}
