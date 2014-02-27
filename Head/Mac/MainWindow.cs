using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Mac
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{

		#region Constructors

		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion


		// This method will be called automatically when the main window "wakes up".
		[Export ("awakeFromNib:")]
		public override void AwakeFromNib()
		{
			string basepath = "/Users/cah/src/git-working-directory/head-race-management/Head/Mac/bin/Debug/";
			Console.WriteLine("awakeFromNib:");
			tblRawEvents.DataSource = new CategoryDataSource(basepath+"Resources/eventexport.csv");
			tblRawCrews.DataSource = new CrewsDataSource (basepath+"Resources/crewexport.csv");
			tblRawCompetitors.DataSource = new TableViewDataSource ();
		}

	}
}

