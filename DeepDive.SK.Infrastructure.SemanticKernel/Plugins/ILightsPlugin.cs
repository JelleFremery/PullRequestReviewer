using System.ComponentModel;
using DeepDive.SK.Domain.Interfaces;

namespace DeepDive.SK.Infrastructure.SemanticKernel.Plugins
{
    public interface ILightsPlugin : IKernelPlugin
    {
        LightModel? ChangeState(int id, LightModel LightModel);
        List<LightModel> GetLights();
        LightModel? GetState([Description("The ID of the light")] int id);
    }
}