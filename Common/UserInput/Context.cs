using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Common.UserInput;

/// <summary>
/// Manages callbacks for handling user inputs such as keyboard keys, mouse buttons, and frame updates.
/// </summary>
public class Context
{
    public Dictionary<Keys, List<Action<FrameEventArgs>>> KeyHeldCallbacks = new();
    public Dictionary<MouseButton, List<Action<FrameEventArgs>>> ButtonHeldCallbacks = new();

    public Dictionary<Keys, List<Action>> KeyDownCallbacks = new();
    public Dictionary<MouseButton, List<Action>> ButtonDownCallbacks = new();

    public Dictionary<Keys, List<Action>> KeyUpCallbacks = new();
    public Dictionary<MouseButton, List<Action>> ButtonUpCallbacks = new();

    public List<Action<FrameEventArgs>> FrameUpdateCallbacks = new();

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

    /// <summary>
    /// Registers keys to be handled
    /// </summary>
    /// <param name="keys">The collection of keys to register.</param>
    public void RegisterKeys(ICollection<Keys> keys)
    {
        foreach (var key in keys)
        {
            RegisterKey(key);
        }
    }

    /// <summary>
    /// Registers mouse buttons to be handled
    /// </summary>
    /// <param name="mouseButtons">The collection of mouse buttons to register.</param>
    public void RegisterMouseButtons(ICollection<MouseButton> mouseButtons)
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

    /// <summary>
    /// Register a callback action that will be performed when a key is pressed down.
    /// </summary>
    /// <param name="key">Key to associate the action with.</param>
    /// <param name="callback">Action that will be performed.</param>
    public void RegisterKeyDownCallback(Keys key, Action callback)
    {
        RegisterCallback(key, callback, KeyDownCallbacks);
    }

    /// <summary>
    /// Register a callback action that will be performed when a key is released.
    /// </summary>
    /// <param name="key">Key to associate the action with.</param>
    /// <param name="callback">Action that will be performed.</param>
    public void RegisterKeyUpCallback(Keys key, Action callback)
    {
        RegisterCallback(key, callback, KeyUpCallbacks);
    }

    /// <summary>
    /// Register a callback action that will be performed when a key is held.
    /// </summary>
    /// <param name="key">Key to associate the action with.</param>
    /// <param name="callback">Action that will be performed.</param>
    public void RegisterKeyHeldCallback(Keys key, Action<FrameEventArgs> callback)
    {
        RegisterCallback(key, callback, KeyHeldCallbacks);
    }

    /// <summary>
    /// Register a callback action that will be performed when a mouse button is pressed down.
    /// </summary>
    /// <param name="button">Mouse button to associate the action with.</param>
    /// <param name="callback">Action that will be performed.</param>
    public void RegisterMouseButtonDownCallback(MouseButton button, Action callback)
    {
        RegisterCallback(button, callback, ButtonDownCallbacks);
    }

    /// <summary>
    /// Register a callback action that will be performed when a mouse button is released.
    /// </summary>
    /// <param name="button">Mouse button to associate the action with.</param>
    /// <param name="callback">Action that will be performed.</param>
    public void RegisterMouseButtonUpCallback(MouseButton button, Action callback)
    {
        RegisterCallback(button, callback, ButtonUpCallbacks);
    }

    /// <summary>
    /// Register a callback action that will be performed when a mouse button is held down.
    /// </summary>
    /// <param name="button">Mouse button to associate the action with.</param>
    /// <param name="callback">Action that will be performed.</param>
    public void RegisterMouseButtonHeldCallback(MouseButton button, Action<FrameEventArgs> callback)
    {
        RegisterCallback(button, callback, ButtonHeldCallbacks);
    }

    /// <summary>
    /// Register a callback action that will be performed when a new frame is rendered.
    /// </summary>
    /// <param name="callback">Action that will be performed.</param>
    public void RegisterUpdateFrameCallback(Action<FrameEventArgs> callback)
    {
        FrameUpdateCallbacks.Add(callback);
    }

    /// <summary>
    /// Register a callback action that will be performed when the mouse is moved.
    /// </summary>
    /// <param name="callback">Action that will be performed.</param>
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

    /// <summary>
    /// Executes all callbacks that should be called when the input is held down.
    /// </summary>
    /// <param name="inputType">Type of the input.</param>
    /// <param name="e">Arguments for frame event.</param>
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
