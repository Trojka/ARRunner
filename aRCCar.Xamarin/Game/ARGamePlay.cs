using System;
using System.Diagnostics;
using ARKit;
using aRCCar.Xamarin.Extensions;
using CoreGraphics;
using SceneKit;
using aRCCar.Xamarin.SpatialMapping;

namespace aRCCar.Xamarin.Game
{
    public class ARGamePlay
    {
        public enum GameState
        {
            None,
            PrepareToScan,
            Scanning,
            Placement,
            StartPositioning,
            Positioning,
            StartCountDown,
            CountDown,
            Start,
            GameOver
        }

        int CountDown = 5;
        DateTime _timeToStart = DateTime.MaxValue;
        int _countDownTime = int.MaxValue;
        int EndCountDownAt = 3;

        SceneManager _sceneManager = new SceneManager();
        EntityPhysics _physics = new EntityPhysics();

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

        public GameState State { get; set; } = GameState.PrepareToScan;

        public ARSession Session { get; set; } = new ARSession();

        public ARKit.ARSCNView SceneView { get; set; }

        public OverlayScene OverlayScene { get; set; }

        public void Update()
        {
            if(State == GameState.PrepareToScan)
            {
                OverlayScene.ShowScanAction();
                State = GameState.Scanning;
            }
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


                //Debug.WriteLine("GameState.Scanning > worldPos.hitType=" + worldPos.hitType.ToString());
                if(worldPos.hitType == PlaneFinding.HitType.Plane && !OverlayScene.ScanActionFinished)
                {
                    //Debug.WriteLine("GameState.Scanning > FinishScanAction");
                    OverlayScene.FinishScanAction();
                    OverlayScene.ShowActionPlacement();
                }
                else if (worldPos.hitType != PlaneFinding.HitType.Plane)
                {
                    if(!OverlayScene.ScanActionShowing)
                    {
                        //Debug.WriteLine("GameState.Scanning > ShowScanAction");
                        OverlayScene.FinishActionPlacement();
                        OverlayScene.ShowScanAction();
                    }

                    if (worldPos.hitType == PlaneFinding.HitType.None)
                        return;
                }

                _sceneManager.PlaceEntityInSceneAtPosition(SceneView.Scene, worldPos.hitPoint.Value, worldPos.hitType == PlaneFinding.HitType.Plane ? SceneManager.EntityState.Preparing : SceneManager.EntityState.Fixed);
            }
            if(State == GameState.Placement)
            {
                _sceneManager.FixEntityAtCurrentPosition(SceneManager.EntityState.Ready);
                _sceneManager.PlaceEntityField(SceneView.Scene);

                _sceneManager.PlacePlacementNode(SceneView.Scene);

                OverlayScene.FinishActionPlacement();
                State = GameState.StartPositioning;
            }
            if(State == GameState.StartPositioning && twoFingerTouchPoint1.HasValue && twoFingerTouchPoint2.HasValue)
            {
                var scenePoint1 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint1.Value, _sceneManager.EntityFieldPosition.Value);
                var scenePoint2 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint2.Value, _sceneManager.EntityFieldPosition.Value);

                if(scenePoint1.HasValue && scenePoint2.HasValue)
                {
                    _sceneManager.InitRotateEntityField(SceneView.Scene, scenePoint1.Value, scenePoint2.Value);

                    State = GameState.Positioning;
                }
            }
            if(State == GameState.Positioning && twoFingerTouchPoint1.HasValue && twoFingerTouchPoint2.HasValue)
            {
                var scenePoint1 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint1.Value, _sceneManager.EntityFieldPosition.Value);
                var scenePoint2 = SceneView.HitTestWithInfiniteHorizontalPlane(twoFingerTouchPoint2.Value, _sceneManager.EntityFieldPosition.Value);

                if(scenePoint1.HasValue && scenePoint2.HasValue)
                {
                    //Debug.WriteLine("scenePoint1: " + scenePoint1.ToString());
                    //Debug.WriteLine("scenePoint2: " + scenePoint2.ToString());
                    //Debug.WriteLine("scenePoint1: " + scenePoint1.ToString() + ", scenePoint2: " + scenePoint2.ToString() + " - X: " + diff.X + ", Z: " + diff.Z + ", angle: " + (angle * 180 / Math.PI));
                    _sceneManager.RotateEntityFieldMarker(scenePoint1.Value, scenePoint2.Value);
                }
            }
            if(State == GameState.StartCountDown)
            {
                _timeToStart = DateTime.Now;
                _countDownTime = 1;
                _sceneManager.StartCountDown();
                OverlayScene.UserInteractionEnabled = true;
                OverlayScene.StartCountDown();
                OverlayScene.ShowActionControls();

                State = GameState.CountDown;
            }
            if(State == GameState.CountDown)
            {
                var elapsedTime = (DateTime.Now - _timeToStart).TotalSeconds;
                //if (elapsedTime >= 1 && elapsedTime < 2)
                //    OverlayScene.ShowCountDown(4);
                //if (elapsedTime >= 2 && elapsedTime < 3)
                //    OverlayScene.ShowCountDown(3);
                //if (elapsedTime >= 3 && elapsedTime < 4)
                //    OverlayScene.ShowCountDown(2);
                //if (elapsedTime >= 4 && elapsedTime < 5)
                    //OverlayScene.ShowCountDown(1);

                if(elapsedTime >= _countDownTime && elapsedTime < _countDownTime+1)
                {
                    OverlayScene.ShowCountDown(CountDown - _countDownTime);
                    _countDownTime++;
                }
                
                if((DateTime.Now - _timeToStart).TotalSeconds >= CountDown)
                {
                    OverlayScene.ShowCountDown(0);
                    _sceneManager.CanStartRun();
                    State = GameState.Start;
                }
            }
            if(State == GameState.Start)
            {
                var elapsedTime = (DateTime.Now - _timeToStart).TotalSeconds;
                if (elapsedTime > CountDown + EndCountDownAt)
                    OverlayScene.EndCountDown();

                var d = (float)_physics.DistanceTravelled(DateTime.Now);
                _sceneManager.MoveDistance(d);
            }
        }

        public void GestureManager_SingleFingerTouchEvent(object sender, Util.EventArgs<Game.SingleFingerTouch> e)
        {
            //Debug.WriteLine("GestureManager_SingleFingerTouchEvent: State: " + State + ", EventState: " + e.Value.State);
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

        enum Actuator
        {
            Left,
            Right
        }

        Actuator _lastFoot = Actuator.Left;
        public void LeftActuator()
        {
            if(State == GameState.CountDown)
            {
                _sceneManager.FalseStart();
                State = GameState.GameOver;
            }

            if(State == GameState.Start)
            {
                if (_lastFoot == Actuator.Left)
                {
                    _sceneManager.Stumble();
                    _physics.Stumble();
                }
                else
                {
                    _physics.ApplyForce();
                }

                _lastFoot = Actuator.Left;
            }
        }

        public void RightActuator()
        {
            if (State == GameState.CountDown)
            {
                _sceneManager.FalseStart();
                State = GameState.GameOver;
            }

            if (State == GameState.Start)
            {
                if (_lastFoot == Actuator.Right)
                {
                    _sceneManager.Stumble();
                    _physics.Stumble();
                }
                else
                {
                    _physics.ApplyForce();
                }

                _lastFoot = Actuator.Right;
            }
        }
    }
}
