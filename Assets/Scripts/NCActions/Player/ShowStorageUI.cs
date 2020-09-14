using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Diagnostics;

namespace NodeCanvas.Tasks.Actions{

	[Category("Player")]
	public class ShowStorageUI : ActionTask<Player>
	{
		public BBParameter<Storage> targetStorage;
		public bool show;
		
		protected override void OnExecute()
		{
			StorageUIManager.ShowStorageUI(show);
			StorageUIManager.SetTargetStorage(targetStorage.value);
			EndAction(true);
		}
	}
}