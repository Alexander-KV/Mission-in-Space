using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveKillEnemies : Objective
    {
        [Tooltip("Chose whether you need to kill every enemies or only a minimum amount")]
        public bool MustKillAllEnemies = true;

        [Tooltip("If MustKillAllEnemies is false, this is the amount of enemy kills required")]
        public int KillsToCompleteObjective = 5;

        [Tooltip("Start sending notification about remaining enemies when this amount of enemies is left")]
        public int NotificationEnemiesRemainingThreshold = 3;

        [Header("Specific Enemies Tracking")]
        [Tooltip("Drag all enemy GameObjects that belong to this objective's zone. If empty, will track all enemies in scene.")]
        public List<GameObject> EnemiesToTrack = new List<GameObject>();

        int m_KillTotal;
        int m_InitialEnemyCount;
        int m_RemainingEnemiesInZone;

        protected override void Start()
        {
            base.Start();

            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);

            // Убираем пустые ссылки
            EnemiesToTrack.RemoveAll(e => e == null);
            m_InitialEnemyCount = EnemiesToTrack.Count;
            m_RemainingEnemiesInZone = EnemiesToTrack.Count;

            if (string.IsNullOrEmpty(Title))
            {
                if (EnemiesToTrack.Count > 0)
                    Title = $"Eliminate {m_InitialEnemyCount} enemies in zone";
                else
                    Title = "Eliminate " + (MustKillAllEnemies ? "all the" : KillsToCompleteObjective.ToString()) + " enemies";
            }

            if (string.IsNullOrEmpty(Description))
                Description = GetUpdatedCounterAmount();
        }

        void OnEnemyKilled(EnemyKillEvent evt)
        {
            if (IsCompleted)
                return;

            GameObject killedEnemy = evt.Enemy != null ? evt.Enemy.gameObject : null;
            if (killedEnemy == null)
                return;

            bool shouldCount = false;
            if (EnemiesToTrack.Count > 0)
            {
                // Ищем убитого врага в нашем списке
                for (int i = 0; i < EnemiesToTrack.Count; i++)
                {
                    if (EnemiesToTrack[i] == killedEnemy)
                    {
                        shouldCount = true;
                        EnemiesToTrack.RemoveAt(i);
                        m_RemainingEnemiesInZone = EnemiesToTrack.Count;
                        break;
                    }
                }
            }
            else
            {
                // Глобальный режим — считаем всех
                shouldCount = true;
            }

            if (!shouldCount)
                return;

            m_KillTotal++;

            if (EnemiesToTrack.Count == 0)
            {
                if (m_InitialEnemyCount > 0)
                    KillsToCompleteObjective = m_InitialEnemyCount;
                else if (MustKillAllEnemies)
                    KillsToCompleteObjective = evt.RemainingEnemyCount + m_KillTotal;
            }

            int targetRemaining = (EnemiesToTrack.Count > 0) ? m_RemainingEnemiesInZone
                : (MustKillAllEnemies ? evt.RemainingEnemyCount : KillsToCompleteObjective - m_KillTotal);

            if (targetRemaining == 0)
            {
                CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }
            else if (targetRemaining == 1)
            {
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? "One enemy left"
                    : string.Empty;
                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
            else
            {
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? targetRemaining + " enemies to kill left"
                    : string.Empty;
                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
        }

        string GetUpdatedCounterAmount()
        {
            if (m_InitialEnemyCount > 0)
            {
                int killed = m_InitialEnemyCount - m_RemainingEnemiesInZone;
                return killed + " / " + m_InitialEnemyCount;
            }
            else
            {
                return m_KillTotal + " / " + KillsToCompleteObjective;
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }
    }
}