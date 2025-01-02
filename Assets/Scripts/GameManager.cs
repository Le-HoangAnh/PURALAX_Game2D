using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] TMP_Text stageText, levelText;
    [SerializeField] private Image titleImage, winImage;
    [SerializeField] private List<Color> colors;
    [SerializeField] private LevelDictionary levelDictionary;
    [SerializeField] private Cell cellPrefab;

    private Dictionary<Vector2Int, Cell> cellsDictionary = new();
    private Level currentLevelObject;
    private string currentLevelName;
    private int winColor;

    private GameState currentGameState;

    private void Awake()
    {
        instance = this;

        stageText.text = "STAGE " + PlayerPrefs.GetInt(Constants.DATA.CURRENT_STAGE).ToString();
        levelText.text = "Level " + PlayerPrefs.GetInt(Constants.DATA.CURRENT_LEVEL).ToString();

        winImage.gameObject.SetActive(false);
        currentGameState = GameState.INPUT;
        SpawnLevel();

        AudioManager.instance.AddButtonSound();
    }

    private void SpawnLevel()
    {

    }

    public void GameRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}

public enum GameState
{
    INPUT, ANIMATION
}
