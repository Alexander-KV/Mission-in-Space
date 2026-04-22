using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimelineCutsceneLoader : MonoBehaviour
{
    [Header("⚙️ Настройки")]
    public PlayableDirector director;
    public string nextSceneName = "MainGame";
    public float delayAfterCutscene = 1.5f;

    void Start()
    {
        if (director == null)
        {
            Debug.LogError("❗ Не назначен PlayableDirector!");
            return;
        }

        // Запускаем Timeline, если нужно
        if (director.time >= director.duration || director.time <= 0)
            director.Play();

        StartCoroutine(WaitForEndAndLoad());
    }

    IEnumerator WaitForEndAndLoad()
    {
        // Ждём, пока время проигрывания не достигнет длительности
        while (director.time < director.duration)
        {
            yield return null;
        }

        yield return new WaitForSeconds(delayAfterCutscene);
        SceneManager.LoadScene(nextSceneName);
    }
}