using Chunks.ChunkManagement.ChunkWorkers;
using Common.UserInput;
using Hyper.PlayerData;
using Hyper.PlayerData.InventorySystem.Items.Pickaxes;
using Hyper.Shaders.LightSourceShader;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using Hyper.Transporters;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.TypingUtils;

namespace Hyper.Controllers;

internal class PlayerController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly IChunkWorker _chunkWorker;

    private readonly AbstractModelShader _modelShader;

    private readonly AbstractObjectShader _objectShader;

    private readonly AbstractLightSourceShader _rayMarkerShader;

    private readonly ITransporter _transporter;

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
        if (_scene.PlayersCar != null)
            return;
        _modelShader.SetUp(_scene.Camera, _scene.LightSources, _scene.FlashLights, shininess: 32, _scene.GlobalScale, _scene.Player.CurrentSphereId);
        _scene.Player.Render(_modelShader, _scene.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition, _scene.Camera.FirstPerson);

        if (_scene.Player.Inventory.SelectedItem is Pickaxe pickaxe)
        {
            _rayMarkerShader.SetUp(_scene.Camera, _scene.Player.CurrentSphereId);
            _scene.Player.RenderRay(
                in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId],
                _rayMarkerShader,
                _scene.GlobalScale,
                _scene.Camera.Curve,
                _scene.Camera.ReferencePointPosition,
                pickaxe.Radius / 5f);
        }
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterKeys(new List<Keys> { Keys.LeftShift, Keys.Space, Keys.W, Keys.S, Keys.A, Keys.D, Keys.C, Keys.F, Keys.Y });
        context.RegisterUpdateFrameCallback((e) =>
        {
            if (_scene.PlayersCar != null)
                return;

            int mul = _scene.Camera.Sphere == 0 ? 1 : -1;
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
                movementDirection += mul * new Vector2(-1, 0);
            }

            if (context.HeldKeys[Keys.D])
            {
                movementDirection += mul * new Vector2(1, 0);
            }

            _scene.Player.UpdateCharacterGoals(_scene.SimulationManager.Simulation, _scene.Camera.Front, (float)e.Time,
                context.HeldKeys[Keys.Space], context.HeldKeys[Keys.LeftShift], movementDirection);

            UpdateCamera(_scene.Camera, _scene.Player);

            if (movementDirection == Vector2.Zero)
                return;

            int targetSphereId = 1 - _scene.Player.CurrentSphereId;
            if (_transporter.TryTeleportTo(targetSphereId, _scene.Player, _scene.SimulationManager.Simulation, out var exitPoint))
            {
                _transporter.UpdateCamera(targetSphereId, _scene.Camera, exitPoint);
                _objectShader.SetInt("characterSphere", targetSphereId);
                _modelShader.SetInt("characterSphere", targetSphereId);
                _rayMarkerShader.SetInt("characterSphere", targetSphereId);
                _scene.Player.FlashLight.CurrentSphereId = targetSphereId;
            }
        });

        context.RegisterKeyDownCallback(Keys.C, () =>
        {
            _scene.TryEnterClosestCar();
        });

        context.RegisterKeyDownCallback(Keys.F, () => _scene.TryFlipClosestCar());

        context.RegisterMouseButtonDownCallback(MouseButton.Left, () =>
        {
            var selectedItem = _scene.Player.Inventory.SelectedItem;
            if (selectedItem == null)
                return;

            if (!_scene.Player.Inventory.IsOpen && !_scene.Player.Hidden)
            {
                if (selectedItem is Pickaxe pickaxe)
                    pickaxe.StartUsing();
                _scene.Player.Inventory.SelectedItem?.Use(_scene);
            }
        });
        context.RegisterMouseButtonDownCallback(MouseButton.Right, () =>
        {
            var selectedItem = _scene.Player.Inventory.SelectedItem;
            if (selectedItem == null)
                return;

            if (!_scene.Player.Inventory.IsOpen && !_scene.Player.Hidden)
            {
                if (!_scene.Player.Inventory.IsOpen && selectedItem is Pickaxe pickaxe)
                    pickaxe.StartUsing();
                _scene.Player.Inventory.SelectedItem?.SecondaryUse(_scene);
            }
        });
        context.RegisterMouseButtonHeldCallback(MouseButton.Left, (e) =>
        {
            if (!_scene.Player.Inventory.IsOpen)
                _scene.Player.Inventory.SelectedItem?.Use(_scene, _chunkWorker, (float)e.Time);
        });
        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) =>
        {
            if (!_scene.Player.Inventory.IsOpen)
                _scene.Player.Inventory.SelectedItem?.SecondaryUse(_scene, _chunkWorker, (float)e.Time);
        });
        context.RegisterKeyDownCallback(Keys.Up, () =>
        {
            if (!_scene.Player.Inventory.IsOpen)
                _scene.Player.Inventory.SelectedItem?.Up();
        });
        context.RegisterKeyDownCallback(Keys.Down, () =>
        {
            if (!_scene.Player.Inventory.IsOpen)
                _scene.Player.Inventory.SelectedItem?.Down();
        });
        context.RegisterKeyDownCallback(Keys.Y, () =>
            _scene.Player.FlashLight.Active = !_scene.Player.FlashLight.Active);
    }

    private void UpdateCamera(Camera camera, Player player)
    {
        camera.ReferencePointPosition = Conversions.ToOpenTKVector(player.PhysicalCharacter.Pose.Position)
            + (camera.FirstPerson ? Vector3.Zero : GetThirdPersonCameraOffset(camera))
            + GetHyperbolicOffset(camera)
            - (camera.Curve > 0 ? camera.SphereCenter : Vector3.Zero);
    }

    private Vector3 GetHyperbolicOffset(Camera camera)
    {
        if (camera.Curve >= 0)
            return Vector3.Zero;
        return -HyperCameraPosition.Multiplier * Vector3.UnitY;
    }

    private Vector3 GetThirdPersonCameraOffset(Camera camera)
        => camera.Up - camera.Front * 5f;

    public void Dispose()
    {
        _modelShader.Dispose();
        _objectShader.Dispose();
        _rayMarkerShader.Dispose();
    }
}