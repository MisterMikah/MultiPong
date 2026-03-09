using UnityEngine;

public class QuitGame : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); // quit in build

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // stop play mode
#endif
        }
    }
}