using UnityEngine;
using UnityEngine.EventSystems;

public class ContextMenuDisplay : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] private IconTypeEnum type;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Right) return;

        switch (type)
        {
            case IconTypeEnum.InventoryIcon:
                ContextMenuManager.Instance.Open(GetComponent<InventoryIcon>());
                break;
            case IconTypeEnum.SkillIcon:
            case IconTypeEnum.StateIcon:
            case IconTypeEnum.PreviewIcon:
                ContextMenuManager.Instance.Open(GetComponent<PreviewIcon>());
                break;
        }
	}
}
