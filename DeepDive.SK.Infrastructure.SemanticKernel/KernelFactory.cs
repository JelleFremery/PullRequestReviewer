using Microsoft.SemanticKernel;

namespace DeepDive.SK.Infrastructure.SemanticKernel
{
    public class KernelFactory : IKernelFactory
    {
        public Kernel GetKernelWithoutPlugins()
        {
            return Kernel
                .CreateBuilder()
                .ConfigureBuilder()
                .Build();
        }
    }
}
