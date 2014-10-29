using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using TimingApp.Data;
using TimingApp.Data.Enums;
using TimingApp.Data.Interfaces;

namespace TimingApp.Portable.Pages
{
	public class LocationPickerPage : ContentPage
	{
		LocationPickerPage(IRace race)
		{
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

			Picker racepicker = new Picker { WidthRequest = 300, Title = "Races" };

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
