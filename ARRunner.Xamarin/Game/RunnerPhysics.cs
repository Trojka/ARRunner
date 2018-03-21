using System;
namespace ARRunner.Xamarin.Game
{
    //http://www.softschools.com/formulas/physics/air_resistance_formula/85/
    public class RunnerPhysics
    {
        // https://en.wikipedia.org/wiki/Density_of_air
        const double DensityAir = 1.225d; // kg/m3

        // http://www.taylors.edu.my/EURECA/2014/downloads/02.pdf
        const double DragCoeffHumanBodyStart = 0.6f;      // Squating
        const double DragCoeffHumanBodyRunning = 1.2f;    // Standing

        const double AreaHumanBodyStart = 0.6f;      // Estimate
        const double AreaHumanBodyRunning = 1f;      // Height * Width = 2 * 0.5

        double _currentSpeed = 0.0f;

        public double BodyMassInKg {
            get;
            set;
        }

        public void ApplyForce(double force){
            // This is a simplefied model.

            // The dragforce is
            // F = ((density * dragCoeff * Area) / 2) * speed^2
            double dragForce = ((DensityAir * DragCoeffHumanBodyRunning * AreaHumanBodyRunning) / 2) * Math.Pow(_currentSpeed, 2);

            double resultingForce = force - dragForce;
        }
    }
}
