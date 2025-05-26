using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState
{
    public int Level { get; private set; }

    public delegate void TransformationEventHandler(PlayerController whichController);
    public event TransformationEventHandler OnPlayerTransformation;

    public void LoadLevel(int level)
    {
        Level = level;

        SceneManager.LoadScene("Level" + level, LoadSceneMode.Single);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Level" + Level, LoadSceneMode.Single);
    }

    public void PlayerTransformed(PlayerController whichController)
    {
        OnPlayerTransformation?.Invoke(whichController);
    }
}
