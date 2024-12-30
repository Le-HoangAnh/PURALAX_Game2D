using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    [SerializeField] private GameObject mainPanel, stagePanel, levelPanel, activeSoundImage;

    [SerializeField] private TMP_Text stageText;

    [SerializeField] private Image stageColorInLevelPanel;

    public List<Color> colors;

    private void Awake()
    {
        instance = this;

        mainPanel.SetActive(true);
        stagePanel.SetActive(false);
    }

    private void OnEnable()
    {
        AudioManager.instance.AddButtonSound();
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void ClickedPlay()
    {
        mainPanel.SetActive(false);
        stagePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        mainPanel.SetActive(true);
        stagePanel.SetActive(false);
    }

    public void BackToStage()
    {
        levelPanel.SetActive(false);
        stagePanel.SetActive(true);
    }

    public void ClickedStage()
    {
        stagePanel.SetActive(false);

        int currentStage = PlayerPrefs.GetInt(Constants.DATA.CURRENT_STAGE);
        stageText.text = "STAGE" + currentStage.ToString();

        levelPanel.SetActive(true);
        stageColorInLevelPanel.color = colors[currentStage - 1];
    }

    public void ToggleSound()
    {
        bool sound = PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND) == 0;
        PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, sound ? 1 : 0);
        activeSoundImage.SetActive(!sound);

        AudioManager.instance.ToggleSound();
    }
}
