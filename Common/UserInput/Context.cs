using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Common.UserInput;

public class Context
{
    private Context() { }

    private static readonly Lazy<Context> _instance = new(() => new Context());

    public static Context Instance => _instance.Value;

    public Dictionary<Keys, List<Action<FrameEventArgs>>> KeyHeldCallbacks = new();
    public Dictionary<MouseButton, List<Action<FrameEventArgs>>> ButtonHeldCallbacks = new();

    public Dictionary<Keys, List<Action>> KeyDownCallbacks = new();
    public Dictionary<MouseButton, List<Action>> ButtonDownCallbacks = new();

    public Dictionary<Keys, List<Action>> KeyUpCallbacks = new();
    public Dictionary<MouseButton, List<Action>> ButtonUpCallbacks = new();

    public List<Action<FrameEventArgs>> FrameUpdateCallbacks = new();
    public List<Action<string[]>> ConsoleInputCallbacks = new();

    public List<Action<MouseMoveEventArgs>> MouseMoveCallbacks = new();

    public Dictionary<Keys, bool> HeldKeys = new();
    public Dictionary<MouseButton, bool> HeldButtons = new();

    public HashSet<Keys> UsedKeys = new();
    public HashSet<MouseButton> UsedMouseButtons = new();

    private void RegisterKey(Keys key)
    {
        if (UsedKeys.Contains(key))
            return;

        UsedKeys.Add(key);
        HeldKeys.Add(key, false);
        RegisterKeyDownCallback(key, () => HeldKeys[key] = true);
        RegisterKeyUpCallback(key, () => HeldKeys[key] = false);
    }

    private void RegisterMouseButton(MouseButton button)
    {
        if (UsedMouseButtons.Contains(button))
            return;

        UsedMouseButtons.Add(button);
        HeldButtons.Add(button, false);
        RegisterMouseButtonDownCallback(button, () => HeldButtons[button] = true);
        RegisterMouseButtonUpCallback(button, () => HeldButtons[button] = false);
    }

    public void RegisterKeys(List<Keys> keys)
    {
        foreach (var key in keys)
        {
            RegisterKey(key);
        }
    }

    public void RegisterMouseButtons(List<MouseButton> mouseButtons)
    {
        foreach (var button in mouseButtons)
        {
            RegisterMouseButton(button);
        }
    }

    private static void RegisterCallback<TInput, TCallback>(TInput input, TCallback callback, Dictionary<TInput, List<TCallback>> mapping)
        where TInput : notnull
    {
        if (!mapping.ContainsKey(input))
        {
            mapping.Add(input, new List<TCallback>() { callback });
        }
        else
        {
            mapping[input].Add(callback);
        }
    }

    public void RegisterKeyDownCallback(Keys key, Action callback)
    {
        RegisterCallback(key, callback, KeyDownCallbacks);
    }

    public void RegisterKeyUpCallback(Keys key, Action callback)
    {
        RegisterCallback(key, callback, KeyUpCallbacks);
    }

    public void RegisterKeyHeldCallback(Keys key, Action<FrameEventArgs> callback)
    {
        RegisterCallback(key, callback, KeyHeldCallbacks);
    }

    public void RegisterMouseButtonDownCallback(MouseButton button, Action callback)
    {
        RegisterCallback(button, callback, ButtonDownCallbacks);
    }

    public void RegisterMouseButtonUpCallback(MouseButton button, Action callback)
    {
        RegisterCallback(button, callback, ButtonUpCallbacks);
    }

    public void RegisterMouseButtonHeldCallback(MouseButton button, Action<FrameEventArgs> callback)
    {
        RegisterCallback(button, callback, ButtonHeldCallbacks);
    }

    public void RegisterUpdateFrameCallback(Action<FrameEventArgs> callback)
    {
        FrameUpdateCallbacks.Add(callback);
    }
    
    public void RegisterConsoleInputCallback(Action<string[]> callback)
    {
        ConsoleInputCallbacks.Add(callback);
    }

    public void RegisterMouseMoveCallback(Action<MouseMoveEventArgs> callback)
    {
        MouseMoveCallbacks.Add(callback);
    }

    private static void ExecuteAllHeldCallbacks<TInput>(Dictionary<TInput, bool> inputHeld,
        Dictionary<TInput, List<Action<FrameEventArgs>>> inputHeldMapping,
        HashSet<TInput> usedInputs, FrameEventArgs e)
        where TInput : notnull
    {
        foreach (var input in usedInputs)
        {
            if (inputHeld.ContainsKey(input) && inputHeldMapping.ContainsKey(input) && inputHeld[input])
            {
                foreach (var callback in inputHeldMapping[input])
                {
                    callback(e);
                }
            }
        }
    }

    public void ExecuteAllHeldCallbacks(InputType inputType, FrameEventArgs e)
    {
        switch (inputType)
        {
            case InputType.MouseButton:
                ExecuteAllHeldCallbacks(HeldButtons, ButtonHeldCallbacks, UsedMouseButtons, e);
                break;
            case InputType.Key:
                ExecuteAllHeldCallbacks(HeldKeys, KeyHeldCallbacks, UsedKeys, e);
                break;
        }
    }
}
