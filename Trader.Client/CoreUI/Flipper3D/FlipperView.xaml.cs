using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Trader.Client.CoreUI.Flipper3D
{
    public partial class FlipperView : UserControl
    {
        public FlipperView()
        {
            InitializeComponent();
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            // setup trackball for moving the model around
            _trackball = new Trackball();
            _trackball.Attach(this);
            _trackball.Servants.Add(myViewport3D);
            _trackball.Enabled = true;

            // Get the mesh objects for changing the material
            var mv3D = myViewport3D.Children[0] as ModelVisual3D;
            var m3DgBase = mv3D.Content as Model3DGroup;

            var m3Dg = m3DgBase.Children[0] as Model3DGroup;
            _topPlane = m3Dg.Children[2] as GeometryModel3D;
            _bottomPlane = m3Dg.Children[3] as GeometryModel3D;

            m3Dg = m3DgBase.Children[1] as Model3DGroup;
            _frontSpinPlane = m3Dg.Children[0] as GeometryModel3D;
            _backSpinPlane = m3Dg.Children[1] as GeometryModel3D;

            AnimateToNextPicture();
        }

        #region Events

        private void OnToggleAutoRun(object sender, RoutedEventArgs e)
        {
            if (_autorun)
            {
                _autorun = false;
            }
            else
            {
                _autorun = true;
                AnimateToNextPicture();
            }
        }

        private void OnSingleStep(object sender, RoutedEventArgs e)
        {
            AnimateToNextPicture();
        }

        #endregion

        #region Private Methods

        private void AnimateToNextPicture()
        {
            int nextPic = _currentPic + 1;

            if (nextPic > MaxPics)
                nextPic = 1;

            DiffuseMaterial dmA = FindResource("Pic01" + _currentPic) as DiffuseMaterial;
            DiffuseMaterial dmB = FindResource("Pic01" + nextPic) as DiffuseMaterial;

            if ((dmA == null) || (dmB == null))
                return;

            _bottomPlane.Material = dmA;
            _frontSpinPlane.Material = dmA;
            _topPlane.Material = dmB;
            _backSpinPlane.Material = dmB;

            _currentPic++;
            if (_currentPic > MaxPics)
                _currentPic = 1;

            Storyboard storyboard = (Storyboard)FindResource("FlipPicTimeline");
            BeginStoryboard(storyboard);
        }

        private void OnFlipPicTimeline(object sender, EventArgs e)
        {
            var clock = (System.Windows.Media.Animation.Clock)sender;

            if (clock.CurrentState == ClockState.Active) // Begun case
            {
                return;
            }

            if (clock.CurrentState != ClockState.Active) // Ended case
            {
                if (_autorun)
                {
                    AnimateToNextPicture();
                }
            }
        }

        #endregion

        #region Globals

        // Geometry models
        private GeometryModel3D _topPlane;
        private GeometryModel3D _bottomPlane;
        private GeometryModel3D _frontSpinPlane;
        private GeometryModel3D _backSpinPlane;

        private Trackball _trackball;
        private int _currentPic = 1;
        private const int MaxPics = 6;

        private bool _autorun;

        #endregion
    }

}
