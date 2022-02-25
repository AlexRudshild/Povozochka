using UnityEngine;
using UnityEngine.EventSystems;

public class FCP_Picker : MonoBehaviour, IPointerDownHandler, IDragHandler
{
	[SerializeField] private FlexibleColorPicker _fcp;
	[SerializeField] private PickerType _pickerType;

	private enum PickerType
	{
		Main, R, G, B, H, S, V, A, Preview, PreviewAlpha
	}

	public void OnDrag(PointerEventData eventData)
	{
		_fcp.PointerUpdate(eventData);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_fcp.SetPointerFocus((int)_pickerType);
		_fcp.PointerUpdate(eventData);
	}
}
