using System;
using AVFoundation;
using CoreGraphics;
using Foundation;
using PhotoSharingApp.Forms.Controls;
using PhotoSharingApp.Forms.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CameraViewFinder), typeof(CameraViewFinderRenderer))]
namespace PhotoSharingApp.Forms.iOS.CustomRenderers
{
    public class CameraViewFinderRenderer : ViewRenderer<CameraViewFinder, UIView>
    {
        AVCaptureSession captureSession;
        AVCaptureDeviceInput captureDeviceInput;
        AVCaptureStillImageOutput stillImageOutput;

        UIView viewFinder;

        void NewElement_InitializeCalled()
        {
            captureSession = new AVCaptureSession();

            var view = (View)Element;

            viewFinder = new UIView() { Frame = new CGRect(0f, 0f, view.Height, view.Width), BackgroundColor = UIColor.Green };
            SetNativeControl(viewFinder);

            var viewLayer = viewFinder.Layer;
            var videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                VideoGravity = AVLayerVideoGravity.ResizeAspect,
                Frame = new CGRect(0f, 0f, view.Height, view.Width)
            };

            viewFinder.Layer.AddSublayer(videoPreviewLayer);

            // SetupLiveCameraStream();


            var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
            ConfigureCameraForDevice(captureDevice);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);

            var dictionary = new NSMutableDictionary();
            dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
            stillImageOutput = new AVCaptureStillImageOutput() { OutputSettings = new NSDictionary() };

            captureSession.AddOutput(stillImageOutput);
            captureSession.AddInput(captureDeviceInput);
            captureSession.StartRunning();


            AuthorizeCameraUse();
        }


        protected override void OnElementChanged(ElementChangedEventArgs<CameraViewFinder> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            e.NewElement.InitializeCalled += NewElement_InitializeCalled;

            try
            {


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"          ERROR: ", ex.Message);
            }

        }


        void ConfigureCameraForDevice(AVCaptureDevice device)
        {
            var error = new NSError();
            if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration();
            }
            else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                device.LockForConfiguration(out error);
                device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                device.UnlockForConfiguration();
            }
            else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
            {
                device.LockForConfiguration(out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                device.UnlockForConfiguration();
            }
        }

        async void AuthorizeCameraUse()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
            if (authorizationStatus != AVAuthorizationStatus.Authorized)
            {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
            }
        }
    }
}

