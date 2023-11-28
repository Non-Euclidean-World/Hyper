using System.Diagnostics;
using Common;
using Hyper.Shaders.DataTypes;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using Hyper.Shaders.SkyboxShader;
using OpenTK.Mathematics;

namespace Hyper.Controllers;
internal class SkyboxController : IController
{
    private readonly Scene _scene;

    private readonly AbstractSkyboxShader _skyboxShader;

    private readonly StandardModelShader _modelShader;

    private readonly StandardObjectShader _objectShader;

    private readonly Skybox.Skybox _skybox;

    private readonly Stopwatch _stopwatch = new();

    private const float DayLengthSeconds = 60 * 0.5f;

    private readonly Vector3 _initialSunVector;

    private readonly Vector3 _initialMoonVector;

    private float _initTimeSeconds;

    private readonly Settings _settings;

    private struct Phase
    {
        public Vector4 SkytopColor;
        public Vector4 HorizonColor;
        public Vector4 SunglareColor;
        public float StarsVisibility;
    }

    private readonly Phase _dawnStart = new()
    {
        SkytopColor = new Vector4(0.35f, 0.65f, 0.89f, 1f), // https://www.wolframalpha.com/input?i=rgb%2889%2C+166%2C+228%29
        HorizonColor = new Vector4(0.15f, 0.37f, 0.66f, 1f), // https://www.wolframalpha.com/input?i=rgb%2837%2C++94%2C+168%29
        SunglareColor = new Vector4(1f, 0.51f, 0f, 1f), // https://www.wolframalpha.com/input?i=%23ff8100
        StarsVisibility = 0.8f,
    };

    private readonly Phase _sunriseStart = new()
    {
        SkytopColor = new Vector4(0.35f, 0.58f, 0.72f, 1f), // https://www.wolframalpha.com/input?i=rgb%2889%2C+149%2C+183%29
        HorizonColor = new Vector4(0.89f, 0.59f, 0.35f, 1f), // https://www.wolframalpha.com/input?i=rgb%28228%2C+151%2C+89%29
        SunglareColor = new Vector4(1f, 0.3f, 0f, 1f), // https://www.wolframalpha.com/input?i=%23ff4d00
        StarsVisibility = 0.3f,
    };

    private readonly Phase _dayStart = new()
    {
        SkytopColor = new Vector4(0.34f, 0.55f, 0.85f, 1f), // https://www.wolframalpha.com/input?i=%23568CD8
        HorizonColor = new Vector4(0.51f, 0.7f, 0.91f, 1.0f), // https://www.wolframalpha.com/input?i=%2382B2E8
        SunglareColor = Vector4.One,
        StarsVisibility = 0.05f,
    };

    private readonly Phase _sunsetStart = new()
    {
        SkytopColor = new Vector4(0.08f, 0.16f, 0.32f, 1f), // https://www.wolframalpha.com/input?i=%23152852
        HorizonColor = new Vector4(0.99f, 0.37f, 0.33f, 1f), // https://www.wolframalpha.com/input?i=%23FD5E53
        SunglareColor = new Vector4(1f, 0.8f, 0.2f, 1f),
        StarsVisibility = 0.2f,
    };

    private readonly Phase _duskStart = new()
    {
        SkytopColor = new Vector4(0.19f, 0f, 0.43f, 1f), // https://www.wolframalpha.com/input?i=%2330016d
        HorizonColor = new Vector4(0.05f, 0.05f, 0.24f, 1f), // https://www.wolframalpha.com/input?i=%09%230d0d3e
        SunglareColor = new Vector4(0.82f, 0.59f, 0.56f, 1f),
        StarsVisibility = 0.5f,
    };

    private readonly Phase _nightStart = new()
    {
        SkytopColor = Vector4.Zero,
        HorizonColor = Vector4.Zero,
        SunglareColor = Vector4.Zero,
        StarsVisibility = 1f
    };

    private DirectionalLight _sunLight;

    private DirectionalLight _moonLight;

    public SkyboxController(Scene scene, AbstractSkyboxShader skyboxShader, StandardModelShader modelShader, StandardObjectShader objectShader, Settings settings)
    {
        _scene = scene;
        _skyboxShader = skyboxShader;
        _modelShader = modelShader;
        _objectShader = objectShader;
        _skybox = new Skybox.Skybox();
        _initialSunVector = -Vector3.UnitZ;
        _initialMoonVector = -Vector3.UnitX;
        _settings = settings;
        _initTimeSeconds = _settings.TimeInSeconds;
        _sunLight = new DirectionalLight
        {
            Ambient = 0.1f,
            Diffuse = 0.8f,
            Specular = 0.1f,
            Direction = new Vector4(_initialSunVector, 0)
        };

        _moonLight = new DirectionalLight
        {
            Ambient = 0.05f,
            Diffuse = 0.2f,
            Specular = 0.05f,
            Direction = new Vector4(_initialMoonVector, 0)
        };

        _stopwatch.Start();
    }

