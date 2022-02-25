using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PreviewIcon : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] private RectTransform _rectTransform = null;
	[SerializeField] private Text _iconCountText = null;
	[SerializeField] private Image _iconImage = null;

	public Image TokenImage = null;
	public Image CornerImage = null;

	public Text IconCountText => _iconCountText;

	public Item Item;

	public RectTransform RectTransform => _rectTransform;
	public Image IconImage => _iconImage;
	public RectTransform[,] element;

	public bool IsInside => transform.parent.GetComponent<Inventory>() != null;
	public bool HasCounter => _iconCountText != null;


	public void OnPointerClick(PointerEventData eventData)
	{
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
				InventoryControl.Instance.AddQuickly(Item);
				break;
            case PointerEventData.InputButton.Right:
				//ContextMenuManager.Instance.Open(this);
				break;
            case PointerEventData.InputButton.Middle:
                break;
        }
	}

	public void UpdateSprite()
	{
		if (Item.IsResource)
		{
			IconCountText.transform.parent.gameObject.SetActive(true);
			TokenImage.transform.parent.gameObject.SetActive(true);
			TokenImage.sprite = Item.Sprite;

			IconImage.sprite = Item.ResourceEnum.GetTokenSprite();
			IconImage.color = Item.Color;
			TokenImage.color = Item.TokenColor;
			CornerImage.sprite = Item.ResourceEnum.GetCornrerSprite();
			CornerImage.color = Item.CornerColor;

			if (Item.StackLimit == 1)
			{
				IconCountText.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				IconCountText.transform.parent.gameObject.SetActive(true);
				IconCountText.text = Item.StackLimit.ToString();
			}
		}
		else
		{
			IconImage.sprite = Item.Sprite;
			IconImage.color = Item.Color;

			IconCountText.transform.parent.gameObject.SetActive(false);
			TokenImage.transform.parent.gameObject.SetActive(false);
		}
	}
}