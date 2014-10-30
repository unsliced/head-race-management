using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using TimingApp.Data;
using TimingApp.Data.Enums;
using TimingApp.Data.Interfaces;
using System.Diagnostics;

namespace TimingApp.Portable.Pages
{
	public class LocationPickerPage : ContentPage
	{
		LocationPickerPage(IRace race)
		{
			// todo - summarise the race details
			// todo - ask for a token 
			// todo - show the available locations to select from 
			Title = race.Code;
		}

		public static NavigationPage Create(IRace race)
		{
			return new NavigationPage(new LocationPickerPage(race));
		}
	}

	public class AddRacePage : ContentPage 
	{
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

	public class RacePickerPage : ContentPage 
	{
		RacePickerPage(IFactory<IRace> raceFactory)
		{
			Padding = new Thickness(20);
			Action action = () =>
			{
				var racePage = new AddRacePage();
				// urgent - question is, how to capture the back button? 
				var page = new NavigationPage(racePage);
				page.Title = "New Race Code"; 
				racePage.Clicked += (object sender, EventArgs e) => 
				{
					Navigation.PopAsync();
					if(!string.IsNullOrEmpty(racePage.NewRaceCode))
						raceFactory.Add(racePage.NewRaceCode);
				};					

				Navigation.PushAsync(page);
			};

			var plus = new ToolbarItem("Add", "Add.png", action, priority: 0);
			ToolbarItems.Add(plus);

			Picker racepicker = new Picker { WidthRequest = 300, Title = "Races" };
			// todo - add the ability to add a new code which will then be looking for boat information
			// todo - consider if new races should be automatically picked up from the dropbox directory 
			IEnumerable<IRace> races = new List<IRace>();
			var keys = new List<string>();
			raceFactory.ListUpdated += (object sender, EventArgs e) => {
				races = raceFactory.Create();
				racepicker.Items.Clear();
				races.Select(r => string.Format("{0} - {1}", r.Code, r.Name)).ForEach (racepicker.Items.Add);
				keys = new List<string>(races.Select(r => r.Code));
			};

			Label header = new Label
			{
				Text = "Adhoc Race",
				Font = Font.SystemFontOfSize(NamedSize.Small),
				HorizontalOptions = LayoutOptions.Center
			};

			Switch switcher = new Switch
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			switcher.Toggled += (object sender, ToggledEventArgs e) => 
				racepicker.IsEnabled = !e.Value;

			// todo - consider how to deal with adhoc races 
			switcher.IsEnabled = false;

			var adhocLayout = new StackLayout {Orientation = StackOrientation.Horizontal, Children = { header, switcher } };

			var button = new Button { Text = "Go!", IsEnabled = true };

			// Accomodate iPhone status bar.
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			button.Clicked += (object sender, EventArgs e) => 
			{
				IRace race = races.Where(r => r.Code == keys[racepicker.SelectedIndex-1]).First();
				var page = LocationPickerPage.Create(race);

				Navigation.PushAsync(page);
			};

			var layout = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { racepicker, adhocLayout, button } 
			};

			Content = layout;
		}


		public static NavigationPage Create(IFactory<IRace> raceFactory)
		{
			return new NavigationPage(new RacePickerPage(raceFactory));
		}
	}
}
