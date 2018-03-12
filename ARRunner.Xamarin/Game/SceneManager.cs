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
            Ready
        }

        SCNNode _runnerNode = null;
        SCNNode _fieldNode = null;

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
            _runnerNode.Geometry.Materials.First().Diffuse.Contents = state == RunnerState.Fixed ? UIColor.Red : UIColor.Orange;
        }

        public void FixRunnerAtCurrentPosition(RunnerState ready)
        {
            _runnerNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Green;
        }

        public void PlaceRunnerField(SCNScene scene)
        {
            var field = new SCNPlane() { Width = 0.2f, Height = 0.4f };

            _fieldNode = new SCNNode();
            _fieldNode.Geometry = field;
            _fieldNode.Position = _runnerNode.Position;
            _fieldNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Orange;
            _fieldNode.EulerAngles = new SCNVector3(-1 * (float)Math.PI / 2, (float)Math.PI / 2, 0);

            scene.RootNode.AddChildNode(_fieldNode);
        }

        SCNNode f1Node;
        SCNNode f2Node;

        SCNVector3? runnerPositionOnDoubleTouch;
        SCNVector3? runnerFieldEulerAnglesOnDoubleTouch;
        SCNVector3? doubleTouchInitPosition;
        float? doubleTouchInitEulerAnglesY;
        public void InitRotateRunnerField(SCNScene scene, SCNVector3 coord1, SCNVector3 coord2)
        {
            var f1 = new SCNBox() { Width = 0.025f, Height = 0.025f, Length = 0.025f, ChamferRadius = 0.0f };

            f1Node = new SCNNode();
            f1Node.Geometry = f1;
            f1Node.Position = coord1;
            f1Node.Geometry.Materials.First().Diffuse.Contents = UIColor.Cyan;

            scene.RootNode.AddChildNode(f1Node); 

            var f2 = new SCNBox() { Width = 0.025f, Height = 0.025f, Length = 0.025f, ChamferRadius = 0.0f };

            f2Node = new SCNNode();
            f2Node.Geometry = f2;
            f2Node.Position = coord2;
            f2Node.Geometry.Materials.First().Diffuse.Contents = UIColor.Blue;

            scene.RootNode.AddChildNode(f2Node); 


            runnerPositionOnDoubleTouch = _runnerNode.Position;
            runnerFieldEulerAnglesOnDoubleTouch = _fieldNode.EulerAngles;

            //Debug.WriteLine("runnerPositionOnDoubleTouch: " + runnerPositionOnDoubleTouch.ToString());
            //Debug.WriteLine("runnerFieldEulerAnglesOnDoubleTouch: " + runnerFieldEulerAnglesOnDoubleTouch.ToString());

            doubleTouchInitPosition = (coord1 + coord2) / 2;
            var len = (coord2 - coord1).Length;
            var zcomp = (coord2 - coord1).Z;
            doubleTouchInitEulerAnglesY = (float)Math.Acos(zcomp / len);

            //Debug.WriteLine("doubleTouchInitPosition: " + doubleTouchInitPosition.ToString());
        }

        public void RotateRunnerField(SCNVector3 coord1, SCNVector3 coord2)
        {
            f1Node.Position = coord1;
            f2Node.Position = coord2;

            var doubleTouchNewPosition = (coord1 + coord2) / 2;
            var len = (coord2 - coord1).Length;
            var zcomp = (coord2 - coord1).Z;
            var doubleTouchNewEulerAnglesY = (float)Math.Acos(zcomp / len);

            //Debug.WriteLine("doubleTouchNewPosition: " + doubleTouchNewPosition.ToString());
            //Debug.WriteLine("RotateRunnerField: coord1:" + coord1.ToString() + ", coord2:" + coord2.ToString() + ", len:" + len + ", zcomp:" + zcomp + ", doubleTouchNewEulerAnglesY:" + doubleTouchNewEulerAnglesY);

            var runnerNewPosition = runnerPositionOnDoubleTouch + (doubleTouchNewPosition - doubleTouchInitPosition);
            var runnerFieldNewEulerAngles = runnerFieldEulerAnglesOnDoubleTouch + new SCNVector3(0, doubleTouchNewEulerAnglesY - doubleTouchInitEulerAnglesY.Value, 0);

            //Debug.WriteLine("runnerNewPosition: " + runnerNewPosition.ToString());
            //Debug.WriteLine("runnerFieldNewEulerAngles: " + runnerFieldNewEulerAngles.ToString());

            _runnerNode.Position = runnerNewPosition.Value;
            _fieldNode.Position = runnerNewPosition.Value;
            _fieldNode.EulerAngles = runnerFieldNewEulerAngles.Value;

        }
    }
}
