using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using TimingApp.Data;
using TimingApp.Data.Interfaces;
using System.Diagnostics;
using TimingApp.Data.Factories;

namespace TimingApp.Portable.Pages
{

	public class AddRacePage : ContentPage 
	{
		// todo - would probably rather this be a slightly more admin type function 
		public event EventHandler Clicked; 
		string _name = string.Empty;
		public string NewRaceCode { get { return _name; } } 

		public AddRacePage()
		{
			Label label = new Label
			{
				Text = "Race Code",
				Font = Font.SystemFontOfSize(NamedSize.Small),
				HorizontalOptions = LayoutOptions.Center
			};
			var entry = new Entry {Placeholder = "code"};
			entry.TextChanged += (object sender, TextChangedEventArgs e) => 
				_name = entry.Text;

			Button button = new Button { Text = "button" };
			button.Clicked += (object sender, EventArgs e) =>
			{
				if(Clicked != null)
					Clicked(this, EventArgs.Empty);
			};

			Content = new StackLayout {
				Children = {
					label,
					entry, button 
				}
			};
		}
	}
	
}
