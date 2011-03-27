/// <summary>
/// This file holds the simple scripting configuration of what will happen when a sensor input is 
/// detected.
/// </summary>


using System;
using System.Configuration;

namespace xs1_data_logging
{
	
	class ScriptingActorElement : ConfigurationElement
	{
	   [ConfigurationProperty("name", IsKey=true, IsRequired=true)]
	   public string Name
	   {
	      get 
			{ 
				return (string)this["name"]; 
			}
	   }
		
		[ConfigurationProperty("watchsensorname")]
		public string WatchSensorName
		{
   			get 
			{ 
				return (string)this["watchsensorname"]; 
			}
		}
		
		[ConfigurationProperty("watchsensorvalue")]
		public double WatchSensorValue
		{
   			get 
			{ 
				return (double)this["watchsensorvalue"]; 
			}
		}
		[ConfigurationProperty("actorname")]
		public string ActorName
		{
   			get 
			{ 
				return (string)this["actorname"]; 
			}
		}
		[ConfigurationProperty("actoraction")]
		public string ActorAction
		{
   			get 
			{ 
				return (string)this["actoraction"]; 
			}
		}
	}
	
	class ScriptingActorElementCollection : ConfigurationElementCollection
	{
	   protected override ConfigurationElement CreateNewElement()
	   {
	      return new ScriptingActorElement();
	   }
	
	   protected override object GetElementKey(ConfigurationElement element)
	   {
	      return ((ScriptingActorElement)element).Name;
	   }
		
	   public ScriptingActorElement this[int index]
	   {
		    get
		    {
		        return (ScriptingActorElement)BaseGet(index);
		    }
		    set
		    {
		        if (BaseGet(index) != null)
		        {
		            BaseRemoveAt(index);
		        }
		        BaseAdd(index, value);
		    }
	    }		
	}
	
	class ScriptingConfigurationSection : ConfigurationSection
	{
	   [ConfigurationProperty("scriptingactors")]
	   public ScriptingActorElementCollection ScriptingActors
	   {
	      get { return (ScriptingActorElementCollection)this["scriptingactors"]; }
	   }
	}
}




