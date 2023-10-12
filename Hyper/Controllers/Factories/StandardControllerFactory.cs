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

namespace Hyper.Controllers.Factories;
internal class StandardControllerFactory : IControllerFactory
{
    private readonly Scene _scene;

    private readonly Context _context;

    private readonly IWindowHelper _windowHelper;

    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    private readonly float _globalScale;

    public StandardControllerFactory(Scene scene, Context context, IWindowHelper windowHelper, ScalarFieldGenerator scalarFieldGenerator, float globalScale)
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
        var chunkWorker = new ChunkWorker(_scene.Chunks, _scene.SimulationManager, chunkFactory, chunkHandler, meshGenerator, settings.RenderDistance);
        var transporter = new NullTransporter();

        var objectShader = StandardObjectShader.Create(_globalScale);
        var modelShader = StandardModelShader.Create(_globalScale);
        var lightSourceShader = StandardLightSourceShader.Create(_globalScale);
        var hudShader = HudShader.Create();

        return new IController[]
        {
            new PlayerController(_scene, chunkWorker, _context, modelShader, objectShader, lightSourceShader, transporter),
            new BotsController(_scene, _context, modelShader, objectShader, transporter, new StandardBotSpawnStrategy(_scene, settings)),
            new ChunksController(_scene, _context, objectShader, chunkWorker),
            new ProjectilesController(_scene, _context, objectShader, transporter),
            new VehiclesController(_scene, _context, objectShader, lightSourceShader, modelShader, transporter),
            new LightSourcesController(_scene, lightSourceShader),
            new HudController(_scene, _windowHelper, hudShader, _context),
        };
    }
}
