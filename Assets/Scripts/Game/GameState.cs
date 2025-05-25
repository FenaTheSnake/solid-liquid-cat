using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState
{
    public int Level { get; private set; }

    public void LoadLevel(int level)
    {
        Level = level;

        SceneManager.LoadScene("Level" + level, LoadSceneMode.Single);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Level" + Level, LoadSceneMode.Single);
    }
}
