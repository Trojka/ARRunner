// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace aRCCar.Xamarin
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        ARKit.ARSCNView sceneView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (sceneView != null) {
                sceneView.Dispose ();
                sceneView = null;
            }
        }
    }
}