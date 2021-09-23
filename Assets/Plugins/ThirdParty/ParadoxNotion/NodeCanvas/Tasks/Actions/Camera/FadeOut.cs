using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions
{

    [Category("Camera")]
    public class FadeOut : ActionTask
    {

        public float fadeTime = 1f;

        public override void OnExecute() {
            CameraFader.current.FadeOut(fadeTime);
        }

        public override void OnUpdate() {
            if ( elapsedTime >= fadeTime )
                EndAction();
        }
    }
}