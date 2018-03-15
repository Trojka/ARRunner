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
        public enum RunnerState
        {
            Fixed,
            Preparing,
            Ready,
            Running,
            Finished
        }

        SCNNode _runnerNode = null;
        SCNNode _fieldNode = null;

        static float _fieldWidth = 0.1f;
        static float _fieldLength = 0.4f;
        static float _runnerInitialPosOnField = 0.05f;

        float _speed = 0.0f;
        float _fieldToRunnerOffset = _fieldLength / 2 - _runnerInitialPosOnField;
        SCNVector3 _runnerDirection = new SCNVector3(1, 0, 0);

        RunnerState _runnerState = RunnerState.Preparing;

        public SCNVector3? RunnerFieldPosition
        {
            get { return _fieldNode?.Position; }
        }

        public void PlaceRunnerInSceneAtPosition(SCNScene scene, SCNVector3 position, RunnerState state)
        {
            if (_runnerNode == null)
            {
                var box = new SCNBox() { Width = 0.025f, Height = 0.025f, Length = 0.025f, ChamferRadius = 0.0f };

                _runnerNode = new SCNNode();
                _runnerNode.Geometry = box;
                _runnerNode.Position = position;

                scene.RootNode.AddChildNode(_runnerNode);
            }

            _runnerNode.Position = position;
            _runnerNode.Geometry.Materials.First().Diffuse.Contents = (state == RunnerState.Fixed) ? UIColor.Red : UIColor.Orange;
        }

        public void FixRunnerAtCurrentPosition(RunnerState ready)
        {
            _runnerNode.Geometry.Materials.First().Diffuse.Contents = UIColor.White;
        }

        public void PlaceRunnerField(SCNScene scene)
        {
            var field = new SCNPlane() { Width = _fieldWidth, Height = _fieldLength };

            _fieldNode = new SCNNode();
            _fieldNode.Geometry = field;
            _fieldNode.Position = _runnerNode.Position + (_runnerDirection * _fieldToRunnerOffset);
            _fieldNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Orange;
            _fieldNode.EulerAngles = new SCNVector3(-1 * (float)Math.PI / 2, (float)Math.PI / 2, 0);

            scene.RootNode.AddChildNode(_fieldNode);
        }

        //SCNNode f1Node;
        //SCNNode f2Node;

        SCNVector3? runnerPositionOnDoubleTouch;
        SCNVector3? runnerEulerAnglesOnDoubleTouch;
        SCNVector3? runnerFieldEulerAnglesOnDoubleTouch;
        SCNVector3? doubleTouchInitPosition;
        float? doubleTouchInitEulerAnglesY;
        public void InitRotateRunnerField(SCNScene scene, SCNVector3 coord1, SCNVector3 coord2)
        {
            //var f1 = new SCNBox() { Width = 0.025f, Height = 0.025f, Length = 0.025f, ChamferRadius = 0.0f };
            //f1Node = new SCNNode();
            //f1Node.Geometry = f1;
            //f1Node.Position = coord1;
            //f1Node.Geometry.Materials.First().Diffuse.Contents = UIColor.Cyan;
            //scene.RootNode.AddChildNode(f1Node); 

            //var f2 = new SCNBox() { Width = 0.025f, Height = 0.025f, Length = 0.025f, ChamferRadius = 0.0f };
            //f2Node = new SCNNode();
            //f2Node.Geometry = f2;
            //f2Node.Position = coord2;
            //f2Node.Geometry.Materials.First().Diffuse.Contents = UIColor.Blue;
            //scene.RootNode.AddChildNode(f2Node); 


            runnerPositionOnDoubleTouch = _runnerNode.Position;
            runnerEulerAnglesOnDoubleTouch = _runnerNode.EulerAngles;
            runnerFieldEulerAnglesOnDoubleTouch = _fieldNode.EulerAngles;

            Debug.WriteLine("runnerPositionOnDoubleTouch: " + runnerPositionOnDoubleTouch.ToString());
            //Debug.WriteLine("runnerFieldEulerAnglesOnDoubleTouch: " + runnerFieldEulerAnglesOnDoubleTouch.ToString());

            doubleTouchInitPosition = (coord1 + coord2) / 2;
            var diff = coord2 - coord1;
            var len = diff.Length;
            var zcomp = diff.Z;
            var dirAngle = (float)Math.Acos(zcomp / len);
            doubleTouchInitEulerAnglesY = (diff.X > 0) ? dirAngle : (2 * (float)Math.PI - dirAngle);

            //Debug.WriteLine("doubleTouchInitPosition: " + doubleTouchInitPosition.ToString());
        }

        public void RotateRunnerField(SCNVector3 coord1, SCNVector3 coord2)
        {
            //f1Node.Position = coord1;
            //f2Node.Position = coord2;

            var doubleTouchNewPosition = (coord1 + coord2) / 2;
            var diff = coord2 - coord1;
            var len = diff.Length;
            var zcomp = diff.Z;
            var dirAngle = (float)Math.Acos(zcomp / len);
            var doubleTouchNewEulerAnglesY = (diff.X > 0) ? dirAngle : (2 * (float)Math.PI - dirAngle);

            //Debug.WriteLine("doubleTouchNewPosition: " + doubleTouchNewPosition.ToString());
            //Debug.WriteLine("RotateRunnerField: coord1:" + coord1.ToString() + ", coord2:" + coord2.ToString() + ", len:" + len + ", zcomp:" + zcomp + ", doubleTouchNewEulerAnglesY:" + doubleTouchNewEulerAnglesY);

            var runnerNewPosition = runnerPositionOnDoubleTouch + (doubleTouchNewPosition - doubleTouchInitPosition);
            var runnerNewEulerAngles = runnerEulerAnglesOnDoubleTouch + new SCNVector3(0, doubleTouchNewEulerAnglesY - doubleTouchInitEulerAnglesY.Value, 0);
            var runnerFieldNewEulerAngles = runnerFieldEulerAnglesOnDoubleTouch + new SCNVector3(0, doubleTouchNewEulerAnglesY - doubleTouchInitEulerAnglesY.Value, 0);

            //Debug.WriteLine("doubleTouchInitEulerAnglesY: " + doubleTouchInitEulerAnglesY.Value
            //+ ", doubleTouchNewEulerAnglesY: " + doubleTouchNewEulerAnglesY 
            //+ ", targetEulerAnglesY: " + (doubleTouchNewEulerAnglesY - doubleTouchInitEulerAnglesY.Value));

            //Debug.WriteLine("runnerNewPosition: " + runnerNewPosition.ToString());
            //Debug.WriteLine("runnerFieldNewEulerAngles: " + runnerFieldNewEulerAngles.ToString());

            _runnerNode.Position = runnerNewPosition.Value;
            _runnerNode.EulerAngles = runnerNewEulerAngles.Value;

            var runnerAngle = _runnerNode.EulerAngles.Y + (Math.PI / 2);
            _runnerDirection = new SCNVector3((float)Math.Sin(runnerAngle), 0, (float)Math.Cos(runnerAngle));

            _fieldNode.Position = _runnerNode.Position + (_runnerDirection * _fieldToRunnerOffset);
            _fieldNode.EulerAngles = runnerFieldNewEulerAngles.Value;

        }

        public void StartCountDown()
        {
            _fieldNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Red;
        }

        SCNVector3 _runnerStartPosition;
        public void CanStartRun()
        {
            _runnerStartPosition = _runnerNode.Position;
            _fieldNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Green;
        }

        public void FalseStart()
        {
            _runnerNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Red;
        }

        public void RunnerStep()
        {
            if (_runnerState == RunnerState.Finished)
                return;

            if (DateTime.Now - _stumbleTimeStamp <= _stumbleDuration)
                return;

            _runnerNode.Geometry.Materials.First().Diffuse.Contents = UIColor.White;
            _speed = _speed + 0.0005f;
        }

        TimeSpan _stumbleDuration = new TimeSpan(0, 0, 0, 0, 500);
        DateTime _stumbleTimeStamp = DateTime.MaxValue;
        public void Stumble()
        {
            _stumbleTimeStamp = DateTime.Now;
            _speed = _speed / 2;
            _runnerNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Orange;
        }

        public void Run()
        {
            if (_runnerState == RunnerState.Finished)
                return;

            _runnerNode.Position = _runnerNode.Position + (_runnerDirection * _speed);
            if ((_runnerNode.Position - _runnerStartPosition).Length > (_fieldLength - _runnerInitialPosOnField))
            {
                _runnerState = RunnerState.Finished;
                _runnerNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Green;
            }
        }
    }
}
