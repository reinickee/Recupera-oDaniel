using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface que define um comportamento para objetos que podem receber dano
public interface IDamageable
{
    void TakeDamage(float damageAmount, int attackerId);
}
