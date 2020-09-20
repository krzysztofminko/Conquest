using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions{

	[Category("UI")]
	[Description("Shows or hides Storage UI.")]
	public class ShowStorageUI : ActionTask
	{
		public bool show;
		public BBParameter<Storage> targetStorage;
		
		protected override void OnExecute()
		{
			StorageUIManager.Instance.ShowStorageUI(show);
			StorageUIManager.Instance.SetTargetStorage(targetStorage.value);
			EndAction(true);
		}
	}
}