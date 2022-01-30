using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Damager : MonoBehaviour
{
    [Range(1, 100)]
    public int damageAmount = 50;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameController.Singleton.Damage(damageAmount);
    }
}
