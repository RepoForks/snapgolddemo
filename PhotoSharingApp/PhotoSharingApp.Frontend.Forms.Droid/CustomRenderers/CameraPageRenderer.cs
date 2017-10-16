using System;
using Android.App;
using Android.Graphics;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Android.Content;
using System.Linq;
using Android.Hardware;
using System.Threading.Tasks;
using PhotoSharingApp.Forms.Controls;
using PhotoSharingApp.Forms.Droid.CustomRenderers;

using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Util;
using System.Collections.Generic;
using Android.OS;
using Android.Runtime;
using Android.Media;
using static Android.Resource;
using Java.Nio;

// Copied mostly from: https://github.com/flusharcade/chapter8-camera/blob/master/Droid/Renderers/CameraView/CameraDroid.cs
using System.Runtime.Remoting.Messaging;
using Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CameraPage), typeof(CameraPageRenderer))]
namespace PhotoSharingApp.Forms.Droid.CustomRenderers
{
    public class CameraPageRenderer : PageRenderer, TextureView.ISurfaceTextureListener, ISensorEventListener
    {
        // Camera
        public CameraDevice CameraDevice;
        CameraManager cameraManager;
        CameraCaptureSession previewSession;
        CaptureRequest.Builder previewBuilder;
        string cameraId;
        int orientation = 90;

        // Orientation
        SensorManager sensorManager;
        Sensor orientationSensor;

        // Layout
        RelativeLayout mainLayout;
        TextureView liveTextureView;
        PaintCodeButton capturePhotoButton;
        Size previewSize;

        //Activity Activity => this.Context as FormsAppCompatActivity;

        public CameraPageRenderer()
        {
            // Instanciate SensorManager
            // But do not listen yet (Listening starts later, when controls get initilialized)
            sensorManager = (SensorManager)Application.Context.GetSystemService(Context.SensorService);
            orientationSensor = sensorManager.GetDefaultSensor(SensorType.Orientation);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
        {
            base.OnElementChanged(e);

            // Setup Interface
            mainLayout = new RelativeLayout(Context);
            liveTextureView = new TextureView(Context);
            liveTextureView.SurfaceTextureListener = this;
            liveTextureView.LayoutParameters = new RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent); ;
            mainLayout.AddView(liveTextureView);
            capturePhotoButton = new PaintCodeButton(Context);
            RelativeLayout.LayoutParams captureButtonParams = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            captureButtonParams.Height = 120;
            captureButtonParams.Width = 120;
            capturePhotoButton.LayoutParameters = captureButtonParams;
            capturePhotoButton.Click += (sender, f) => { TakePhoto(); };
            mainLayout.AddView(capturePhotoButton);
            AddView(mainLayout);

            // Initialize CameraManager
            cameraManager = (CameraManager)Context.GetSystemService(Context.CameraService);
            cameraId = cameraManager.GetCameraIdList().FirstOrDefault();
            if (cameraId != null)
            {
                var characteristics = cameraManager.GetCameraCharacteristics(cameraId);
                var map = (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                previewSize = map.GetOutputSizes(Java.Lang.Class.FromType(typeof(SurfaceTexture)))[0];
            }

            // Subscribe to Init and Dispose Events
            // These events get raised by the page when it is ready to display the stream or when
            // the stream should stop (for example when navigated away)
            (Element as Controls.CameraPage).OnInitialize += Handle_OnInitialize;
            (Element as Controls.CameraPage).OnDispose += Handle_OnDispose;
        }

        void Handle_OnInitialize(EventArgs args)
        {
            // Start listening for orientation changes
            sensorManager.RegisterListener(this, orientationSensor, SensorDelay.Normal);

            // Start camera stream
            if (cameraId != null)
            {
                var cameraStateListener = new CameraStateListener(this);
                cameraManager.OpenCamera(cameraId, cameraStateListener, null);
            }
        }

        void Handle_OnDispose(EventArgs args)
        {
            // Stop listening for orientation changes
            sensorManager.UnregisterListener(this);

            // Stop camera stram
            StopCamera();
        }

        private void StopCamera()
        {
            if (previewSession != null && previewSession.IsReprocessable)
                previewSession?.StopRepeating();

            CameraDevice.Close();
        }

        public void StartPreview()
        {
            if (liveTextureView.SurfaceTexture == null)
                return;

            var texture = liveTextureView.SurfaceTexture;
            texture.SetDefaultBufferSize(previewSize.Width, previewSize.Height);
            var surface = new Surface(texture);

            previewBuilder = CameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
            previewBuilder.AddTarget(surface);

            CameraDevice.CreateCaptureSession(new List<Surface> { surface }, new CameraCaptureStateListener
            {
                OnConfigureFailedAction = (CameraCaptureSession session) =>
                {
                },
                OnConfiguredAction = (CameraCaptureSession session) =>
                {
                    previewSession = session;
                    previewBuilder.Set(CaptureRequest.ControlMode, new Java.Lang.Integer((int)ControlMode.Auto));

                    var thread = new HandlerThread("CameraPicture");
                    thread.Start();
                    var backgroundHandler = new Handler(thread.Looper);
                    previewSession.SetRepeatingRequest(previewBuilder.Build(), null, backgroundHandler);
                }
            }, null);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (!changed)
                return;
            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);
            mainLayout.Measure(msw, msh);
            mainLayout.Layout(0, 0, r - l, b - t);

            capturePhotoButton.SetX(mainLayout.Width / 2 - 60);
            capturePhotoButton.SetY(mainLayout.Height - 200);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                (Element as Controls.CameraPage).Cancel();
                return false;
            }
            return base.OnKeyDown(keyCode, e);
        }

