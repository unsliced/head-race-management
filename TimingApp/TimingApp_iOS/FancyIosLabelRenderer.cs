using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using MonoTouch.UIKit;
using TimingApp.Portable.Pages;
using System.Diagnostics;
using TimingApp_iOS;
using System.Drawing;
using System.ComponentModel;

//[assembly: ExportRenderer (typeof(GestureLabel), typeof(FancyIosLabelRenderer))]
namespace TimingApp_iOS
{
	public class FancyIosLabelRenderer : LabelRenderer
	{
		UILongPressGestureRecognizer longPressGestureRecognizer;
		// todo - cannot seem to get the pinch to work in debug? 
		UIPinchGestureRecognizer pinchGestureRecognizer;
		UIPanGestureRecognizer panGestureRecognizer;
		// todo - the swipe doesn't seem to work 
		UISwipeGestureRecognizer swipeGestureRecognizer;
		UIRotationGestureRecognizer rotationGestureRecognizer;

		protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged (e);

			longPressGestureRecognizer = new UILongPressGestureRecognizer (() => Debug.WriteLine ("Long Press"));
			pinchGestureRecognizer = new UIPinchGestureRecognizer (() => Debug.WriteLine ("Pinch"));
			panGestureRecognizer = new UIPanGestureRecognizer (() => Debug.WriteLine ("Pan"));
			swipeGestureRecognizer = new UISwipeGestureRecognizer (() => Debug.WriteLine ("Swipe"));
			rotationGestureRecognizer = new UIRotationGestureRecognizer (() => Debug.WriteLine ("Rotation"));

			if (e.NewElement == null) {
				if (longPressGestureRecognizer != null) {
					this.RemoveGestureRecognizer (longPressGestureRecognizer);
				}
				if (pinchGestureRecognizer != null) {
					this.RemoveGestureRecognizer (pinchGestureRecognizer);
				}
				if (panGestureRecognizer != null) {
					this.RemoveGestureRecognizer (panGestureRecognizer);
				}
				if (swipeGestureRecognizer != null) {
					this.RemoveGestureRecognizer (swipeGestureRecognizer);
				}
				if (rotationGestureRecognizer != null) {
					this.RemoveGestureRecognizer (rotationGestureRecognizer);
				}
			}

			if (e.OldElement == null) {
				this.AddGestureRecognizer (longPressGestureRecognizer);
				this.AddGestureRecognizer (pinchGestureRecognizer);
				this.AddGestureRecognizer (panGestureRecognizer);
				this.AddGestureRecognizer (swipeGestureRecognizer);
				this.AddGestureRecognizer (rotationGestureRecognizer);
			}
		}
	}
}

