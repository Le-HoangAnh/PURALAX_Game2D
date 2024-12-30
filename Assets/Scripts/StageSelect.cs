using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    [SerializeField] private int buttonStage;
    
    private Button button;
    private GameObject activeImage;
    private Image stageImage;
    private TMPro.TMP_Text countText;

    private void Awake()
    {
        button = GetComponent<Button>();
        activeImage = transform.GetChild(0).gameObject;
        stageImage = activeImage.GetComponent<Image>();
        countText = GetComponentInChildren<TMPro.TMP_Text>();
    }

    private void OnEnable()
    {
        countText.text = buttonStage.ToString();

        stageImage.color = MainMenuManager.instance.colors[buttonStage - 1];
        string currentStageName = Constants.DATA.CURRENT_STAGE + "_" + buttonStage.ToString();
        int stageActive = PlayerPrefs.HasKey(currentStageName) ? PlayerPrefs.GetInt(currentStageName) : 0;
        
        if (buttonStage == 1)
        {
            stageActive = 1;
            PlayerPrefs.SetInt(currentStageName, stageActive);
        }
        
        activeImage.SetActive(stageActive == 1);
    }

    private void Start()
    {
        button.onClick.AddListener(UpdateStage);
    }

    private void UpdateStage()
    {
        if (!activeImage.activeInHierarchy) return;
        PlayerPrefs.SetInt(Constants.DATA.CURRENT_STAGE, buttonStage);
        MainMenuManager.instance.ClickedStage();
    }
}
