using System;
using System.Threading;
using System.Collections.Generic;
using sones.storage;
using Newtonsoft.Json;


namespace hacs
{
	public class GoogleLatitudeThread
	{
		public bool Shutdown = false;
		private ConsoleOutputLogger ConsoleOutputLogger;
		private Int32 LatitudeUpdateTime = 30;
		//private long ObjectCacheSize = 100000;
		private TinyOnDiskStorage googlelatitudestore;

		public static Dictionary<String,GoogleLatitudeDataObject> CurrentLocations = new Dictionary<string, GoogleLatitudeDataObject>();

		public GoogleLatitudeThread(ConsoleOutputLogger Logger, TinyOnDiskStorage DataStore,Int32 UpdateTime = 30000)
		{
			ConsoleOutputLogger = Logger;
			LatitudeUpdateTime = UpdateTime;
			googlelatitudestore = DataStore;
		}

		public void Run()
		{
			ConsoleOutputLogger.WriteLine("Google Latitude Thread started");

			while (!Shutdown)
			{
				try
				{
					foreach(GoogleLatitudeAccount Account in GoogleLatitudeConfiguration.GoogleLatitudeAccounts.GoogleLatitudeIDs)
					{
						// retrieve new data for this account...
						#region Retrieve Data
						GoogleLatitudeDataObject retrievedData = GoogleLatitudeLocationUpdater.UpdateLatitudeLocation(Account.Name,Account.GoogleLatitudeID,ConsoleOutputLogger);
						#endregion

						if (retrievedData != null)
						{
							#region store/update
							// check if CurrentLocations contains a data set for this account
							if (CurrentLocations.ContainsKey(Account.GoogleLatitudeID))
							{
								// check if the coordinates have been updated...
								GoogleLatitudeDataObject alreadyStored = CurrentLocations[Account.GoogleLatitudeID];

								if ((alreadyStored.Latitude != retrievedData.Latitude)||(alreadyStored.Longitude != retrievedData.Longitude))
								{
									CurrentLocations[Account.GoogleLatitudeID] = retrievedData;
									// store to disk
									googlelatitudestore.Write(retrievedData.Serialize());
									ConsoleOutputLogger.WriteLine("GoogleLatitude: "+retrievedData.AccountName+" - "+retrievedData.reverseGeocode+" - "+retrievedData.Latitude+","+retrievedData.Longitude);
								}
							}
							else
							{
								// it's new! add it!
								CurrentLocations.Add(Account.GoogleLatitudeID, retrievedData);
								googlelatitudestore.Write(retrievedData.Serialize());
								ConsoleOutputLogger.WriteLine("GoogleLatitude: "+retrievedData.AccountName+" - "+retrievedData.reverseGeocode+" - "+retrievedData.Latitude+","+retrievedData.Longitude);
							}
							#endregion
						}
					}
				}
				catch (Exception)
				{                   
					Thread.Sleep(100);
				}
				
				Thread.Sleep(LatitudeUpdateTime);
			}
			
			
		}

	}
}

