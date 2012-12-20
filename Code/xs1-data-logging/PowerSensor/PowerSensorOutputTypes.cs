using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xs1_data_logging
{
    /// <summary>
    /// what are the output options for Power Sensors
    /// </summary>
    public enum PowerSensorOutputs
    {
        HourkWh,
        HourPeakkWh,
        CalculatedkWhCounterTotal,
        CalculatedHourlykWh,
        CalculatedDailykWh,
		CalculateWeeklykWh
    }
}
