using System;
using Xamarin.Forms;
using System.Collections.Generic;
using TimingApp.Data;
using TimingApp.Data.Interfaces;

namespace TimingApp.Portable.Pages
{
	public class TimingMasterDetailPage : MasterDetailPage 
	{
		// todo: put a ticking clock into the title bar 
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
		}
	}

	class FinisherCell : ViewCell 
	{
		public FinisherCell()
		{
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
				BackgroundColor = Color.White,
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

		static StackLayout CreateRaceLayout()
		{
			var label = CreateRaceLabel();
			return new StackLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { 
					label 
				},
				BackgroundColor = Color.Pink,
			};
		}
	}
}
