using Chunks.ChunkManagement.ChunkWorkers;
using Common.UserInput;
using Hyper.PlayerData.InventorySystem.Items;
using Hyper.Shaders.LightSourceShader;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using Hyper.Transporters;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class PlayerController : IController, IInputSubscriber
{
    private readonly Scene _scene;
    
    private readonly IChunkWorker _chunkWorker;

    private readonly AbstractModelShader _modelShader;

    private readonly AbstractObjectShader _objectShader;

    private readonly AbstractLightSourceShader _rayMarkerShader;

    private readonly ITransporter _transporter;

    private bool _showBoundingBoxes = false;

    public PlayerController(Scene scene, IChunkWorker chunkWorker, Context context, AbstractModelShader modelShader, AbstractObjectShader objectShader, AbstractLightSourceShader rayMarkerShader, ITransporter sphericalTransporter)
    {
        _scene = scene;
        _chunkWorker = chunkWorker;
        _modelShader = modelShader;
        _objectShader = objectShader;
        _rayMarkerShader = rayMarkerShader;
        _transporter = sphericalTransporter;
        RegisterCallbacks(context);
    }

    public void Render()
    {
        _modelShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Player.CurrentSphereId);
        _scene.Player.Render(_modelShader, _modelShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition, _scene.Camera.FirstPerson);

        if (_scene.Player.Inventory.SelectedItem is not null && _scene.Player.Inventory.SelectedItem.Cursor == CursorType.BuildBlock)
        {
            _rayMarkerShader.SetUp(_scene.Camera);
            _scene.Player.RenderRay(in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId], _rayMarkerShader, _rayMarkerShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }

        if (!_showBoundingBoxes) return;
        _objectShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Player.CurrentSphereId);
        _scene.Player.PhysicalCharacter.RenderBoundingBox(_objectShader, _objectShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterKeys(new List<Keys> { Keys.LeftShift, Keys.Space, Keys.W, Keys.S, Keys.A, Keys.D });
        context.RegisterUpdateFrameCallback((e) =>
        {
            // TODO commented out until we have context switching
            /*float steeringSum = 0;
            if (context.HeldKeys[Keys.A]) steeringSum += 1;
            if (context.HeldKeys[Keys.D]) steeringSum -= 1;
            float targetSpeedFraction = context.HeldKeys[Keys.W] ? 1f : context.HeldKeys[Keys.S] ? -1f : 0;*/
            Vector2 movementDirection = default;
            if (context.HeldKeys[Keys.W])
            {
                movementDirection = new Vector2(0, 1);
            }

            if (context.HeldKeys[Keys.S])
            {
                movementDirection += new Vector2(0, -1);
            }

            if (context.HeldKeys[Keys.A])
            {
                if (_scene.Player.CurrentSphereId == 0)
                    movementDirection += new Vector2(-1, 0);
                else
                    movementDirection -= new Vector2(-1, 0);
            }

            if (context.HeldKeys[Keys.D])
            {
                if (_scene.Player.CurrentSphereId == 0)
                    movementDirection += new Vector2(1, 0);
                else
                    movementDirection -= new Vector2(1, 0);
            }

            _scene.Player.UpdateCharacterGoals(_scene.SimulationManager.Simulation, _scene.Camera.Front, (float)e.Time,
                context.HeldKeys[Keys.Space], context.HeldKeys[Keys.LeftShift], movementDirection);

            int targetSphereId = 1 - _scene.Player.CurrentSphereId;
            if (_transporter.TryTeleportTo(targetSphereId, _scene.Player, _scene.SimulationManager.Simulation, out var exitPoint))
            {
                _transporter.UpdateCamera(targetSphereId, _scene.Camera, exitPoint);
                _objectShader.SetInt("characterSphere", targetSphereId);
                _modelShader.SetInt("characterSphere", targetSphereId);
                _rayMarkerShader.SetInt("characterSphere", targetSphereId);
            }

            _scene.Camera.UpdateWithCharacter(_scene.Player);
        });

        context.RegisterKeyDownCallback(Keys.F3, () => _showBoundingBoxes = !_showBoundingBoxes);
        
        context.RegisterMouseButtonDownCallback(MouseButton.Left, () => _scene.Player.Inventory.SelectedItem?.Use(_scene));
        context.RegisterMouseButtonDownCallback(MouseButton.Right, () => _scene.Player.Inventory.SelectedItem?.SecondaryUse(_scene));
        context.RegisterMouseButtonHeldCallback(MouseButton.Left, (e) => _scene.Player.Inventory.SelectedItem?.Use(_scene, _chunkWorker, (float)e.Time));
        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) => _scene.Player.Inventory.SelectedItem?.SecondaryUse(_scene, _chunkWorker, (float)e.Time));
    }

    public void Dispose()
    {
        _modelShader.Dispose();
        _objectShader.Dispose();
        _rayMarkerShader.Dispose();
    }
}