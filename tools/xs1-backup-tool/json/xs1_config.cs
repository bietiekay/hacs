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
    public class xs1_config_infotype_ipconfiguration
    {
        public String ip;
        public String netmask;
        public String gateway;
        public String dns;
    }

    public class xs1_config_infotype
    {
        public String devicename;
        public String hardware;
        public String bootloader;
        public String firmware;
        public Int32 Systems;
        public Int32 maxactuators;
        public Int32 maxsensors;
        public Int32 maxtimers;
        public Int32 maxscripts;
        public Int32 maxrooms;
        public Int64 uptime;
        public String[] features;
        public String mac;
        public String autoip;
        public xs1_config_infotype_ipconfiguration current = new xs1_config_infotype_ipconfiguration();
        public xs1_config_infotype_ipconfiguration saved = new xs1_config_infotype_ipconfiguration();
    }

    public class xs1_config
    {
        public Int32 version;
        public string type;
        public xs1_config_infotype info = new xs1_config_infotype();
    }
}
