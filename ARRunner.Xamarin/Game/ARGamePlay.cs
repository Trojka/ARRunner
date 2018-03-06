using System;
using System.Diagnostics;
using ARKit;
using ARRunner.Xamarin.Extensions;
using ARRunner.Xamarin.SpatialMapping;
using CoreGraphics;

namespace ARRunner.Xamarin.Game
{
    public class ARGamePlay
    {
        public enum GameState
        {
            None,
            Scanning,
            Placement,
            StartPositioning,
            Positioning
        }

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

        public GameState State { get; set; } = GameState.Scanning;

        public ARSession Session { get; set; } = new ARSession();

        public ARKit.ARSCNView SceneView { get; set; }

        public void Update()
        {
            if(State == GameState.Scanning)
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
            if(State == GameState.Placement)
            {
                sceneManager.FixRunnerAtCurrentPosition(SceneManager.RunnerState.Ready);
                sceneManager.PlaceRunnerField(SceneView.Scene);
                State = GameState.StartPositioning;
            }
            if(State == GameState.StartPositioning && twoFingerTouchPoint1.HasValue && twoFingerTouchPoint2.HasValue)
            {
                var scenePoint1 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint1.Value, sceneManager.RunnerFieldPosition.Value);
                var scenePoint2 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint2.Value, sceneManager.RunnerFieldPosition.Value);

                if(scenePoint1.HasValue && scenePoint2.HasValue)
                {
                    sceneManager.InitRotateRunnerField(SceneView.Scene, scenePoint1.Value, scenePoint2.Value);

                    State = GameState.Positioning;
                }
            }
            if(State == GameState.Positioning && twoFingerTouchPoint1.HasValue && twoFingerTouchPoint2.HasValue)
            {
                var scenePoint1 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint1.Value, sceneManager.RunnerFieldPosition.Value);
                var scenePoint2 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint2.Value, sceneManager.RunnerFieldPosition.Value);

                if(scenePoint1.HasValue && scenePoint2.HasValue)
                {
                    //var diff = scenePoint2.Value - scenePoint1.Value;
                    //Debug.WriteLine("scenePoint1: " + scenePoint1.ToString());
                    //Debug.WriteLine("scenePoint2: " + scenePoint2.ToString());
                    //var angle = Math.Acos(diff.X / diff.Z);
                    //Debug.WriteLine("scenePoint1: " + scenePoint1.ToString() + ", scenePoint2: " + scenePoint2.ToString() + " - X: " + diff.X + ", Z: " + diff.Z + ", angle: " + (angle * 180 / Math.PI));
                    sceneManager.RotateRunnerField(scenePoint1.Value, scenePoint2.Value);
                }
            }
        }

        public void GestureManager_SingleFingerTouchEvent(object sender, Util.EventArgs<Game.SingleFingerTouch> e)
        {
            //Debug.WriteLine("GestureManager_SingleFingerTouchEvent: " + e.Value.State);
            if(State == GameState.Scanning && e.Value.State == GestureState.End)
            {
                State = GameState.Placement;
            }
        }

        CGPoint? twoFingerTouchPoint1;
        CGPoint? twoFingerTouchPoint2;

        private int count = 0;
        public void GestureManager_TwoFingerTouchEvent(object sender, Util.EventArgs<Game.TwoFingerTouch> e)
        {
            //Debug.WriteLine("GestureManager_TwoFingerTouchEvent: " + e.Value.State);
            if(State == GameState.StartPositioning && e.Value.State == GestureState.Start)
            {
                twoFingerTouchPoint1 = e.Value.Coord1;
                twoFingerTouchPoint2 = e.Value.Coord2;
            }

            if (State == GameState.Positioning && e.Value.State == GestureState.Change)
            {
                //count++;
                //if(count == 100)
                //{
                //    Debug.WriteLine("twoFingerTouchPoint1: " + twoFingerTouchPoint1.ToString() + ", twoFingerTouchPoint2: " + twoFingerTouchPoint2.ToString());
                //    count = 0;
                //}

                twoFingerTouchPoint1 = e.Value.Coord1;
                twoFingerTouchPoint2 = e.Value.Coord2;
            }

            if (State == GameState.Positioning && e.Value.State == GestureState.End)
            {
                twoFingerTouchPoint1 = null;
                twoFingerTouchPoint2 = null;

                State = GameState.StartPositioning;
            }

        }
    }
}
