using Godot;

public partial class GloGodotWorldEnv : Node3D
{
    public override void _Ready()
    {
        // Create a new WorldEnvironment node
        var worldEnvironment = new WorldEnvironment();

        // Set the environment to a new Environment instance
        var environment = new Environment();
        worldEnvironment.Environment = environment;

        // Add the WorldEnvironment node to the scene tree
        AddChild(worldEnvironment);

        // Set up sky color
        SetSkyColor(environment, new Color(0.5f, 0.7f, 0.9f));  // Example color

        // Configure SSAO
        ConfigureSSAO(environment, intensity: 1.0f, radius: 1.5f);

        // Configure SSIL
        ConfigureSSIL(environment, ssilEnabled: true, ssilIntensity: 0.6f);

        // Configure SDFGI
        ConfigureSDFGI(environment, sdgfiEnabled: true, intensity: 1.2f, maxDistance: 500.0f);
    }

    private void SetSkyColor(Environment environment, Color color)
    {
        var sky                    = new Sky();
        var skyMaterial            = new ProceduralSkyMaterial();
        skyMaterial.SkyTopColor    = color;
        sky.SkyMaterial            = skyMaterial;
        environment.BackgroundMode = Environment.BGMode.Sky;
        environment.Sky            = sky;
    }

    private void ConfigureSSAO(Environment environment, float intensity, float radius)
    {
        environment.SsaoEnabled    = true;
        environment.SsaoIntensity = intensity;
        environment.SsaoRadius    = radius;
    }

    private void ConfigureSSIL(Environment environment, bool ssilEnabled, float ssilIntensity)
    {
        environment.SsilEnabled   = ssilEnabled;
        environment.SsilIntensity = ssilIntensity;
    }

    private void ConfigureSDFGI(Environment environment, bool sdgfiEnabled, float intensity, float maxDistance)
    {
        environment.SdfgiEnabled     = sdgfiEnabled;
        environment.SdfgiEnergy      = intensity;
        environment.SdfgiMaxDistance = maxDistance;
    }
}
