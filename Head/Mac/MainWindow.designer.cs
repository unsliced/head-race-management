// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace Mac
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		MonoMac.AppKit.NSTableView tblRawCompetitors { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tblRawCrews { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tblRawEvents { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblRawCompetitors != null) {
				tblRawCompetitors.Dispose ();
				tblRawCompetitors = null;
			}

			if (tblRawCrews != null) {
				tblRawCrews.Dispose ();
				tblRawCrews = null;
			}

			if (tblRawEvents != null) {
				tblRawEvents.Dispose ();
				tblRawEvents = null;
			}
		}
	}

	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
