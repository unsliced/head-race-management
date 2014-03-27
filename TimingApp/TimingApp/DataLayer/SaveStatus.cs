using System;
using TimingApp.Model;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace TimingApp.DataLayer
{
	public class SaveStatus
	{
		public bool Success { get; set; }
		public string Repo { get; set; } 
		public DateTime WriteTime { get; set; } 
	}
}
