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

	public class SplashDialogViewController : DialogViewController 
	{
		public event Action<string, string> Changed;

		public string Race { get; set; }
		public string Location { get; set; } 
		public string Secret { get; set; } 

		public SplashDialogViewController() : base (UITableViewStyle.Plain, null)
		{
			Race = string.Empty;
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
			var race = new StyledStringElement ("Race: Vets Head 2014"){ TextColor = UIColor.DarkGray, BackgroundColor = UIColor.Red };;
			var location = new EntryElement ("Location", "where are you?", Location);
			var code = new EntryElement ("Code", "your secret code", Secret);
			location.Changed += (object sender, EventArgs e) => {
				Location = location.Value;
			};
			code.Changed += (object sender, EventArgs e) => 
			{
				Secret = code.Value;
			};
			var submit = new StyledStringElement ("Submit") { TextColor = UIColor.White, BackgroundColor = UIColor.Red };
			submit.Tapped += () =>  {
				if(!string.IsNullOrEmpty(Location) && !string.IsNullOrEmpty(Secret))
					Changed(Location, Secret);
			};

			Root = new RootElement("Settings") { new Section("Race") { race }, new Section("Who are you?") { location, code }, new Section("Proceed") { submit} } ;
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
