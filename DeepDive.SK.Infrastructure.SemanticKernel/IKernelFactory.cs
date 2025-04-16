using Microsoft.SemanticKernel;

namespace DeepDive.SK.Infrastructure.SemanticKernel;

public interface IKernelFactory
{
    Kernel GetKernelWithoutPlugins();
}