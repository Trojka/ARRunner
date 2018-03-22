using System;
namespace ARRunner.Xamarin.Game
{
    // This is a simplified model.
    public class RunnerPhysics
    {
        // https://en.wikipedia.org/wiki/Density_of_air
        const double DensityAir = 1.225d; // kg/m3

        // http://www.taylors.edu.my/EURECA/2014/downloads/02.pdf
        const double DragCoeffHumanBodyStart = 0.6f;      // Squating
        const double DragCoeffHumanBodyRunning = 1.2f;    // Standing

        const double AreaHumanBodyStart = 0.6f;      // Estimate
        const double AreaHumanBodyRunning = 1f;      // Height * Width = 2 * 0.5

        DateTime? _currentTime = null;
        double _currentSpeed = 0.0f;
        double _currentForce = 0.0f;

        public double BodyMassInKg {
            get;
            set;
        }

        public void ApplyForce(double applyForceInNewton) {
            //http://www.softschools.com/formulas/physics/air_resistance_formula/85/

            // The dragforce is
            // F = ((density * dragCoeff * Area) / 2) * speed^2
            double dragForce = ((DensityAir * DragCoeffHumanBodyRunning * AreaHumanBodyRunning) / 2) * Math.Pow(_currentSpeed, 2);

            // The resulting force is the applied force minus the drag:
            _currentForce = applyForceInNewton - dragForce;

        }

        public double DistanceTravelled(DateTime t) {
            if(!_currentTime.HasValue) {
                _currentTime = t;
                return 0;
            }

            //https://www.quora.com/How-do-you-find-distance-when-time-and-acceleration-are-given

            var elapsedTime = t - _currentTime.Value;
            double dt = elapsedTime.TotalSeconds;

            // Then the acceleration is force divided by mass:
            double a = _currentForce / BodyMassInKg;

            // The distance travelled is:
            double d = Math.Max(0, (_currentSpeed * dt) + (a * Math.Pow(dt, 2) / 2));

            _currentTime = t;
            _currentSpeed = _currentSpeed + a * dt;

            return d;
        }
    }
}
