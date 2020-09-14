using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions{

	[Category("Input")]
	public class InputHint : ConditionTask
	{
        [RequiredField]
        public BBParameter<string> buttonName = "Fire1";
        public BBParameter<string> hintText;

        protected override string info
        {
            get { return "Down " + buttonName.ToString(); }
        }

        protected override bool OnCheck()
        {
            return InputHints.GetButtonDown(buttonName.value, hintText?.value);
        }
    }
}