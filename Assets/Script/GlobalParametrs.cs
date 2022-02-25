using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalParametrs
{
	public static int LayerMaskCell = 8;
	public static int CellsSize = 40; // ширина и высота одной клетки (иконки типа 1:1)
	public static Vector2 ScreenSize = new Vector2(1920, 1080);

	public static Sprite ErrorSprite => Resources.Load<Sprite>("Error");

	public static string SpriteFolderPath = Application.dataPath + "/Sprites/";
	public static string ItemsFolderPath = Application.persistentDataPath + "/Items/";
	public static string InvetoryFolderPath = Application.persistentDataPath + "/Inventory/";
}