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

	public class RacePickerPage : ContentPage 
	{
		RacePickerPage(IFactory<IRace> raceFactory)
		{
			Padding = new Thickness(20);
			Action action = async () =>
			{
				// todo - bring up a dialog box for the new one 
				var page = new ContentPage();
				var result = await page.DisplayAlert("Title", "Message", "Accept", "Cancel");
				// todo - double check that the code is not something that we've seen before. 
				Debug.WriteLine("success: {0}", result);
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
