using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockWall : MonoBehaviour
{
    [SerializeField] EnemyHealthSystem enemyHealthSystem;

    void Update()
    {
        // Eliminar al enemigo pasado por parámetro desde el Inspector desbloquea el camino bloqueado por el muro
        if (enemyHealthSystem != null && enemyHealthSystem.isDead) { 
            Destroy(this.gameObject);
        }
    }
}
