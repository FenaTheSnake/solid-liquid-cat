using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState
{
    public int Level { get; private set; }

    public delegate void TransformationEventHandler(PlayerController whichController);
    public event TransformationEventHandler OnPlayerTransformation;

    Transition _transition;
    int _targetLevel;

    public void LoadLevel(int level)
    {
        _targetLevel = level;
        TransitionFadeIn();

        //Level = level;

        //SceneManager.LoadScene("Level" + level, LoadSceneMode.Single);
    }

    public void RestartLevel()
    {
        _targetLevel = Level;
        TransitionFadeIn();
        //SceneManager.LoadScene("Level" + Level, LoadSceneMode.Single);
    }

    public void PlayerTransformed(PlayerController whichController)
    {
        OnPlayerTransformation?.Invoke(whichController);
    }

    void TransitionFadeIn()
    {
        if(_transition == null) _transition = GameObject.Find("Transition").GetComponent<Transition>();
        _transition.Show();
    }
    void TransitionFadeOut()
    {
        if(_transition == null) _transition = GameObject.Find("Transition").GetComponent<Transition>();
        _transition.Hide();
    }

    public void OnTransitionFinished(bool transitionIsShownNow)
    {
        if (!transitionIsShownNow) return;

        Level = _targetLevel;
        SceneManager.LoadScene("Level" + _targetLevel, LoadSceneMode.Single);
        TransitionFadeOut();
    }
}
