using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryControl : MonoBehaviour
{
    [SerializeField] private List<Inventory> _inventors;
    [SerializeField] private RectTransform _overlapRect;

    [SerializeField] private GameObject _inventoryPrefab;
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private GameObject _tokentPrefab;

    public static InventoryControl Instance;

    private Vector3 _lastMousePosition;

    public GameObject PointPrefab;

    public Vector2 LastItemPosition { get; private set; }
    public Vector2 LastItemSize { get; private set; }
    public Vector2 LastItemScale { get; private set; }
    public Quaternion LastItemRotate { get; private set; }

    private InventoryIcon _currretIcon;
    private Inventory _currretInventory;

    private readonly PointerEventData _pointerData = new PointerEventData(EventSystem.current);

    public Color HigthLigthColor = new Color32(255, 255, 255, 0);
    public Color HigthLigthColor1 = new Color32(0, 128, 0, 128);
    public Color HigthLigthColor2 = new Color32(128, 0, 0, 128);

    private Color color = new Color(0, 0.5f, 0);

    private List<InventoryCells> _oldCells = null;

    private readonly bool IsDublicate = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ReFillListInvetors();

        BuildGridsForAll();

        //foreach (var item in ItemManager.Instance.Items)
        //{
        //	AddQuickly(item);
        //}
        _inventors[0].LoadIcons();
        //AddQuickly(ItemManager.Instance.Items[0]);
        //AddQuickly(ItemManager.Instance.Items[1]);
        ColorizeLockedCells();
    }

    private void Update()
    {
        if (_currretIcon)
        {
            if (Input.GetButtonDown("Rotate"))
            {
                _currretIcon.RectTransform.Rotate(new Vector3(0, 0, 90 * Input.GetAxis("Rotate")));
                _lastMousePosition = Vector3.zero;
            }
            if (Input.GetButtonDown("Flip"))
            {
                _currretIcon.RectTransform.localScale = new Vector2(_currretIcon.RectTransform.localScale.x * -1, _currretIcon.RectTransform.localScale.y);
                _lastMousePosition = Vector3.zero;
            }
        }

        if (_currretIcon)
        {

            if (_lastMousePosition != Input.mousePosition)
            {
                _lastMousePosition = Input.mousePosition;

                _currretIcon.RectTransform.position = _lastMousePosition;
            }

            if (Input.GetMouseButtonUp(0) && !IsDublicate)
            {
                if (!Similar())
                {
                    AddCurrent();
                }
                _lastMousePosition = Vector3.zero;
            }
        }
    }

    private IEnumerator ColorizeUnderItem()
    {
        while (_currretIcon)
        {
            for (int i = _inventors.Count - 1; i >= 0; i--)
            {
                if (Input.mousePosition.PointIsInside(_inventors[i].RectTransform))
                {
                    _currretInventory = _inventors[i];
                    break;
                }
            }

            var cells = GetNotOverlapCell(_currretIcon.element);

            if (cells != _oldCells && _oldCells != null || cells == null && _oldCells != null)
            {
                foreach (var cell in _oldCells)
                {
                    cell.SetColor(HigthLigthColor);
                }
                _oldCells = null;
            }

            if (cells != null)
            {
                foreach (var cell in cells)
                {
                    cell.SetColor(color);
                }
            }
            _oldCells = cells;
            yield return new WaitForSeconds(0.05f);
        }
        yield break;
    }

    private async void AsyncColorizeUnderItem()
    {
        List<InventoryCells> cells;
        //cells = await Task.Run(() => GetNotOverlapCell(_currretIcon.element));
        await Task.Run(() =>
        {
            cells = GetNotOverlapCell(_currretIcon.element);
            while (_currretIcon)
            {


                if (cells != _oldCells && _oldCells != null || cells == null && _oldCells != null)
                {
                    foreach (var cell in _oldCells)
                    {
                        cell.SetColor(HigthLigthColor);
                    }
                    _oldCells = null;
                }

                _oldCells = cells;
                Task.Delay(1000);
            }
        });
    }

    private void ColorizeLockedCells()
    {
        foreach (var inventory in _inventors)
        {
            foreach (var cell in inventory.Cells)
            {
                if (cell.IsLocked)
                {
                    cell.SetColor(new Color32(0, 0, 0, 60));
                }
                else
                {
                    if (cell.Color == new Color32(0, 0, 0, 60))
                    {
                        cell.SetColor(Color.clear);
                    }
                }
            }
        }
    }

    public void RemoveInventory(Inventory inventory)
    {
        _inventors.Remove(inventory);
    }

    public void UpdateCells(string id)
    {
        foreach (var inventory in _inventors)
        {
            inventory.UpdateCells(id);
        }
        ColorizeLockedCells();
    }

    public void UpdateIcons(string id)
    {
        foreach (var inventory in _inventors)
        {
            inventory.UpdateIcon(id);
        }
    }

    public void UpdateNames(string id, string name)
    {
        foreach (var inventory in _inventors)
        {
            if (inventory.Id == id)
            {
                inventory.UpdateName(name);
            }
        }
    }

    public void Dublicate(InventoryIcon icon)
    {
        var dubItem = new Item(icon.Item);
        ItemManager.Instance.Items.Add(dubItem);

        AddQuickly(icon.Inventory, dubItem, icon.Counter);
        //var dubIcon = SetIcon(icon.Inventory, dubItem, icon.counter);

        //dubIcon.RectTransform.position = Vector3.zero;
        //dubIcon.RectTransform.rotation = icon.RectTransform.rotation;
        //dubIcon.RectTransform.sizeDelta = icon.RectTransform.sizeDelta;

        //IsDublicate = true;

        //BeginDrag(dubIcon);

        ItemManager.SaveItem(dubItem);

        IconAdder.UpdateIcons();
    }


    public void CreateItem()
    {
        var item = new Item(true);
        ItemManager.Instance.Items.Add(item);
        AddQuickly(item);
        ItemManager.SaveItem(item);
        IconAdder.UpdateIcons();
    }

