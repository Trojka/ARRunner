using System;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using SceneKit;
using UIKit;

namespace ARRunner.Xamarin.Game
{
    public class SceneManager
    {
        public enum EntityState
        {
            Fixed,
            Preparing,
            Ready,
            Moving,
            Finished
        }

        SCNMaterial _greyMaterial = null;
        SCNMaterial _colorMaterial = null;

        SCNNode _entityNode = null;
        SCNNode _fieldNode = null;

        SCNNode _wingNode = null;

        static float _fieldWidth = 0.1f;
        static float _fieldLength = 0.4f;
        static float _entityInitialPosOnField = 0.05f;

        //float _speed = 0.0f;
        float _fieldToEntityOffset = _fieldLength / 2 - _entityInitialPosOnField;
        SCNVector3 _entityDirection = new SCNVector3(1, 0, 0);

        EntityState _entityState = EntityState.Preparing;

        public SCNVector3? EntityFieldPosition
        {
            get { return _fieldNode?.Position; }
        }

        public void PlaceEntityInSceneAtPosition(SCNScene scene, SCNVector3 position, EntityState state)
        {
            if (_entityNode == null)
            {

                var car = SCNScene.FromFile("Models.scnassets/rc_car");
                var carRootNode = car.RootNode.FindChildNode("rccarBody", true);

                _wingNode = carRootNode; //.FindChildNode("wing", true);

                _greyMaterial = new SCNMaterial();
                _greyMaterial.Diffuse.Contents = UIImage.FromFile("Models.scnassets/rc_car_grey_texture.png");

                _colorMaterial = new SCNMaterial();
                _colorMaterial.Diffuse.Contents = UIImage.FromFile("Models.scnassets/rc_car_texture.png");

                //var box = new SCNBox() { Width = 0.025f, Height = 0.025f, Length = 0.025f, ChamferRadius = 0.0f };

                //var boxNode = new SCNNode();
                //boxNode.Geometry = box;
                //boxNode.Position = position;

                carRootNode.Scale = new SCNVector3(0.01f, 0.01f, 0.01f);
                carRootNode.EulerAngles = new SCNVector3(0, (float)Math.PI / 2, 0);
                carRootNode.Position = position;

                _entityNode = carRootNode;

                //SCNVector3 minBB = new SCNVector3();
                //SCNVector3 maxBB = new SCNVector3();
                //carRootNode.GetBoundingBox(ref minBB, ref maxBB);

                //scene.RootNode.AddChildNode(carRootNode);
                scene.RootNode.AddChildNode(_entityNode);
            }

            _entityNode.Position = position;

            //if(state == EntityState.Fixed)
            //{
            //    while (_entityNode.Geometry.Materials.Count() != 0)
            //        _entityNode.Geometry.RemoveMaterial(0);
            //    _entityNode.Geometry.InsertMaterial(_greyMaterial, 0);

            //    while (_wingNode.Geometry.Materials.Count() != 0)
            //        _wingNode.Geometry.RemoveMaterial(0);
            //    _wingNode.Geometry.InsertMaterial(_greyMaterial, 0);

            //}
            //else 
            //{
            //    while (_entityNode.Geometry.Materials.Count() != 0)
            //        _entityNode.Geometry.RemoveMaterial(0);
            //    _entityNode.Geometry.InsertMaterial(_colorMaterial, 0);

            //    while (_wingNode.Geometry.Materials.Count() != 0)
            //        _wingNode.Geometry.RemoveMaterial(0);
            //    _wingNode.Geometry.InsertMaterial(_colorMaterial, 0);
            //}
            //_entityNode.Geometry.Materials.First().Diffuse.Contents = (state == EntityState.Fixed) ? UIColor.Red : UIColor.Orange;
        }

        public void FixEntityAtCurrentPosition(EntityState ready)
        {
            _entityNode.Geometry.Materials.First().Diffuse.Contents = UIColor.White;
        }

