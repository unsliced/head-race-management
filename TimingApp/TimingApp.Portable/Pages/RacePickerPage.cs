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
	public class RacePickerPage : ContentPage 
	{
		RacePickerPage(IRepository repo)
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
						repo.AddRaceCode(racePage.NewRaceCode);
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
			repo.RaceListUpdated += (object sender, EventArgs e) => {
				races = repo.RaceList;
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

			var button = new Button { Text = "Go!", IsEnabled = false };

			racepicker.SelectedIndexChanged += (object sender, EventArgs e) => 
			{
				if(racepicker.SelectedIndex >= 0 && !string.IsNullOrEmpty(keys[racepicker.SelectedIndex]))
					button.IsEnabled = true;
			};

			// Accomodate iPhone status bar.
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			button.Clicked += (object sender, EventArgs e) => 
			{
				IRace race = races.Where(r => r.Code == keys[racepicker.SelectedIndex]).First();
				repo.SetRace(race.Code);
				var page = LocationPickerPage.Create(repo, race);

				Navigation.PushAsync(page);
			};

			var layout = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { racepicker, button } // adhocLayout
			};

			Content = layout;
		}


//		public static NavigationPage Create(IRepository raceFactory)
//		{
//			return new NavigationPage(new RacePickerPage(raceFactory));
//		}
		public static Page Create(IRepository raceFactory)
		{
			return new NavigationPage(new RacePickerPage(raceFactory));
		}
	}
}
