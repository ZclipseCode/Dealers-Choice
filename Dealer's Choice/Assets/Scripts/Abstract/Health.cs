using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    [SerializeField] int totalHealth = 1;
    int currentHealth;

    private void Start()
    {
        currentHealth = totalHealth;
    }

    public void Damage(int damage)
    {
        if (currentHealth - damage > 0)
        {
            currentHealth -= damage;
        }
        else
        {
            currentHealth = 0;
            Death();
        }
    }

    public void Heal(int heal)
    {
        if (currentHealth + heal > totalHealth)
        {
            currentHealth = totalHealth;
        }
        else
        {
            currentHealth += heal;
        }
    }

    public abstract void Death();
}
