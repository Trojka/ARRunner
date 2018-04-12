using System;
using System.Linq;
using SceneKit;
using UIKit;

namespace aRCCar.Xamarin.Game
{
    public class PlacementNode : SCNNode
    {
        const float INNER_RADIUS = 0.06f;
        const float RING_THICKNESS = 0.01f;
        const float RING_HEIGHT = 0.001f;

        public PlacementNode()
        {
            var geometry = new SCNTube() { InnerRadius = INNER_RADIUS, OuterRadius = INNER_RADIUS + RING_THICKNESS, Height = RING_HEIGHT };

            this.Geometry = geometry;
            this.Geometry.Materials.First().Diffuse.Contents = UIColor.LightGray;
        }
    }
}
