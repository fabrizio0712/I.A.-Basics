using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private Dictionary<GameObject, int> dicKeySpawnPoints;
    [SerializeField] private List<GameObject> keySpawnPoints;
    [SerializeField] private List<int> keySpawnPointsChance;
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private GameObject objectivesPanel;
    [SerializeField] private GameObject objective1;
    [SerializeField] private GameObject objective2;
    [SerializeField] private Text lives;
    private float totalProbabilitie;
    void Start()
    {
        dicKeySpawnPoints = new Dictionary<GameObject, int>();
        finalPanel.SetActive(false);
        playPanel.SetActive(true);
        totalProbabilitie = 0;
        if (keySpawnPoints.Count == keySpawnPointsChance.Count) 
        {
            for (int i = 0; i < keySpawnPoints.Count; i++) 
            {
                dicKeySpawnPoints.Add(keySpawnPoints[i], keySpawnPointsChance[i]);
                totalProbabilitie += keySpawnPointsChance[i];
            }
        }
        float random = UnityEngine.Random.Range(0, totalProbabilitie);
        foreach (var item in dicKeySpawnPoints) 
        {
            random -= item.Value;
            if (random < 0) 
            {
                key.transform.position = item.Key.transform.position;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        lives.text = player.GetComponent<Player>().Lives.ToString();
        if (player.GetComponent<Player>().Lives <= 0) 
        {
            EndGame();
        }
        if (!key.active) 
        {
            objective1.SetActive(false);
            objective2.SetActive(true);
            door.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            EndGame();
        }
    }
    public void StartGame() 
    {
        playPanel.SetActive(false);
        player.GetComponent<PlayerController>().moveEnable = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Replay() 
    {
        SceneManager.LoadScene("Level 1");
    }
    public void EndGame() 
    {
        player.GetComponent<PlayerController>().moveEnable = false;
        finalPanel.SetActive(true);
    }
}
