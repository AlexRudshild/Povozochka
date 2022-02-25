using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellsEditor : MonoBehaviour
{
    private const int minI = 1;

    [SerializeField] private IconEditor iconEditor;

    [Space]

    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private RectTransform _gridRect;

    [Space]

    [SerializeField] private Image _image;
    [SerializeField] private TMP_InputField _height;
    [SerializeField] private TMP_InputField _width;

    [SerializeField] private GameObject _PrefabCell;

    public int Height = minI;
    public int Width = minI;

    public bool[,] Cells = null;

    public void UpdateHeight()
    {
        UpdateCells();
        if (_height.text != string.Empty)
        {
            Height = int.Parse(_height.text);
        }
        else
        {
            Height = minI;
            _height.text = "1";
        }

        UpdateGrid();
    }

    public void UpdateWidth()
    {
        UpdateCells();
        if (_width.text != string.Empty)
        {
            Width = int.Parse(_width.text);
        }
        else
        {
            Width = minI;
            _width.text = "1";
        }

        UpdateGrid();
    }

    public void SetCells(bool[,] cells, Sprite sprite)
    {
        Cells = cells;
        Width = cells.GetLength(minI);
        Height = cells.GetLength(0);
        _image.sprite = sprite;
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        Width = Width < minI ? minI : Width;
        Height = Height < minI ? minI : Height;

        _gridRect.sizeDelta = new Vector2(500 * Mathf.Clamp((float)Width / Height, 0f, 1f), 500 * Mathf.Clamp((float)Height / Width, 0f, 1f));
        _grid.cellSize = new Vector2(_gridRect.sizeDelta.x / Width, _gridRect.sizeDelta.y / Height);

        _height.text = Height.ToString();
        _width.text = Width.ToString();

        foreach (var cell in _grid.transform.GetComponentsInChildren<CellEdit>())
        {
            DestroyImmediate(cell.gameObject);
        }

        if (Cells == null)
        {
            Cells = new bool[,] { { true } };
        }

        var cells = new bool[Height, Width];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Cells.GetLength(1) > x && Cells.GetLength(0) > y)
                {
                    cells[y, x] = Cells[y, x];
                }
                else
                {
                    cells[y, x] = true;
                }

                var cell = Instantiate(_PrefabCell, _grid.transform).GetComponent<CellEdit>();
                cell.IsEnable = cells[y, x];
            }
        }
        Cells = cells;
    }

    public void UpdateCells()
    {
        var cells = _grid.transform.GetComponentsInChildren<CellEdit>();
        int i = 0;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Cells[y, x] = cells[i].IsEnable;
                i++;
            }
        }
    }

    public void Save()
    {
        UpdateCells();

        iconEditor.SetCells(Cells);

        this.gameObject.SetActive(false);
    }
}
