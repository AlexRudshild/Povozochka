using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenuManager : MonoBehaviour
{
	public static ContextMenuManager Instance;

	[SerializeField] private EventSystem eventSystem;
	[SerializeField] private RectTransform _PanelRect;
	[SerializeField] private InventoryControler _inventoryController;

	[Header("Menu")]

	[SerializeField] private GameObject _contexMenu;
	[SerializeField] private IconEditor _editMenu;

	[Header("Buttons")]

	[SerializeField] private GameObject _open;
	[SerializeField] private GameObject _equip;
	[SerializeField] private GameObject _use;
	[SerializeField] private GameObject _inspect;
	[SerializeField] private GameObject _drop;
	[SerializeField] private GameObject _dublicate;
	[SerializeField] private GameObject _delete;
	[SerializeField] private GameObject _edit;

	private InventoryIcon _inventoryIcon;
	private Item _item;

	private PointerEventData _pointerData = new PointerEventData(EventSystem.current);

	// Start is called before the first frame update
	void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}

		_open.GetComponent<Button>().onClick.AddListener(() => { Close(); Open(); });
		_equip.GetComponent<Button>().onClick.AddListener(() => { Close(); Equip(); });
		_use.GetComponent<Button>().onClick.AddListener(() => { Close(); Use(); });
		_inspect.GetComponent<Button>().onClick.AddListener(() => { Close(); Inspect(); });
		_drop.GetComponent<Button>().onClick.AddListener(() => { Close(); Drop(); });
		_dublicate.GetComponent<Button>().onClick.AddListener(() => { Close(); Dublicate(); });
		_delete.GetComponent<Button>().onClick.AddListener(() => { Close(); Delete(Input.GetButton("Shift")); });
		_edit.GetComponent<Button>().onClick.AddListener(() => { Close(); Edit(); });
	}

	// Update is called once per frame
	void Update()
	{
		if (_contexMenu.activeSelf && Input.mousePosition.PointIsInside(_PanelRect) == false)
		{
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
			{
				Close();
			}
		}

		if (Input.GetButtonDown("Delete"))
		{
			_inventoryIcon = _inventoryController.RaycastUI(Input.mousePosition);

			if (_inventoryIcon != null)
			{
				_item = _inventoryIcon.Item;
				Delete(Input.GetButton("Shift"));
            }
            else
            {
				PointerEventData pointer = new PointerEventData(eventSystem);

				pointer.position = Input.mousePosition;

				var ray = new List<RaycastResult>();

				eventSystem.RaycastAll(pointer, ray);

				_item = ray[0].gameObject.GetComponent<PreviewIcon>()?.Item;

				if (_item != null)
					Delete(true);
			}

			NameText.HideName();
        }
	}

	public void Open(PreviewIcon preview)
	{
		if (Input.GetMouseButton(0) || Input.GetMouseButton(2))
			return;

		_item = preview.Item;

		_open.SetActive(false);
		_equip.SetActive(false);
		_use.SetActive(false);
		_inspect.SetActive(false);
		_drop.SetActive(false);

		_contexMenu.transform.position = new Vector2(Input.mousePosition.x + 3, Input.mousePosition.y - 3);
		_contexMenu.SetActive(true);
	}

	public void Open(InventoryIcon icon)
	{
		if (Input.GetMouseButton(0) || Input.GetMouseButton(2))
			return;

		_item = icon.Item;

		_open.SetActive(_item.IsOpen);
		_equip.SetActive(_item.IsEquip);
		_use.SetActive(_item.IsUse);
		_inspect.SetActive(_item.IsInspect);
		_drop.SetActive(_item.IsDrop);

		_inventoryIcon = icon;
		_contexMenu.transform.position = new Vector2(Input.mousePosition.x + 3, Input.mousePosition.y - 3);
		_contexMenu.SetActive(true);
	}

	public void Close()
	{
		_contexMenu.SetActive(false);
	}

	public void Open()
	{
		_inventoryIcon.Item.Open.Open(_inventoryIcon.Item);
		Debug.Log("Open");
		_item = null;
		_inventoryIcon = null;
	}

	public void Equip()
	{
		Debug.Log("Equip");
		_item = null;
		_inventoryIcon = null;
	}

	public void Use()
	{
		Debug.Log("Use");
		_item = null;
		_inventoryIcon = null;
	}

	public void Inspect()
	{
		Debug.Log("Inspect");
		_item = null;
		_inventoryIcon = null;
	}

	public void Drop()
	{
		Debug.Log("Drop");
		_item = null;
		_inventoryIcon = null;
	}

	public void Dublicate()
	{
		if (_inventoryIcon)
		{
			_inventoryController.Dublicate(_inventoryIcon);
		}
		else
		{
			var dubItem = new Item(_item);
			ItemManager.Instance.Items.Add(dubItem);
			ItemManager.SaveItem(dubItem);
			IconAdder.UpdateIcons();
		}
		Debug.Log("Dublicate");
		_item = null;
		_inventoryIcon = null;
	}

	public void Delete(bool dellItem)
	{
		if (dellItem || !_inventoryIcon)
		{
			_inventoryController.DeleteItem(_item);
			IconAdder.UpdateIcons();
		}
		else
		{
			_inventoryController.DeleteIcon(_inventoryIcon);
		}

		Debug.Log("Delete");
		_item = null;
		_inventoryIcon = null;
	}

	public void Edit()
	{
		if (!_editMenu.isActiveAndEnabled)
		{
			_editMenu.gameObject.SetActive(true);
			_editMenu.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
		}
		_editMenu.transform.SetAsLastSibling();

		_editMenu.OpenItem(_item);

		IconAdder.UpdateIcons();

		Debug.Log("Edit");
		_item = null;
		_inventoryIcon = null;
	}
}
