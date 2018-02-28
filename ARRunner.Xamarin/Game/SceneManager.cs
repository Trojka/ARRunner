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

        SCNNode _cursorNode = null;
        SCNNode _fieldNode = null;

        public void PlaceRunnerInSceneAtPosition(SCNScene scene, SCNVector3 position, RunnerState state)
        {
            if (_cursorNode == null)
            {
                var box = new SCNBox() { Width = 0.025f, Height = 0.025f, Length = 0.025f, ChamferRadius = 0.0f };

                _cursorNode = new SCNNode();
                _cursorNode.Geometry = box;
                _cursorNode.Position = position;

                scene.RootNode.AddChildNode(_cursorNode);
            }

            _cursorNode.Position = position;
            _cursorNode.Geometry.Materials.First().Diffuse.Contents = state == RunnerState.Fixed ? UIColor.Red : UIColor.Orange;
        }

        public void FixRunnerAtCurrentPosition(RunnerState ready)
        {
            _cursorNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Green;
        }

        public void PlaceRunnerField(SCNScene scene)
        {
            var field = new SCNPlane() { Width = 0.2f, Height = 0.4f };

            _fieldNode = new SCNNode();
            _fieldNode.Geometry = field;
            _fieldNode.Position = _cursorNode.Position;
            _fieldNode.Geometry.Materials.First().Diffuse.Contents = UIColor.Orange;
            _fieldNode.EulerAngles = new SCNVector3(-1 * (float)Math.PI / 2, (float)Math.PI / 2, 0);

            scene.RootNode.AddChildNode(_fieldNode);
        }

        public void RotateRunnerField(CGPoint coord1, CGPoint coord2)
        {
            ;
        }
    }
}
