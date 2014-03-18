using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using TimingApp.ApplicationLayer;
using DropBoxSync.iOS;

namespace TimingApp
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// Get your own App Key and Secret from https://www.dropbox.com/developers/apps
		const string DropboxSyncKey = "e4c0b0e0d6xc1c4";
		const string DropboxSyncSecret = "xziw89y2z0fefu5";

		// class-level declarations
		UISplitViewController splitViewController;
		UIWindow window;
		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{


			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			splitViewController = new TimingSplitViewController ();

			window.RootViewController = splitViewController;


			// make the window visible
			window.MakeKeyAndVisible ();
		
			// The account manager stores all the account info. Create this when your app launches
			var manager = new DBAccountManager (DropboxSyncKey, DropboxSyncSecret);
			DBAccountManager.SharedManager = manager;



			var account = manager.LinkedAccount;
			if (account != null) {
				var filesystem = new DBFilesystem (account);
				DBFilesystem.SharedFilesystem = filesystem;
			}    
			else
				DBAccountManager.SharedManager.LinkFromController(splitViewController);
			//			

			return true;
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			var account = DBAccountManager.SharedManager.HandleOpenURL (url);
			if (account != null) {
				var filesystem = new DBFilesystem (account);
				DBFilesystem.SharedFilesystem = filesystem;
				Console.WriteLine ("App linked successfully!");
				return true;
			} else {
				Console.WriteLine ("App is not linked");
				return false;
			}
		}
	}


}

