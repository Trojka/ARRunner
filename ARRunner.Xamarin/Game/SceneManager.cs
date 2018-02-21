using System;
using System.Linq;
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
    }
}
