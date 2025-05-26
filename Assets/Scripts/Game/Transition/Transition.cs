using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Transition : MonoBehaviour
{
    bool _fadeIn;
    bool _activated;

    float _alpha;

    Image _image;

    GameState _gameState;

    [Inject]
    void Construct(GameState gamestate)
    {
        _gameState = gamestate;

        DontDestroyOnLoad(transform.parent.gameObject);
        DontDestroyOnLoad(this);
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if (!_activated) return;

        if (_fadeIn)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _alpha);
            _alpha += 0.05f;
            if (_alpha >= 1.25f)
            {
                _activated = false;
                _gameState.OnTransitionFinished(true);
            }
        }
        else
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _alpha);
            _alpha -= 0.05f;
            if (_alpha <= -0.25f)
            {
                _activated = false;
                _gameState.OnTransitionFinished(false);
            }
        }
    }

    public void Show()
    {
        _fadeIn = true;
        _activated = true;
        _alpha = 0;
    }

    public void Hide()
    {
        _fadeIn = false;
        _activated = true;
        _alpha = 1.0f;
    }
}
