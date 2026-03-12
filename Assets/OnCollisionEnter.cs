using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 10;

    void OnCollisionEnter(Collision collision)
    {
        // Проверяем, столкнулись ли с врагом
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Достаём скрипт EnemyHealth и вызываем TakeDamage
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }
}