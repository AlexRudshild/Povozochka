using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class InventorySaver
{
    //private static bool _isCrypt = false;
    //private static string nullCell = "*"; // символ, который обозначает пустую ячейку

    //private static string Crypt(string text)
    //{
    //	if (!_isCrypt) return text;

    //	string result = string.Empty;
    //	foreach (char j in text)
    //	{
    //		result += (char)((int)j ^ 47);
    //	}
    //	return result;
    //}

    private static string Path(string uniqName) // путь сохранения
    {
        if (!Directory.Exists(GlobalParametrs.InvetoryFolderPath))
        {
            Directory.CreateDirectory(GlobalParametrs.InvetoryFolderPath);
        }

        return GlobalParametrs.InvetoryFolderPath + "/" + uniqName + ".save";
    }

    public static void Save(Inventory inventory, List<InventoryIcon> icons)
    {
        FileStream file = new FileStream(Path(inventory.Id), FileMode.Create);

        BinaryFormatter binary = new BinaryFormatter();
        binary.Serialize(file, new SaveIcons(icons));
        file.Close();

        StreamWriter writer = new StreamWriter(GlobalParametrs.InvetoryFolderPath + "test.json");
        var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
        jsonSerializer.Serialize(writer, new SaveIcons(icons));
        writer.Close();

        //StreamWriter writer = new StreamWriter(Path(inventory.Id.ToString()));

        //foreach (InventoryIcon i in Items)
        //{
        //	writer.WriteLine(Crypt("itemId=" + i.Item.Id));
        //	writer.WriteLine(Crypt("counter=" + i.counter));
        //	writer.WriteLine(Crypt("pX=" + i.RectTransform.localPosition.x));
        //	writer.WriteLine(Crypt("pY=" + i.RectTransform.localPosition.y));
        //	writer.WriteLine(Crypt("rZ=" + i.RectTransform.rotation.z));
        //	writer.WriteLine(Crypt("rW=" + i.RectTransform.rotation.w));
        //	writer.WriteLine(string.Empty); // пустая строка, обязательный ключ, запускает создание иконки
        //}

        //string field = string.Empty;
        //foreach (var cell in inventory.Cells)
        //{
        //	if (field.Length > 0) field += ",";
        //	if (cell.ItemId == "") field += nullCell; else field += cell.ItemId;
        //}

        //writer.WriteLine(Crypt("field=" + field));

        //writer.Close();
    }

    public static List<InventoryIcon> Load(Inventory inventory)
    {
        if (!File.Exists(Path(inventory.Id)))
        {
            return new List<InventoryIcon>();
        }

        var file = new FileStream(Path(inventory.Id), FileMode.Open);

        BinaryFormatter binary = new BinaryFormatter();
        SaveIcons save;
        try
        {
            save = (SaveIcons)binary.Deserialize(file);
        }
        catch (System.Exception)
        {
            file.Close();
            File.Delete(Path(inventory.Id));
            return new List<InventoryIcon>();
        }
        file.Close();

        return save.GetIcons(inventory);

        //var Items = new List<InventoryIcon>();

        //InventoryIcon tmp = null;
        //string itemId = "";
        //int counter = 0;
        //Vector2 pos = Vector2.zero;
        //Vector2 rot = Vector2.zero;

        //string[] field = new string[] { };

        //StreamReader reader = new StreamReader(Path(inventory.Id.ToString()));

        //while (!reader.EndOfStream)
        //{
        //	string value = Crypt(reader.ReadLine());

        //	string[] result = value.Split(new char[] { '=' });

        //	switch (result[0]) // фильтруем ключи
        //	{
        //		case "itemId":
        //			itemId = result[1];
        //			break;
        //		case "counter":
        //			counter = int.Parse(result[1]);
        //			break;
        //		case "pX":
        //			pos.x = float.Parse(result[1]);
        //			break;
        //		case "pY":
        //			pos.y = float.Parse(result[1]);
        //			break;
        //		case "rZ":
        //			rot.x = float.Parse(result[1]);
        //			break;
        //		case "rW":
        //			rot.y = float.Parse(result[1]);
        //			break;
        //			break;
        //		case "field":
        //			field = result[1].Split(new char[] { ',' });
        //			break;
        //	}

        //    if (value == string.Empty)
        //    {
        //        Item curItem = null;
        //        foreach (var item in ItemManager.Instance.Items)
        //        {
        //            if (item.Id == itemId)
        //            {
        //                curItem = item;
        //                break;
        //            }
        //        }

        //        if (curItem != null)
        //        {
        //            tmp = InventoryControler.Instance.SetIcon(inventory, curItem, counter);
        //            tmp.RectTransform.localPosition = pos;
        //            tmp.RectTransform.localRotation = new Quaternion(0, 0, rot.x, rot.y);
        //            tmp.RectTransform.SetParent(inventory.ContentRect);
        //            Items.Add(tmp);
        //        }
        //    }
        //}

        //if (field.Length > 0 && inventory.Cells.Length == field.Length)
        //{
        //	int i = 0;
        //	foreach (var cell in inventory.Cells)
        //	{
        //		if (field[i].CompareTo(nullCell) == 0)
        //		{
        //			cell.ItemId = "";
        //		}
        //		else
        //		{
        //			cell.ItemId = field[i];
        //		}
        //		i++;
        //	}
        //}

        //reader.Close();

        //return Items;
    }
}

[System.Serializable]
public class SaveIcons
{
    public Icons[] icons = null;

    [System.Serializable]
    public struct Vec3
    {
        public float x, y, z;

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vec3(Vector3 vector)
        {
            this.x = vector.x;
            this.y = vector.y;
            this.z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static Vec3 one => new Vec3(1, 1, 1);
    }

    [System.Serializable]
    public struct Quat
    {
        public float x, y, z, w;

        public Quat(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quat(Quaternion quaternion)
        {
            this.x = quaternion.x;
            this.y = quaternion.y;
            this.z = quaternion.z;
            this.w = quaternion.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }

    [System.Serializable]
    public struct Icons
    {
        public Vec3 pos;
        public bool isFlip;
        public bool isBlock;
        public Quat rot;
        public string id;
        public int count;

        public Icons(InventoryIcon icon)
        {
            pos = new Vec3(icon.RectTransform.localPosition);
            rot = new Quat(icon.RectTransform.rotation);
            isFlip = icon.RectTransform.localScale.x < 0;
            isBlock = icon.IconBlock;
            id = icon.Item.Id;
            count = icon.Counter;
        }
    }

    public SaveIcons(List<InventoryIcon> icons)
    {

        this.icons = new Icons[icons.Count];

        for (int i = 0; i < icons.Count; i++)
        {
            this.icons[i] = new Icons(icons[i]);
        }
    }

    public List<InventoryIcon> GetIcons(Inventory inventory)
    {
        var items = new List<InventoryIcon>();

        foreach (var icon in icons)
        {
            Item curItem = null;

            foreach (var item in ItemManager.Instance.Items)
            {
                if (item.Id == icon.id)
                {
                    curItem = item;
                    break;
                }
            }

            if (curItem)
            {
                var item = InventoryControler.Instance.SetIcon(inventory, curItem, icon.count, icon.isBlock);
                item.RectTransform.localPosition = icon.pos.ToVector3();

                item.RectTransform.localScale = new Vector3(icon.isFlip ? -1 : 1, 1, 1);

                item.RectTransform.localRotation = icon.rot.ToQuaternion();
                items.Add(item);
            }
        }

        return items;
    }
}