        public void PlaceEntityField(SCNScene scene)
        {
            var field = new SCNPlane() { Width = _fieldWidth, Height = _fieldLength };

            _fieldNode = new SCNNode();
            _fieldNode.Geometry = field;
            _fieldNode.Position = _entityNode.Position + (_entityDirection * _fieldToEntityOffset);
            _fieldNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Orange;
            _fieldNode.EulerAngles = new SCNVector3(-1 * (float)Math.PI / 2, (float)Math.PI / 2, 0);

            scene.RootNode.AddChildNode(_fieldNode);
        }

        SCNVector3? entityPositionOnDoubleTouch;
        SCNVector3? entityEulerAnglesOnDoubleTouch;
        SCNVector3? entityFieldEulerAnglesOnDoubleTouch;
        SCNVector3? doubleTouchInitPosition;
        float? doubleTouchInitEulerAnglesY;
        public void InitRotateEntityField(SCNScene scene, SCNVector3 coord1, SCNVector3 coord2)
        {
            entityPositionOnDoubleTouch = _entityNode.Position;
            entityEulerAnglesOnDoubleTouch = _entityNode.EulerAngles;
            entityFieldEulerAnglesOnDoubleTouch = _fieldNode.EulerAngles;

            doubleTouchInitPosition = (coord1 + coord2) / 2;
            var diff = coord2 - coord1;
            var len = diff.Length;
            var zcomp = diff.Z;
            var dirAngle = (float)Math.Acos(zcomp / len);
            doubleTouchInitEulerAnglesY = (diff.X > 0) ? dirAngle : (2 * (float)Math.PI - dirAngle);
        }

        public void RotateEntityField(SCNVector3 coord1, SCNVector3 coord2)
        {
            var doubleTouchNewPosition = (coord1 + coord2) / 2;
            var diff = coord2 - coord1;
            var len = diff.Length;
            var zcomp = diff.Z;
            var dirAngle = (float)Math.Acos(zcomp / len);
            var doubleTouchNewEulerAnglesY = (diff.X > 0) ? dirAngle : (2 * (float)Math.PI - dirAngle);

            var entityNewPosition = entityPositionOnDoubleTouch + (doubleTouchNewPosition - doubleTouchInitPosition);
            var entityNewEulerAngles = entityEulerAnglesOnDoubleTouch + new SCNVector3(0, doubleTouchNewEulerAnglesY - doubleTouchInitEulerAnglesY.Value, 0);
            var entityFieldNewEulerAngles = entityFieldEulerAnglesOnDoubleTouch + new SCNVector3(0, doubleTouchNewEulerAnglesY - doubleTouchInitEulerAnglesY.Value, 0);

            _entityNode.EulerAngles = entityNewEulerAngles.Value;
            _entityNode.Position = entityNewPosition.Value;

            var entityAngle = _entityNode.EulerAngles.Y; // + (Math.PI / 2);
            _entityDirection = new SCNVector3((float)Math.Sin(entityAngle), 0, (float)Math.Cos(entityAngle));

            _fieldNode.Position = _entityNode.Position + (_entityDirection * _fieldToEntityOffset);
            _fieldNode.EulerAngles = entityFieldNewEulerAngles.Value;

        }

        public void StartCountDown()
        {
            _fieldNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Red;
        }

        SCNVector3 _entityStartPosition;
        public void CanStartRun()
        {
            _entityStartPosition = _entityNode.Position;
            _fieldNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Green;
        }

        public void FalseStart()
        {
            //_entityNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Red;
        }

        public void Stumble()
        {
            //_entityNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Orange;
        }

        public void MoveDistance(float d)
        {

            if (_entityState == EntityState.Finished)
                return;

            _entityNode.Position = _entityNode.Position + (_entityDirection * d);
            if ((_entityNode.Position - _entityStartPosition).Length > (_fieldLength - _entityInitialPosOnField))
            {
                _entityState = EntityState.Finished;
                //_entityNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Green;
            }
        }
    }
}
