using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource effectSource;
    [SerializeField] private AudioClip clickSound;

    private bool isSoundMuted;

    public bool IsSoundMuted
    {
        get
        {
            isSoundMuted = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ?
                PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND) : 1) == 0;
            return isSoundMuted;
        }

        set
        {
            isSoundMuted = value;
            PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, isSoundMuted ? 0 : 1);
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, IsSoundMuted ? 0 : 1);
        effectSource.mute = IsSoundMuted;
    }

    public void AddButtonSound()
    {
        var buttons = FindObjectsOfType<Button>(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.AddListener(() => { PlaySound(clickSound); });
        }
    }

    public void PlaySound(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    public void ToggleSound()
    {
        effectSource.mute = IsSoundMuted;
    }
}
