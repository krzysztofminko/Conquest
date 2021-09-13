using Items;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace UI.Actions
{
	[Category("UI")]
	public class GetSelectedItemEntity : ActionTask
	{
		public BBParameter<ItemEntity> selectedItemEntity;
		public BBParameter<bool> playerStorageIsSelected;

		protected override void OnExecute()
		{
			selectedItemEntity.value = StorageUIManager.Instance.SelectedItemEntity;
			playerStorageIsSelected.value = StorageUIManager.Instance.PlayerStorageIsSelected;

			EndAction(true);
		}
	}
}