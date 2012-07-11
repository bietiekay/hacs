using System;
using System.Collections.Generic;
using sones.Storage;
using hacs.xs1;
using sones.storage;

namespace xs1_data_logging
{
	/// <summary>
	/// Data cache - this class holds a configureable sized cache for deserialized and.
	/// </summary>
	public class DataCache
	{
		private Dictionary<OnDiscAdress,XS1_DataObject> Cache;
		private List<OnDiscAdress> HouseKeepingList;
		private Int32 MaximumNoOfCacheItems;
		private TinyOnDiskStorage sensor_data;

		public DataCache (Int32 MaximumNumberOfCacheItems, TinyOnDiskStorage storage)
		{
			HouseKeepingList = new List<OnDiscAdress>();
			Cache = new Dictionary<OnDiscAdress, XS1_DataObject>();
			MaximumNoOfCacheItems = MaximumNumberOfCacheItems;
			sensor_data = storage;
		}

		private void AddToCache(OnDiscAdress adress,XS1_DataObject data)
		{
			lock(Cache)
			{
				// it's already in here
				if (Cache.ContainsKey(adress))
					return;

				Cache.Add(adress,data);

				HouseKeepingList.Add(adress);

				if (HouseKeepingList.Count > MaximumNoOfCacheItems)
				{
					// remove from cache
					Cache.Remove(HouseKeepingList[0]);
					// remove from housekeepinglist
					HouseKeepingList.RemoveAt(0);
				}
			}
		}

		public XS1_DataObject ReadFromCache(OnDiscAdress adress)
		{
			lock(Cache)
			{
				try
				{
					if (!Cache.ContainsKey(adress))
					{
                        XS1_DataObject dataobject = new XS1_DataObject();
						dataobject.Deserialize(sensor_data.Read(adress));

						// add to cache
						AddToCache(adress,dataobject);
						return dataobject;
					}
					else
						return Cache[adress];
				}
				catch(Exception)
				{
				}
			}
            return null;
		}
	}
}

