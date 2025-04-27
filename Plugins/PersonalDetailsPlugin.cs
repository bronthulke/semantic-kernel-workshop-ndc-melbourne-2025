using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.ComponentModel;

namespace SemanticKernelWorkshop.Plugins;

public class PersonalDetailsPlugin
{
    private readonly IConfiguration _configuration;

    public PersonalDetailsPlugin(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [KernelFunction("get_full_name")]
    [Description("You are a plugin responsible for sharing information about the user's name.")]
    [return: Description("A string that is the user's name")]
    public string GetOwnerFullName(
        Kernel kernel
    )
    {
        return _configuration["OwnerFirstName"] + " " + _configuration["OwnerLastName"];
    }

    [KernelFunction("get_short_name")]
    [Description("You are a plugin responsible for sharing the short version of the user's name - their first name")]
    [return: Description("A string that is a short version of the user's name")]
    public string GetOwnerShortNameAsync(
        Kernel kernel
    )
    {
        return _configuration["OwnerFirstName"] ?? "";
    }
}
