﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace Microsoft.Tye
{
    public sealed class BuildDockerImageStep : ServiceExecutor.Step
    {
        public override string DisplayText => "Building Docker Image...";

        public string Environment { get; set; } = "production";

        public override async Task ExecuteAsync(OutputContext output, Application application, ServiceEntry service)
        {
            if (SkipWithoutProject(output, service, out var project))
            {
                return;
            }

            if (SkipWithoutContainerInfo(output, service, out var container))
            {
                return;
            }

            if (SkipForEnvironment(output, service, Environment))
            {
                return;
            }

            if (!await DockerDetector.Instance.IsDockerInstalled.Value)
            {
                throw new CommandException($"Cannot generate a docker image for '{service.Service.Name}' because docker is not installed.");
            }

            if (!await DockerDetector.Instance.IsDockerConnectedToDaemon.Value)
            {
                throw new CommandException($"Cannot generate a docker image for '{service.Service.Name}' because docker is not running.");
            }

            await DockerContainerBuilder.BuildContainerImageAsync(output, application, service, project, container);
        }
    }
}
