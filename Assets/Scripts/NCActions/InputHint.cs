using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions{

	[Category("Input")]
	public class InputHint : ConditionTask
	{
        public enum ActionType { Down, Up, Hold }

        [RequiredField]
        public BBParameter<string> buttonName = "Fire1";
        public BBParameter<string> hintText;
        public ActionType actionType;

        protected override string info
        {
            get { return (actionType == ActionType.Hold ? "Hold " : (actionType == ActionType.Down? "Down " : "Up")) + buttonName.ToString(); }
        }

        protected override bool OnCheck()
        {
            return actionType == ActionType.Hold ? InputHints.GetButtonHold(buttonName.value, hintText?.value) : (actionType == ActionType.Down ? InputHints.GetButtonDown(buttonName.value, hintText?.value) : InputHints.GetButtonUp(buttonName.value, hintText?.value));
        }
    }
}