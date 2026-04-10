namespace SecurityProject.Commands;

public static class DockerCommands
{
    public static readonly CommandSpec DockerImageList =
        new("docker", ["image", "ls", "--all", "--format", "json"]);

    public static readonly CommandSpec DockerContainerList =
        new("docker", ["container", "ls", "--all", "--format", "json"]);

    public static readonly CommandSpec DockerScoutCvesNginxLatest =
        new("docker", ["scout", "cves", "nginx:latest"]);
}
