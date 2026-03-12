using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50; // Максимальное здоровье
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // Задаём начальное здоровье
    }

    // Метод для нанесения урона
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Враг получил урон: " + amount + " | HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Враг убит!");
        Destroy(gameObject); // Уничтожаем врага
    }
}