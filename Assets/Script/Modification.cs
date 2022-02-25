using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ListMod
{
	public static void Move<T>(this List<T> list, int index, int j)
	{
		var elem = list[index];
		list.RemoveAt(index);
		list.Insert(j, elem);
	}

	public static void Swap<T>(this List<T> list, int i, int j)
	{
		var elem1 = list[i];
		var elem2 = list[j];

		list[i] = elem2;
		list[j] = elem1;
	}

	public static void Add<T>(this List<T> list, List<T> value)
	{
		foreach (var item in value)
		{
			list.Add(item);
		}
	}

	public static void Remove<T>(this List<T> list, List<T> value)
	{
		foreach (var item in value)
		{
			list.Remove(item);
		}
	}
}

public static class Vector3Mod
{
	public static bool PointIsInside(this Vector3 point, RectTransform corners)
	{
		var worldCorners = new Vector3[4];
		corners.GetWorldCorners(worldCorners);

		if (point.x > worldCorners[0].x && point.x < worldCorners[2].x
			&& point.y > worldCorners[0].y && point.y < worldCorners[2].y)
			return true;

		return false;
	}

	public static bool PointIsInside(this Vector3 point, Rect corners)
	{
		if (point.x > corners.x && point.x < corners.width
			&& point.y > corners.y && point.y < corners.height)
			return true;

		return false;
	}

	public static Vector3 Add(this Vector3 a, float b)
	{
		return new Vector3(a.x + b, a.y + b, a.z + b);
	}
}

public static class Vector2Mod
{
	public static bool PointIsInside(this Vector2 point, RectTransform corners)
	{
		var worldCorners = new Vector3[4];
		corners.GetWorldCorners(worldCorners);

		if (point.x > worldCorners[0].x && point.x < worldCorners[2].x
			&& point.y > worldCorners[0].y && point.y < worldCorners[2].y)
			return true;

		return false;
	}

	public static bool PointIsInside(this Vector2 point, Rect corners)
	{
		if (point.x > corners.x && point.x < corners.width
			&& point.y > corners.y && point.y < corners.height)
			return true;

		return false;
	}

	public static Vector2 ToVector2(this Vector3 a)
	{
		return new Vector2(a.x, a.y);
	}

	public static Vector2 Add(this Vector2 a, float b)
	{
		return new Vector2(a.x + b, a.y + b);
	}
}

public static class StringMod
{
	public static string FileName(this string path)
	{
		var split = path.Split('/');
		return split[split.Length - 1].Split('.')[0];
	}
	public static Color ToColorRBGA(this string color)
	{
		var rgba = color.Replace("RGBA(", "").Replace(")", "").Split(',').Replace(".", ",").ToFloat();
		return new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
	}

	public static int[] ToInt(this string[] _string)
	{
		var value = new int[_string.Length];
		for (int i = 0; i < _string.Length; i++)
		{
			value[i] = int.Parse(_string[i]);
		}
		return value;
	}

	public static float[] ToFloat(this string[] _string)
	{
		var value = new float[_string.Length];
		for (int i = 0; i < _string.Length; i++)
		{
			value[i] = float.Parse(_string[i]);
		}
		return value;
	}

	public static string[] Replace(this string[] _string, string oldValue, string newValue)
	{
		for (int i = 0; i < _string.Length; i++)
		{
			_string[i] = _string[i].Replace(oldValue, newValue);
		}
		return _string;
	}

	public static IEnumerable<string> ParseTokens(this string input)
	{
		var charItor = input.GetEnumerator();

		while (charItor.MoveNext())
		{
			yield return ReadToken(charItor);
		}
	}

	private static string ReadToken(CharEnumerator charItor)
	{
		var sb = new StringBuilder();

		int isIn = 0;
		do
		{
			if (charItor.Current == ',' && isIn <= 0)
			{
				break;
			}
			if (charItor.Current == '}')
			{
				isIn--;
			}
			if (charItor.Current == '{')
			{
				isIn++;
			}
			sb.Append(charItor.Current);
		}
		while (charItor.MoveNext());

		return sb.ToString();
	}

	private static bool IsEndOfTheToken(char c)
	{
		return c != ',';
	}
}

public static class RectTransformMod
{
	public static int GetLengthIgnoreNull(this RectTransform[,] rects)
	{
		int count = 0;
		foreach (var rect in rects)
		{
			if (rect != null)
				count++;
		}
		return count;
	}
	public static int CountNull(this RectTransform[,] rects)
	{
		int count = 0;
		foreach (var rect in rects)
		{
			if (rect == null)
				count++;
		}
		return count;
	}
}


public static class ResourceEnumMod
{
	public static string GetSize(this ResourceEnum resourceEnum)
	{
		return resourceEnum.ToString().Replace("Size", "");
	}

	public static bool[,] CellsFromSize(this ResourceEnum resource)
	{
		switch (resource)
		{
			case ResourceEnum.Size2x2:
				return new bool[,] { { true, true }, { true, true } };
			case ResourceEnum.Size4x2:
				return new bool[,] { { true, true }, { true, true }, { true, true }, { true, true } };
			default:
				return new bool[,] { { true } };
		}
	}

	public static Sprite GetTokenSprite(this IconTypeEnum iconType)
	{
		if (iconType == IconTypeEnum.InventoryIcon)
		{
			return null;
		}
		return Resources.Load<Sprite>("tokens/" + iconType.ToString());
	}

	public static Sprite GetCornrerSprite(this IconTypeEnum iconType)
	{
		if (iconType == IconTypeEnum.InventoryIcon)
		{
			return null;
		}
		return Resources.Load<Sprite>("tokens/Corner" + iconType.ToString());
	}

	public static Sprite GetTokenSprite(this ResourceEnum resourceEnum)
	{
		return Resources.Load<Sprite>("tokens/Item/" + resourceEnum.ToString());
	}

	public static Sprite GetCornrerSprite(this ResourceEnum resourceEnum)
	{
		return Resources.Load<Sprite>("tokens/Item/Corner" + resourceEnum.ToString());
	}
}

