using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

public class ItemManager : MonoBehaviour
{
	public static ItemManager Instance;

	public List<Item> Items = new List<Item>();
	//private static readonly bool _isCrypt = false;

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}

		foreach (var path in Directory.GetFiles(GlobalParametrs.ItemsFolderPath))
		{
			if (IsSave(path))
			{
				var item = LoadItem(path);
				if (item != null)
				{
					Items.Add(item);
				}
			}
		}

		foreach (var item in Items)
		{
			SaveItem(item);
		}

		StreamWriter writer = new StreamWriter(GlobalParametrs.ItemsFolderPath + "test.json");
		var jsonSerializer = new JsonSerializer();
		var json = JsonUtility.ToJson(new SaveItem(Items[0]), true);
		jsonSerializer.Serialize(writer, new SaveItem(Items[0]));
		writer.Close();
	}

	private static bool IsImage(string path)
	{
		var extensions = new[] { "png", "jpg", "jpeg" };

		return extensions.Contains(path.Split('.')[path.Split('.').Length - 1]);
	}

	private static bool IsSave(string path)
	{
		var extensions = new[] { "save" };

		return extensions.Contains(path.Split('.')[path.Split('.').Length - 1]);
	}

	private List<Sprite> GetSprites()
	{

		var sprites = new List<Sprite>();

		foreach (var path in Directory.GetFiles(GlobalParametrs.SpriteFolderPath))
		{
			if (IsImage(path))
			{
				var texture = new Texture2D(0, 0);
				texture.filterMode = FilterMode.Bilinear;
				texture.LoadImage(File.ReadAllBytes(path));

				sprites.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
				sprites[sprites.Count - 1].name = path.FileName();
			}
		}

		return sprites;
	}

	public static Sprite GetSprite(string path)
	{
		if (File.Exists(path) && IsImage(path))
		{
			var texture = new Texture2D(0, 0);
			texture.filterMode = FilterMode.Bilinear;
			texture.LoadImage(File.ReadAllBytes(path));

			return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
		}

		return GlobalParametrs.ErrorSprite;
	}

	private static bool[,] FillCellsFromImage(Texture2D texture)
	{
		var height = texture.height / 128;
		var width = texture.width / 128;

		var tt = new bool[height, width];

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				tt[y, x] = false;

				var color = texture.GetPixels(x * 128, texture.height - (y + 1) * 128, 128, 128);

				foreach (var item in color)
				{
					if (item.a != 0)
					{
						tt[y, x] = true;
						break;
					}
				}
			}
		}

		return tt;
	}

	public static void SaveItem(Item item)
	{
		FileStream file = new FileStream(Path(item.Id.ToString()), FileMode.Create);

		BinaryFormatter binary = new BinaryFormatter();
		binary.Serialize(file, new SaveItem(item));
		file.Close();


		//StreamWriter writer = new StreamWriter(Path(item.Id.ToString()));
		//writer.WriteLine(Crypt("[" + item.Name + "]"));
		//writer.WriteLine(Crypt("[" + item.Description.Replace("\n", "\\n") + "]"));
		//writer.WriteLine(Crypt("Id=" + item.Id));
		//writer.WriteLine(Crypt("Sprite=" + item.SpritePath));
		//writer.WriteLine(Crypt("StackLimit=" + item.StackLimit));
		//writer.WriteLine(Crypt("ResourceEnum=" + item.ResourceEnum));
		//writer.WriteLine(Crypt("Color=" + item.Color));
		//writer.WriteLine(Crypt("TokenColor=" + item.TokenColor));
		//writer.WriteLine(Crypt("CornerColor=" + item.CornerColor));

		//string cellsStr = "";
		//for (int y = 0; y < item.Cells.GetLength(0); y++)
		//{
		//    if (y != 0)
		//        cellsStr += ",";
		//    for (int x = 0; x < item.Cells.GetLength(1); x++)
		//    {
		//        if (x != 0)
		//            cellsStr += ",";
		//        else
		//            cellsStr += "{";
		//        cellsStr += item.Cells[y, x];
		//    }
		//    cellsStr += "}";
		//}
		//writer.WriteLine(Crypt("Cells=" + cellsStr));

		//if (item.ItemControllers != null)
		//{
		//    var itemControllers = "";
		//    foreach (var controller in item.ItemControllers)
		//    {
		//        if (itemControllers != "")
		//        {
		//            itemControllers += ",";
		//        }
		//        itemControllers += controller.ToString();
		//    }
		//    writer.WriteLine(Crypt("ItemControllers=" + itemControllers));
		//}
		//writer.Close();
		//await Task.Run(() =>
		//{
		//});
	}

	public static Item LoadItem(string path)
	{
		if (!File.Exists(path)) return null;

		var file = new FileStream(path, FileMode.Open);

		BinaryFormatter binary = new BinaryFormatter();
		SaveItem save = null;

		try
		{
			save = (SaveItem)binary.Deserialize(file);
		}
		catch (System.Exception)
		{ }

		file.Close();

		if (save == null || save.Id != path.FileName())
		{
			File.Delete(path);
			return null;
		}

		var item = save.ToItem();

		item.Sprite = GetSprite(GlobalParametrs.SpriteFolderPath + item.SpritePath);

		return item;

		//string name = string.Empty;
		//string description = string.Empty;
		//int id = 0;
		//int stackLimit = 1;
		//Sprite sprite = null;
		//string spritePath = string.Empty;
		//Color color = Color.white;
		//Color tokenColor = Color.white;
		//Color cornerColor = Color.white;
		//var itemControllers = new List<ItemController>();
		//bool[,] cells = null;
		//ResourceEnum resource = ResourceEnum.Size1x1;

		//StreamReader reader = new StreamReader(path);

		//while (!reader.EndOfStream)
		//{
		//    string value = Crypt(reader.ReadLine());

		//    string[] result = value.Split(new char[] { '=' });

		//    if (result.Length == 1 && name == string.Empty)
		//    {
		//        name = value.Trim(new char[] { '[', ']' });
		//        continue;
		//    }
		//    else if (result.Length == 1 && description == string.Empty)
		//    {
		//        description = value.Trim(new char[] { '[', ']' }).Replace("\\n", "\n");
		//        continue;
		//    }

		//    switch (result[0]) // фильтруем ключи
		//    {
		//        case "Id":
		//            id = int.Parse(result[1]);
		//            break;
		//        case "ResourceEnum":
		//            resource = (ResourceEnum)Enum.Parse(typeof(ResourceEnum), result[1]);
		//            break;
		//        case "StackLimit":
		//            stackLimit = int.Parse(result[1]);
		//            break;
		//        case "Sprite":
		//            sprite = GetSprite(GlobalParametrs.SpriteFolderPath + result[1]);
		//            spritePath = result[1];
		//            break;
		//        case "Color":
		//            color = result[1].ToColorRBGA();
		//            break;
		//        case "TokenColor":
		//            tokenColor = result[1].ToColorRBGA();
		//            break;
		//        case "CornerColor":
		//            cornerColor = result[1].ToColorRBGA();
		//            break;
		//        case "Cells":
		//            int w = 0;

		//            var cellList = new List<List<bool>>();

		//            foreach (var cellsY in StringMod.ParseTokens(result[1]))
		//            {
		//                var cellsX = cellsY.Trim(new char[] { '{', '}' }).Split(',');

		//                cellList.Add(new List<bool>());

		//                for (int x = 0; x < cellsX.Length; x++)
		//                {
		//                    cellList[w].Add(bool.Parse(cellsX[x]));
		//                }

		//                w++;
		//            }
		//            if (cells == null)
		//                cells = new bool[cellList.Count, cellList[0].Count];

		//            for (int y = 0; y < cellList.Count; y++)
		//            {
		//                for (int x = 0; x < cellList[y].Count; x++)
		//                {
		//                    cells[y, x] = cellList[y][x];
		//                }
		//            }
		//            break;
		//        case "ItemControllers":
		//            foreach (var itemController in StringMod.ParseTokens(result[1]))
		//            {
		//                if (itemController.Contains("OpenItem"))
		//                {
		//                    var openItem = itemController.Replace("\"OpenItem\": ", "").Trim(new char[] { '{', '}' }).Split(',').ToInt();
		//                    itemControllers.Add(new OpenItem(openItem[0], openItem[1]));
		//                }
		//                else if (itemController.Contains("UseItem"))
		//                {
		//                    itemControllers.Add(new UseItem());
		//                }
		//                else if (itemController.Contains("InspectItem"))
		//                {
		//                    itemControllers.Add(new InspectItem());
		//                }
		//                else if (itemController.Contains("DropItem"))
		//                {
		//                    itemControllers.Add(new DropItem());
		//                }
		//            }
		//            break;
		//    }
		//}

		//if (resource != ResourceEnum.none)
		//{
		//    item = new Item(name, description, id, stackLimit, sprite, spritePath, resource, color, tokenColor, cornerColor, itemControllers);
		//}
		//else
		//{
		//    if (sprite == null)
		//    {
		//        return null;
		//    }
		//    if (cells == null)
		//    {
		//        cells = FillCellsFromImage(sprite.texture);
		//    }
		//    item = new Item(name, description, id, sprite, spritePath, cells, color, itemControllers);
		//}

		//reader.Close();
	}

	private static string Path(string uniqName) // путь сохранения
	{
		if (!Directory.Exists(GlobalParametrs.ItemsFolderPath)) Directory.CreateDirectory(GlobalParametrs.ItemsFolderPath);
		return GlobalParametrs.ItemsFolderPath + "/" + uniqName + ".save";
	}

	//private static string Crypt(string text)
	//{
	//    if (!_isCrypt) return text;

	//    string result = string.Empty;
	//    foreach (char j in text)
	//    {
	//        result += (char)((int)j ^ 47);
	//    }
	//    return result;
	//}

	public void AddItem(Sprite item)
	{
		//Items.Add(new Item(item.name, Items.Count, item, FillCellsFromImage(item.texture), Color.white, ));
	}

}