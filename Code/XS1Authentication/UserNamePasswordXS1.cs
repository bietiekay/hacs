using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xs1_data_logging
{
    public class UserNamePasswordXS1List
    {
        public List<UserNamePasswordXS1> XS1Devices;
    }

    public class UserNamePasswordXS1
    {
        public String Username;
        public String Password;
        public String XS1Server;
    }
}
