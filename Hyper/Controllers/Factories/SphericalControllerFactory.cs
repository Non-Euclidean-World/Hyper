using Chunks.ChunkManagement;
using Chunks.ChunkManagement.ChunkWorkers;
using Chunks.MarchingCubes;
using Chunks.MarchingCubes.MeshGenerators;
using Common;
using Common.UserInput;
using Hud.Shaders;
using Hyper.Controllers.Bots;
using Hyper.Controllers.Bots.Spawn;
using Hyper.Shaders.LightSourceShader;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using Hyper.Transporters;
using OpenTK.Mathematics;

namespace Hyper.Controllers.Factories;
internal class SphericalControllerFactory : IControllerFactory
{
    private readonly Vector3i[] _sphereCenters;

    private readonly Vector3 _lowerSphereCenter;

    private readonly float _globalScale;

    private readonly Scene _scene;

    private readonly Context _context;

    private readonly IWindowHelper _windowHelper;

    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    public SphericalControllerFactory(Scene scene, Context context, IWindowHelper windowHelper, ScalarFieldGenerator scalarFieldGenerator, float globalScale)
    {
        _globalScale = globalScale;
        var sphere0Center = new Vector3i(0, 0, 0);
        var sphere1Center = new Vector3i((int)(MathF.PI / _globalScale) * 10, 0, 0);
        _sphereCenters = new[] { sphere0Center, sphere1Center };
        _scene = scene;
        _scene.SphereCenters = _sphereCenters;
        _context = context;
        _windowHelper = windowHelper;
        _scalarFieldGenerator = scalarFieldGenerator;
        _lowerSphereCenter = new Vector3(sphere1Center.X, sphere1Center.Y, sphere1Center.Z) * _globalScale;
    }

    public IController[] CreateControllers(Settings settings)
    {
        float cutoffRadius = 1.02f * MathF.PI / 2 / _globalScale; // 2% margin to remove the gap
        var meshGenerator = new SphericalMeshGenerator(cutoffRadius, _sphereCenters);
        var chunkFactory = new SphericalChunkFactory(_scalarFieldGenerator, _sphereCenters, _globalScale, meshGenerator);
        var chunkHandler = new ChunkHandler(settings.SaveName, meshGenerator);
        var chunkWorker = new NonGenerativeChunkWorker(_scene.Chunks, _scene.SimulationManager, chunkFactory, chunkHandler, meshGenerator);
        var transporter = new SphericalTransporter(cutoffRadius, _sphereCenters);

        var objectShader = SphericalObjectShader.Create(_globalScale, _lowerSphereCenter);
        var modelShader = SphericalModelShader.Create(_globalScale, _lowerSphereCenter);
        var lightSourceShader = SphericalLightSourceShader.Create(_globalScale, _lowerSphereCenter);
        var hudShader = HudShader.Create();

        return new IController[]
        {
            new PlayerController(_scene, chunkWorker, _context, modelShader, objectShader, lightSourceShader, transporter),
            new BotsController(_scene, _context, modelShader, transporter, new SphericalBotSpawnStrategy(_scene, settings)),
            new ChunksController(_scene, _context, objectShader, chunkWorker),
            new ProjectilesController(_scene, _context, objectShader, transporter),
            new VehiclesController(_scene, _context, objectShader, lightSourceShader, modelShader, transporter, new CarSpawnStrategy(_scene, settings)),
            new LightSourcesController(_scene, lightSourceShader),
            new HudController(_scene, _windowHelper, hudShader, _context),
            new SphericalSkyboxController(modelShader, objectShader),
            new BoundingShapesController(_scene, lightSourceShader, _context),
        };
    }
}
