using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace TimingApp.Portable.Pages
{
	public class RaceCell : ViewCell 
	{
		public RaceCell() 
		{
			View = CreateRaceLayout(); //  CreateQuestionLayout();
		}

		static Label CreateRaceLabel()
		{
			// TODO : show a thumbnail and then option to download a larger thumbnail for presentation (will need to consider space usage) 

			// TODO: truncate the single line, if the line needs truncating 
			// At the moment there is no way to dynamically resize the cell based on unknown future contents, 
			// http://forums.xamarin.com/discussion/20308/how-do-we-use-the-new-listview-hasunevenrows-property-to-fit-height-to-content-on-ios 
			// so we should just fudge it here and come back to it. 
			var label = new Label {
				// HorizontalOptions = LayoutOptions.Fill,
				//				VerticalOptions = LayoutOptions.StartAndExpand,
				Font = Font.SystemFontOfSize(NamedSize.Small), 
				LineBreakMode = LineBreakMode.TailTruncation, 
				//BackgroundColor = Color.Silver,
			};
			label.SetBinding(Label.TextProperty, "Name");
			//			questionLabel.SetBinding(Label.HeightProperty,
			//			                         "QuestionItem.Text",
			//			                         BindingMode.Default,
			//			                         new TextToRowHeightConverter());
			return label;
		}

		static StackLayout CreateRaceLayout()
		{
			var label = CreateRaceLabel();

			//			var answerLabel = new Label {
			//				HorizontalOptions = LayoutOptions.Start, 
			//				Font = Font.SystemFontOfSize(NamedSize.Micro, FontAttributes.Italic),
			//				TextColor = Color.Blue
			//			};
			//			answerLabel.SetBinding(Label.TextProperty, "AnswerItem.Text");
			//
			//			var flagsLabel = new Label {
			//				HorizontalOptions = LayoutOptions.End, 
			//				Font = Font.SystemFontOfSize(NamedSize.Micro, FontAttributes.Italic),
			//				TextColor = Color.Silver
			//			};
			//			flagsLabel.SetBinding(Label.TextProperty, "Flags", BindingMode.Default, new FlagsToStringsConverter());
			//
			//			var answerLayout = new StackLayout {
			//				HorizontalOptions = LayoutOptions.FillAndExpand,
			//				VerticalOptions = LayoutOptions.End,
			//				Orientation = StackOrientation.Horizontal,
			//				HeightRequest = 25,
			//				Children = {
			//					answerLabel,
			//					flagsLabel
			//				}
			//
			//			};

			return new StackLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = { 
					label // , answerLayout 
				},
				BackgroundColor = Color.Pink,
			};
		}
	}
}
