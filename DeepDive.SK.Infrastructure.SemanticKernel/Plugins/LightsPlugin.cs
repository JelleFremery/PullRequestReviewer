using System.ComponentModel;
using DeepDive.SK.Domain.Interfaces;
using Microsoft.SemanticKernel;

namespace DeepDive.SK.Infrastructure.SemanticKernel.Plugins;

public class LightsPlugin : IKernelPlugin
{
    // Mock data for the lights
    private readonly List<LightModel> lights =
    [
      new LightModel { Id = 1, Name = "Table Lamp", IsOn = false, Brightness = 100, Hex = "FF0000" },
      new LightModel { Id = 2, Name = "Porch light", IsOn = false, Brightness = 50, Hex = "00FF00" },
      new LightModel { Id = 3, Name = "Chandelier", IsOn = true, Brightness = 75, Hex = "0000FF" }
    ];

    [KernelFunction("get_lights")]
    [Description("Gets a list of lights and their current state")]
    [return: Description("An array of lights")]
    public List<LightModel> GetLights()
    {
        return lights;
    }

    [KernelFunction("get_state")]
    [Description("Gets the state of a particular light")]
    [return: Description("The state of the light")]
    public LightModel? GetState([Description("The ID of the light")] int id)
    {
        // Get the state of the light with the specified ID
        return lights.Find(light => light.Id == id);
    }

    [KernelFunction("change_state")]
    [Description("Changes the state of the light")]
    [return: Description("The updated state of the light; will return null if the light does not exist")]
    public LightModel? ChangeState(int id, LightModel LightModel)
    {
        var light = lights.Find(light => light.Id == id);

        if (light == null)
        {
            return null;
        }

        // Update the light with the new state
        light.IsOn = LightModel.IsOn;
        light.Brightness = LightModel.Brightness;
        light.Hex = LightModel.Hex;

        return light;
    }
}