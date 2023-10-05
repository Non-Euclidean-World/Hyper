using Common;

namespace Hyper.Controllers.Factories;
public interface IControllerFactory
{
    IController[] CreateControllers(Settings settings);
}
