using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelWorkshop.Plugins;

[Description("Enter a description of your plugin here.")]
public class MyPluginTemplate(bool turnedOn = false)
{
    [KernelFunction, Description("Enter a description for this function here")]
    public string DoSomething()
    {
        return "just a string";
    }

    [KernelFunction, Description("Enter a description for this function here")]
    public string DoSomethingElse()
    {
        return "another string";
    }
}