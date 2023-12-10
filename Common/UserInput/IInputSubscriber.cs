namespace Common.UserInput;

/// <summary>
/// Defines a contract for classes that subscribe to and handle input events by registering callbacks within a given context.
/// </summary>
public interface IInputSubscriber
{
    /// <summary>
    /// Registers input callbacks within the provided context to handle input events.
    /// </summary>
    /// <param name="context">The context in which input callbacks will be registered.</param>
    void RegisterCallbacks(Context context);
}
