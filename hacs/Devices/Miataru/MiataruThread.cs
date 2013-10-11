using System;
using System.Threading;
using System.Collections.Generic;
using sones.storage;
using Newtonsoft.Json;
using miataruclientcsharp;


namespace hacs
{
	public class MiataruThread
	{
		public bool Shutdown = false;
		private ConsoleOutputLogger ConsoleOutputLogger;
		private Int32 MiataruUpdateTime = 30;
		//private long ObjectCacheSize = 100000;
		private TinyOnDiskStorage miatarustore;

		public static Dictionary<String,MiataruDataObject> CurrentLocations = new Dictionary<string, MiataruDataObject>();

		public MiataruThread(ConsoleOutputLogger Logger, TinyOnDiskStorage DataStore,Int32 UpdateTime = 30000)
		{
			ConsoleOutputLogger = Logger;
			MiataruUpdateTime = UpdateTime;
			miatarustore = DataStore;
		}

		public void Run()
		{
			ConsoleOutputLogger.WriteLine("Miataru Thread started");

			miataruclient client = new miataruclient ();

			while (!Shutdown)
			{
				try
				{
					foreach(MiataruAccount Account in MiataruConfiguration.MiataruAccountConfigFile.MiataruAccounts)
					{
						// retrieve new data for this account...
						#region Retrieve Data
						List<MiataruLocation> Locations = client.GetLastLocationForDevice(Account.MiataruDeviceID,Account.MiataruServerURL);
						#endregion

						#region handle retrieved location data
						if (Locations != null)
						{
							// create a MiataruDataObject out of the last known location
							MiataruDataObject retrievedData = new MiataruDataObject(Account.Name,Locations[0].Device,Locations[0].Timestamp,Locations[0].Latitude,Locations[0].Longitude,Locations[0].HorizontalAccuracy);


							if (CurrentLocations.ContainsKey(retrievedData.DeviceID))
							{
								// check if the coordinates have been updated...
								MiataruDataObject alreadyStored = CurrentLocations[Account.MiataruDeviceID];

								if ((alreadyStored.Latitude != retrievedData.Latitude)||(alreadyStored.Longitude != retrievedData.Longitude))
								{
									CurrentLocations[Account.MiataruDeviceID] = retrievedData;
									// store to disk
									miatarustore.Write(retrievedData.Serialize());
									ConsoleOutputLogger.WriteLine("Miataru: "+retrievedData.AccountName+" - "+retrievedData.Latitude+","+retrievedData.Longitude+","+retrievedData.AccuracyInMeters);
								}

							}
							else
							{
								// it's new! add it!
								CurrentLocations.Add(retrievedData.DeviceID, retrievedData);
								miatarustore.Write(retrievedData.Serialize());
                                ConsoleOutputLogger.WriteLine("Miataru: " + retrievedData.AccountName + " - " + retrievedData.Latitude + "," + retrievedData.Longitude + "," + retrievedData.AccuracyInMeters);
							}
						}
						else
						{
							ConsoleOutputLogger.WriteLine("Miataru returned NULL for "+Account.MiataruDeviceID+" on "+Account.MiataruServerURL);
						}
						#endregion

					}
				}
				catch (Exception e)
				{
                    ConsoleOutputLogger.WriteLine(e.Message+" - "+e.StackTrace);
					Thread.Sleep(100);
				}
				
				Thread.Sleep(MiataruUpdateTime);
			}
			
			
		}

	}
}

