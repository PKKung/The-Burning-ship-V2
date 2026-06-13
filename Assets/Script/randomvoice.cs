using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomVoiceManager : MonoBehaviour
{
    public static RandomVoiceManager instance;

    [Header("Audio List (ใส่ได้หลายเสียง)")]
    public List<AudioSource> randomVoices;

    [Header("Settings")]
    public float minWaitTime = 45f; // เวลาต่ำสุดที่ต้องรอ (วินาที)
    public float maxWaitTime = 75f; // เวลาสูงสุดที่ต้องรอ (วินาที)
    public bool isRandomizing = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(RandomVoiceRoutine());
    }

    IEnumerator RandomVoiceRoutine()
    {
        while (isRandomizing)
        {
            // สุ่มเวลาที่จะรอ (เช่น ระหว่าง 45 ถึง 75 วินาที เพื่อให้เฉลี่ยอยู่ที่ 1 นาที)
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            if (randomVoices.Count > 0)
            {
                // สุ่มเลือก 1 ใน 5 เสียง
                int randomIndex = Random.Range(0, randomVoices.Count);
                AudioSource selectedVoice = randomVoices[randomIndex];

                if (selectedVoice != null)
                {
                    selectedVoice.Play();
                    Debug.Log("สุ่มเล่นเสียงที่: " + (randomIndex + 1));
                }
            }
        }
    }
}