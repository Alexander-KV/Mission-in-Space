using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.Game
{
    public class ObjectiveManager : MonoBehaviour
    {
        [Header("Sequential Objectives")]
        public List<Objective> ObjectivesSequence = new List<Objective>();

        int m_CurrentObjectiveIndex = -1;
        bool m_AllObjectivesCompleted = false;
        bool m_IsTransitioning = false;

        void Start()
        {
            // Деактивируем все цели в последовательности
            foreach (var obj in ObjectivesSequence)
            {
                if (obj != null)
                    obj.gameObject.SetActive(false);
            }

            // Активируем первую
            if (ObjectivesSequence.Count > 0)
            {
                m_CurrentObjectiveIndex = 0;
                ObjectivesSequence[0].gameObject.SetActive(true);
            }
        }

        void Update()
        {
            if (m_AllObjectivesCompleted || ObjectivesSequence.Count == 0)
                return;

            if (m_CurrentObjectiveIndex >= 0 && m_CurrentObjectiveIndex < ObjectivesSequence.Count)
            {
                Objective current = ObjectivesSequence[m_CurrentObjectiveIndex];
                if (current != null && current.IsCompleted && !m_IsTransitioning)
                {
                    StartCoroutine(AdvanceToNextObjective(current));
                }
            }
        }

        IEnumerator AdvanceToNextObjective(Objective completedObjective)
        {
            m_IsTransitioning = true;

            // Даём время на анимацию завершения в UI
            yield return new WaitForSeconds(1f);

            if (completedObjective != null)
                Destroy(completedObjective.gameObject);

            m_CurrentObjectiveIndex++;
            if (m_CurrentObjectiveIndex < ObjectivesSequence.Count)
            {
                ObjectivesSequence[m_CurrentObjectiveIndex].gameObject.SetActive(true);
            }
            else
            {
                m_AllObjectivesCompleted = true;
                EventManager.Broadcast(Events.AllObjectivesCompletedEvent);
            }

            m_IsTransitioning = false;
        }
    }
}