        public void TakePhoto()
        {
            var characteristics = cameraManager.GetCameraCharacteristics(CameraDevice.Id);
            var jpegSizes = ((StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap)).GetOutputSizes((int)ImageFormatType.Jpeg);
            var width = jpegSizes[0].Width;
            var height = jpegSizes[0].Height;

            var reader = ImageReader.NewInstance(width, height, ImageFormatType.Jpeg, 1);
            var outputSurfaces = new List<Surface> { reader.Surface };

            CaptureRequest.Builder captureBuilder = CameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
            captureBuilder.AddTarget(reader.Surface);
            captureBuilder.Set(CaptureRequest.ControlMode, (int)ControlMode.Auto);

            // Orientation
            var windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            SurfaceOrientation rotation = windowManager.DefaultDisplay.Rotation;
            captureBuilder.Set(CaptureRequest.JpegOrientation, orientation);

            var readerListener = new ImageAvailableListener();
            readerListener.Photo += (sender, e) =>
            {
                (Element as Controls.CameraPage).SetPhotoResult(e, width, height);
            };

            HandlerThread thread = new HandlerThread("CameraPicture");
            thread.Start();
            Handler backgroundHandler = new Handler(thread.Looper);
            reader.SetOnImageAvailableListener(readerListener, backgroundHandler);

            var captureListener = new CameraCaptureListener();

            captureListener.PhotoComplete += (sender, e) =>
            {
                StartPreview();
            };

            CameraDevice.CreateCaptureSession(outputSurfaces, new CameraCaptureStateListener()
            {
                OnConfiguredAction = (CameraCaptureSession session) =>
                {
                    try
                    {
                        previewSession = session;
                        session.Capture(captureBuilder.Build(), captureListener, backgroundHandler);
                    }
                    catch (CameraAccessException ex)
                    {
                        Log.WriteLine(LogPriority.Info, "Capture Session error: ", ex.ToString());
                    }
                }
            }, backgroundHandler);
        }

        #region TextureView.ISurfaceTextureListener implementations

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            var viewSurface = surface;


            var windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            var rotation = windowManager.DefaultDisplay.Rotation;
            var matrix = new Matrix();
            var viewRect = new RectF(0, 0, width, height);
            var bufferRect = new RectF(0, 0, previewSize.Width, previewSize.Height);

            var centerX = viewRect.CenterX();
            var centerY = viewRect.CenterY();

            if (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                matrix.PostRotate(90 * ((int)rotation - 2), centerX, centerY);
            }

            liveTextureView.SetTransform(matrix);

            //camera = Android.Hardware.Camera.Open();
            //var parameters = camera.GetParameters();
            //var aspect = ((decimal)height) / ((decimal)width);

            //// Find the preview aspect ratio that is closest to the surface aspect
            //var previewSize = parameters.SupportedPreviewSizes
            //                            .OrderBy(s => Math.Abs(s.Width / (decimal)s.Height - aspect))
            //                            .First();

            //System.Diagnostics.Debug.WriteLine($"Preview sizes: {parameters.SupportedPreviewSizes.Count}");

            //parameters.SetPreviewSize(previewSize.Width, previewSize.Height);
            //camera.SetParameters(parameters);

