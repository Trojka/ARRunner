using System;
using System.Diagnostics;
using ARKit;
using ARRunner.Xamarin.Extensions;
using ARRunner.Xamarin.SpatialMapping;
using CoreGraphics;
using SceneKit;

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
            Positioning,
            StartCountDown,
            CountDown,
            Start,
            GameOver
        }

        TimeSpan CountDown = new TimeSpan(0, 0, 5);
        DateTime _timeToStart = DateTime.MaxValue;

        SceneManager _sceneManager = new SceneManager();
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

                _sceneManager.PlaceRunnerInSceneAtPosition(SceneView.Scene, worldPos.hitPoint.Value, worldPos.hitType == PlaneFinding.HitType.Plane ? SceneManager.RunnerState.Preparing : SceneManager.RunnerState.Fixed);
            }
            if(State == GameState.Placement)
            {
                _sceneManager.FixRunnerAtCurrentPosition(SceneManager.RunnerState.Ready);
                _sceneManager.PlaceRunnerField(SceneView.Scene);
                State = GameState.StartPositioning;
            }
            if(State == GameState.StartPositioning && twoFingerTouchPoint1.HasValue && twoFingerTouchPoint2.HasValue)
            {
                var scenePoint1 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint1.Value, _sceneManager.RunnerFieldPosition.Value);
                var scenePoint2 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint2.Value, _sceneManager.RunnerFieldPosition.Value);

                if(scenePoint1.HasValue && scenePoint2.HasValue)
                {
                    _sceneManager.InitRotateRunnerField(SceneView.Scene, scenePoint1.Value, scenePoint2.Value);

                    State = GameState.Positioning;
                }
            }
            if(State == GameState.Positioning && twoFingerTouchPoint1.HasValue && twoFingerTouchPoint2.HasValue)
            {
                var scenePoint1 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint1.Value, _sceneManager.RunnerFieldPosition.Value);
                var scenePoint2 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint2.Value, _sceneManager.RunnerFieldPosition.Value);

                if(scenePoint1.HasValue && scenePoint2.HasValue)
                {
                    //Debug.WriteLine("scenePoint1: " + scenePoint1.ToString());
                    //Debug.WriteLine("scenePoint2: " + scenePoint2.ToString());
                    //Debug.WriteLine("scenePoint1: " + scenePoint1.ToString() + ", scenePoint2: " + scenePoint2.ToString() + " - X: " + diff.X + ", Z: " + diff.Z + ", angle: " + (angle * 180 / Math.PI));
                    _sceneManager.RotateRunnerField(scenePoint1.Value, scenePoint2.Value);
                }
            }
            if(State == GameState.StartCountDown)
            {
                _timeToStart = DateTime.Now;
                _sceneManager.StartCountDown();

                State = GameState.CountDown;
            }
            if(State == GameState.CountDown)
            {
                if((DateTime.Now - _timeToStart) > CountDown)
                {
                    _sceneManager.CanStartRun();
                    State = GameState.Start;
                }
            }
            if(State == GameState.Start)
            {
                _sceneManager.Run();
            }
        }

        public void GestureManager_SingleFingerTouchEvent(object sender, Util.EventArgs<Game.SingleFingerTouch> e)
        {
            Debug.WriteLine("GestureManager_SingleFingerTouchEvent: State: " + State + ", EventState: " + e.Value.State);
            if(State == GameState.Scanning && e.Value.State == GestureState.End)
            {
                State = GameState.Placement;
            }
            if (State == GameState.StartPositioning && e.Value.State == GestureState.End)
            {
                State = GameState.StartCountDown;
            }
        }

        CGPoint? twoFingerTouchPoint1;
        CGPoint? twoFingerTouchPoint2;

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

        enum Foot
        {
            Left,
            Right
        }

        Foot _lastFoot = Foot.Left;
        public void LeftFoot()
        {
            if(State == GameState.CountDown)
            {
                _sceneManager.FalseStart();
                State = GameState.GameOver;
            }

            if(State == GameState.Start)
            {
                if (_lastFoot == Foot.Left)
                    _sceneManager.Stumble();
                else
                    _sceneManager.RunnerStep();

                _lastFoot = Foot.Left;
            }
        }

        public void RightFoot()
        {
            if (State == GameState.CountDown)
            {
                _sceneManager.FalseStart();
                State = GameState.GameOver;
            }

            if (State == GameState.Start)
            {
                if (_lastFoot == Foot.Right)
                    _sceneManager.Stumble();
                else
                    _sceneManager.RunnerStep();

                _lastFoot = Foot.Right;
            }
        }
    }
}
