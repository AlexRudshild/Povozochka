using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryCells : MonoBehaviour {

	[SerializeField] private RectTransform _rectTransform = null;

	[SerializeField] private Image _image = null;
	public string ItemId { get; set; } = "";

	public string InventoryId = "";
	public bool IsLocked => ItemId != "";


	public RectTransform rectTransform
	{
		get{ return _rectTransform; }
	}

	public void SetRectTransform(RectTransform tr)
	{
		_rectTransform = tr;
	}

	public Color Color
	{
		get { return _image.color; }
	}

	public void SetColor(Color color)
	{
		_image.color = color;
	}
}
