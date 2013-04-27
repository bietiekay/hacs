using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hacs
{
    public enum actor_status
    {
        On,
        Off,
        OnOff,
        OnWaitOff
    }

    public class current_actor_status
    {
        public String ActorName;
        public actor_status Status;
		public DateTime LastUpdate;

        public current_actor_status(String Name, actor_status _Status)
        {
            ActorName = Name;
            Status = _Status;
			LastUpdate = DateTime.Now;
        }
    }
}
