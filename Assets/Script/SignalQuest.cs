using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections; // เพิ่มตัวนี้เพื่อให้ใช้ Coroutine ได้
using TMPro;

public class SignalQuestManager : MonoBehaviour
{
    public static SignalQuestManager instance;

    [Header("Quest Status")]
    public int currentRound = 1;
    public int partsCollected = 0;
    public GameObject sparePartPrefab;

    [Header("UI Settings")]
    public TextMeshProUGUI alertText;
    public float displayTime = 2.5f;

    [Header("Scenes Config")]
    public List<string> allSceneNames = new List<string> { "Map1", "Map2", "Map3" };
    private List<string> targetScenes = new List<string>();

    [Header("Broken Sequence Settings")]
    public AudioSource brokenAudio;
    public MonoBehaviour playerMovement;

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

        GameObject foundText = GameObject.Find("AlertText");
        if (foundText != null)
        {
            alertText = foundText.GetComponent<TextMeshProUGUI>();
            alertText.gameObject.SetActive(false);
        }

        for (int i = targetScenes.Count - 1; i >= 0; i--)
        {
            if (targetScenes[i] == currentSceneName)
            {
                SpawnAtRandomPointInScene();
                targetScenes.RemoveAt(i);
            }
        }
    }

    public void PrepareGlobalSpawns()
    {
        targetScenes.Clear();
        int amountToSpawn = currentRound;

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
    }

    // --- ส่วนที่แก้ไข: เพิ่มการโหลดฉากจบ ---
    public void OnSignalSuccess()
    {
        if (currentRound < 3)
        {
            currentRound++;
            partsCollected = 0;
            PrepareGlobalSpawns();

            if (currentRound == 2)
            {
                HeatSystem.instance.SetNewRoundDifficulty(1.5f);
            }
            else if (currentRound == 3)
            {
                HeatSystem.instance.SetNewRoundDifficulty(2.5f);
            }
            ShowAlert($"Signal Sent! Round {currentRound} Started.");
        }
        else
        {
            // ชนะเกม: ส่งครบ 3 รอบ
            ShowAlert("All Signals Sent! You Win!");
            Debug.Log("Game Finished: 3 Rounds Completed.");

            // เรียกใช้ Coroutine เพื่อรอเวลาแล้ววาร์ปไปฉากจบ
            StartCoroutine(WaitAndGoToVictory());
        }
    }

    IEnumerator WaitAndGoToVictory()
    {
        // รอ 3 วินาทีให้ผู้เล่นอ่านข้อความชนะก่อน
        yield return new WaitForSeconds(3f);

        // เปลี่ยน "VictoryScene" เป็นชื่อฉากจบชนะของคุณใน Build Settings
        SceneManager.LoadScene("End win");
    }

    public bool IsReadyToSignal()
    {
        return partsCollected >= currentRound;
    }

    public void ResetQuestForNewGame()
    {
        currentRound = 1;
        partsCollected = 0;
        targetScenes.Clear();
        Debug.Log("Reset Quest Manager Done!");
    }
}