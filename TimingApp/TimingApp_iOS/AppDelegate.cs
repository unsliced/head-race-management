using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TimingApp.Portable.Pages;
using Xamarin.Forms;
using DropBoxSync.iOS;
using System.Threading.Tasks;

namespace TimingApp_iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;

		const string DropboxSyncKey = "xw2r68bt9gp77ok";
		const string DropboxSyncSecret = "5kccgmm3uclpqlr";

		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var manager = new DBAccountManager (DropboxSyncKey, DropboxSyncSecret);
			DBAccountManager.SharedManager = manager;

			Xamarin.Forms.Forms.Init();

			// create a new window instance based on the screen size
			window = new UIWindow(UIScreen.MainScreen.Bounds);

			Task.Factory.StartNew (() => {
				this.BeginInvokeOnMainThread (() => {
					var account = DBAccountManager.SharedManager.LinkedAccount;
					if (account != null) {
						var filesystem = new DBFilesystem (account);
						DBFilesystem.SharedFilesystem = filesystem;
						SetupDropbox ();
					} else
						manager.LinkFromController (window.RootViewController);
				});
			});

			// If you have defined a root view controller, set it here:
			window.RootViewController = RacePickerPage.Create(DropboxDatabase.Shared).CreateViewController();;
			
			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			var account = DBAccountManager.SharedManager.HandleOpenURL (url);
			SetupDropbox ();
			return account != null;
		}

		void SetupDropbox ()
		{
			var t = Task.Factory.StartNew (() => {
				DropboxDatabase.Shared.Init ();
			});
		}
	}
}

