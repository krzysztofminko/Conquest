using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{
	[Category("✫ Utility")]
	public class IsNotNull : ConditionTask
	{
		public BBParameter<object> variable;

		protected override string info
		{
			get => variable + " is not null";
		}

		protected override bool OnCheck() => variable.value != null;
	}
}