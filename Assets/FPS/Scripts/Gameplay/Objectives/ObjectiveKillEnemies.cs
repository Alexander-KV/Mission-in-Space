using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.FPS.Game;

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
        public List<GameObject> EnemiesToTrack = new List<GameObject>();
        public List<GameObject> EnemiesToActivate = new List<GameObject>();

        [Header("Completion Message UI")]
        public GameObject CompletionMessageUI;
        public float MessageDisplayTime = 5f;

        [Header("🎯 Тип миссии")]
        [Tooltip("Если true - миссия завершится только после побега. Если false - после убийства всех врагов")]
        public bool requiresBombAndEscape = false;

        [Header("🎯 Бомба и побег")]
        public bool isBossKilled = false;
        public bool isBombPlanted = false;
        public bool isEscaped = false;

        int m_KillTotal;
        int m_InitialEnemyCount;
        int m_RemainingEnemiesInZone;

        protected override void Start()
        {
            base.Start();

            foreach (var enemy in EnemiesToActivate)
            {
                if (enemy != null) enemy.SetActive(true);
            }

            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);

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

            CompletionUIMonitor.RegisterObjective(this);
        }

        void OnEnemyKilled(EnemyKillEvent evt)
        {
            if (IsCompleted) return;

            GameObject killedEnemy = evt.Enemy != null ? evt.Enemy.gameObject : null;
            if (killedEnemy == null) return;

            bool shouldCount = false;
            if (EnemiesToTrack.Count > 0)
            {
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
            else shouldCount = true;

            if (!shouldCount) return;

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
                if (requiresBombAndEscape && !isBossKilled)
                {
                    isBossKilled = true;
                    Debug.Log("✅ Босс убит! Теперь установи бомбу и беги к кораблю...");
                    UpdateObjective("БОСС УБИТ", "Установи бомбу в реакторном отсеке", "");
                    // UI надписи теперь управляется ТОЛЬКО SimpleWinTrigger
                }
                else
                {
                    CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
                    CompletionUIMonitor.NotifyObjectiveCompleted(this);
                }
            }
            else if (targetRemaining == 1)
            {
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? "One enemy left" : string.Empty;
                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
            else
            {
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? targetRemaining + " enemies to kill left" : string.Empty;
                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
        }

        string GetUpdatedCounterAmount()
        {
            if (m_InitialEnemyCount > 0)
                return (m_InitialEnemyCount - m_RemainingEnemiesInZone) + " / " + m_InitialEnemyCount;
            else
                return m_KillTotal + " / " + KillsToCompleteObjective;
        }

        public void CompleteMissionAfterEscape()
        {
            if (isEscaped) return;
            isEscaped = true;
            Debug.Log(" МИССИЯ ВЫПОЛНЕНА!");
            CompleteObjective("ПОБЕДА!", "Вы уничтожили босса и сбежали!", "Миссия выполнена!");
            CompletionUIMonitor.NotifyObjectiveCompleted(this);
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }
    }

    // ==================== МОНИТОР ====================
    public class CompletionUIMonitor : MonoBehaviour
    {
        static CompletionUIMonitor instance;
        static bool monitorCreated = false;
        Dictionary<ObjectiveKillEnemies, (GameObject ui, float duration, bool completed)> tracked = new Dictionary<ObjectiveKillEnemies, (GameObject, float, bool)>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            if (monitorCreated) return;
            monitorCreated = true;
            GameObject monitorObj = new GameObject("CompletionUIMonitor");
            DontDestroyOnLoad(monitorObj);
            instance = monitorObj.AddComponent<CompletionUIMonitor>();
        }

        void Awake() { DisableAllCompletionUIs(); SceneManager.sceneLoaded += OnSceneLoaded; }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) { DisableAllCompletionUIs(); }

        void DisableAllCompletionUIs()
        {
            var allObjectives = FindObjectsByType<ObjectiveKillEnemies>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var obj in allObjectives)
            {
                if (obj.CompletionMessageUI != null)
                {
                    obj.CompletionMessageUI.SetActive(false);
                    if (!tracked.ContainsKey(obj)) tracked[obj] = (obj.CompletionMessageUI, obj.MessageDisplayTime, false);
                }
            }
        }

        public static void RegisterObjective(ObjectiveKillEnemies objective)
        {
            if (instance == null || objective.CompletionMessageUI == null) return;
            if (!instance.tracked.ContainsKey(objective))
            {
                instance.tracked.Add(objective, (objective.CompletionMessageUI, objective.MessageDisplayTime, false));
                objective.CompletionMessageUI.SetActive(false);
            }
        }

        public static void NotifyObjectiveCompleted(ObjectiveKillEnemies objective)
        {
            if (instance == null) return;
            if (instance.tracked.TryGetValue(objective, out var data))
            {
                data.completed = true;
                instance.tracked[objective] = data;
                if (data.ui != null) data.ui.SetActive(true);
                if (data.duration > 0f) instance.StartCoroutine(instance.HideAfterDelay(data.ui, data.duration));
            }
        }

        IEnumerator HideAfterDelay(GameObject ui, float delay) { yield return new WaitForSeconds(delay); if (ui != null) ui.SetActive(false); }

        void Update()
        {
            foreach (var kvp in tracked)
            {
                var obj = kvp.Key;
                var data = kvp.Value;
                if (obj == null || data.ui == null) continue;
                if (!obj.IsCompleted && data.ui.activeSelf) data.ui.SetActive(false);
            }
            RemoveDestroyedObjectives();
        }

        void RemoveDestroyedObjectives()
        {
            var keysToRemove = new List<ObjectiveKillEnemies>();
            foreach (var kvp in tracked) if (kvp.Key == null) keysToRemove.Add(kvp.Key);
            foreach (var key in keysToRemove) tracked.Remove(key);
        }

        void OnDestroy() { SceneManager.sceneLoaded -= OnSceneLoaded; }
    }

    public static class DictionaryExtensions
    {
        public static void RemoveWhere<TKey, TValue>(this Dictionary<TKey, TValue> dict, System.Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            var keysToRemove = new List<TKey>();
            foreach (var kvp in dict) if (predicate(kvp)) keysToRemove.Add(kvp.Key);
            foreach (var key in keysToRemove) dict.Remove(key);
        }
    }
}