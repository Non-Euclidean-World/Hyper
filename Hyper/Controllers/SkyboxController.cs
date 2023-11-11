using System.Diagnostics;
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

    private readonly float _dayLengthSeconds = 60; // length of the day in seconds

    private readonly Vector3 _initialSunVector; // vector pointing to the sun

    private float _initTimeSeconds = 0;

    private struct Phase
    {
        public Vector4 SkytopColor;
        public Vector4 HorizonColor;
        public Vector4 SunglareColor;
        public float StarsVisibility;
    }

    private Phase _dawnStart = new()
    {
        SkytopColor = new Vector4(0.35f, 0.65f, 0.89f, 1f), // https://www.wolframalpha.com/input?i=rgb%2889%2C+166%2C+228%29
        HorizonColor = new Vector4(0.15f, 0.37f, 0.66f, 1f), // https://www.wolframalpha.com/input?i=rgb%2837%2C++94%2C+168%29
        SunglareColor = new Vector4(1f, 0.51f, 0f, 1f), // https://www.wolframalpha.com/input?i=%23ff8100
        StarsVisibility = 0.8f,
    };

    private Phase _sunriseStart = new()
    {
        SkytopColor = new Vector4(0.35f, 0.58f, 0.72f, 1f), // https://www.wolframalpha.com/input?i=rgb%2889%2C+149%2C+183%29
        HorizonColor = new Vector4(0.89f, 0.59f, 0.35f, 1f), // https://www.wolframalpha.com/input?i=rgb%28228%2C+151%2C+89%29
        SunglareColor = new Vector4(1f, 0.3f, 0f, 1f), // https://www.wolframalpha.com/input?i=%23ff4d00
        StarsVisibility = 0.3f,
    };

    private Phase _dayStart = new()
    {
        SkytopColor = new Vector4(0.34f, 0.55f, 0.85f, 1f), // https://www.wolframalpha.com/input?i=%23568CD8
        HorizonColor = new Vector4(0.51f, 0.7f, 0.91f, 1.0f), // https://www.wolframalpha.com/input?i=%2382B2E8
        SunglareColor = Vector4.One,
        StarsVisibility = 0.05f,
    };

    private Phase _sunsetStart = new()
    {
        SkytopColor = new Vector4(0.08f, 0.16f, 0.32f, 1f), // https://www.wolframalpha.com/input?i=%23152852
        HorizonColor = new Vector4(0.99f, 0.37f, 0.33f, 1f), // https://www.wolframalpha.com/input?i=%23FD5E53
        SunglareColor = new Vector4(1f, 0.8f, 0.2f, 1f),
        StarsVisibility = 0.2f,
    };

    private Phase _duskStart = new()
    {
        SkytopColor = new Vector4(0.19f, 0f, 0.43f, 1f), // https://www.wolframalpha.com/input?i=%2330016d
        HorizonColor = new Vector4(0.05f, 0.05f, 0.24f, 1f), // https://www.wolframalpha.com/input?i=%09%230d0d3e
        SunglareColor = new Vector4(0.82f, 0.59f, 0.56f, 1f),
        StarsVisibility = 0.5f,
    };

    private Phase _nightStart = new()
    {
        SkytopColor = Vector4.Zero,
        HorizonColor = Vector4.Zero,
        SunglareColor = Vector4.Zero,
        StarsVisibility = 1f
    };

    public SkyboxController(Scene scene, AbstractSkyboxShader skyboxShader, StandardModelShader modelShader, StandardObjectShader objectShader)
    {
        _scene = scene;
        _skyboxShader = skyboxShader;
        _modelShader = modelShader;
        _objectShader = objectShader;
        _skybox = new Skybox.Skybox(skyboxShader.GlobalScale);
        _initialSunVector = -Vector3.UnitZ;
        _stopwatch.Start();
    }

    public void Dispose()
    {
        _skyboxShader.Dispose();
    }

    // current time in seconds
    private float GetCurrentTime()
    {
        float currentTime = _stopwatch.ElapsedMilliseconds / 1000.0f + _initTimeSeconds;
        if (currentTime > _dayLengthSeconds * 2)
        {
            _stopwatch.Restart();
            if (_initTimeSeconds != 0)
                _initTimeSeconds = 0;
        }
        return currentTime;
    }

    private (Phase, Phase, float) GetInterval(float time)
    {
        float sunriseStartTime = 0f;
        float dayStartTime = 0.1f * _dayLengthSeconds;
        float dayEndTime = 0.9f * _dayLengthSeconds;
        float sunsetStartTime = 1f * _dayLengthSeconds;
        float duskStartTime = 1.15f * _dayLengthSeconds;
        float nightStartTime = 1.3f * _dayLengthSeconds;
        float nightEndTime = 1.7f * _dayLengthSeconds;
        float dawnStartTime = 1.9f * _dayLengthSeconds;

        if (time >= sunriseStartTime && time < dayStartTime)
        {
            return (_sunriseStart, _dayStart, GetT(sunriseStartTime, dayStartTime, time));
        }

        if (time >= dayStartTime && time < dayEndTime)
        {
            return (_dayStart, _dayStart, 1);
        }

        if (time >= dayEndTime && time < sunsetStartTime)
        {
            return (_dayStart, _sunsetStart, GetT(dayEndTime, sunsetStartTime, time));
        }

        if (time >= sunsetStartTime && time < duskStartTime)
        {
            return (_sunsetStart, _duskStart, GetT(sunsetStartTime, duskStartTime, time));
        }

        if (time >= duskStartTime && time < nightStartTime)
        {
            return (_duskStart, _nightStart, GetT(duskStartTime, nightStartTime, time));
        }
        if (time >= nightStartTime && time < nightEndTime)
        {
            return (_nightStart, _nightStart, 1);
        }
        if (time >= nightEndTime && time < dawnStartTime)
        {
            return (_nightStart, _dawnStart, GetT(nightEndTime, dawnStartTime, time));
        }

        return (_dawnStart, _sunriseStart, GetT(dawnStartTime, 2 * _dayLengthSeconds, time));

        static float GetT(float begin, float end, float time)
        {
            return (time - begin) / (end - begin);
        }
    }

    public void Render()
    {
        _skyboxShader.SetUp(_scene.Camera);

        var interval = GetInterval(GetCurrentTime());
        _skyboxShader.SetStruct("prevPhase", interval.Item1);
        _skyboxShader.SetStruct("nextPhase", interval.Item2);
        _skyboxShader.SetFloat("phaseT", interval.Item3);

        _skybox.RotationX = GetCurrentTime() / _dayLengthSeconds * MathF.PI;
        var sunVector = _initialSunVector * Matrix3.CreateRotationX(_skybox.RotationX);
        DirectionalLight sunLight = new DirectionalLight
        {
            Ambient = 0.1f,
            Diffuse = 0.8f,
            Specular = 1f,
            Direction = new Vector4(sunVector, 0) //GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(sunVector, _scene.Camera.ReferencePointPosition, _scene.Camera.Curve, _modelShader.GlobalScale), _scene.Camera.Curve)
        };

        EnvironmentInfo envInfo = new EnvironmentInfo
        {
            PrevPhaseSunLightColor = interval.Item1.SunglareColor.Xyz,
            NextPhaseSunLightColor = interval.Item2.SunglareColor.Xyz,
            PhaseT = interval.Item3,
            PrevPhaseNightAmbient = 0.2f * interval.Item1.StarsVisibility,
            NextPhaseNightAmbient = 0.2f * interval.Item2.StarsVisibility,
        };

        _modelShader.SetBool("hasSun", true);
        _modelShader.SetStruct("sunLight", sunLight);
        _modelShader.SetStruct("envInfo", envInfo);

        _objectShader.SetBool("hasSun", true);
        _objectShader.SetStruct("sunLight", sunLight);
        _objectShader.SetStruct("envInfo", envInfo);

        _skyboxShader.SetVector3("sunVector", sunVector);

        _skybox.Render(_skyboxShader);
    }
}
