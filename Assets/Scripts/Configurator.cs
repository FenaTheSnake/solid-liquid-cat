using UnityEngine;

public enum Difficulty
{
    EASY,
    NORMAL,
    HARD
}

public class Configurator : MonoBehaviour
{
    public Difficulty difficulty = Difficulty.NORMAL;

    // The values that each button will set
    public Difficulty button1Value = Difficulty.EASY;
    public Difficulty button2Value = Difficulty.NORMAL;
    public Difficulty button3Value = Difficulty.HARD;

    [SerializeField] Turret turret;

    // Rectangles for button positions
    private Rect button1Rect = new Rect(10, 10, 120, 50);
    private Rect button2Rect = new Rect(10, 70, 120, 50);
    private Rect button3Rect = new Rect(10, 130, 120, 50);
    private Rect valueRect = new Rect(10, 190, 200, 80);

    void UpdateDifficulty(Difficulty newDifficulty)
    {
        difficulty = newDifficulty;

        switch(difficulty)
        {
            case Difficulty.EASY:
                turret.accuracy = 0.4f;
                turret.shootSpeed = 1.5f;
                break;
            case Difficulty.NORMAL:
                turret.accuracy = 0.8f;
                turret.shootSpeed = 1.0f;
                break;
            case Difficulty.HARD:
                turret.accuracy = 0.95f;
                turret.shootSpeed = 0.75f;
                break;
        }
    }

    void OnGUI()
    {
        // Button 1
        if (GUI.Button(button1Rect, "Set to " + button1Value))
        {
            UpdateDifficulty(button1Value);
        }

        // Button 2
        if (GUI.Button(button2Rect, "Set to " + button2Value))
        {
            UpdateDifficulty(button2Value);
        }

        // Button 3
        if (GUI.Button(button3Rect, "Set to " + button3Value))
        {
            UpdateDifficulty(button3Value);
        }

        // Display the current value
        //GUI.Label(valueRect, "Current Difficulty: " + difficulty + "\nTurret Accuracy: " + turret.accuracy * 100 + "%\nShoot Speed: " + turret.shootSpeed);
    }

    // Optional: You can also use this value in Update or other methods
    void Update()
    {
        // For example, you could use the currentValue here
        // Debug.Log("Current value is: " + currentValue);
    }
}
