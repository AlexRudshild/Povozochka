using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{ 
	[SerializeField] private InventoryCells[,] _cells = null;
	[SerializeField] private GameObject _cellPrefab = null;

	[SerializeField] private TMP_Text _nameLabel = null;
	[SerializeField] private RectTransform _rectTransform = null;
	[SerializeField] private RectTransform _toolbarRect = null;
	[SerializeField] private RectTransform _gridRect = null;
	[SerializeField] private RectTransform _contentRect = null;
	[SerializeField] private Shader _shaderGrid= null;
	[SerializeField] private int _height = 0;
	[SerializeField] private int _width = 0;
	[SerializeField] private string _id = "";

	public RectTransform RectTransform => _rectTransform;
	public RectTransform ToolbarRect => _toolbarRect;
	public RectTransform GridRect => _gridRect;
	public RectTransform ContentRect => _contentRect;

	public int Width => _width;
	public int Height => _height;
	public string Id => _id;

	public InventoryCells[,] Cells { get { return _cells; } }

	private List<InventoryIcon> Icons;

	public void Close()
	{
		InventoryControler.Instance.RemoveInventory(this);

		SaveIcons();

		DestroyImmediate(gameObject);
	}

	public void Open(string name, string id, int width, int height)
	{
		gameObject.name = name;
		_id = id;
		_width = width;
		_height = height;
	}

	public void UpdateIcon(string id)
	{
		foreach (var item in Icons)
		{
			if (item.Item.Id == id)
			{
				item.UpdateSprite();
			}
		}
		SaveIcons();
	}

	public void UpdateCells(string id)
	{
		foreach (var item in Icons)
		{
			if (item.Item.Id == id)
			{
				item.UpdateCells();
			}
		}
		UpdateCells();
	}

	public void UpdateName(string name)
	{
		gameObject.name = name;
		_nameLabel.text = name;
	}

	public void AddIcon(InventoryIcon icon)
	{
		if (Icons == null) Icons = new List<InventoryIcon>();
		Icons.Add(icon);
		SaveIcons();
	}

	public void RemoveIcon(InventoryIcon icon)
	{
		Icons.Remove(icon);
		SaveIcons();
	}

	public void DeleteItems(Item item)
	{
		var icons = new List<InventoryIcon>(Icons);
		foreach (var icon in icons)
		{
			if (icon.Item == item)
			{
				Icons.Remove(icon);
				DestroyImmediate(icon.gameObject);
			}
		}
		UpdateCells();
		SaveIcons();
	}

	public void SaveIcons()
	{
		InventorySaver.Save(this, Icons);
	}

	public void LoadIcons()
	{
		Icons = InventorySaver.Load(this);
		UpdateCells();
	}

	public void UpdateCells()
	{
		foreach (var cell in Cells)
		{
			cell.ItemId = "";
		}
		if (Icons == null)
		{
			return;
		}

		foreach (var cell in Cells)
		{
			foreach (var item in Icons)
			{
				if (cell.IsLocked)
				{
					break;
				}
				foreach (var rect in item.element)
				{
					if (!item.IconBlock && rect != null && rect.position.ToVector2() == cell.rectTransform.position.ToVector2())
					{
						cell.ItemId = item.Item.Id;
						break;
					}
					else
					{
						cell.ItemId = "";
					}
				}
			}
		}
	}

	public void BuildGrid()
	{
		var cells = _gridRect.transform.GetComponentsInChildren<InventoryCells>();

		foreach (var cell in cells)
		{
			DestroyImmediate(cell.gameObject);
		}

		var cellSize = GlobalParametrs.CellsSize;

		Vector2 delta = new Vector2(cellSize * _width, cellSize * _height);

		_rectTransform.sizeDelta = new Vector2(delta.x + 1, delta.y + 1 + _toolbarRect.sizeDelta.y);
		_gridRect.anchoredPosition = new Vector2(0, -_toolbarRect.sizeDelta.y);
		_gridRect.sizeDelta = delta;
		_contentRect.anchoredPosition = new Vector2(0, -_toolbarRect.sizeDelta.y);
		_contentRect.sizeDelta = delta;

		var grid = new Material(_shaderGrid);

		grid.SetVector("GridSize", new Vector2(_width, _height));

		_gridRect.gameObject.GetComponent<Image>().material = grid;

		float posX = -(cellSize * _width / 2f + cellSize / 2f);
		float posY = cellSize * _height / 2f + cellSize / 2f;
		float Xreset = posX;
		int i = 0;
		_cells = new InventoryCells[_height, _width];
		for (int y = 0; y < _height; y++)
		{
			posY -= cellSize;
			for (int x = 0; x < _width; x++)
			{
				posX += cellSize;
				RectTransform tr = Instantiate(_cellPrefab, _gridRect).GetComponent<RectTransform>();
				tr.sizeDelta = new Vector2(cellSize, cellSize);
				tr.localScale = Vector3.one;
				tr.anchoredPosition = new Vector2(posX, posY);
				tr.name = "Cell_" + i;
				tr.GetComponent<InventoryCells>().InventoryId = Id;
				_cells[y, x] = tr.GetComponent<InventoryCells>();
				i++;
			}
			posX = Xreset;
		}
		_nameLabel.text = name;
	}
}
