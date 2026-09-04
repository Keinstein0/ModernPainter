using Android.App;
using Android.Content;
using Android.Opengl;
using Android.OS;
using Javax.Microedition.Khronos.Egl;
using Javax.Microedition.Khronos.Opengles;

namespace ModernPainter.Mobile
{
    [Activity(Label = "ModernPainter", MainLauncher = true, Exported = true)]
    public class MainActivity : Activity
    {
        private NativeGLView? _glView;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Initialize custom GLSurfaceView
            _glView = new NativeGLView(this);
            SetContentView(_glView);
        }

        protected override void OnResume()
        {
            base.OnResume();
            _glView?.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _glView?.OnPause();
        }
    }

    public class NativeGLView : GLSurfaceView, GLSurfaceView.IRenderer
    {
        public NativeGLView(Context context) : base(context)
        {
            // Request an OpenGL ES 3.0 context
            SetEGLContextClientVersion(3);
            SetRenderer(this);

            // Render continuously to handle redraws
            RenderMode = Rendermode.Continuously;
        }

        public void OnSurfaceCreated(IGL10? gl, Javax.Microedition.Khronos.Egl.EGLConfig? config)
        {
            // Set background color to Cornflower Blue (RGBA: 0.392, 0.584, 0.929, 1.0)
            GLES30.GlClearColor(0.392f, 0.584f, 0.929f, 1.0f);
        }

        public void OnSurfaceChanged(IGL10? gl, int width, int height)
        {
            // Adjust the viewport based on geometry changes
            GLES30.GlViewport(0, 0, width, height);
        }

        public void OnDrawFrame(IGL10? gl)
        {
            // Clear the color buffer
            GLES30.GlClear(GLES30.GlColorBufferBit);
        }
    }
}