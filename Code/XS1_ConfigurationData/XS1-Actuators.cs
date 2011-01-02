using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hacs.xs1.configuration
{
    public class actuator_function
    {
        public String type;
        public String dsc;
    }

    public class XS1Actuator
    {
        public String name;
        public Int32 id;
        public String type;
        public double value;
        public Int64 utime;
        public String unit;
        public actuator_function[] function;
    }

    public class XS1ActuatorList
    {
        public Int32 version;
        public String type;
        public Int32 utc_offset;
        public String dst;

        public XS1Actuator[] actuator;

    }
}