            //camera.SetPreviewTexture(surface);
            //StartCamera();
        }

        public bool OnSurfaceTextureDestroyed(Android.Graphics.SurfaceTexture surface)
        {
            StopCamera();
            return true;
        }

        public void OnSurfaceTextureSizeChanged(Android.Graphics.SurfaceTexture surface, int width, int height)
        {
        }

        public void OnSurfaceTextureUpdated(Android.Graphics.SurfaceTexture surface)
        {
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //throw new NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Orientation)
            {
                float xAxis = e.Values[1];
                float yAxis = e.Values[2];

                if ((yAxis <= 25) && (yAxis >= -25) && (xAxis >= -160))
                {
                    // CHANGED TO PORTRAIT
                    if (orientation != 90)
                        orientation = 90;

                }
                else if ((yAxis < -25) && (xAxis >= -20))
                {
                    // CHANGED TO LANDSCAPE RIGHT
                    if (orientation != 180)
                        orientation = 180;

                }
                else if ((yAxis > 25) && (xAxis >= -20))
                {
                    // CHANGED TO LANDSCAPE LEFT
                    if (orientation != 0)
                        orientation = 0;

                }
            }
        }
        #endregion
    }

    public class PaintCodeButton : Button
    {
        public PaintCodeButton(Context context) : base(context)
        {
            Background.Alpha = 0;
        }

        protected override void OnDraw(Canvas canvas)
        {
            var frame = new Rect(Left, Top, Right, Bottom);

            Paint paint;
            // Local Colors
            var color = Android.Graphics.Color.White;

            RectF bezierRect = new RectF(
                frame.Left + (float)Java.Lang.Math.Floor((frame.Width() - 120f) * 0.5f + 0.5f),
                frame.Top + (float)Java.Lang.Math.Floor((frame.Height() - 120f) * 0.5f + 0.5f),
                frame.Left + (float)Java.Lang.Math.Floor((frame.Width() - 120f) * 0.5f + 0.5f) + 120f,
                frame.Top + (float)Java.Lang.Math.Floor((frame.Height() - 120f) * 0.5f + 0.5f) + 120f);
            Path bezierPath = new Path();
            bezierPath.MoveTo(frame.Left + frame.Width() * 0.5f, frame.Top + frame.Height() * 0.08333f);
            bezierPath.CubicTo(frame.Left + frame.Width() * 0.41628f, frame.Top + frame.Height() * 0.08333f, frame.Left + frame.Width() * 0.33832f, frame.Top + frame.Height() * 0.10803f, frame.Left + frame.Width() * 0.27302f, frame.Top + frame.Height() * 0.15053f);
            bezierPath.CubicTo(frame.Left + frame.Width() * 0.15883f, frame.Top + frame.Height() * 0.22484f, frame.Left + frame.Width() * 0.08333f, frame.Top + frame.Height() * 0.3536f, frame.Left + frame.Width() * 0.08333f, frame.Top + frame.Height() * 0.5f);
            bezierPath.CubicTo(frame.Left + frame.Width() * 0.08333f, frame.Top + frame.Height() * 0.73012f, frame.Left + frame.Width() * 0.26988f, frame.Top + frame.Height() * 0.91667f, frame.Left + frame.Width() * 0.5f, frame.Top + frame.Height() * 0.91667f);
            bezierPath.CubicTo(frame.Left + frame.Width() * 0.73012f, frame.Top + frame.Height() * 0.91667f, frame.Left + frame.Width() * 0.91667f, frame.Top + frame.Height() * 0.73012f, frame.Left + frame.Width() * 0.91667f, frame.Top + frame.Height() * 0.5f);
            bezierPath.CubicTo(frame.Left + frame.Width() * 0.91667f, frame.Top + frame.Height() * 0.26988f, frame.Left + frame.Width() * 0.73012f, frame.Top + frame.Height() * 0.08333f, frame.Left + frame.Width() * 0.5f, frame.Top + frame.Height() * 0.08333f);
            bezierPath.Close();
            bezierPath.MoveTo(frame.Left + frame.Width(), frame.Top + frame.Height() * 0.5f);
            bezierPath.CubicTo(frame.Left + frame.Width(), frame.Top + frame.Height() * 0.77614f, frame.Left + frame.Width() * 0.77614f, frame.Top + frame.Height(), frame.Left + frame.Width() * 0.5f, frame.Top + frame.Height());
            bezierPath.CubicTo(frame.Left + frame.Width() * 0.22386f, frame.Top + frame.Height(), frame.Left, frame.Top + frame.Height() * 0.77614f, frame.Left, frame.Top + frame.Height() * 0.5f);
            bezierPath.CubicTo(frame.Left, frame.Top + frame.Height() * 0.33689f, frame.Left + frame.Width() * 0.0781f, frame.Top + frame.Height() * 0.19203f, frame.Left + frame.Width() * 0.19894f, frame.Top + frame.Height() * 0.10076f);
            bezierPath.CubicTo(frame.Left + frame.Width() * 0.28269f, frame.Top + frame.Height() * 0.03751f, frame.Left + frame.Width() * 0.38696f, frame.Top, frame.Left + frame.Width() * 0.5f, frame.Top);
            bezierPath.CubicTo(frame.Left + frame.Width() * 0.77614f, frame.Top, frame.Left + frame.Width(), frame.Top + frame.Height() * 0.22386f, frame.Left + frame.Width(), frame.Top + frame.Height() * 0.5f);
            bezierPath.Close();

            paint = new Paint();
            paint.SetStyle(Android.Graphics.Paint.Style.Fill);
            paint.Color = (color);
            canvas.DrawPath(bezierPath, paint);

            paint = new Paint();
            paint.StrokeWidth = (1f);
            paint.StrokeMiter = (10f);
            canvas.Save();
            paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            paint.Color = (Android.Graphics.Color.Black);
            canvas.DrawPath(bezierPath, paint);
            canvas.Restore();

            RectF ovalRect = new RectF(
                frame.Left + (float)Java.Lang.Math.Floor(frame.Width() * 0.12917f) + 0.5f,
                frame.Top + (float)Java.Lang.Math.Floor(frame.Height() * 0.12083f) + 0.5f,
                frame.Left + (float)Java.Lang.Math.Floor(frame.Width() * 0.87917f) + 0.5f,
                frame.Top + (float)Java.Lang.Math.Floor(frame.Height() * 0.87083f) + 0.5f);
            Path ovalPath = new Path();
            ovalPath.AddOval(ovalRect, Path.Direction.Cw);

            paint = new Paint();
            paint.SetStyle(Android.Graphics.Paint.Style.Fill);
            paint.Color = (color);
            canvas.DrawPath(ovalPath, paint);

            paint = new Paint();
            paint.StrokeWidth = (1f);
            paint.StrokeMiter = (10f);
            canvas.Save();
            paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            paint.Color = (Android.Graphics.Color.Black);
            canvas.DrawPath(ovalPath, paint);
            canvas.Restore();
        }
    }

    public class CameraStateListener : CameraDevice.StateCallback
    {
        CameraPageRenderer renderer;

        public CameraStateListener(CameraPageRenderer renderer)
        {
            this.renderer = renderer;
        }

        public override void OnOpened(CameraDevice camera)
        {
            if (renderer != null)
            {
                renderer.CameraDevice = camera;
                renderer.StartPreview();
            }
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            if (renderer != null)
            {
                camera.Close();
                renderer.CameraDevice = null;
            }
        }

        public override void OnError(CameraDevice camera, Android.Hardware.Camera2.CameraError error)
        {
            camera.Close();

            if (renderer != null)
            {
                renderer.CameraDevice = null;
            }
        }
    }

    public class CameraCaptureStateListener : CameraCaptureSession.StateCallback
    {
        /// <summary>
        /// The on configure failed action.
        /// </summary>
        public Action<CameraCaptureSession> OnConfigureFailedAction;

        /// <summary>
        /// The on configured action.
        /// </summary>
        public Action<CameraCaptureSession> OnConfiguredAction;

        /// <summary>
        /// Ons the configure failed.
        /// </summary>
        /// <param name="session">Session.</param>
        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            if (OnConfigureFailedAction != null)
            {
                OnConfigureFailedAction(session);
            }
        }

        /// <summary>
        /// Ons the configured.
        /// </summary>
        /// <param name="session">Session.</param>
        public override void OnConfigured(CameraCaptureSession session)
        {
            if (OnConfiguredAction != null)
            {
                OnConfiguredAction(session);
            }
        }
    }

    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        /// <summary>
        /// Occurs when photo.
        /// </summary>
        public event EventHandler<byte[]> Photo;

        public void OnImageAvailable(ImageReader reader)
        {
            Image image = null;

            try
            {
                image = reader.AcquireLatestImage();
                ByteBuffer buffer = image.GetPlanes()[0].Buffer;
                byte[] imageData = new byte[buffer.Capacity()];
                buffer.Get(imageData);

                Photo?.Invoke(this, imageData);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (image != null)
                {
                    image.Close();
                }
            }
        }
    }

    public class CameraCaptureListener : CameraCaptureSession.CaptureCallback
    {
        public event EventHandler PhotoComplete;

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            PhotoComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}