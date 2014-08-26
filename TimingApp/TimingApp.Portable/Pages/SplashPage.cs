using System;
using Xamarin.Forms;
using TimingApp.Portable.DataLayer;
using TimingApp.Portable.Model;
using System.Collections.Generic;
using System.Linq;

namespace TimingApp.Portable.Pages
{

	public class SplashPage : ContentPage 
	{
		SplashPage()
		{
			var timingManager = new TimingItemManager ();
			Padding = new Thickness(20);

			Picker picker = new Picker { WidthRequest = 300, Title = "Races" };
			var location = new Entry { Placeholder = "Where are you?"};
			var token = new Entry { Placeholder = "What is your secret?"};
			var button = new Button { Text = "Go!", IsEnabled = false };

			IList<Race> races = timingManager.Races.ToList();
			races.Select(r => r.Name).ForEach (picker.Items.Add);

			picker.SelectedIndexChanged += (object sender, EventArgs e) => 
			{
				button.IsEnabled = !string.IsNullOrEmpty(location.Text) && !string.IsNullOrEmpty(token.Text) && picker.SelectedIndex >= 0;
			};

			location.PropertyChanging += (object sender, PropertyChangingEventArgs e) => button.IsEnabled = !string.IsNullOrEmpty(location.Text) && !string.IsNullOrEmpty(token.Text) && picker.SelectedIndex >= 0;
			token.PropertyChanging += (object sender, PropertyChangingEventArgs e) => button.IsEnabled = !string.IsNullOrEmpty(location.Text) && !string.IsNullOrEmpty(token.Text) && picker.SelectedIndex >= 0;

			button.Clicked += (object sender, EventArgs e) => 
			{
				Race race = races[picker.SelectedIndex];
				timingManager.Race = race;
				timingManager.Location = location.Text;
				timingManager.Token = token.Text;
				var page = new TimingMasterDetailPage(timingManager);

				Navigation.PushAsync(page);
			};

			var layout = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { location, token, picker, button } 
			};
		
			Content = layout;
		}

		public static NavigationPage Create()
		{
			return new NavigationPage(new SplashPage());
		}
	}
	
}
