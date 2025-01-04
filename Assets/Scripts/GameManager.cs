using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private TMP_Text stageText, levelText;
    [SerializeField] private Image titleImage, winImage;
    [SerializeField] private LevelDictionary levelDictionary;
    [SerializeField] private Cell cellPrefab;

    public List<Color> colors;
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
        int currentStage = PlayerPrefs.GetInt(Constants.DATA.CURRENT_STAGE);
        string currentStageName = Constants.DATA.CURRENT_STAGE + "_" + currentStage.ToString();
        int currentLevel = PlayerPrefs.GetInt(Constants.DATA.CURRENT_LEVEL);
        string currentLevelName = currentStageName + "_" + currentLevel.ToString();

        currentLevelObject = levelDictionary.GetLevel(currentLevelName);
        winColor = currentLevelObject.winColor;

        //SPAWN ALL CELLS
        for (int i = 0; i < currentLevelObject.row; i++)
        {
            for (int j = 0; j < currentLevelObject.column; j++)
            {
                Vector3 spawnPos = new(i + 0.5f, j + 0.5f);
                Cell temp = Instantiate(cellPrefab, spawnPos, Quaternion.identity);
                cellsDictionary[new(i, j)] = temp;
            }
        }

        //SPAWN COLORED CELLS
        foreach (var item in currentLevelObject.cellData)
        {
            cellsDictionary[item.gridPos].InitializeCell(item);
        }

        //SET UP THE CAMERA
        float size = 0f;
        if (currentLevelObject.column <= currentLevelObject.row)
        {
            size = (currentLevelObject.row / 2) + 3.5f;
        }
        else
        {
            size = (currentLevelObject.column / 2) + 4.5f;
        }
        Camera.main.orthographicSize = size;
        Camera.main.transform.Translate(currentLevelObject.row / 2f, currentLevelObject.column / 2f, 0);

        //CHANGE TITLE COLOR
        titleImage.color = colors[winColor];
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
