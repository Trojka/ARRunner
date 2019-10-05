using System;
using System.Diagnostics;

namespace aRCCar.Xamarin.Game
{
    // This is a simplified model.
    public class EntityPhysics
    {
        DateTime? _currentTime = null;
        double _currentSpeed = 0.0d;
        double _currentForce = 0.0d;

        bool _forceApplied = true;

        TimeSpan _stumbleDuration = new TimeSpan(0, 0, 0, 0, 500);
        DateTime _stumbleTimeStamp = DateTime.MaxValue;

        public double BodyMass
        {
            get;
            set;
        } = 25;

        public double ApplyForceInNewton
        {
            get;
            set;
        } = 2;

        public void ApplyForce() {
            _forceApplied = false;
        }

        public void InvalidActivity()
        {
            _stumbleTimeStamp = DateTime.Now;
        }

        public double DistanceTravelled(DateTime t) {
            if(!_currentTime.HasValue) {
                _currentTime = t;
                return 0;
            }

            //http://www.softschools.com/formulas/physics/air_resistance_formula/85/
            // The dragforce is
            // F = ((density * dragCoeff * Area) / 2) * speed^2
            double dragCoeff = 2000;
            double dragForce = dragCoeff * Math.Pow(_currentSpeed, 2);

            _currentForce = -dragForce;
            if(!_forceApplied)
            {
                // The resulting force is the applied force minus the drag:
                _currentForce = ApplyForceInNewton - dragForce;
                _forceApplied = true;
            }

            //https://www.quora.com/How-do-you-find-distance-when-time-and-acceleration-are-given
            var elapsedTime = t - _currentTime.Value;
            double dt = elapsedTime.TotalSeconds;

            // Then the acceleration is force divided by mass:
            double a = _currentForce / BodyMass;

            // The distance travelled is (we're not going to move backwards, so minimum is zero):
            double d = Math.Max(0, (_currentSpeed * dt) + (a * Math.Pow(dt, 2) / 2));

            _currentTime = t;
            _currentSpeed = _currentSpeed + a * dt;

            //Debug.Print("MoveDistance: a=" + a + ", d=" + d + ", _currentForce=" + _currentForce + ", dragForce=" + dragForce);

            return d;
        }
    }
}
