using System;
using System.Diagnostics;

namespace ARRunner.Xamarin.Game
{
    // This is a simplified model.
    public class RunnerPhysics
    {
        //// https://en.wikipedia.org/wiki/Density_of_air
        //const double DensityAir = 1.225d; // kg/m3

        //// http://www.taylors.edu.my/EURECA/2014/downloads/02.pdf
        //const double DragCoeffHumanBodyStart = 0.6d;      // Squating
        //const double DragCoeffHumanBodyRunning = 1.2d;    // Standing

        //const double AreaHumanBodyStart = 0.6d;      // Estimate
        //const double AreaHumanBodyRunning = 1d;      // Height * Width = 2 * 0.5

        DateTime? _currentTime = null;
        double _currentSpeed = 0.0d;
        double _currentForce = 0.0d;

        bool _forceApplied = true;

        TimeSpan _stumbleDuration = new TimeSpan(0, 0, 0, 0, 500);
        DateTime _stumbleTimeStamp = DateTime.MaxValue;

        public double BodyMassInKg
        {
            get;
            set;
        } = 50;

        public double ApplyForceInNewton
        {
            get;
            set;
        } = 2;

        public void ApplyForce() {
            _forceApplied = false;
        }

        public void Stumble()
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
            double dragCoeff = 2000; //DensityAir * DragCoeffHumanBodyRunning * AreaHumanBodyRunning / 2; // = 0.735
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
            double a = _currentForce / BodyMassInKg;

            // The distance travelled is (we're not going to run backwards, so minimum is zero):
            double d = Math.Max(0, (_currentSpeed * dt) + (a * Math.Pow(dt, 2) / 2));

            _currentTime = t;
            _currentSpeed = _currentSpeed + a * dt;

            Debug.Print("MoveDistance: a=" + a + ", d=" + d + ", _currentForce=" + _currentForce + ", dragForce=" + dragForce);

            return d;
        }
    }
}
