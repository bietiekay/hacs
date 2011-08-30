using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xs1_data_logging
{
    public enum actor_status
    {
        On,
        Off,
        OnOff
    }

    public class current_actor_status
    {
        public String ActorName;
        public actor_status Status;

        public current_actor_status(String Name, actor_status _Status)
        {
            ActorName = Name;
            Status = _Status;
        }
    }
}
