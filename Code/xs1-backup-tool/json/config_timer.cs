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
    public class timer_time
    {
        public String hour;
        public String min;
        public String sec;
    }

    public class timer_actuator
    {
        public String name;
        public Int32 function;
    }

    public class timer_config
    {
        public Int32 number;
        public String name;
        public String type;
        public String[] weekdays;
        public timer_time time;
        public Int32 random;
        public Int32 offset;
        public Int32 earliest;
        public timer_actuator actuator;
    }

    public class timer_sensor
    {
        public Int32 version;
        public string type;
        public timer_config timer = new timer_config();
    }
}
