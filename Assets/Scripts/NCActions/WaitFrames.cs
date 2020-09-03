using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Utility")]
    public class WaitFrames : ActionTask
    {

        public BBParameter<int> waitFrames = 1;
        public CompactStatus finishStatus = CompactStatus.Success;

        private int passedFrames;

        protected override string info
        {
            get { return string.Format("Wait {0} frames.", waitFrames); }
        }

        protected override void OnUpdate()
        {
            passedFrames++;
            if (passedFrames >= waitFrames.value)
            {
                EndAction(finishStatus == CompactStatus.Success ? true : false);
            }
        }
    }
}