using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Toggle MusicToggle;
    public Toggle EffectToggle;
    public InputField InitTimerInput;
    public InputField GameTimerInput;

    public static MenuController Instance;

    public GameObject ConfigMenu;

    private string PlayMusicKey = "CanPlayMusic";
    private string PlayEffectsKey = "CanPlayEffects";
    private string InitTimerKey = "InitTimerValue";
    private string GameTimerKey = "GameTimerValue";

    private void Awake()
    {
        Instance = this;

        if (PlayerPrefs.HasKey(PlayMusicKey))
        {
            DontDestroyConfig.Instance.CanPlayMusic = GetBool(PlayMusicKey);
            MusicToggle.isOn = DontDestroyConfig.Instance.CanPlayMusic;
        }
        if (PlayerPrefs.HasKey(PlayEffectsKey))
        {
            DontDestroyConfig.Instance.CanPlayEffects = GetBool(PlayEffectsKey);
            EffectToggle.isOn = DontDestroyConfig.Instance.CanPlayEffects;
        }

        if (PlayerPrefs.HasKey(InitTimerKey))
        {
            DontDestroyConfig.Instance.InitTimerValue = Getint(InitTimerKey);
        }
        if (PlayerPrefs.HasKey(GameTimerKey))
        {
            DontDestroyConfig.Instance.GameTimerValue = Getint(GameTimerKey);
        }

        InitTimerInput.text = DontDestroyConfig.Instance.InitTimerValue.ToString();
        GameTimerInput.text = DontDestroyConfig.Instance.GameTimerValue.ToString();
    }

    public void SetConfigState(bool state)
    {
        ConfigMenu.SetActive(state);

    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Loading");
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }

    public int Getint(string KeyName)
    {
        return PlayerPrefs.GetInt(KeyName);
    }

    public static void SetBool(string key, bool state)
    {
        PlayerPrefs.SetInt(key, state ? 1 : 0);
    }

    public static bool GetBool(string key)
    {
        int value = PlayerPrefs.GetInt(key);

        if (value == 1)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public void ValueChanged()
    {
        Debug.Log("CHANGED");
        DontDestroyConfig.Instance.CanPlayMusic = MusicToggle.isOn;
        DontDestroyConfig.Instance.CanPlayEffects = EffectToggle.isOn;

        if (InitTimerInput.text != string.Empty)
        {
            DontDestroyConfig.Instance.InitTimerValue = int.Parse(InitTimerInput.text);
        }
        if (GameTimerInput.text != string.Empty)
        {
            DontDestroyConfig.Instance.GameTimerValue = int.Parse(GameTimerInput.text);
        }


        SetBool(PlayMusicKey, DontDestroyConfig.Instance.CanPlayMusic);
        SetBool(PlayEffectsKey, DontDestroyConfig.Instance.CanPlayEffects);
        SetInt(InitTimerKey, DontDestroyConfig.Instance.InitTimerValue);
        SetInt(GameTimerKey, DontDestroyConfig.Instance.GameTimerValue);
        
    }
}
