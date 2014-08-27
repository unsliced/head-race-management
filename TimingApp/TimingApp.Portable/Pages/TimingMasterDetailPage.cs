using System;
using Xamarin.Forms;
using System.Collections.Generic;
using TimingApp.Data;
using TimingApp.Data.Interfaces;

namespace TimingApp.Portable.Pages
{
	public class TimingMasterDetailPage : MasterDetailPage 
	{
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

			var listView = new ListView();
			listView.SetBinding (ListView.ItemsSourceProperty, "Finished");
			listView.ItemTemplate = new DataTemplate(typeof(BoatCell));

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
				// todo: fill in the GPS at some point 
				manager.SaveBoat(boat, DateTime.Now, string.Empty);
			};
			Content = listView;
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
