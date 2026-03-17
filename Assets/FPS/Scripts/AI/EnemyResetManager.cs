using UnityEngine;
using Unity.FPS.AI;

public class EnemyResetManager : MonoBehaviour
{
    void Start()
    {
        // Даем время на инициализацию всех компонентов
        Invoke(nameof(ResetAllEnemies), 0.2f); // Увеличил задержку до 0.2
    }

    void ResetAllEnemies()
    {
        Debug.Log("=== ПРИНУДИТЕЛЬНЫЙ СБРОС ВСЕХ ВРАГОВ ===");

        // Сбрасываем обычных врагов
        EnemyController[] allEnemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        Debug.Log($"Найдено EnemyController: {allEnemies.Length}");
        foreach (EnemyController enemy in allEnemies)
        {
            if (enemy != null)
            {
                Debug.Log($"Сбрасываю врага: {enemy.gameObject.name}");
                enemy.ResetEnemy();
            }
        }

        // Сбрасываем турели
        EnemyTurret[] allTurrets = FindObjectsByType<EnemyTurret>(FindObjectsSortMode.None);
        Debug.Log($"Найдено EnemyTurret: {allTurrets.Length}");
        foreach (EnemyTurret turret in allTurrets)
        {
            if (turret != null)
            {
                Debug.Log($"Сбрасываю турель: {turret.gameObject.name}");
                turret.ResetTurret();
            }
        }

        Debug.Log("=== СБРОС ЗАВЕРШЕН ===");
    }
}