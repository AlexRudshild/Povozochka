using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveItem
{
    [System.Serializable]
    public struct Col
    {
        public float r, g, b, a;

        Col(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public void ToCol(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }

        public override string ToString()
        {
            return r + "," + g + "," + b + "," + a;
        }
    }

    public string Name;
    public string Description;
    public string Id;
    public string SpritePath;
    public bool[,] Cells;
    public ItemController Controller;
    public ResourceEnum ResourceEnum;
    public int StackLimit;
    public Col Color;
    public Col TokenColor;
    public Col CornerColor;

    public SaveItem(Item item)
    {
        Name = item.Name;
        Description = item.Description;
        Id = item.Id;
        SpritePath = item.SpritePath;
        Cells = item.Cells;
        Controller = item.Controller;
        ResourceEnum = item.ResourceEnum;
        StackLimit = item.StackLimit;
        Color.ToCol(item.Color);
        TokenColor.ToCol(item.TokenColor);
        CornerColor.ToCol(item.CornerColor);
    }

    public Item ToItem()
    {
        var item = new Item();
        item.Name = Name;
        item.Description = Description;
        item.Id = Id;
        item.SpritePath = SpritePath;
        item.Controller = Controller;
        item.ResourceEnum = ResourceEnum;

        if (ResourceEnum != ResourceEnum.none)
            item.Cells = ResourceEnum.CellsFromSize();
        else 
            item.Cells = Cells;

        item.StackLimit = StackLimit;
        item.Color = Color.ToColor();
        item.TokenColor = TokenColor.ToColor();
        item.CornerColor = CornerColor.ToColor();
        return item;
    }
}