    public void Dispose()
    {
        _settings.TimeInSeconds = GetCurrentTime();
        _skyboxShader.Dispose();
    }

    // current time in seconds
    private float GetCurrentTime()
    {
        float currentTime = _stopwatch.ElapsedMilliseconds / 1000.0f + _initTimeSeconds;
        if (currentTime > DayLengthSeconds * 2)
        {
            _stopwatch.Restart();
            _initTimeSeconds = 0;
        }
        return currentTime;
    }

    private (Phase, Phase, float) GetInterval(float time)
    {
        const float sunriseStartTime = 0f;
        const float dayStartTime = 0.1f * DayLengthSeconds;
        const float dayEndTime = 0.9f * DayLengthSeconds;
        const float sunsetStartTime = 1f * DayLengthSeconds;
        const float duskStartTime = 1.15f * DayLengthSeconds;
        const float nightStartTime = 1.3f * DayLengthSeconds;
        const float nightEndTime = 1.7f * DayLengthSeconds;
        const float dawnStartTime = 1.9f * DayLengthSeconds;

        if (time >= sunriseStartTime && time < dayStartTime)
            return (_sunriseStart, _dayStart, GetT(sunriseStartTime, dayStartTime, time));

        if (time >= dayStartTime && time < dayEndTime)
            return (_dayStart, _dayStart, 1);

        if (time >= dayEndTime && time < sunsetStartTime)
            return (_dayStart, _sunsetStart, GetT(dayEndTime, sunsetStartTime, time));

        if (time >= sunsetStartTime && time < duskStartTime)
            return (_sunsetStart, _duskStart, GetT(sunsetStartTime, duskStartTime, time));

        if (time >= duskStartTime && time < nightStartTime)
            return (_duskStart, _nightStart, GetT(duskStartTime, nightStartTime, time));

        if (time >= nightStartTime && time < nightEndTime)
            return (_nightStart, _nightStart, 1);

        if (time >= nightEndTime && time < dawnStartTime)
            return (_nightStart, _dawnStart, GetT(nightEndTime, dawnStartTime, time));

        return (_dawnStart, _sunriseStart, GetT(dawnStartTime, 2 * DayLengthSeconds, time));

        static float GetT(float begin, float end, float time) => (time - begin) / (end - begin);
    }

    private void UpdateSunLight(ref Vector3 sunVector) => _sunLight.Direction = new Vector4(sunVector, 0);

    private void UpdateMoonLight(ref Vector3 moonVector) => _moonLight.Direction = new Vector4(moonVector, 0);

    private static EnvironmentInfo CreateEnvInfo(ref (Phase, Phase, float) interval) => new()
    {
        PrevPhaseSunLightColor = interval.Item1.SunglareColor.Xyz * (1 - interval.Item1.StarsVisibility) * (1 - interval.Item1.StarsVisibility),
        NextPhaseSunLightColor = interval.Item2.SunglareColor.Xyz * (1 - interval.Item2.StarsVisibility) * (1 - interval.Item2.StarsVisibility),
        PrevPhaseMoonLightColor = new Vector3(0.55f, 0.62f, 0.79f) * interval.Item1.StarsVisibility,
        NextPhaseMoonLightColor = new Vector3(0.55f, 0.62f, 0.79f) * interval.Item2.StarsVisibility,
        PhaseT = interval.Item3,
        PrevPhaseNightAmbient = 0.05f * interval.Item1.StarsVisibility,
        NextPhaseNightAmbient = 0.05f * interval.Item2.StarsVisibility,
    };

    public void Render()
    {
        _skyboxShader.SetUp(_scene.Camera);
        float time = GetCurrentTime();

        var interval = GetInterval(time);
        _skyboxShader.SetStruct("prevPhase", interval.Item1);
        _skyboxShader.SetStruct("nextPhase", interval.Item2);
        _skyboxShader.SetFloat("phaseT", interval.Item3);

        _skybox.RotationX = time / DayLengthSeconds * MathF.PI;
        var sunVector = _initialSunVector * Matrix3.CreateRotationX(_skybox.RotationX);
        var moonVector = _initialMoonVector * Matrix3.CreateRotationZ(_skybox.RotationX);

        UpdateSunLight(ref sunVector);
        UpdateMoonLight(ref moonVector);
        EnvironmentInfo envInfo = CreateEnvInfo(ref interval);

        _modelShader.SetBool("hasSun", true);
        _modelShader.SetStruct("sunLight", _sunLight);
        _modelShader.SetStruct("moonLight", _moonLight);
        _modelShader.SetStruct("envInfo", envInfo);

        _objectShader.SetBool("hasSun", true);
        _objectShader.SetStruct("sunLight", _sunLight);
        _objectShader.SetStruct("moonLight", _moonLight);
        _objectShader.SetStruct("envInfo", envInfo);

        _skyboxShader.SetVector3("sunVector", sunVector);

        _skybox.Render(_skyboxShader);
    }
}
