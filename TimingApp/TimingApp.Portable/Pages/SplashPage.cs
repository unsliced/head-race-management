using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using TimingApp.Data;
using TimingApp.Data.Enums;

namespace TimingApp.Portable.Pages
{

	public class SplashPage : ContentPage 
	{
		SplashPage()
		{

			Padding = new Thickness(20);

			Picker racepicker = new Picker { WidthRequest = 300, Title = "Races" };
			Picker locationPicker = new Picker { WidthRequest = 300, Title = "Location" };
			var token = new Entry { Placeholder = "What is your secret?"};
			var button = new Button { Text = "Go!", IsEnabled = false };



			var races = TimingItemManager.RaceCodes;
			races.Select(kvp => string.Format("{0} - {1}", kvp.Key, kvp.Value)).ForEach (racepicker.Items.Add);
			var keys = new List<string>(races.Keys);

			var locations = ((Endpoint[])Enum.GetValues(typeof(Endpoint))).ToList<Endpoint>().OrderBy(x => x.ToString()).ToList();
			locations.Select(l => l.ToString()).ForEach(locationPicker.Items.Add);

			racepicker.SelectedIndexChanged += (object sender, EventArgs e) => 
			{
				button.IsEnabled = locationPicker.SelectedIndex >= 0 && !string.IsNullOrEmpty(token.Text) && racepicker.SelectedIndex >= 0;
			};
			locationPicker.SelectedIndexChanged += (object sender, EventArgs e) => 
			{
				button.IsEnabled = locationPicker.SelectedIndex >= 0 && !string.IsNullOrEmpty(token.Text) && racepicker.SelectedIndex >= 0;
			};

			bool sequence = false;
			Label header = new Label
			{
				Text = "Sequence",
				Font = Font.SystemFontOfSize(NamedSize.Small),
				HorizontalOptions = LayoutOptions.Center
			};

			Switch switcher = new Switch
			{
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			switcher.Toggled += (object sender, ToggledEventArgs e) => sequence = e.Value;

			// Accomodate iPhone status bar.
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			// Build the page.
			var sequenceLayout = new StackLayout { Children = { header,	switcher } };

			token.PropertyChanging += (object sender, PropertyChangingEventArgs e) => button.IsEnabled = locationPicker.SelectedIndex >= 0  && !string.IsNullOrEmpty(token.Text) && racepicker.SelectedIndex >= 0;

			button.Clicked += (object sender, EventArgs e) => 
			{
				var timingManager = new TimingItemManager(keys[racepicker.SelectedIndex], token.Text, locations[locationPicker.SelectedIndex], sequence);
				var page = new TimingMasterDetailPage(timingManager);

				Navigation.PushAsync(page);
			};

			var layout = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { locationPicker, token, racepicker, button, sequenceLayout } 
			};
		
			Content = layout;
		}


		public static NavigationPage Create()
		{
			return new NavigationPage(new SplashPage());
		}
	}
	
}
