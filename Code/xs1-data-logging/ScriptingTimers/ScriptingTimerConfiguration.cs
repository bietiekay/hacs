using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TimerName DateTime_Start(YearDoesn'tMatter) TimerPeriodDurationDays Duration_Minutes_Start Duration_Minutes_End OperationMode SwitchName JitterYesNo MinimumOnTimeMinutes

namespace xs1_data_logging
{
    public enum scripting_timer_operation_modes
    {
        OnOff
    }


    public class ScriptingTimerElement
    {
        public String TimerName;
        public DateTime Start;
        public DateTime End;    
        public Int32 Duration_Start;
        public Int32 Duration_End;
        public scripting_timer_operation_modes OperationMode;
        public String SwitchName;
        public Boolean Jitter;
        public Boolean MinimumOnTime;
    }

    public class ScriptingTimerConfiguration
    {

    }
}
