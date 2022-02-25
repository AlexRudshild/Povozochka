using UnityEngine;
using UnityEngine.EventSystems;

public class NameDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string GetName()
    {
        var icon = GetComponent<InventoryIcon>();

        if (icon)
        {
            return icon.Item.Name;
        }

        var preview = GetComponent<PreviewIcon>();

        if (preview)
        {
            return preview.Item.Name;
        }

        return string.Empty;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        NameText.ShowName(GetName());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        NameText.HideName();
    }
}
