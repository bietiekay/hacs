using System;
using System.Collections.Generic;

namespace MAXDebug
{
	public class House
	{
		public String Name;
		public List<Room> Rooms;
		public H_Message CubeInformation;

		public House ()
		{
			Rooms = new List<Room>();
		}
	}
}

