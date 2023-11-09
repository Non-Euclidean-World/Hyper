using System.Diagnostics;
using Common.UserInput;
using Hyper.Shaders.SkyboxShader;
using OpenTK.Mathematics;

namespace Hyper.Controllers;
internal class SkyboxController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractSkyboxShader _skyboxShader;

    private readonly Skybox.Skybox _skybox;

    private readonly Stopwatch _stopwatch = new();

    private readonly float _dayLength = 40; // length of the day in seconds

    private struct Phase
    {
        public Vector4 SkytopColor;
        public Vector4 HorizonColor;
        public float StarsVisibility;
    }

    private Phase _dawnStart = new Phase
    {
        SkytopColor = new Vector4(0.35f, 0.65f, 0.89f, 1f), // https://www.wolframalpha.com/input?i=rgb%2889%2C+166%2C+228%29
        HorizonColor = new Vector4(0.15f, 0.37f, 0.66f, 1f), // https://www.wolframalpha.com/input?i=rgb%2837%2C++94%2C+168%29
        StarsVisibility = 0.8f,
    };

    private Phase _sunriseStart = new Phase
    {
        SkytopColor = new Vector4(0.35f, 0.58f, 0.72f, 1f), // https://www.wolframalpha.com/input?i=rgb%2889%2C+149%2C+183%29
        HorizonColor = new Vector4(0.89f, 0.59f, 0.35f, 1f), // https://www.wolframalpha.com/input?i=rgb%28228%2C+151%2C+89%29
        StarsVisibility = 0.3f,
    };

    private Phase _dayStart = new Phase
    {
        SkytopColor = new Vector4(0.34f, 0.55f, 0.85f, 1f), // https://www.wolframalpha.com/input?i=%23568CD8
        HorizonColor = new Vector4(0.51f, 0.7f, 0.91f, 1.0f), // https://www.wolframalpha.com/input?i=%2382B2E8
        StarsVisibility = 0.05f,
    };

    private Phase _sunsetStart = new Phase
    {
        SkytopColor = new Vector4(0.08f, 0.16f, 0.32f, 1f), // https://www.wolframalpha.com/input?i=%23152852
        HorizonColor = new Vector4(0.99f, 0.37f, 0.33f, 1f), // https://www.wolframalpha.com/input?i=%23FD5E53
        StarsVisibility = 0.3f,
    };

    private Phase _duskStart = new Phase
    {
        SkytopColor = new Vector4(0.19f, 0f, 0.43f, 1f), // https://www.wolframalpha.com/input?i=%2330016d
        HorizonColor = new Vector4(0.05f, 0.05f, 0.24f, 1f), // https://www.wolframalpha.com/input?i=%09%230d0d3e
        StarsVisibility = 0.8f,
    };

    private Phase _nightStart = new Phase
    {
        SkytopColor = Vector4.Zero,
        HorizonColor = Vector4.Zero,
        StarsVisibility = 1f
    };

    public SkyboxController(Scene scene, AbstractSkyboxShader skyboxShader, Context context)
    {
        _scene = scene;
        _skyboxShader = skyboxShader;
        _skybox = new Skybox.Skybox(skyboxShader.GlobalScale);
        _stopwatch.Start();
        RegisterCallbacks(context);
    }

    public void Dispose()
    {
        _skyboxShader.Dispose();
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterUpdateFrameCallback((e) =>
        {
            _skybox.RotationX = _stopwatch.ElapsedMilliseconds / 1000.0f / _dayLength * MathF.PI;
            if (_stopwatch.ElapsedMilliseconds / 1000f > _dayLength * 2)
                _stopwatch.Restart();
        });
    }

    bool[] printed = new bool[8];

    private (Phase, Phase, float) GetInterval(float time)
    {
        float sunriseStartTime = 0;
        float dayStartTime = 0.1f * _dayLength;
        float dayEndTime = 0.8f * _dayLength;
        float sunsetStartTime = 0.9f * _dayLength;
        float duskStartTime = _dayLength;
        float nightStartTime = 1.1f * _dayLength;
        float nightEndTime = 1.8f * _dayLength;
        float dawnStartTime = 1.9f * _dayLength;

        if (time >= sunriseStartTime && time < dayStartTime)
        {
            if (!printed[0])
            {
                Console.WriteLine("sunrise - day");
                printed[0] = true;
            }

            return (_sunriseStart, _dayStart, getT(sunriseStartTime, dayStartTime, time));
        }

        if (time >= dayStartTime && time < dayEndTime)
        {
            if (!printed[1])
            {
                Console.WriteLine("day");
                printed[1] = true;
            }
            return (_dayStart, _dayStart, 1);
        }

        if (time >= dayEndTime && time < sunsetStartTime)
        {
            if (!printed[2])
            {
                Console.WriteLine("day - sunset");
                printed[2] = true;
            }
            return (_dayStart, _sunsetStart, getT(dayEndTime, sunsetStartTime, time));
        }

        if (time >= sunsetStartTime && time < duskStartTime)
        {
            if (!printed[3])
            {
                Console.WriteLine("sunset - dusk");
                printed[3] = true;
            }
            return (_sunsetStart, _duskStart, getT(sunsetStartTime, duskStartTime, time));
        }

        if (time >= duskStartTime && time < nightStartTime)
        {
            if (!printed[4])
            {
                Console.WriteLine("dusk - night");
                printed[4] = true;
            }
            return (_duskStart, _nightStart, getT(duskStartTime, nightStartTime, time));
        }
        if (time >= nightStartTime && time < nightEndTime)
        {
            if (!printed[5])
            {
                Console.WriteLine("night");
                printed[5] = true;
            }
            return (_nightStart, _nightStart, 1);
        }
        if (time >= nightEndTime && time < dawnStartTime)
        {
            if (!printed[6])
            {
                Console.WriteLine("night - dawn");
                printed[6] = true;
            }
            return (_nightStart, _dawnStart, getT(nightEndTime, dawnStartTime, time));
        }

        if (!printed[7])
        {
            Console.WriteLine("dawn - sunrise");
            printed[7] = true;
        }
        return (_dawnStart, _sunriseStart, getT(dawnStartTime, 2 * _dayLength, time));

        static float getT(float begin, float end, float time)
        {
            return (time - begin) / (end - begin);
        }
    }

    public void Render()
    {
        _skyboxShader.SetUp(_scene.Camera);
        var interval = GetInterval(_stopwatch.ElapsedMilliseconds / 1000f);
        _skyboxShader.SetStruct("prevPhase", interval.Item1);
        _skyboxShader.SetStruct("nextPhase", interval.Item2);
        _skyboxShader.SetFloat("phaseT", interval.Item3);

        _skybox.Render(_skyboxShader);
    }
}
