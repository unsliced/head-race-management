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

			// an event handler to enable the go button 
			Action canIGo = () => button.IsEnabled = locationPicker.SelectedIndex >= 1 && !string.IsNullOrEmpty(token.Text) && racepicker.SelectedIndex >= 1;

			var races = TimingItemManager.RaceCodes;
			racepicker.Items.Add("-");
			races.Select(kvp => string.Format("{0} - {1}", kvp.Key, kvp.Value)).ForEach (racepicker.Items.Add);
			var keys = new List<string>(races.Keys);

			locationPicker.Items.Add("-");
			var locations = ((Endpoint[])Enum.GetValues(typeof(Endpoint))).ToList<Endpoint>().OrderBy(x => x.ToString()).ToList();
			locations.Select(l => l.ToString()).ForEach(locationPicker.Items.Add);

			racepicker.SelectedIndexChanged += (object sender, EventArgs e) => canIGo();
			locationPicker.SelectedIndexChanged += (object sender, EventArgs e) => canIGo();

			bool sequence = false;
			Label header = new Label
			{
				Text = "sequence-only",
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
			var sequenceLayout = new StackLayout {Orientation = StackOrientation.Horizontal, Children = { header, switcher } };

			token.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => canIGo();

			button.Clicked += (object sender, EventArgs e) => 
			{
				var timingManager = new TimingItemManager(keys[racepicker.SelectedIndex], token.Text, locations[locationPicker.SelectedIndex-1], sequence);
				var page = new TimingMasterDetailPage(timingManager);

				Navigation.PushAsync(page);
			};

			var layout = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { locationPicker, token, racepicker, sequenceLayout, button } 
			};
		
			Content = layout;
		}


		public static NavigationPage Create()
		{
			return new NavigationPage(new SplashPage());
		}

		// idea: overview of results 
	}
	
}
