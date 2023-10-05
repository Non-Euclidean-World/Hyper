using Character.Shaders;
using Chunks.ChunkManagement;
using Chunks.ChunkManagement.ChunkWorkers;
using Chunks.MarchingCubes;
using Chunks.MarchingCubes.MeshGenerators;
using Common;
using Common.UserInput;
using Hud.Shaders;
using Hyper.Shaders;
using Hyper.Transporters;

namespace Hyper.Controllers.Factories;
internal class StandardControllerFactory : IControllerFactory
{
    private readonly Scene _scene;

    private readonly Context _context;

    private readonly IWindowHelper _windowHelper;

    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    private readonly float _globalScale;

    public StandardControllerFactory(Scene scene, Context context, IWindowHelper windowHelper, ScalarFieldGenerator scalarFieldGenerator, float globalScale = 0.05f)
    {
        _scene = scene;
        _context = context;
        _windowHelper = windowHelper;
        _scalarFieldGenerator = scalarFieldGenerator;
        _globalScale = globalScale;
    }

    public IController[] CreateControllers(Settings settings)
    {
        var meshGenerator = new MeshGenerator();
        var chunkFactory = new ChunkFactory(_scalarFieldGenerator, meshGenerator);

        var chunkHandler = new ChunkHandler(settings.SaveName, meshGenerator);
        var chunkWorker = new ChunkWorker(_scene.Chunks, _scene.SimulationManager, chunkFactory, chunkHandler, meshGenerator);
        var transporter = new NullTransporter();

        var objectShader = StandardObjectShader.Create(_globalScale);
        var modelShader = StandardModelShader.Create(_globalScale);
        var lightSourceShader = StandardLightSourceShader.Create(_globalScale);
        var hudShader = HudShader.Create();

        return new IController[]
        {
            new PlayerController(_scene, _context, modelShader, objectShader, lightSourceShader, transporter),
            new BotsController(_scene, _context, modelShader, objectShader),
            new ChunksController(_scene, _context, objectShader, chunkWorker),
            new ProjectilesController(_scene, _context, objectShader, transporter),
            new VehiclesController(_scene, _context, objectShader),
            new LightSourcesController(_scene, lightSourceShader),
            new HudController(_scene, _context, _windowHelper, hudShader),
        };
    }
}
