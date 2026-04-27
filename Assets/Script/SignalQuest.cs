using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro; // Required for TextMeshProUGUI control

public class SignalQuestManager : MonoBehaviour
{
    public static SignalQuestManager instance;

    [Header("Quest Status")]
    public int currentRound = 1;
    public int partsCollected = 0;
    public GameObject sparePartPrefab;

    [Header("UI Settings")]
    public TextMeshProUGUI alertText; // Reference to the UI Text
    public float displayTime = 2.5f;   // How long the message stays visible

    [Header("Scenes Config")]
    // List of scene names (Must match exactly with Build Settings)
    public List<string> allSceneNames = new List<string> { "Map1", "Map2", "Map3" };

    // List of scenes randomly chosen to spawn parts for the current round
    private List<string> targetScenes = new List<string>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        // First random spawn initialization when the game starts at Round 1
        if (currentRound == 1 && targetScenes.Count == 0)
        {
            PrepareGlobalSpawns();
        }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentSceneName = scene.name;

        // --- Automatic AlertText Search System ---
        // Looks for a TextMeshPro object named "AlertText" in the current scene
        GameObject foundText = GameObject.Find("AlertText");
        if (foundText != null)
        {
            alertText = foundText.GetComponent<TextMeshProUGUI>();
            alertText.gameObject.SetActive(false); // Hide by default
        }

        // Check if the loaded scene is one of the target spawn scenes
        for (int i = targetScenes.Count - 1; i >= 0; i--)
        {
            if (targetScenes[i] == currentSceneName)
            {
                SpawnAtRandomPointInScene();
                // Remove from targets once spawned so it doesn't duplicate if re-entering
                targetScenes.RemoveAt(i);
            }
        }
    }

    // --- Global Randomization: Selects scenes for parts to appear ---
    public void PrepareGlobalSpawns()
    {
        targetScenes.Clear();
        int amountToSpawn = currentRound; // Round 1 = 1 scene, Round 2 = 2 scenes, etc.

        for (int i = 0; i < amountToSpawn; i++)
        {
            string selectedScene = allSceneNames[Random.Range(0, allSceneNames.Count)];
            targetScenes.Add(selectedScene);
        }
        Debug.Log("Target Scenes Initialized: " + string.Join(", ", targetScenes));
    }

    void SpawnAtRandomPointInScene()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("PartSpawnPoint");

        if (points.Length > 0)
        {
            int randomIndex = Random.Range(0, points.Length);
            Instantiate(sparePartPrefab, points[randomIndex].transform.position, Quaternion.identity);
            Debug.Log("<color=yellow>A spare part has spawned in this scene!</color>");
        }
    }

    // --- Alert System: Pops up a message and hides it after time ---
    public void ShowAlert(string message)
    {
        if (alertText != null)
        {
            alertText.text = message;
            alertText.gameObject.SetActive(true);

            CancelInvoke("HideAlert");
            Invoke("HideAlert", displayTime);
        }
    }

    void HideAlert()
    {
        if (alertText != null) alertText.gameObject.SetActive(false);
    }

    public void CollectPart()
    {
        partsCollected++;

        string msg = (partsCollected >= currentRound)
            ? $"Parts Complete! ({partsCollected}/{currentRound})"
            : $"Part Collected! ({partsCollected}/{currentRound})";

        ShowAlert(msg);
        Debug.Log($"<color=green>Item Collected!</color> Current Inventory: {partsCollected}/{currentRound}");
    }

    public void OnSignalSuccess()
    {
        if (currentRound < 3)
        {
            currentRound++;
            partsCollected = 0;
            PrepareGlobalSpawns(); // Reroll scenes for the next round
            if (currentRound == 2)
            {
                // จบรอบ 1 เข้าสู่รอบ 2: ฐานเพิ่ม 1.5
                HeatSystem.instance.SetNewRoundDifficulty(1.5f);
            }
            else if (currentRound == 3)
            {
                // จบรอบ 2 เข้าสู่รอบ 3: ฐานเพิ่ม 2.5
                HeatSystem.instance.SetNewRoundDifficulty(2.5f);
            }
            ShowAlert($"Signal Sent! Round {currentRound} Started.");
        }
        else
        {
            ShowAlert("All Signals Sent! You Win!");
            Debug.Log("Game Finished: 3 Rounds Completed.");
        }
    }

    public bool IsReadyToSignal()
    {
        return partsCollected >= currentRound;
    }
}