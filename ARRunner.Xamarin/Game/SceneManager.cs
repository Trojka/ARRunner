using System;
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
        public void InitRotateRunnerField(SCNVector3 coord1, SCNVector3 coord2)
        {
            var f1 = new SCNBox() { Width = 0.025f, Height = 0.025f, Length = 0.025f, ChamferRadius = 0.0f };

            f1Node = new SCNNode();
            f1Node.Geometry = f1;
            f1Node.Position = coord1;
            f1Node.Geometry.Materials.First().Diffuse.Contents = UIColor.Green;
        }

        public void RotateRunnerField(SCNVector3 coord1, SCNVector3 coord2)
        {
            f1Node.Position = coord1;
        }
    }
}
