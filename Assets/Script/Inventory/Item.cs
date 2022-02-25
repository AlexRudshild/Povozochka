using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class Item
{
	public string Name = string.Empty;
	public string Description = string.Empty;
	public string Id = string.Empty;
	public Sprite Sprite = null;
	public string SpritePath = null;
	public bool[,] Cells = null;
	public ItemController Controller = new ItemController();
	public ResourceEnum ResourceEnum = ResourceEnum.none;
	public int StackLimit = 1;
	public Color Color = Color.white;
	public Color TokenColor = Color.white;
	public Color CornerColor = Color.white;

	public bool IsResource => ResourceEnum != ResourceEnum.none;

	public bool IsInspect => Controller.Drop != null;
	public bool IsEquip => Controller.Equip != null;
	public bool IsUse => Controller.Use != null;
	public bool IsDrop => Controller.Drop != null;
	public bool IsOpen => Controller.Open != null;
	public bool IsEdit => Controller.Edit != null;

	public DropItem Inspect => Controller.Drop;
	public EquipItem Equip => Controller.Equip;
	public UseItem Use => Controller.Use;
	public DropItem Drop => Controller.Drop;
	public OpenItem Open => Controller.Open;
	public EditItem Edit => Controller.Edit;

	public bool ContainController(ControllersEnum controllers)
	{
		switch (controllers)
		{
			case ControllersEnum.InspectItem:
				return IsInspect;
			case ControllersEnum.EquipItem:
				return IsEquip;
			case ControllersEnum.UseItem:
				return IsUse;
			case ControllersEnum.DropItem:
				return IsDrop;
			case ControllersEnum.OpenItem:
				return IsOpen;
		}
		return false;
	}

	public static bool operator true(Item item)
	{
		return item != null;
	}

	public static bool operator false(Item item)
	{
		return item == null;
	}

	public Item()
	{

	}

	public Item(bool randId)
	{
		if (randId)
		{
			Id = RandomId();
		}
		Cells = new bool[,] { { true } };
		Sprite = GlobalParametrs.ErrorSprite;
	}

	public Item(Item item)
	{
		Name = item.Name;
		Description = item.Description;
		Id = RandomId();
		Sprite = item.Sprite;
		SpritePath = item.SpritePath;
		Cells = item.Cells;
		Controller = item.Controller;
		ResourceEnum = item.ResourceEnum;
		StackLimit = item.StackLimit;
		Color = item.Color;
		TokenColor = item.TokenColor;
		CornerColor = item.CornerColor;
	}
	private string RandomId()
	{
		// Получаем количество слов и букв за слово.
		int num_letters = 10;

		// Создаем массив букв, которые мы будем использовать.
		char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();

		Random rand = new Random();

		string id = "";

		for (int j = 1; j <= num_letters; j++)
		{
			int letter_num = rand.Next(0, letters.Length - 1);

			id += letters[letter_num];
		}

		return id;
	}

	public Item(string name, string description, string id, Sprite sprite, string spritePath, bool[,] cells, Color color, ItemController itemControllers)
	{
		Name = name;
		Description = description;
		Id = id;
		Sprite = sprite;
		SpritePath = spritePath;
		Cells = cells;
		Color = color;
		Controller = itemControllers;
	}
	public Item(string name, string description, string id, int stackLimit, Sprite sprite, string spritePath, ResourceEnum resourceEnum, Color color, Color tokenColor, Color cornerColor, ItemController itemControllers)
		: this(name, description, id, sprite, spritePath, resourceEnum.CellsFromSize(), color, itemControllers)
	{
		StackLimit = stackLimit;
		TokenColor = tokenColor;
		ResourceEnum = resourceEnum;
		CornerColor = cornerColor;
	}
}

[System.Serializable]
public enum ResourceEnum
{
	none, Size1x1, Size2x2, Size4x2
}

public enum ControllersEnum
{
	InspectItem, EquipItem, UseItem, DropItem, OpenItem
}

[System.Serializable]
public class ItemController
{
	public InspectItem Inspect;
	public EquipItem Equip;
	public UseItem Use;
	public DropItem Drop;
	public OpenItem Open;
	public EditItem Edit;

	public static bool operator true(ItemController item)
	{
		return item != null;
	}

	public static bool operator false(ItemController item)
	{
		return item == null;
	}

	public override string ToString()
	{
		return "\"" + base.ToString()+ "\": {}";
	}
}

[System.Serializable]
public class InspectItem
{
}

[System.Serializable]
public class EquipItem
{
}

[System.Serializable]
public class UseItem
{
}

[System.Serializable]
public class DropItem
{
}

[System.Serializable]
public class OpenItem
{
	public int Width;
	public int Heigth;

	public bool IsWhiteList = false;
	public List<string> IdList = new List<string>();

	public OpenItem(int width, int heigth)
	{
		Width = width;
		Heigth = heigth;
	}

	public void Open(Item item)
	{
		InventoryControl.Instance.OpenInventory(item.Name, item.Id, Width, Heigth);
	}

	public bool CheckIdInIdList(string id)
	{
		foreach (var i in IdList)
		{
			if (id == i)
			{
				return IsWhiteList;
			}
		}
		return !IsWhiteList;
	}

	public override string ToString()
	{
		return "\"OpenItem\": {" + Width + "," + Heigth + "}";
	}
}

[System.Serializable]
public class EditItem
{
}


public class Token
{
	public string Name { get; private set; }
	public Sprite Sprite { get; private set; }
	public Color SriteColor { get; private set; }
	public Color TokenColor { get; private set; }
	public int StackLimit { get; private set; }

	public Token(string name, int id, Sprite sprite, bool[,] cells, Color color) 
	{

		//Name = name;
		//Type = type;
		//Sprite = sprite;
		//StackLimit = stackLimit; (string name, TokenEnum type, Sprite sprite, int stackLimit, Color spritecolor, Color tokencolor)
		// SriteColor = spritecolor;
		//TokenColor = tokencolor;
	}
}
