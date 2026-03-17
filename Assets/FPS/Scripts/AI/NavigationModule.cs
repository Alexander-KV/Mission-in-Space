using UnityEngine;
using UnityEngine.AI; // Добавьте этот using

namespace Unity.FPS.AI
{
    // Component used to override values on start from the NavmeshAgent component in order to change
    // how the agent is moving
    public class NavigationModule : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("The maximum speed at which the enemy is moving (in world units per second).")]
        public float MoveSpeed = 0f;

        [Tooltip("The maximum speed at which the enemy is rotating (degrees per second).")]
        public float AngularSpeed = 0f;

        [Tooltip("The acceleration to reach the maximum speed (in world units per second squared).")]
        public float Acceleration = 0f;

        // Приватная ссылка на NavMeshAgent
        private NavMeshAgent m_NavMeshAgent;

        private void Awake()
        {
            // Пытаемся найти NavMeshAgent на этом же объекте или на родителе
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            if (m_NavMeshAgent == null)
            {
                m_NavMeshAgent = GetComponentInParent<NavMeshAgent>();
            }

            if (m_NavMeshAgent == null)
            {
                Debug.LogError($"NavigationModule на {gameObject.name} не может найти NavMeshAgent!", this);
            }
        }

        private void Start()
        {
            // Применяем параметры к агенту при старте
            ApplyParameters();
        }

        // Метод для применения параметров к агенту
        public void ApplyParameters()
        {
            if (m_NavMeshAgent != null)
            {
                if (MoveSpeed > 0)
                    m_NavMeshAgent.speed = MoveSpeed;

                if (AngularSpeed > 0)
                    m_NavMeshAgent.angularSpeed = AngularSpeed;

                if (Acceleration > 0)
                    m_NavMeshAgent.acceleration = Acceleration;
            }
        }

        // ⚡⚡⚡ НОВЫЙ МЕТОД - сброс навигации ⚡⚡⚡
        public void ResetNavigation()
        {
            if (m_NavMeshAgent == null)
            {
                m_NavMeshAgent = GetComponent<NavMeshAgent>() ?? GetComponentInParent<NavMeshAgent>();
            }

            if (m_NavMeshAgent != null)
            {
                Debug.Log($"Сброс навигации для {gameObject.name}");

                // Включаем агента
                m_NavMeshAgent.enabled = true;

                // Останавливаем текущее движение
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.ResetPath();
                m_NavMeshAgent.velocity = Vector3.zero;

                // Применяем параметры заново
                ApplyParameters();

                // Важно! Проверяем, находится ли агент на NavMesh
                if (!m_NavMeshAgent.isOnNavMesh)
                {
                    Debug.LogWarning($"{gameObject.name} не на NavMesh, пробуем восстановить...");
                    WarpToNavMesh();
                }
            }
        }

        // Метод для принудительного перемещения на NavMesh
        public void WarpToNavMesh()
        {
            if (m_NavMeshAgent == null) return;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            {
                m_NavMeshAgent.Warp(hit.position);
                transform.position = hit.position;
                Debug.Log($"{gameObject.name} перемещен на NavMesh");
            }
            else
            {
                Debug.LogError($"{gameObject.name} не может найти NavMesh поблизости!");
            }
        }

        // Вспомогательный метод для проверки состояния
        public bool IsOnNavMesh()
        {
            return m_NavMeshAgent != null && m_NavMeshAgent.isOnNavMesh;
        }

        // Вспомогательный метод для получения агента
        public NavMeshAgent GetAgent()
        {
            return m_NavMeshAgent;
        }
    }
}