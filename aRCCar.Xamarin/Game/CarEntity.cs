using System;
using System.Linq;
using Foundation;
using SceneKit;
using UIKit;

namespace aRCCar.Xamarin.Game
{
    public class CarEntity : SCNReferenceNode
    {
        SCNMaterial _greyMaterial = null;
        SCNMaterial _colorMaterial = null;

        SCNNode _carBodyNode = null;
        SCNNode _wingNode = null;


        private bool _isEnabled;

        public CarEntity()
            :base (NSUrl.FromString(
                NSBundle.MainBundle.BundleUrl.AbsoluteString + $"Models.scnassets/rc_car.dae"))
        {

            _greyMaterial = new SCNMaterial();
            _greyMaterial.Diffuse.Contents = UIImage.FromFile("Models.scnassets/rc_car_grey_texture.png");

            _colorMaterial = new SCNMaterial();
            _colorMaterial.Diffuse.Contents = UIImage.FromFile("Models.scnassets/rc_car_texture.png");
        }

		public override void Load()
		{
            base.Load();

            _carBodyNode = this.FindChildNode("rccarBody", true);
            _wingNode = this.FindChildNode("wing", true);

		}

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set {
                _isEnabled = value;
                if(_isEnabled)
                {
                    while (_carBodyNode.Geometry.Materials.Count() != 0)
                        _carBodyNode.Geometry.RemoveMaterial(0);
                    _carBodyNode.Geometry.InsertMaterial(_colorMaterial, 0);

                    while (_wingNode.Geometry.Materials.Count() != 0)
                        _wingNode.Geometry.RemoveMaterial(0);
                    _wingNode.Geometry.InsertMaterial(_colorMaterial, 0);
}
                else 
                {
                    while (_carBodyNode.Geometry.Materials.Count() != 0)
                        _carBodyNode.Geometry.RemoveMaterial(0);
                    _carBodyNode.Geometry.InsertMaterial(_greyMaterial, 0);

                    while (_wingNode.Geometry.Materials.Count() != 0)
                        _wingNode.Geometry.RemoveMaterial(0);
                    _wingNode.Geometry.InsertMaterial(_greyMaterial, 0);
                }

            }
        }
	}
}
