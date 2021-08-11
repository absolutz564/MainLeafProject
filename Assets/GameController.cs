using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public PlayerController PlayerC;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI ArrowsText;
    public List<GameObject> ItensToSpawn;

    public List<GameObject> EnemiesToSpawn;

    public TextMeshProUGUI TimerText;

    public TextMeshProUGUI EndGameDiedText;
    public TextMeshProUGUI EndGameScoreText;

    public TextMeshProUGUI InitTimerValueText;

    public Transform InitSpawnPosition;
    public Transform FinalSpawnPosition;

    public GameObject TimerOverPanel;

    public int Score = 0;
    public int EnemiesDied = 0;
    public GameObject UIElements;

    public int HealthValueToIncrease = 20;
    public int ArrowsToIncrease = 10;


    public bool Playable = false;
    public GameObject PauseMenu;

    public AudioSource ThemeMusic;
    public List<AudioSource> SoundEffects;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPauseState(true);
        }
    }


    public void GoToMenu()
    {
        DestroyImmediate(DontDestroyConfig.Instance.gameObject);
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void SetPauseState(bool state)
    {
        PauseMenu.SetActive(state);
        if (state)
        {
            Time.timeScale = 0;
            SetCursorState(true);
        }
        else
        {
            Time.timeScale = 1;
            SetCursorState(false);
            PauseMenu.SetActive(false);
        }
    }

    public void IncreasePlayerPoints()
    {
        Score += 50;
        UpdateScore(Score);
    }

    public void HealthPlayer()
    {
        if (PlayerC.Life + HealthValueToIncrease <= 100)
        {
            UpdatePlayerLife(true, HealthValueToIncrease);
        }
        else
        {
            PlayerC.Life = 100;
            UpdatePlayerLife(true, 0);
        }
    }


    public void UpdatePlayerLife(bool increase, int value)
    {
        if (increase)
        {
            PlayerC.Life += value;
        }
        else
        {
            PlayerC.Life -= value;
        }

        PlayerC.HealthBar.value = PlayerC.Life;
    }

    void Awake()
    {
        Instance = this;
        ArrowsText.text = "Arrows: " + PlayerC.Arrows;

        SetInitTimerText();
        InvokeRepeating("StartTimer", 1, 1);
        ConfigSounds();
    }

    void ConfigSounds()
    {
        if (!DontDestroyConfig.Instance.CanPlayMusic)
        {
            ThemeMusic.Stop();
        }
        if (!DontDestroyConfig.Instance.CanPlayEffects)
        {
            StopAllSounds();
        }
    }

    void StopAllSounds()
    {
        foreach (AudioSource src in SoundEffects)
        {
            src.volume = 0;
        }
    }
    void SetInitTimerText()
    {
        if (DontDestroyConfig.Instance != null)
        {
            InitTimerValueText.text = DontDestroyConfig.Instance.InitTimerValue.ToString();
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    void StartTimer()
    {
        DontDestroyConfig.Instance.InitTimerValue -= 1;

        SetInitTimerText();
        if (DontDestroyConfig.Instance.InitTimerValue == 0)
        {
            UIElements.SetActive(true);
            PlayerC.CanShot = true;
            PlayerC.PMovement.enabled = true;
            PlayerC.MControl.enabled = true;
            InitTimerValueText.enabled = false;
            Playable = true;
            InvokeRepeating("InstantiateEnemies", 1, Random.Range(5, 8));

            InvokeRepeating("CountDown", 1, 1);
        }
    }


    void InstantiateEnemies()
    {
        if (DontDestroyConfig.Instance.GameTimerValue > 0)
        {
            Debug.Log("Tentando instanciar");
            foreach (GameObject obj in EnemiesToSpawn)
            {
                int value = Random.Range(0, 2);
                if (value == 1)
                {
                    Instantiate(obj, new Vector3(Random.Range(InitSpawnPosition.position.x, FinalSpawnPosition.position.x), InitSpawnPosition.position.y, Random.Range(InitSpawnPosition.position.z, FinalSpawnPosition.position.z)), Quaternion.identity);
                }
            }
        }

    }

    void ConfigEndGameValues()
    {
        Debug.Log("O tempo acabou");
        StopAllSounds();
        TimerOverPanel.SetActive(true);
        SetCursorState(true);
        
        EndGameDiedText.text = "Enemies died: " + EnemiesDied;
        EndGameScoreText.text = "Score: " + Score;

    }
    void CountDown()
    {
        DontDestroyConfig.Instance.GameTimerValue -= 1;
        TimerText.text = DontDestroyConfig.Instance.GameTimerValue.ToString();
        if (DontDestroyConfig.Instance.GameTimerValue == 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        Debug.Log("End Game");
        StopAllSounds();
        PlayerC.enabled = false;
        PlayerC.PMovement.enabled = false;
        Playable = false;
        CancelInvoke();
        ConfigEndGameValues();
    }

    public void HitPlayer()
    {
        PlayerC.Hit();
    }

    public void UpdateScore(int value)
    {
        ScoreText.text = "Score: " + value;
    }

    public void UpdateArrows(bool increase)
    {
        if (increase)
        {
            PlayerC.Arrows += ArrowsToIncrease;
        }
        else
        {
            PlayerC.Arrows -= 1;
        }
        ArrowsText.text = "Arrows: " + PlayerC.Arrows;
    }

    public void SpawnItems(Transform pos)
    {
        foreach (GameObject obj in ItensToSpawn)
        {
            int value = Random.Range(0, 3);
            if (value == 1)
            {
                Instantiate(obj, new Vector3(pos.position.x + Random.Range(-0.8f, 0.8f), pos.position.y + 0.3f, pos.position.z + Random.Range(-0.8f, 0.8f)), Quaternion.identity);
            }
        }
    }

    public void SetCursorState(bool state)
    {
        //Enable Mouse Movement
        Cursor.visible = state;
        if (state)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //Disable mouse
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
