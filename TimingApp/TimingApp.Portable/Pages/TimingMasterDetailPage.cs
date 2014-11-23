using System;
using Xamarin.Forms;
using System.Collections.Generic;
using TimingApp.Data;
using TimingApp.Data.Interfaces;
using System.Diagnostics;
using System.Globalization;

namespace TimingApp.Portable.Pages
{
	// idea: basic instructions? 
	// idea: colour scheme? 
	public class TimingMasterDetailPage : MasterDetailPage 
	{
		// todo: put a ticking clock/summary (e.g. number of finishers, progress bar, etc.) into the title bar 
		// idea: show the status of the saved 
		TimingMasterDetailPage (TimingItemManager manager)
		{
			// todo - why have we got two navigation bars at the top? 
			Master = new TimingMasterPage();
			Master.BindingContext = manager;
			Detail = new TimingDetailPage ();
			Detail.BindingContext = manager;
		}

		public static NavigationPage Create(TimingItemManager manager)
		{
			return new NavigationPage(new TimingMasterDetailPage(manager));
		}
	}

	class TimingMasterPage : ContentPage
	{
		public TimingMasterPage()
		{
			Title = "Finishers";

			var listView = new ListView() { HasUnevenRows = true };
			listView.SetBinding (ListView.ItemsSourceProperty, "Finished");
			listView.ItemTemplate = new DataTemplate(typeof(SeenCell));

			// todo - click on a finished boat to be able to edit the notes, just a cancel/ok dialog box 

			Content = listView;
		}
	}

	class TimingDetailPage : ContentPage
	{
		public TimingDetailPage()
		{
			// as suggested at 
			Action action = async () =>
			{
				var page = new ContentPage();
				var result = await page.DisplayAlert("Title", "Message", "Accept", "Cancel");
				Debug.WriteLine("success: {0}", result);
			};
			var more = new ToolbarItem("More", "More.png", action, priority: 1);
			var add = new ToolbarItem("New", "New.png", () => Debug.WriteLine("new"), priority: 0);
			var plus = new ToolbarItem("Add", "Add.png", () => Debug.WriteLine("add"), priority: 0);
//			ToolbarItems.Add(new ToolbarItem("Filter", "filter.png", async () =>
//			{
//				var page = new ContentPage();
//				var result = await page.DisplayAlert("Title", "Message", "Accept", "Cancel");
//				Debug.WriteLine("success: {0}", result);
//			}));
			ToolbarItems.Add(more);
			ToolbarItems.Add(add);
			ToolbarItems.Add(plus);

			var listView = new ListView();
			listView.SetBinding (ListView.ItemsSourceProperty, "Unfinished");
			listView.ItemTemplate =  new DataTemplate(typeof(UnseenCell));;
			listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => 
			{
				IBoat boat = (IBoat)e.SelectedItem;
				TimingItemManager manager = (TimingItemManager)BindingContext;
				// idea: fill in the GPS at some point 
				manager.SaveBoat(boat, DateTime.Now, string.Empty);
			};
			Content = listView;

			// fixme: we need to have an unnumbered item in the list when we start (or delegate entirely to the nav bar?) 

			// idea: hide a non-starter
			// idea: re-order a known crew that's going to be massively out of order 
		}
	}

	class SeenCell : TextCell
	{
		public SeenCell()
		{
			SetBinding(TextProperty, new Binding("Boat.PrettyName"));
			SetBinding(DetailProperty, new Binding("PrettyTime"));
			SetBinding(TextColorProperty, new Binding( "Boat.Number", BindingMode.Default, new StartNumberToColourConverter(Color.Gray)));

		}
	}

	class UnseenCell : TextCell
	{
		public UnseenCell()
		{
			SetBinding(TextProperty, new Binding("PrettyName"));
			SetBinding(TextColorProperty, new Binding( "Number", BindingMode.Default, new StartNumberToColourConverter(Color.Black)));
		}
	}

	public class StartNumberToColourConverter : IValueConverter
	{
		readonly Color _baseColour;

		public StartNumberToColourConverter(Color baseColour)
		{
			_baseColour = baseColour;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int i = 0;
			Color col = Color.Blue;
			if(Int32.TryParse(value.ToString(), out i))
				col = i < 0 ? Color.Red : _baseColour;
			return col;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
