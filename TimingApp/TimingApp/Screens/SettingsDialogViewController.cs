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
using System.Threading.Tasks;

namespace TimingApp
{
	public class SettingsDialogViewController : DialogViewController 
	{
		public event Action Changed;
		public event Action Clear;

		public string Location { get; set; } 
		public string Secret { get; set; } 

		public SettingsDialogViewController() : base (UITableViewStyle.Plain, null)
		{
			Location = string.Empty;
			Secret = string.Empty;
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
			Root = new RootElement("Settings") { new Section("Where are you?") { location, code }, new Section("Clear") { clear} } ;
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
