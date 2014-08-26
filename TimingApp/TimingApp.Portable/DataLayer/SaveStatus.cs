using System;
using TimingApp.Portable.Model;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace TimingApp.Portable.DataLayer
{
	public class SaveStatus
	{
		public bool Success { get; set; }
		public string Repo { get; set; } 
		public DateTime WriteTime { get; set; } 
	}
}