#if UNITY_EDITOR
    [ContextMenu("Refill List")]
#endif
    public void ReFillListInvetors()
    {
#if UNITY_EDITOR
        Undo.RecordObject(gameObject, "Refill List");
#endif
        var _inventorsArr = GetComponentsInChildren<Inventory>();

        _inventors = new List<Inventory>();

        foreach (var item in _inventorsArr)
        {
            _inventors.Add(item);
        }
    }

    public void BeginDrag(InventoryIcon currretIcon)
    {
        _currretIcon = currretIcon;

        LastItemPosition = new Vector2(_currretIcon.transform.position.x, _currretIcon.transform.position.y);
        LastItemSize = _currretIcon.RectTransform.sizeDelta;
        LastItemScale = _currretIcon.RectTransform.localScale;
        LastItemRotate = _currretIcon.RectTransform.localRotation;

        _currretIcon.RectTransform.sizeDelta = _currretIcon.RectTransform.sizeDelta * 0.95f;

        _currretIcon.SetRaycastToPointTarget(false);
        if (!_currretIcon.IconBlock)
            SetItemCell(_currretIcon.Inventory.Id, _currretIcon.element, null);
        _currretIcon.RectTransform.SetParent(_overlapRect);

        currretIcon.Inventory.RemoveIcon(currretIcon);
        _currretInventory = currretIcon.Inventory;
        //AsyncColorizeUnderItem();

        //foreach (var item in ItemManager.Instance.Items)
        //{
        //	if (item.Id == _currretInventory.Id)
        //	{
        //		if (item.ItemController.IsOpen)
        //		{
        //			item.ItemController.Open.BlackList.Remove(_currretIcon.item.Id);
        //			break;
        //		}
        //		break;
        //	}
        //}
        StartCoroutine(ColorizeUnderItem());
    }

    public void DoubleClick(InventoryIcon currretIcon)
    {
        var open = currretIcon.Item.Open;

        if (open != null)
        {
            open.Open(currretIcon.Item);
        }
        /*
        if (currretIcon.item.OnUse.DestroyOnUse)
        {
			currretIcon.counter -= 1;
            if (currretIcon.counter <= 0)
            {
				currretIcon.Inventory.RemoveItem(currretIcon);
				DestroyImmediate(currretIcon.gameObject);
            }
        }*/
    }

    private void ResetDrag()
    {
        /*
		foreach (InventoryCells cell in _inventors[0].Cells)
		{
			cell.Item = _currretIcon.item;
		}*/
        if (_oldCells != null)
        {
            foreach (var cell in _oldCells)
            {
                cell.SetColor(HigthLigthColor);
            }
        }

        if (_currretIcon == null)
        {
            return;
        }

        _currretIcon.SetRaycastToPointTarget(true);

        _currretIcon.transform.position = LastItemPosition;
        _currretIcon.RectTransform.sizeDelta = LastItemSize;
        _currretIcon.RectTransform.localScale = LastItemScale;
        _currretIcon.RectTransform.localRotation = LastItemRotate;
        _currretIcon.RectTransform.SetParent(_currretIcon.Inventory.ContentRect.transform);

        if (!_currretIcon.IconBlock)
            SetItemCell(_currretIcon.Inventory.Id, _currretIcon.element, _currretIcon.Item);

        _currretIcon.Inventory.AddIcon(_currretIcon);

        _currretIcon = null;
    }

    private void SetItemCell(string inventoryId, RectTransform[,] element, Item item)
    {
        foreach (RectTransform tr in element)
        {
            if (tr == null)
            {
                continue;
            }

            InventoryCells t = RaycastUI(inventoryId, tr.position);
            if (t == null)
            {
                continue;
            }

            if (item == null)
            {
                t.ItemId = "";
            }
            else
            {
                t.ItemId = item.Id;
            }
        }
        ColorizeLockedCells();
    }

    public void AddQuickly(Item item)
    {
        if (_inventors.Count < 1)
        {
            return;
        }
        AddQuickly(_inventors[_inventors.Count - 1], item, 1);
    }

    public void AddQuickly(Inventory inventory, Item item)
    {
        AddQuickly(inventory, item, 1);
    }

    public void AddQuickly(Inventory inventory, Item item, int count) // быстрое добавление
    {
        if (inventory == null)
        {
            return;
        }

        List<InventoryCells> targetCell = GetCells(inventory, item);

        if (targetCell != null)
        {
            _currretIcon = SetIcon(inventory, item, count, false);
            if (!_currretIcon)
            {
                return;
            }

            AddCurrentIcon(inventory, targetCell);
            ColorizeLockedCells();
        }
        else
        {
            //InventoryDrop.DropItem(val, count);
            Debug.Log(this + " --> item: [ " + item.Name + " ] count: [ " + count + " ] | Предмет(ы) добавить невозможно! В инвентаре нет места!");
        }
    }

    private List<InventoryCells> GetCells(Inventory inventory, Item item)
    {
        var targetCell = new List<InventoryCells>();

        var iconHeigth = item.Cells.GetLength(0);
        var iconWidth = item.Cells.GetLength(1);

        int height = inventory.Height - iconHeigth;
        int width = inventory.Width - iconWidth;

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                targetCell = ChekCell(inventory.Cells, item, new Vector2Int(x, y));

                if (targetCell != null)
                {
                    return targetCell;
                }
            }
        }

        return targetCell;
    }

    private List<InventoryCells> ChekCell(InventoryCells[,] curCell, Item item, Vector2Int pos)
    {
        var targetCell = new List<InventoryCells>();

        int height = item.Cells.GetLength(0) + pos.y;
        int width = item.Cells.GetLength(1) + pos.x;

        for (int y = pos.y; y < height; y++)
        {
            for (int x = pos.x; x < width; x++)
            {
                if (item.Cells[y - pos.y, x - pos.x])
                {
                    if (!curCell[y, x].IsLocked)
                    {
                        targetCell.Add(curCell[y, x]);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        return targetCell;
    }

    public List<InventoryCells> IsOverlap(RectTransform[,] rectTransform) // проверка на перекрытие
    {
        var targetCell = new List<InventoryCells>();
        foreach (RectTransform tr in rectTransform)
        {
            if (tr == null)
            {
                continue;
            }

            InventoryCells t = RaycastUI(_currretInventory.Id, tr.position);
            if (t == null || t.IsLocked)
            {
                return null;
            }

            if (targetCell.Contains(t) == false)
            {
                targetCell.Add(t);
            }
        }

        if (targetCell.Count == 0)
        {
            return null;
        }

        return targetCell;
    }

    public List<InventoryCells> IsOverlap(string inventoryId, RectTransform[,] rectTransform) // проверка на перекрытие
    {
        var targetCell = new List<InventoryCells>();
        foreach (RectTransform tr in rectTransform)
        {
            if (tr == null)
            {
                continue;
            }

            InventoryCells t = RaycastUI(inventoryId, tr.position);
            if (t == null || t.IsLocked)
            {
                return null;
            }

            if (targetCell.Contains(t) == false)
            {
                targetCell.Add(t);
            }
        }

        if (targetCell.Count == 0)
        {
            return null;
        }

        return targetCell;
    }

    public InventoryCells RaycastUI(string inventoryId, Vector3 position) // рейкаст по UI клеткам инвентаря
    {
        _pointerData.position = position;

        var _raycastResult = new List<RaycastResult>();

        EventSystem.current.RaycastAll(_pointerData, _raycastResult);

        foreach (var raycast in _raycastResult)
        {
            if (raycast.gameObject.layer == GlobalParametrs.LayerMaskCell)
            {
                var cell = raycast.gameObject.GetComponent<InventoryCells>();

                if (cell.InventoryId == inventoryId)
                {
                    return cell;
                }
            }
        }

        return null;
    }

    public InventoryIcon RaycastUI(Vector3 position) // рейкаст по иконке
    {
        _pointerData.position = position;

        var _raycastResult = new List<RaycastResult>();

        EventSystem.current.RaycastAll(_pointerData, _raycastResult);

        foreach (var raycast in _raycastResult)
        {
            return raycast.gameObject.transform.parent.GetComponent<InventoryIcon>();
        }

        return null;
    }

    private Vector2 GetCenterPosition(List<InventoryCells> targetCell, Inventory inventory)
    {
        Vector3[] worldCorners = new Vector3[4];

        inventory.RectTransform.GetWorldCorners(worldCorners);

        float[] x = new float[2];
        float[] y = new float[2];

        x[0] = targetCell[0].transform.position.x;
        x[1] = targetCell[0].transform.position.x;
        y[0] = targetCell[0].transform.position.y;
        y[1] = targetCell[0].transform.position.y;

        foreach (var item in targetCell)
        {
            if (x[0] < item.transform.position.x)
            {
                x[0] = item.transform.position.x;
            }
            else if (x[1] > item.transform.position.x)
            {
                x[1] = item.transform.position.x;
            }

            if (y[0] < item.transform.position.y)
            {
                y[0] = item.transform.position.y;
            }
            else if (y[1] > item.transform.position.y)
            {
                y[1] = item.transform.position.y;
            }

        }

        return new Vector2(x[0] / 2f + x[1] / 2f, y[0] / 2f + y[1] / 2f);
    }

    private void AddCurrent() // добавление иконки
    {
        var targetCell = IsOverlap(_currretIcon.element);

        if (targetCell == null)
        {
            ResetDrag();
            return;
        }

        if (_currretIcon.Item.IsOpen && _currretInventory.Id == _currretIcon.Item.Id)
        {
            ResetDrag();
            return;
        }

        //if (CheckIdInBlackList(_currretInventory.Id) || !CheckIdInWhiteList(_currretIcon.item.Id))
        //      {
        //	ResetDrag();
        //	return;
        //}

        //      foreach (var item in ItemManager.Instance.Items)
        //      {
        //          if (item.Id == _currretInventory.Id)
        //	{
        //		foreach (var itemController in item.ItemControllers)
        //		{
        //			var openItem = itemController as OpenItem;
        //			if (openItem)
        //			{
        //                      if (openItem.BlackList.Contains(_currretIcon.item.Id))
        //                      {
        //					break;
        //                      }
        //				openItem.BlackList.Add(_currretIcon.item.Id);
        //				foreach (var test in _currretIcon.item.ItemControllers)
        //				{
        //					var wqe = test as OpenItem;
        //					if (wqe)
        //					{
        //						wqe.BlackList.Add(openItem.BlackList);
        //					}
        //				}
        //                      foreach (var id in openItem.BlackList)
        //				{
        //					Debug.Log(id.ToString());
        //				}
        //				break;
        //			}
        //		}
        //		break;
        //	}
        //      }

        if (_oldCells != null)
        {
            foreach (var cell in _oldCells)
            {
                cell.SetColor(HigthLigthColor);
            }

            _oldCells = null;
        }

        foreach (InventoryCells cell in targetCell)
        {
            cell.ItemId = _currretIcon.Item.Id;
        }

        _currretIcon.SetRaycastToPointTarget(true);

        _currretIcon.transform.position = GetCenterPosition(targetCell, _currretInventory);
        _currretIcon.RectTransform.SetParent(_currretInventory.ContentRect);

        _currretIcon.RectTransform.sizeDelta = LastItemSize;

        _currretIcon.Inventory = _currretInventory;

        _currretIcon.IconBlock = false;

        _currretIcon.Inventory.AddIcon(_currretIcon);

        _currretIcon = null;


        ColorizeLockedCells();
        //if (current != null) Destroy(current.gameObject);
    }


    private bool Similar() // поиск похожей иконки
    {
        InventoryIcon t = RaycastUI(Input.mousePosition);

        if (t == null || t.Item.Id != _currretIcon.Item.Id)
        {
            return false;
        }


        if (!(t.Item.IsResource) || t.Counter + _currretIcon.Counter > t.Item.StackLimit)
        {
            return false;
        }



        if (_oldCells != null)
        {
            foreach (var cell in _oldCells)
            {
                cell.SetColor(HigthLigthColor);
            }
        }

        _oldCells = null;

        t.Counter += _currretIcon.Counter;
        t.IconCountText.text = t.Counter.ToString();

        DestroyImmediate(_currretIcon.gameObject);

        _currretIcon = null;


        return true;
    }

    //private bool CheckIdInBlackList(int id)
    //{
    //	foreach (var controller in _currretIcon.item.ItemControllers)
    //	{
    //		if (controller is OpenItem open && open.CheckIdInBlackList(id))
    //		{
    //			return true;
    //		}
    //	}
    //	return false;
    //}

    //private bool CheckIdInWhiteList(int id)
    //{
    //	foreach (var controller in _currretIcon.item.ItemControllers)
    //	{
    //		if (controller is OpenItem open && open.CheckIdInWhiteList(id))
    //		{
    //			return true;
    //		}
    //	}
    //	return false;
    //}

    private List<InventoryCells> GetNotOverlapCell(RectTransform[,] rectTransform) // рейкаст по UI клеткам инвентаря
    {
        var cells = new List<InventoryCells>();

        foreach (RectTransform tr in rectTransform)
        {
            if (tr == null)
            {
                continue;
            }

            InventoryCells t = RaycastUI(_currretInventory.Id, tr.position);

            if (t == null || t.IsLocked)
            {
                continue;
            }

            //         if (t.IsLocked)
            //{
            //	t.SetColor(HigthLigthColor2);
            //	cells.Add(t);
            //	continue;
            //}

            if (cells.Contains(t) == false)
            {
                t.SetColor(HigthLigthColor1);
                cells.Add(t);
            }
        }

        if (cells.Count == 0)
        {
            return null;
        }

        //if (cells != null)
        //{
        //	foreach (var cell in cells)
        //	{
        //		//cell.SetColor(color);
        //	}
        //}
        if (_currretIcon.Item.IsOpen && _currretInventory.Id == _currretIcon.Item.Id || cells.Count != rectTransform.Length - rectTransform.CountNull())
        {
            color = HigthLigthColor2;
            return cells;
        }


        //if (CheckIdInBlackList(_currretInventory.Id) || cells.Count != rectTransform.Length - rectTransform.CountNull())
        //      {
        //          color = HigthLigthColor2;
        //          return cells;
        //      }



        color = HigthLigthColor1;
        return cells;
    }

    private void AddCurrentIcon(Inventory inventory, List<InventoryCells> targetCell) // добавление иконки
    {
        _currretIcon.transform.position = GetCenterPosition(targetCell, inventory);

        foreach (InventoryCells cell in targetCell)
        {
            cell.ItemId = _currretIcon.Item.Id;
        }

        _currretIcon.RectTransform.SetParent(inventory.ContentRect);

        _currretIcon.Inventory.AddIcon(_currretIcon);

        _currretIcon = null;
    }

    public InventoryIcon SetIcon(Inventory inventory, Item item, int count, bool isBlock) // создание и настройка новой иконки
    {
        InventoryIcon clone = Instantiate(_iconPrefab, inventory.ContentRect).GetComponent<InventoryIcon>();

        clone.Item = item;
        clone.Counter = count;

        clone.UpdateSprite();

        clone.gameObject.name = item.Name;
        clone.RectTransform.localScale = Vector3.one;

        clone.UpdateCells();
        clone.IconBlock = isBlock;

        clone.Inventory = inventory;
        return clone;
    }

    public void OpenInventory(string name, string id, int width, int height)
    {
        int i = 0;
        foreach (var inventory in _inventors)
        {
            if (inventory.Id == id)
            {
                inventory.transform.SetAsLastSibling();
                _inventors.Move(i, _inventors.Count - 1);
                return;
            }
            i++;
        }
        var newInventoryRect = Instantiate(_inventoryPrefab).GetComponent<RectTransform>();
        var newInventory = newInventoryRect.GetComponent<Inventory>();

        newInventoryRect.SetParent(gameObject.transform);
        newInventoryRect.anchoredPosition = Vector2.zero;
        newInventoryRect.localScale = Vector3.one;
        newInventory.Open(name, id, width, height);

        _inventors.Add(newInventory);

        newInventory.BuildGrid();

        newInventory.LoadIcons();

        ColorizeLockedCells();
    }

    public void DeleteItem(Item item)
    {
        File.Delete(GlobalParametrs.ItemsFolderPath + item.Id + ".save");

        foreach (var inventory in _inventors)
        {
            inventory.DeleteItems(item);
        }

        ItemManager.Instance.Items.Remove(item);
        ColorizeLockedCells();
    }

    public void DeleteIcon(InventoryIcon icon)
    {
        icon.Inventory.RemoveIcon(icon);

        SetItemCell(icon.Inventory.Id, icon.element, null);

        DestroyImmediate(icon.gameObject);
    }

#if UNITY_EDITOR
    [ContextMenu("Build grids for all")]
#endif
    public void BuildGridsForAll() // инструмент для создания сетки
    {
#if UNITY_EDITOR
        Undo.RecordObject(gameObject, "Refill List");
#endif
        ReFillListInvetors();

        foreach (var inventory in _inventors)
        {
            inventory.BuildGrid();
        }
    }

}

//#if UNITY_EDITOR
//using UnityEngine;
//using System.Collections;
//using UnityEditor;

//[CustomEditor(typeof(InventoryControl))]

//public class InventoryEditor : Editor {
//	public override void OnInspectorGUI()
//	{

//		DrawDefaultInspector();
//		InventoryControl e = (InventoryControl)target;
//		GUILayout.Label("Build Grid:", EditorStyles.boldLabel);
//		if (GUILayout.Button("Create / Update"))
//		{
//			e.BuildGridsForAll();
//		}
//		GUILayout.Label("ReFill Invetors:", EditorStyles.boldLabel);
//		if (GUILayout.Button("ReFill"))
//		{
//			e.ReFillListInvetors();
//		}
//	}
//}
//#endif