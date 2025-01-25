var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DeepDive_SK_Presentation_API>("deepdive-sk-presentation-api");

builder.Build().Run();
