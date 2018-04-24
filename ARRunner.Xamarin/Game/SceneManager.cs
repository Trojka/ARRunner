using System;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using SceneKit;
using UIKit;

namespace aRCCar.Xamarin.Game
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

        MovingEntity _entityNode = null;
        TrackNode _fieldNode = null;
        PlacementNode _placementNode = null;


        //float _speed = 0.0f;
        //float _fieldToEntityOffset = _fieldLength / 2 - _entityInitialPosOnField;
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

                var car = new MovingEntity();
                car.Load();

                car.Position = position;

                _entityNode = car;
                scene.RootNode.AddChildNode(_entityNode);
            }

            _entityNode.Position = position;

            if(state == EntityState.Fixed)
            {
                _entityNode.IsEnabled = false;
            }
            else 
            {
                _entityNode.IsEnabled = true;
            }
        }

        public void FixEntityAtCurrentPosition(EntityState ready)
        {
            //_entityNode.Geometry.Materials.First().Diffuse.Contents = UIColor.White;
        }

        public void PlaceEntityField(SCNScene scene)
        {
            _fieldNode = new TrackNode();
            _fieldNode.Position = _entityNode.Position + (_entityDirection * _fieldNode.EntityStartPositionOffet);

            scene.RootNode.AddChildNode(_fieldNode);
        }

        public void PlacePlacementNode(SCNScene scene){
            _placementNode =  new PlacementNode();
            _placementNode.Position = _entityNode.Position;

            scene.RootNode.AddChildNode(_placementNode);
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

        public void RotateEntityFieldMarker(SCNVector3 coord1, SCNVector3 coord2)
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

            _fieldNode.Position = _entityNode.Position + (_entityDirection * _fieldNode.EntityStartPositionOffet);
            _fieldNode.EulerAngles = entityFieldNewEulerAngles.Value;

            _placementNode.Position = _entityNode.Position;
        }

        public void StartCountDown()
        {
            _placementNode.RemoveFromParentNode();
            _fieldNode.ShowFullTrack(); //Geometry.Materials.First().Diffuse.Contents = UIColor.Red;
        }

        SCNVector3 _entityStartPosition;
        public void CanStartRun()
        {
            _entityStartPosition = _entityNode.Position;
            //_fieldNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Green;
        }

        public void FalseStart()
        {
            //_entityNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Red;
        }

        public void InvalidActivity()
        {
            //_entityNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Orange;
        }

        public void MoveDistance(float d)
        {

            if (_entityState == EntityState.Finished)
                return;

            _entityNode.Position = _entityNode.Position + (_entityDirection * d);
            if ((_entityNode.Position - _entityStartPosition).Length > (_fieldNode.Length - _fieldNode.EntityStartPositionOffet))
            {
                _entityState = EntityState.Finished;
            }
        }
    }
}
