using Physics.Collisions;

namespace Character.Vehicles;
public class SpaceMustang : FourWheeledCar
{
    public SpaceMustang(SimpleCar simpleCar, int currentSphereId)
        : base(simpleCar, new FourWheeledCarModel(CarBodyResource.Instance, CarWheelResource.Instance, 2f), currentSphereId)
    {
    }
}
