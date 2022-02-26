using SFB;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconEditor : MonoBehaviour
{
	[Header("Menu")]

	[SerializeField] private TMP_InputField _name;
	[SerializeField] private TMP_InputField _description;
	[SerializeField] private TMP_InputField _stackLimit;
	[SerializeField] private TMP_Dropdown _resourceSize;
	[SerializeField] private Image[] _colorButton;
	[SerializeField] private GameObject[] _controllers;
	[SerializeField] private TMP_InputField[] _size;

	[Space]

	[SerializeField] private CellsEditor _cellsEditor;

	[Header("Icon")]

	[SerializeField] private Image _iconImage;
	[SerializeField] private Image _tokenImage;
	[SerializeField] private Image _cornerImage;

	[SerializeField] private Material _itemMaterial;

	private Item _curItem;

	private bool[,] cells;

	public int SelectedColor { get; private set; } = 0;

	public Color color = new Color(0, 0.5f, 0);

	public FlexibleColorPicker fcp;

	public Color externalColor;
	private Color internalColor;

	private Sprite _sprite;
	private string _spritePath;


	// Start is called before the first frame update
	private void Awake()
	{
		internalColor = externalColor;
		_resourceSize.onValueChanged.AddListener(delegate { ChangeResourceSize(_resourceSize.value); });
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		if (fcp.isActiveAndEnabled)
		{
			//apply color of this script to the FCP whenever it is changed by the user
			if (internalColor != externalColor)
			{
				fcp.Color = externalColor;
				internalColor = externalColor;
			}

			if (_colorButton[SelectedColor].color != fcp.Color)
			{
				_colorButton[SelectedColor].color = fcp.Color;

				if ((ResourceEnum)_resourceSize.value == ResourceEnum.none)
				{
					_iconImage.color = _colorButton[0].color;
					_tokenImage.color = _colorButton[1].color;
					_cornerImage.color = _colorButton[2].color;
				}
				else
				{
					_tokenImage.color = _colorButton[0].color;
					_iconImage.color = _colorButton[1].color;
					_cornerImage.color = _colorButton[2].color;
				}
			}
			//extract color from the FCP and apply it to the object material
		}
	}

	public void SelectColor(int count)
	{
		if (count < 0)
		{
			SelectedColor = 0;
		}
		else if (count > _colorButton.Length)
		{
			SelectedColor = _colorButton.Length - 1;
		}
		else
		{
			SelectedColor = count;
		}

		if (!fcp.isActiveAndEnabled)
		{
			fcp.gameObject.SetActive(true);
			fcp.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
		}
		fcp.transform.SetAsLastSibling();
		fcp.Color = _colorButton[SelectedColor].color;
		externalColor = _colorButton[SelectedColor].color;
	}

	public void OpenItem(Item item)
	{
		_cellsEditor.gameObject.SetActive(false);
		_curItem = item;
		if (item.IsResource)
		{
			//_iconImage.material = _cornerImage.material;
			_colorButton[0].color = item.TokenColor;
			_colorButton[1].color = item.Color;
			_colorButton[2].color = item.CornerColor;
		}
		else
		{
			//_iconImage.material = _itemMaterial;
			_colorButton[0].color = item.Color;
			_colorButton[1].color = item.TokenColor;
			_colorButton[2].color = item.CornerColor;
		}
		externalColor = _colorButton[SelectedColor].color;

		if (fcp.isActiveAndEnabled)
		{
			fcp.Color = externalColor;
		}

		_resourceSize.value = (int)item.ResourceEnum;
		ChangeResourceSize((int)item.ResourceEnum);

		for (int i = 0; i < _controllers.Length; i++)
		{
			_controllers[i].GetComponentInChildren<Toggle>().isOn = _curItem.ContainController((ControllersEnum)i);
		}
		if (_curItem.IsOpen)
		{
			_size[0].text = _curItem.Open.Width.ToString();
			_size[1].text = _curItem.Open.Heigth.ToString();
		}

		cells = item.Cells;

		_sprite = item.Sprite;
		_spritePath = item.SpritePath;
		_name.text = item.Name;
		_description.text = item.Description;
	}

	public void ChangeImage()
	{
		AsyncChangeImage();
	}

	public void SetCells(bool[,] cells)
	{
		this.cells = cells;
	}

	private async void AsyncChangeImage()
	{
		string path = string.Empty;
		byte[] fileContent = null;

		await Task.Run(() =>
		{
			var extensions = new[] {  //какие файлы вообще можно открыть
			new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )
			};
			var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

			if (paths.Length != 0)
			{
				path = paths[0].Split('\\')[paths[0].Split('\\').Length - 1];

				fileContent = File.ReadAllBytes(paths[0]);
			}
		});


		if (path == string.Empty || fileContent == null)
		{
			return;
		}

		if (Directory.Exists(GlobalParametrs.SpriteFolderPath) == false)
		{
			Directory.CreateDirectory(GlobalParametrs.SpriteFolderPath);
		}

		File.WriteAllBytes(GlobalParametrs.SpriteFolderPath + path, fileContent);

		_sprite = ItemManager.GetSprite(GlobalParametrs.SpriteFolderPath + path);
		_spritePath = path;

		if ((ResourceEnum)_resourceSize.value == ResourceEnum.none)
		{
			_iconImage.sprite = _sprite;
		}
		else
		{
			_tokenImage.sprite = _sprite;
		}
	}

	public void ChangeResourceSize(int newMode)
	{
		ChangeResourceSize((ResourceEnum)newMode);
	}


	public void ChangeResourceSize(ResourceEnum size)
	{
		bool isResource = size != ResourceEnum.none;

		_cornerImage.enabled = isResource;
		_tokenImage.enabled = isResource;
		_stackLimit.gameObject.SetActive(isResource);
		if (_stackLimit.text == string.Empty)
		{
			_stackLimit.text = _curItem.StackLimit.ToString();
		}

		if (isResource)
		{
			_tokenImage.color = _colorButton[0].color;
			_iconImage.color = _colorButton[1].color;
			_cornerImage.color = _colorButton[2].color;

			_iconImage.sprite = size.GetTokenSprite();
			_cornerImage.sprite = size.GetCornrerSprite();

			if (_curItem)
			{
				_tokenImage.sprite = _curItem.Sprite;
			}
		}
		else
		{
			_iconImage.color = _colorButton[0].color;

			if (_curItem)
			{
				_iconImage.sprite = _curItem.Sprite;
			}
		}
	}

	public void ChangeController(int controller)
	{
		ChangeController((ControllersEnum)controller);
	}

	public void ChangeController(ControllersEnum controller)
	{
		//bool isOn = _controllers[(int)controller].GetComponentInChildren<Toggle>().isOn;

		//Debug.Log(isOn.ToString() + " " + controller.ToString());
	}

	public void Save()
	{
		_curItem.Name = _name.text;
		_curItem.Description = _description.text;

		_curItem.Color = _iconImage.color;
		_curItem.TokenColor = _tokenImage.color;
		_curItem.CornerColor = _cornerImage.color;

		_curItem.Sprite = _sprite;
		_curItem.SpritePath = _spritePath;

		_curItem.ResourceEnum = (ResourceEnum)_resourceSize.value;

		for (int i = 0; i < _controllers.Length; i++)
		{
			bool isOn = _controllers[i].GetComponentInChildren<Toggle>().isOn;
			var controller = (ControllersEnum)i;

			if (isOn)
			{

				switch (controller)
				{
					case ControllersEnum.InspectItem:
						_curItem.Controller.Inspect = new InspectItem();
						break;
					case ControllersEnum.EquipItem:
						_curItem.Controller.Equip = new EquipItem();
						break;
					case ControllersEnum.UseItem:
						_curItem.Controller.Use = new UseItem();
						break;
					case ControllersEnum.DropItem:
						_curItem.Controller.Drop = new DropItem();
						break;
					case ControllersEnum.OpenItem:
						_curItem.Controller.Open = new OpenItem(int.Parse(_size[0].text), int.Parse(_size[1].text));
						break;
				}
			}
			else
			{

				switch (controller)
				{
					case ControllersEnum.InspectItem:
						_curItem.Controller.Inspect = null;
						break;
					case ControllersEnum.EquipItem:
						_curItem.Controller.Equip = null;
						break;
					case ControllersEnum.UseItem:
						_curItem.Controller.Use = null;
						break;
					case ControllersEnum.DropItem:
						_curItem.Controller.Drop = null;
						break;
					case ControllersEnum.OpenItem:
						_curItem.Controller.Open = null;
						break;
				}
			}
		}


		if (_curItem.IsResource)
		{
			_curItem.StackLimit = int.Parse(_stackLimit.text);
		}
		if (_curItem.Cells != cells)
		{
			_curItem.Cells = cells;
			InventoryControler.Instance.UpdateCells(_curItem.Id);
		}
		ItemManager.SaveItem(_curItem);
		InventoryControler.Instance.UpdateIcons(_curItem.Id);
		InventoryControler.Instance.UpdateNames(_curItem.Id, _curItem.Name);
	}

	public void OpenCellsEditor()
	{
		_cellsEditor.gameObject.SetActive(true);
		_cellsEditor.SetCells(cells, _sprite);
	}
}



public enum IconTypeEnum
{
	InventoryIcon, SkillIcon, StateIcon, PreviewIcon
}