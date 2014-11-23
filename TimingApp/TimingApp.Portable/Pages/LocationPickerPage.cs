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
	public class LocationPickerPage : ContentPage
	{
		string _token;
		LocationPickerPage(IRepository repo, IRace race)
		{
			// todo - summarise the race details
			Title = race.Code;

			Picker locationpicker = new Picker { WidthRequest = 300, Title = "Locations" };
			locationpicker.Items.Clear();
			repo.LocationList.Select(l => l.Name).ForEach (locationpicker.Items.Add);

			var entry = new Entry {Placeholder = "token"};
			entry.TextChanged += (object sender, TextChangedEventArgs e) => 
				_token = entry.Text;

			var button = new Button { Text = "Go!", IsEnabled = true };

			// Accomodate iPhone status bar.
			this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			button.Clicked += (object sender, EventArgs e) => 
			{
				IEnumerable<IBoat> boats = repo.BoatList;
				var location = 
					new LocationFactory()
						.SetName(locationpicker.Items[locationpicker.SelectedIndex])
						.SetToken(_token)
						.SetItems(repo)
						.Create();
				var tim = new TimingItemManager(new List<IRepository> { repo}, location, boats);
				var page = TimingMasterDetailPage.Create(tim);

				Navigation.PushAsync(page);
			};

			Content = new StackLayout {
				Children = {
					locationpicker,
					entry, button 
				}
			};

		}

		public static NavigationPage Create(IRepository repo, IRace race)
		{
			return new NavigationPage(new LocationPickerPage(repo, race));
		}
	}
	
}
