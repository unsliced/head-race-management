using System;
using Xamarin.Forms;
using System.Collections.Generic;
using TimingApp.Data;
using TimingApp.Data.Interfaces;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace TimingApp.Portable.Pages
{
	// idea: basic instructions? 
	public class TimingMasterDetailPage : MasterDetailPage 
	{
		TimingMasterDetailPage (TimingItemManager manager)
		{
			Master = new TimingMasterPage();
			Master.BindingContext = manager;
			Detail = new TimingDetailPage ();
			Detail.BindingContext = manager;
			Title = manager.Title;
		}

		public static Page Create(TimingItemManager manager)
		{
			return new TimingMasterDetailPage(manager);
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
			listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => 
			{
				ISequenceItem item = (ISequenceItem)e.SelectedItem;
				var page = new EntryDemoPage(item);
				Navigation.PushAsync(page);
			};

			Content = listView;
		}							
	}

	class EntryDemoPage : ContentPage
	{
		public EntryDemoPage(ISequenceItem item)
		{
			Label header = new Label
			{
				Text = item.Boat.PrettyName,
				Font = Font.SystemFontOfSize(NamedSize.Medium, FontAttributes.Bold),
				HorizontalOptions = LayoutOptions.Center
			};

			var entry = new Entry {
				Keyboard = Keyboard.Text,
				Placeholder = "Notes",
				Text = item.Notes,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			entry.TextChanged += (object sender, TextChangedEventArgs e) => 
				item.Notes = entry.Text;

			// Build the page.
			this.Content = new StackLayout
			{
				Children = 
				{
					header, entry
				}
			};
		}
	}

	class TimingDetailPage : ContentPage
	{
		public TimingDetailPage()
		{
			ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
			IDictionary<IBoat, DateTime> times = new Dictionary<IBoat, DateTime>();

			Action saveBoats = () =>
			{
				TimingItemManager manager = (TimingItemManager)BindingContext;
				try
				{
					locker.EnterWriteLock();
					manager.SaveBoat(times.Where(kvp => kvp.Value > DateTime.MinValue).Select(t => new Tuple<IBoat, DateTime, string>(t.Key, t.Value, string.Empty)));
					times.Clear();
				}
				finally
				{
					locker.ExitWriteLock();
				}
			};
			Action unidentified = () =>
			{
				((TimingItemManager)BindingContext).Unidentified();
			};
// 			var anchor = new ToolbarItem("Anchor", "Anchor.png", () => Debug.WriteLine("anchor"), priority: 3);
			var plus = new ToolbarItem("Add", "Add.png", () => unidentified(), priority: 3);
			var sync = new ToolbarItem("Sync", "Syncing.png", saveBoats, priority: 2);
			ToolbarItem more=null;
			Action refreshToolbar = () => 
			{
				ToolbarItems.Clear();
				ToolbarItems.Add(plus);
				ToolbarItems.Add(sync);
				ToolbarItems.Add(more);
			};
			more = new ToolbarItem("More", "More.png", refreshToolbar, priority: 1);
			refreshToolbar();

			Action<IBoat, bool> flipflop = (IBoat boat, bool seen) =>
			{
				try
				{
					locker.EnterWriteLock();
					boat.Seen = seen;
					if(times.ContainsKey(boat))
						times.Remove(boat);
					times.Add(boat, seen ? DateTime.Now : DateTime.MinValue);
				}
				finally
				{
					locker.ExitWriteLock();
				}
			};


			var listView = new ListView();
			listView.SetBinding (ListView.ItemsSourceProperty, "Unfinished");
			Action<IBoat> press = async (IBoat boat) => 
			{
				string pin = "Pin to top";
				string send = "Send to End";
				var sheet = await DisplayActionSheet (string.Format("Boat {0}: Send to ... ?", boat.Number), "Cancel", "Scratch", pin, send);
				if(sheet == pin)
				{
					ToolbarItem item = null;
					Action action = () => 
					{
						flipflop(boat, true);
						ToolbarItems.Remove(item);
					};
					item = new ToolbarItem(boat.Number.ToString(), null, action, priority: -1);

					ToolbarItems.Add(item);
				}
				if(sheet == send)
				{
					boat.End = true;
				}
				Debug.WriteLine("Action: " + sheet); 
			};

			listView.ItemTemplate = new DataTemplate(() => new UnseenCell(press));
			listView.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => 
			{
				Debug.WriteLine("underlying change");
			};
			listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => 
			{
				if(e.SelectedItem == null)
					return;
				IBoat boat = (IBoat)e.SelectedItem;
				flipflop(boat, !boat.Seen);
				listView.SelectedItem = null;
			};
			SearchBar searchBar = new SearchBar
			{
				Placeholder = "Filter list",
			};
			searchBar.TextChanged += (object sender, TextChangedEventArgs e) => 
			{
				TimingItemManager manager = (TimingItemManager)BindingContext;
				manager.Filter(searchBar.Text);
			};
			Content = new StackLayout () 
			{
				Children={searchBar, listView}
			};

			// idea: hide a non-starter
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

	public class QuickerLabel : View
	{
		public string Text = "";

		protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
		{
			return new SizeRequest(new Size(100, 20));
		}
	}

	class UnseenCell : ViewCell
	{
		public static readonly BindableProperty FooProperty = 
			BindableProperty.Create<UnseenCell, IBoat> (w => w.Foo, null);

		public IBoat Foo {
			get { return (IBoat)GetValue (FooProperty); }
			set { SetValue (FooProperty, value); } 
		}

		public UnseenCell(Action<IBoat> pressAction)
		{
			var sw = new Stopwatch();
			sw.Start();
			var label = new Label
			{
				Font = Font.SystemFontOfSize(NamedSize.Large), 
				LineBreakMode = LineBreakMode.TailTruncation, 
			};
			label.SetBinding(Label.TextProperty, "PrettyName");

			var button = new Button { Text = "Actions" , BackgroundColor = Color.Silver};
			SetBinding(FooProperty, new Binding("."));

			button.Clicked += (sender, e) => {
				pressAction(Foo);
			};
				
			BindingContextChanged += (object sender, EventArgs e) => { 
				if(Foo.Number < 0) 
					button.IsVisible = false;
				};

			var seen = new Label
			{
				Font = Font.SystemFontOfSize(NamedSize.Medium), 
				LineBreakMode = LineBreakMode.TailTruncation, 
				TextColor = Color.Purple
			};
			seen.SetBinding(Label.TextProperty, new Binding( "Seen", BindingMode.Default, new BoolToStringConverter("Seen")));

			var sent = new Label
			{
				Font = Font.SystemFontOfSize(NamedSize.Medium), 
				LineBreakMode = LineBreakMode.TailTruncation, 
				TextColor = Color.Purple
			};
			sent.SetBinding(Label.TextProperty, new Binding( "End", BindingMode.Default, new BoolToStringConverter("Relegated")));

			var layout = new StackLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.Center,
				Children = { 
					label, button, seen, sent
				},				
			};
			layout.SetBinding( Layout.BackgroundColorProperty, new Binding( "BackgroundColour" ) );

			sw.Stop();
			ReportStopwatch(sw, "cell");
			View = layout;
		}
		void ReportStopwatch(Stopwatch sw, string arg)
		{
			// Get the elapsed time as a TimeSpan value.
			TimeSpan ts = sw.Elapsed;

			// Format and display the TimeSpan value. 
			string elapsedTime = String.Format("{4} - {0:00}:{1:00}:{2:00}.{3:000}",
				ts.Hours, ts.Minutes, ts.Seconds,
				ts.Milliseconds, arg );

			Debug.WriteLine(elapsedTime);
		}
	}

	class StartNumberToColourConverter : IValueConverter
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

	class BoatToColourConverter : IValueConverter
	{
		readonly Color _baseColour;

		public BoatToColourConverter(Color baseColour)
		{
			_baseColour = baseColour;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IBoat boat = value as IBoat;
			return (boat as IBoat).Seen ? Color.Gray : _baseColour;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	class BoolToStringConverter : IValueConverter
	{
		string _string;

		public BoolToStringConverter(string str)
		{
			_string = str;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? _string : string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
