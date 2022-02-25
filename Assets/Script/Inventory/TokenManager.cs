using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenManager : MonoBehaviour
{
    public static TokenManager Instance = null;

    public Sprite[] ItemSprites = null;

    public List<Token> Tokens = null;


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        //var sprits = GetSprits();

        Tokens = new List<Token>();


        //Tokens.Add(new Token("arbalet", TokenEnum.Skill, ItemSprites[0], 1, Color.white, Color.white));
        /*
        foreach (var item in sprits)
        {
            Tokens.Add(new Token(item.name, item, war(item.texture), 1, Color.white));
        }*/
    }
}
