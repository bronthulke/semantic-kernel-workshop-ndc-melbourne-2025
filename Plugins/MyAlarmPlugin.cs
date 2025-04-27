using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelWorkshop.Plugins;

/// <summary>
/// Class that represents a controllable alarm.
/// </summary>
[Description("Represents a controllable alarm")]
public class MyAlarmPlugin
{
    private string _hour;

    public MyAlarmPlugin(string providedHour)
    {
        this._hour = providedHour;
    }

    [KernelFunction, Description("Sets an alarm at the provided time")]
    public string SetAlarm(string time)
    {
        this._hour = time;
        return GetCurrentAlarm();
    }

    [KernelFunction, Description("Get current alarm set")]
    public string GetCurrentAlarm()
    {
        return $"Alarm set for {_hour}";
    }
}