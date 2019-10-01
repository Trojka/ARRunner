using System;
using System.Linq;
using Foundation;
using SceneKit;
using UIKit;

namespace aRCCar.Xamarin.Game
{
    public class MovingEntity : SCNReferenceNode
    {
        SCNMaterial _greyMaterial = null;
        SCNMaterial _colorMaterial = null;

        float _modelScale = 0.01f;

        SCNNode _carBodyNode = null;
        SCNNode _wingNode = null;
        SCNNode _pipeNode = null;

        SCNParticleSystem _exhaust;
        nfloat _configuredBirthRate;

        private bool _isEnabled;

        public MovingEntity()
            :base (NSUrl.FromString(
                NSBundle.MainBundle.BundleUrl.AbsoluteString + $"Models.scnassets/rc_car.dae"
            ))
        {

            _greyMaterial = new SCNMaterial();
            _greyMaterial.Diffuse.Contents = UIImage.FromFile("Models.scnassets/rc_car_grey_texture.png");

            _colorMaterial = new SCNMaterial();
            _colorMaterial.Diffuse.Contents = UIImage.FromFile("Models.scnassets/rc_car_texture.png");

            _exhaust = SCNParticleSystem.Create("Exhaust", "Models.scnassets");
            _configuredBirthRate = _exhaust.BirthRate;
            _exhaust.BirthRate = 0;
            _exhaust.ParticleSize = _modelScale;
        }

		public override void Load()
		{
            base.Load();

            _carBodyNode = this.FindChildNode("rccarBody", true);
            _wingNode = this.FindChildNode("wing", true);
            _pipeNode = this.FindChildNode("pipe", true);

            _pipeNode.AddParticleSystem(_exhaust);

            this.Scale = new SCNVector3(_modelScale, _modelScale, _modelScale);
            this.EulerAngles = new SCNVector3(0, (float)Math.PI / 2, 0);
		}

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set {
                _isEnabled = value;
                if (_isEnabled)
                {
                    while (_carBodyNode.Geometry.Materials.Count() != 0)
                        _carBodyNode.Geometry.RemoveMaterial(0);
                    _carBodyNode.Geometry.InsertMaterial(_colorMaterial, 0);

                    while (_wingNode.Geometry.Materials.Count() != 0)
                        _wingNode.Geometry.RemoveMaterial(0);
                    _wingNode.Geometry.InsertMaterial(_colorMaterial, 0);

                    _exhaust.BirthRate = _configuredBirthRate;
                }
                else
                {
                    while (_carBodyNode.Geometry.Materials.Count() != 0)
                        _carBodyNode.Geometry.RemoveMaterial(0);
                    _carBodyNode.Geometry.InsertMaterial(_greyMaterial, 0);

                    while (_wingNode.Geometry.Materials.Count() != 0)
                        _wingNode.Geometry.RemoveMaterial(0);
                    _wingNode.Geometry.InsertMaterial(_greyMaterial, 0);

                    _exhaust.BirthRate = 0;
                }

            }
        }
	}
}
