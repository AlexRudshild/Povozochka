using UnityEngine;
using UnityEngine.EventSystems;

public class PointTarget : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.parent.GetComponent<InventoryIcon>().BeginDrag();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetBackImage(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetBackImage(false);
    }

    public void SetBackImage(bool value)
    {
        transform.parent.GetComponent<InventoryIcon>().SetBackImage(value);
    }
}
