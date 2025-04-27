using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelWorkshop.Plugins;

public class PublicHolidaysPlugin
{
    List<PublicHoliday> _publicHolidays = new()
    {
        new PublicHoliday { Date = new DateTime(2024, 1, 1), Name = "New Year's Day", Country = "AU", LocalName = "New Year's Day", Fixed = true },
        new PublicHoliday { Date = new DateTime(2024, 1, 26), Name = "Australia Day", Country = "AU", LocalName = "Australia Day", Fixed = true },
        new PublicHoliday { Date = new DateTime(2024, 3, 29), Name = "Good Friday", Country = "AU", LocalName = "Good Friday", Fixed = false },
        new PublicHoliday { Date = new DateTime(2024, 4, 1), Name = "Easter Monday", Country = "AU", LocalName = "Easter Monday", Fixed = false },
        new PublicHoliday { Date = new DateTime(2024, 12, 25), Name = "Christmas Day", Country = "AU", LocalName = "Christmas Day", Fixed = true },
        new PublicHoliday { Date = new DateTime(2024, 12, 26), Name = "Boxing Day", Country = "AU", LocalName = "Boxing Day", Fixed = true },
        new PublicHoliday { Date = new DateTime(2024, 3, 11), Name = "Labour Day", Country = "AU", LocalName = "Labour Day", Fixed = false, State = "VIC" },
        new PublicHoliday { Date = new DateTime(2024, 9, 27), Name = "Grand Final Parade Day", Country = "AU", LocalName = "Grand Final Parade Day", Fixed = false, State = "VIC" },
        new PublicHoliday { Date = new DateTime(2024, 11, 2), Name = "Melbourne Cup", Country = "AU", LocalName = "Melbourne Cup", Fixed = false, State = "VIC" }
    };

    [KernelFunction("get_public_holidays")]
    [Description("Gets a list of public holidays")]
    [return: Description("An array of public holidays")]
    public async Task<List<PublicHoliday>> GetPublicHolidaysAsync()
    {
        return _publicHolidays;
    }
}

public class PublicHoliday
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? State { get; set; }
    public string LocalName { get; set; } = string.Empty;
    public bool Fixed { get; set; } = false;
}