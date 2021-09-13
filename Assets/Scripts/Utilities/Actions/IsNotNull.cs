using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Utilities.Actions
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