using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using System.Collections;

public class BotLobbyUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject lobbyPanel;
    public Transform botListContent;
    public TMP_Dropdown difficultyDropdown;
    public Button addBotButton;
    public Button startGameButton;
    public Button exitButton;

    [Header("Prefabs")]
    public GameObject botEntryPrefab;

    [Header("Bot")]
    public GameObject turretPrefab;

    private List<BotInfo> bots = new List<BotInfo>();

    public class BotInfo
    {
        public string name;
        public Difficulty difficulty;
        public GameObject uiEntry;
    }

    private void Start()
    {
        // Initialize UI
        lobbyPanel.SetActive(true);

        // Setup dropdown options
        difficultyDropdown.ClearOptions();
        difficultyDropdown.AddOptions(new List<string>(System.Enum.GetNames(typeof(Difficulty))));

        // Button listeners
        addBotButton.onClick.AddListener(AddBot);
        startGameButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);

        GameObject.DontDestroyOnLoad(gameObject);
    }

    private void AddBot()
    {
        Difficulty selectedDifficulty = (Difficulty)difficultyDropdown.value;

        // Create new bot
        BotInfo newBot = new BotInfo
        {
            name = "Bot_" + Random.Range(1000, 9999),
            difficulty = selectedDifficulty
        };

        // Create UI entry
        GameObject entry = Instantiate(botEntryPrefab, botListContent);
        newBot.uiEntry = entry;

        // Setup entry UI
        BotEntryUI entryUI = entry.GetComponent<BotEntryUI>();
        entryUI.Initialize(newBot.name, selectedDifficulty.ToString(), () => RemoveBot(newBot));

        // Add to list
        bots.Add(newBot);

        Debug.Log($"Added bot: {newBot.name} ({newBot.difficulty})");
    }

    private void RemoveBot(BotInfo bot)
    {
        if (bots.Contains(bot))
        {
            bots.Remove(bot);
            Destroy(bot.uiEntry);
            Debug.Log($"Removed bot: {bot.name}");
        }
    }

    IEnumerator LoadSceneAndGetPosition()
    {
        // Load the scene and wait for it to complete
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SampleScene");

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Now it's safe to find the object
        var turretPosition = GameObject.Find("TurretPosition").transform.position;
        int index = 0;

        foreach (var bot in bots)
        {
            var turret = Instantiate(turretPrefab, turretPosition + Vector3.right * index * 2.0f, Quaternion.identity);
            index += 1;

            switch(bot.difficulty)
            {
                case Difficulty.EASY:
                    turret.GetComponent<Turret>().accuracy = 0.4f;
                    turret.GetComponent<Turret>().shootSpeed = 1.5f;
                    break;
                case Difficulty.NORMAL:
                    turret.GetComponent<Turret>().accuracy = 0.8f;
                    turret.GetComponent<Turret>().shootSpeed = 1.0f;
                    break;
                case Difficulty.HARD:
                    turret.GetComponent<Turret>().accuracy = 0.95f;
                    turret.GetComponent<Turret>().shootSpeed = 0.75f;
                    break;
            }
        }
    }

    private void StartGame()
    {
        lobbyPanel.SetActive(false);
        Debug.Log("Starting game with " + bots.Count + " bots");

        StartCoroutine(LoadSceneAndGetPosition());
        //SceneManager.LoadScene("SampleScene");
        //var turretPosition = GameObject.Find("TurretPosition").transform.position;
        //foreach (var bot in bots)
        //{
        //    Instantiate(turretPrefab, turretPosition, Quaternion.identity);
        //}


    }

    private void ExitGame()
    {
        Debug.Log("Exiting game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}