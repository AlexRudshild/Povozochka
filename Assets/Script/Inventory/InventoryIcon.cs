using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryIcon : MonoBehaviour
{
    public RectTransform RectTransform = null;

    public Image IconImage = null;
    public Image TokenImage = null;
    public Image CornerImage = null;

    public Text IconCountText = null;
    public Item Item;
    public int Counter;
    public RectTransform[,] element;

    private bool _iconBlock = true;
    public bool IconBlock
    {
        get { return _iconBlock; }
        set
        {
            _iconBlock = value;

            if (_iconBlock)
            {
                ColorClear = new Color(255, 0, 0, 0.5f);
            }
            else
            {
                ColorClear = Color.clear;
            }

            SetBackImage(false);
        }
    }

    public Color ColorColorize = new Color32(255, 255, 255, 80);

    public Color ColorClear = Color.clear;

    public Inventory Inventory { get; set; }

    private readonly float doubleClickTimeLimit = 0.5f;

    private IEnumerator ClickEvent()
    {
        var rect = new Rect(Input.mousePosition.ToVector2().Add(-3f), Input.mousePosition.ToVector2().Add(3f));
        //pause a frame so you don't pick up the same mouse down event.
        yield return null; // wait for the next frame

        float count = 0f;
        while (count < doubleClickTimeLimit && Input.mousePosition.PointIsInside(rect))
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoubleClick();
                yield break;
            }
            count += Time.deltaTime;// increment counter by change in time between frames
            yield return null; // wait for the next frame
        }
        if (Input.GetMouseButton(0))
        {
            SetBackImage(false);
            InventoryControler.Instance.BeginDrag(this);
        }
        yield break;
    }

    public static bool operator true(InventoryIcon icon)
    {
        return icon != null;
    }

    public static bool operator false(InventoryIcon icon)
    {
        return icon == null;
    }

    private void DoubleClick()
    {
        InventoryControler.Instance.DoubleClick(this);
    }

    public void UpdateCells()
    {
        var iconHeight = Item.Cells.GetLength(0);
        var iconWidth = Item.Cells.GetLength(1);

        foreach (var point in GetComponentsInChildren<PointTarget>())
        {
            DestroyImmediate(point.gameObject);
        }

        var cellsSize = GlobalParametrs.CellsSize;

        var iconSize = new Vector2(cellsSize * iconWidth, cellsSize * iconHeight);
        RectTransform.sizeDelta = iconSize;

        element = new RectTransform[iconHeight, iconWidth];

        for (int y = 0; y < iconHeight; y++)
        {
            for (int x = 0; x < iconWidth; x++)
            {
                if (Item.Cells[y, x])
                {
                    var curElement = Instantiate(InventoryControler.Instance.PointPrefab, transform).GetComponent<RectTransform>();
                    curElement.gameObject.name = "point-" + x + ":" + y;

                    element[y, x] = curElement;
                    curElement.localScale = Vector3.one;

                    curElement.sizeDelta = new Vector2(cellsSize, cellsSize);
                    curElement.anchoredPosition = new Vector2(-cellsSize / 2f * (iconWidth - 1) + cellsSize * x, cellsSize / 2f * (iconHeight - 1) - cellsSize * y);
                }
            }
        }

        IconBlock = true;
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
                IconCountText.text = Counter.ToString();
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

    public void BeginDrag()
    {
        StartCoroutine(ClickEvent());
    }

    public void SetBackImage(bool value)
    {
        if (element == null)
        {
            return;
        }
        foreach (var item in element)
        {
            if (item == null) continue;

            if (value)
            {
                item.gameObject.GetComponent<Image>().color = ColorColorize;
            }
            else
            {
                item.gameObject.GetComponent<Image>().color = ColorClear;
            }
        }
    }

    public void SetRaycastToPointTarget(bool value)
    {
        foreach (var item in element)
        {
            if (item == null) continue;

            item.gameObject.GetComponent<Image>().raycastTarget = value;
        }
    }


    void DoOperation(double x, double y, Operation op)
    {
        double result = op switch
        {
            Operation.Add => x + y,
            Operation.Subtract => x - y,
            Operation.Multiply => x * y,
            Operation.Divide => x / y,
            _ => throw new System.NotImplementedException()
        };
    }

    enum Operation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
}