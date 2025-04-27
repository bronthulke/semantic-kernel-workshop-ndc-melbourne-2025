using System;
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelWorkshop.Plugins;

/// <summary>
/// Simple plugin that just returns the time.
/// </summary>
public class MyTimePlugin
{
    [KernelFunction, Description("A plugin that gets the current time")]
    public DateTimeOffset Time() => DateTimeOffset.Now;
}