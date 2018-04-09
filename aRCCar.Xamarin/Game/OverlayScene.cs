using System;
using CoreGraphics;
using SpriteKit;
using UIKit;

namespace aRCCar.Xamarin.Game
{
    public class OverlayScene : SKScene
    {
        public OverlayScene(CGSize size) : base(size)
        {
            BackgroundColor = UIColor.Clear;

            AddChild(new SKLabelNode("Chalkduster")
            {
                Text = "Hello World",
                FontSize = 30,
                Position = new CGPoint(Frame.GetMidX(), Frame.GetMidY()),
            });

        }
    }
}
