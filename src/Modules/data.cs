using System;
using System.Collections;
using System.Collections.Generic;

namespace LinearsBot
{
	[Serializable]
	internal class StaffList
	{
		public List<string> staffList { get; set; }
	}

	[Serializable]
	internal class ServerList
	{
		public Dictionary<string, string[]> serverList { get; set; }
	}
}
