using System;
using Xamarin.Forms;
using System.Collections.Generic;
using TimingApp.Data;
using TimingApp.Data.Interfaces;
using System.Diagnostics;

namespace TimingApp.Portable.Pages
{
	// idea: basic instructions? 
	// idea: colour scheme? 
	public class TimingMasterDetailPage : MasterDetailPage 
	{
		// todo: put a ticking clock/summary (e.g. number of finishers, progress bar, etc.) into the title bar 
		// idea: show the status of the saved 
		public TimingMasterDetailPage (TimingItemManager manager)
		{
			Master = new TimingMasterPage();
			Master.BindingContext = manager;
			Detail = new TimingDetailPage ();
			Detail.BindingContext = manager;
		}
	}

	class TimingMasterPage : ContentPage
	{
		public TimingMasterPage()
		{
			Title = "Finishers";

			var listView = new ListView() { HasUnevenRows = true };
			listView.SetBinding (ListView.ItemsSourceProperty, "Finished");
			listView.ItemTemplate = new DataTemplate(typeof(FinisherCell));

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
			listView.ItemTemplate = new DataTemplate(typeof(BoatCell));
			listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => 
			{
				IBoat boat = (IBoat)e.SelectedItem;
				TimingItemManager manager = (TimingItemManager)BindingContext;
				// idea: fill in the GPS at some point 
				manager.SaveBoat(boat, DateTime.Now, string.Empty);
			};
			Content = listView;

			// fixme: we need to have an unnumbered item in the list ... 

			// idea: hide a non-starter
			// idea: re-order a known crew that's going to be massively out of order 
		}
	}

	class FinisherCell : ViewCell 
	{
		public FinisherCell()
		{
			IBoat boat = (IBoat)BindingContext;

			var name = new Label {
				Font = Font.SystemFontOfSize(NamedSize.Small), 
				LineBreakMode = LineBreakMode.TailTruncation, 
			};
			name.SetBinding(Label.TextProperty, "Name");

			var time = new Label {
				Font = Font.SystemFontOfSize(NamedSize.Small), 
				LineBreakMode = LineBreakMode.TailTruncation, 
			};
			time.SetBinding(Label.TextProperty, "VisibleTime");

			var layout = new StackLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { 
					name, time 
				},
				BackgroundColor = boat.Number > 0 ? Color.White : Color.Yellow ,
			};
			View = layout;
		}
	}

	class BoatCell : ViewCell 
	{
		public BoatCell() 
		{
			View = CreateRaceLayout(); //  CreateQuestionLayout();
		}

		static Label CreateRaceLabel()
		{
			var label = new Label {
				// HorizontalOptions = LayoutOptions.Fill,
				//				VerticalOptions = LayoutOptions.StartAndExpand,
				Font = Font.SystemFontOfSize(NamedSize.Small), 
				LineBreakMode = LineBreakMode.TailTruncation, 
				//BackgroundColor = Color.Silver,
			};
			label.SetBinding(Label.TextProperty, "Name");
			return label;
		}

		StackLayout CreateRaceLayout()
		{
			IBoat boat = (IBoat)BindingContext;
			var label = CreateRaceLabel();
			return new StackLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { 
					label 
				},
				BackgroundColor = boat.Number > 0 ? Color.Pink : Color.Yellow,
			};
		}
	}
}
