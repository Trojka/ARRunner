using System;
using System.Linq;
using ARKit;
using Foundation;
using SceneKit;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using ARRunner.Xamarin.SpatialMapping;
using ARRunner.Xamarin.Game;

namespace ARRunner.Xamarin
{
    public partial class ViewController : IARSCNViewDelegate
    {
        //SCNNode _cursorNode = null;
        //SceneManager sceneManager = new SceneManager();

        [Export("renderer:updateAtTime:")]
        public void RendererUpdateAtTime(SCNSceneRenderer renderer, double updateAtTime)
        {
            gamePlay.Update();
        }

        [Export("renderer:didAddNode:forAnchor:")]
        public void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
        }

        [Export("renderer:didUpdateNode:forAnchor:")]
        public void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
        }
    }
}
