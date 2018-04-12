using System;
using System.Linq;
using CoreGraphics;
using SceneKit;
using UIKit;

namespace aRCCar.Xamarin.Game
{
    public class TrackNode : SCNNode
    {
        const float LINE_RADIUS = 0.0005f;

        static float _fieldWidth = 0.1f;
        static float _fieldLength = 0.4f;

        static float _entityInitialPosOnField = 0.05f;

        readonly UIColor TRACK_COLOR = UIColor.Orange;

        readonly UIColor TRANSPARENTTRACK_COLOR = UIColor.Blue;

        SCNNode _trackNode;

        public TrackNode()
        {
            Width = _fieldWidth;
            Length = _fieldLength;

            EntityStartPositionOffet = _fieldLength / 2 - _entityInitialPosOnField;

            _trackNode = CreateTransparentTrackNode();
            this.AddChildNode(_trackNode);
        }

        public void ShowFullTrack()
        {
            _trackNode.RemoveFromParentNode();
            _trackNode = CreateTrackNode();
            this.AddChildNode(_trackNode);
        }

        private SCNNode CreateTrackNode()
        {
            var geometry = new SCNPlane() { Width = this.Width, Height = this.Length };

            var trackNode = new SCNNode();
            trackNode.Geometry = geometry;
            trackNode.Geometry.Materials.First().Diffuse.Contents = TRACK_COLOR;
            trackNode.EulerAngles = new SCNVector3(-1 * (float)Math.PI / 2, (float)Math.PI / 2, 0);
            trackNode.Position = new SCNVector3(0f, 0f, 0f);

            return trackNode;
        }

        private SCNNode CreateTransparentTrackNode()
        {
            var transparentTrackNode = new SCNNode();

            var geometryLength = new SCNCylinder() { Height = _fieldLength, Radius = LINE_RADIUS };
            var geometryWidth = new SCNCylinder() { Height = _fieldWidth, Radius = LINE_RADIUS };

            var lengthNode1 = new SCNNode();
            lengthNode1.Geometry = geometryLength;
            lengthNode1.Geometry.Materials.First().Diffuse.Contents = TRANSPARENTTRACK_COLOR;
            lengthNode1.EulerAngles = new SCNVector3(-1 * (float)Math.PI / 2, (float)Math.PI / 2, 0);
            lengthNode1.Position = new SCNVector3(0f, 0f, _fieldWidth / 2);

            transparentTrackNode.AddChildNode(lengthNode1);

            var lengthNode2 = new SCNNode();
            lengthNode2.Geometry = geometryLength;
            lengthNode2.Geometry.Materials.First().Diffuse.Contents = TRANSPARENTTRACK_COLOR;
            lengthNode2.EulerAngles = new SCNVector3(-1 * (float)Math.PI / 2, (float)Math.PI / 2, 0);
            lengthNode2.Position = new SCNVector3(0f, 0f, -1f * _fieldWidth / 2);

            transparentTrackNode.AddChildNode(lengthNode2);

            var widthNode1 = new SCNNode();
            widthNode1.Geometry = geometryWidth;
            widthNode1.Geometry.Materials.First().Diffuse.Contents = TRANSPARENTTRACK_COLOR;
            widthNode1.EulerAngles = new SCNVector3(-1 * (float)Math.PI / 2, 0, 0);
            widthNode1.Position = new SCNVector3(_fieldLength / 2, 0f, 0);

            transparentTrackNode.AddChildNode(widthNode1);

            var widthNode2 = new SCNNode();
            widthNode2.Geometry = geometryWidth;
            widthNode2.Geometry.Materials.First().Diffuse.Contents = TRANSPARENTTRACK_COLOR;
            widthNode2.EulerAngles = new SCNVector3(-1 * (float)Math.PI / 2, 0, 0);
            widthNode2.Position = new SCNVector3(-1 * _fieldLength / 2, 0f, 0);

            transparentTrackNode.AddChildNode(widthNode2);

            return transparentTrackNode;
        }

        public float Width {
            get;
            private set;
        }

        public float Length
        {
            get;
            private set;
        }

        public float EntityStartPositionOffet {
            get;
            private set;
        }
    }
}
