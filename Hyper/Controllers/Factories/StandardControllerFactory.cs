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
using Hyper.Shaders.SkyboxShader;
using Hyper.Transporters;

namespace Hyper.Controllers.Factories;
internal class StandardControllerFactory : IControllerFactory
{
    private readonly Scene _scene;

    private readonly Context _context;

    private readonly IWindowHelper _windowHelper;

    private readonly ScalarFieldGenerator _scalarFieldGenerator;

    public StandardControllerFactory(Scene scene, Context context, IWindowHelper windowHelper, ScalarFieldGenerator scalarFieldGenerator)
    {
        _scene = scene;
        _context = context;
        _windowHelper = windowHelper;
        _scalarFieldGenerator = scalarFieldGenerator;
    }

    public IController[] CreateControllers(Settings settings)
    {
        var meshGenerator = new MeshGenerator();
        var chunkFactory = new ChunkFactory(_scalarFieldGenerator, meshGenerator);

        var chunkHandler = new ChunkHandler(settings.SaveName, meshGenerator);
        var chunkWorker = new ChunkWorker(_scene.Chunks, _scene.SimulationManager, chunkFactory, chunkHandler, meshGenerator, settings.RenderDistance);
        var transporter = new NullTransporter();

        var objectShader = StandardObjectShader.Create();
        var modelShader = StandardModelShader.Create();
        var lightSourceShader = StandardLightSourceShader.Create();
        var hudShader = HudShader.Create();
        var skyboxShader = StandardSkyboxShader.Create();

        _scene.SpawnPlayer(new PlayerSpawnStrategy(_scene, settings).GetSpawnLocation());

        return new IController[]
        {
            new PlayerController(_scene, chunkWorker, _context, modelShader, objectShader, lightSourceShader, transporter),
            new BotsController(_scene, _context, modelShader, transporter, new StandardBotSpawnStrategy(_scene, settings)),
            new ChunksController(_scene, _context, objectShader, chunkWorker),
            new ProjectilesController(_scene, _context, objectShader, transporter),
            new VehiclesController(_scene, _context, objectShader, lightSourceShader, modelShader, transporter, new CarSpawnStrategy(_scene, settings)),
            new LightSourcesController(_scene, lightSourceShader),
            new BoundingShapesController(_scene, lightSourceShader, _context),
            new SkyboxController(_scene, skyboxShader, modelShader, objectShader, settings), // skybox has to be rendered just before GUI stuff
            new HudController(_scene, _windowHelper, hudShader, _context),
        };
    }
}
