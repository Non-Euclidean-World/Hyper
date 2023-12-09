using Physics.Collisions;

namespace Character.Vehicles;
/// <summary>
/// The car that is used in the game.
/// </summary>
public class SpaceMustang : FourWheeledCar
{
    /// <summary>
    /// Creates an instance of the car.
    /// </summary>
    /// <param name="simpleCar">The base car.</param>
    /// <param name="currentSphereId">The id of the sphere the car starts in.</param>
    public SpaceMustang(SimpleCar simpleCar, int currentSphereId)
        : base(simpleCar, new FourWheeledCarModel(CarBodyResource.Instance, CarWheelResource.Instance, 2f), currentSphereId)
    {
    }
}
