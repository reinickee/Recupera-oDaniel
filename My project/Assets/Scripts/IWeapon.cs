using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface que define um comportamento para armas
public interface IWeapon
{
    void Fire();  // M�todo para disparar a arma
    float ReloadTime { get; } // Propriedade para o tempo de recarga da arma
}