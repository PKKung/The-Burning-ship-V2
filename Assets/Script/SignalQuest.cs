using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
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
        // เริ่มต้นเกมครั้งแรกสุด
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

        // ค้นหา UI Alert ในฉากใหม่ทุกครั้ง
        GameObject foundText = GameObject.Find("AlertText");
        if (foundText != null)
        {
            alertText = foundText.GetComponent<TextMeshProUGUI>();
            alertText.gameObject.SetActive(false);
        }

        // ระบบสุ่มเกิดของในฉากที่กำหนด
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
        Debug.Log("Target Scenes Initialized for Round " + currentRound + ": " + string.Join(", ", targetScenes));
    }

    void SpawnAtRandomPointInScene()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("PartSpawnPoint");

        if (points.Length > 0)
        {
            int randomIndex = Random.Range(0, points.Length);
            Instantiate(sparePartPrefab, points[randomIndex].transform.position, Quaternion.identity);
            Debug.Log("<color=yellow>A spare part has spawned!</color>");
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

    public void OnSignalSuccess()
    {
        if (currentRound < 3)
        {
            currentRound++;
            partsCollected = 0;
            PrepareGlobalSpawns();

            // ปรับระดับความยากความร้อนตามรอบ
            if (currentRound == 2) { HeatSystem.instance.SetNewRoundDifficulty(0.5f); }
            else if (currentRound == 3) { HeatSystem.instance.SetNewRoundDifficulty(0.5f); }

            ShowAlert($"Signal Sent! Round {currentRound} Started.");
        }
        else
        {
            // ชนะเกมเมื่อส่งครบ 3 ครั้ง
            ShowAlert("All Signals Sent! You Win!");
            StartCoroutine(WaitAndGoToVictory());
        }
    }

    IEnumerator WaitAndGoToVictory()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("End win");
    }

    public bool IsReadyToSignal()
    {
        return partsCollected >= currentRound;
    }

    // --- ฟังก์ชันรีเซ็ตที่แก้ไขแล้ว ---
    public void ResetQuestForNewGame()
    {
        // 1. หยุดการนับถอยหลังวาร์ปที่อาจค้างอยู่จากเกมรอบก่อน
        StopAllCoroutines();

        // 2. รีเซ็ตค่าตัวเลขกลับไปเริ่มต้น
        currentRound = 1;
        partsCollected = 0;

        // 3. ล้างรายชื่อฉากและสุ่มใหม่สำหรับรอบที่ 1 ทันที
        targetScenes.Clear();
        PrepareGlobalSpawns();

        Debug.Log("SignalQuestManager: Reset Complete. Ready for New Game.");
    }
}