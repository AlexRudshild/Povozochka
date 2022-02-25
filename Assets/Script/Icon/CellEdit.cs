using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellEdit : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool _isEnable;
    public bool IsEnable
    {
        get
        {
            return _isEnable;
        }
        set
        {
            _isEnable = value;
            UpdateCell();
        }
    }
    private Color _defaultColor = new Color(1, 1, 1, 0.4f);
    private Color _disableColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            IsEnable = !IsEnable;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        if (Input.GetMouseButton(0))
        {
            IsEnable = !IsEnable;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateCell();
    }

    public void UpdateCell()
    {
        if (IsEnable)
        {
            GetComponent<Image>().color = _defaultColor;
        }
        else
        {
            GetComponent<Image>().color = _disableColor;
        }
    }
}
