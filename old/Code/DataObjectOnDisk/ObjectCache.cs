using System;
using System.Collections.Generic;
using sones.Storage;
using sones.storage;

namespace sones.storage
{
	/// <summary>
	/// Data cache - this class holds a configureable sized cache for deserialized and.
	/// </summary>
	public class ObjectCache
	{
		private Dictionary<long,object> Cache;
		private List<long> HouseKeepingList;
		private long MaximumNoOfCacheItems;

		public ObjectCache(long MaximumNumberOfCacheItems)
		{
			HouseKeepingList = new List<long>();
			Cache = new Dictionary<long, object>();
			MaximumNoOfCacheItems = MaximumNumberOfCacheItems;
		}

		public void AddToCache(OnDiscAdress adress,object data)
		{
            lock(Cache)
			{
				if (Cache.ContainsKey(adress.End))
				{
					// update it...
					Cache.Remove(adress.End);
					Cache.Add(adress.End,data);
				}
				else
				{
					// add it
					Cache.Add(adress.End,data);
					HouseKeepingList.Add(adress.End);

					if (HouseKeepingList.Count > MaximumNoOfCacheItems)
					{
						Cache.Remove(HouseKeepingList[0]);
						HouseKeepingList.RemoveAt(0);
					}
				}
			}
		}

		public object ReadFromCache(OnDiscAdress adress)
		{
			lock(Cache)
			{
				if (!Cache.ContainsKey(adress.End))
				{
					// it's not in the cache
					return null; 
				}
				else
                {
					return Cache[adress.End];
                }
			}
            return null;
		}

		public void EmptyCache()
		{
			Cache.Clear();
		}
	}
}

