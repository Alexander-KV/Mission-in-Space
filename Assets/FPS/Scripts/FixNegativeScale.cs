using UnityEngine;

public class FixNegativeScale : MonoBehaviour
{
    [ContextMenu("Fix Negative Scale In Children")]
    void FixScale()
    {
        Transform[] all = GetComponentsInChildren<Transform>();

        foreach (Transform t in all)
        {
            Vector3 scale = t.localScale;

            if (scale.x < 0 || scale.y < 0 || scale.z < 0)
            {
                Debug.Log("Fixed scale on: " + t.name);

                scale.x = Mathf.Abs(scale.x);
                scale.y = Mathf.Abs(scale.y);
                scale.z = Mathf.Abs(scale.z);

                t.localScale = scale;
            }
        }

        Debug.Log("Finished fixing negative scale.");
    }
}