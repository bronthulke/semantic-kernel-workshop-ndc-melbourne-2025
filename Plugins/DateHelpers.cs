using System;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SemanticKernelWorkshop.Plugins;

public class DateHelpers
{
    [KernelFunction("count_sundays_between_dates")] 
    [Description("You are a plugin that counts the number of Sundays between two dates.")]
    [return: Description("The number of Sundays between the two dates.")]
    public static int CountSundaysBetweenDates(DateTime startDate, DateTime endDate)
    {
        // Ensure the start date is earlier than the end date
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date must be earlier than or equal to end date.");
        }

        // Find the first Sunday on or after the start date
        DateTime firstSunday = startDate.AddDays((7 - (int)startDate.DayOfWeek) % 7);

        // If the first Sunday is after the end date, there are no Sundays in the range
        if (firstSunday > endDate)
        {
            return 0;
        }

        // Find the last Sunday on or before the end date
        DateTime lastSunday = endDate.AddDays(-(int)endDate.DayOfWeek);

        // Calculate the total number of Sundays
        int totalSundays = ((lastSunday - firstSunday).Days / 7) + 1;

        return totalSundays;
    }
}
