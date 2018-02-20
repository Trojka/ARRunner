using System;
using ARKit;
using ARRunner.Xamarin.SpatialMapping;
using CoreGraphics;

namespace ARRunner.Xamarin.Game
{
    public class ARGamePlay
    {
        public ARSession Session { get; set; } = new ARSession();

        public ARKit.ARSCNView SceneView { get; set; }

        SceneManager sceneManager = new SceneManager();

        static private ARFrame currentFrame;
        static public ARFrame CurrentFrame
        {
            get => currentFrame;

            private set
            {
                if (currentFrame != null)
                {
                    currentFrame.Dispose();
                }
                currentFrame = value;
            }
        }

        public void Update()
        {
            if (Session?.CurrentFrame == null)
            {
                return;
            }
            // Vital for memory: Single location to set current frame! (Note: Assignment disposes existing frame -- see `set`
            ARGamePlay.CurrentFrame = Session.CurrentFrame;


            var screenRect = SceneView.Bounds;
            var screenCenter = new CGPoint(screenRect.GetMidX(), screenRect.GetMidY());

            var worldPos = PlaneFinding.FindNearestWorldPointToScreenPoint(screenCenter, SceneView, null);

            if (worldPos.hitType == PlaneFinding.HitType.None)
                return;

            sceneManager.PlaceRunnerInSceneAtPosition(SceneView.Scene, worldPos.hitPoint.Value, worldPos.hitType == PlaneFinding.HitType.Plane ? SceneManager.RunnerState.Preparing : SceneManager.RunnerState.Fixed);
        }
    }
}
