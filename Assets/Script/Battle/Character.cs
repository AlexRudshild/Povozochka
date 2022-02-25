using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Character : MonoBehaviour
{
    [SerializeField] private TMP_Text textHealth = null;
    [SerializeField] private TMP_Text textDamage = null;

    private int _health = 0;
    private int _damage = 0;

    public int Damage => _damage;

    public void SetStats(int health, int damage)
    {
        _health = health;
        _damage = damage;

        textHealth.text = health.ToString(); 
        textDamage.text = damage.ToString(); 
    }

    public void MoveTo(Vector3 position)
    {
        transform.position = position;
    }

    public void MinusHp(int value)
    {
        _health -= value;
        textHealth.text = _health.ToString(); 
    }

    public void AddHp(int value)
    {
        _health += value;
        textHealth.text = _health.ToString(); 
    }
}
