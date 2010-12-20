/*
* h.a.c.s (home automation control server) - http://github.com/bietiekay/hacs
* Copyright (C) 2010 Daniel Kirstenpfad
*
* hacs is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* hacs is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with hacs. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xs1_backup_tool
{
    public class sensor_config
    {
         public Int32 number;
        public Int32 id;
         public String name;
         public String system;
         public String type;
         public Int32 hc1;
         public Int32 hc2;
         public Int32 address;
         public float factor;
         public float offset;
        public Int32 room;
        public Int32 x;
        public Int32 y;
        public Int32 z;
        public String log;
    }

    public class config_sensor
    {
        public string version;
        public string type;
        public sensor_config sensor = new sensor_config();
    }
}
