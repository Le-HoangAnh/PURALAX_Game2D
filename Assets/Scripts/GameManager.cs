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
    [SerializeField] private AudioClip moveClip, updateClip, winClip, loseClip;

    public List<Color> colors;
    private Dictionary<Vector2Int, Cell> cellsDictionary = new();
    private Level currentLevelObject;
    private string currentLevelName;
    private int winColor;

    private GameState currentGameState;
    private Vector2Int startClickGrid, endClickGrid;
    private float stateDelay;

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

    private void Update()
    {
        if (currentGameState != GameState.INPUT) return;

        Vector3 inputPos;
        Vector2Int currentClickedPos;

        if (Input.GetMouseButtonDown(0))
        {
            inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentClickedPos = new ((int) inputPos.x, (int) inputPos.y);
            startClickGrid = currentClickedPos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentClickedPos = new((int)inputPos.x, (int)inputPos.y);
            endClickGrid = currentClickedPos;
            endClickGrid = GetDirection(endClickGrid - startClickGrid);

            Debug.Log("STARTPOS: " + startClickGrid);
            Debug.Log("OFFSET: " + endClickGrid);

            currentGameState = GameState.ANIMATION;
            CalculateMoves();
        }
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

    private Vector2Int GetDirection(Vector2Int offset)
    {
        Vector2Int result;
        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
        {
            result = new Vector2Int(offset.x > 0 ? 1 : -1, 0);
        }
        else
        {
            result = new Vector2Int(0, offset.y > 0 ? 1 : -1);
        }
        return result;
    }

    private bool IsValidPos(Vector2Int pos)
    {
        return !(pos.x >= currentLevelObject.row || pos.x < 0 || pos.y < 0 || pos.y >= currentLevelObject.column);
    }

    private void PlayUpdateSound()
    {
        AudioManager.instance.PlaySound(updateClip);
    }

    private IEnumerator SwitchStateAfterDelay()
    {
        while (stateDelay > 0f)
        {
            stateDelay -= Time.deltaTime;
            yield return null;
        }

        currentGameState = GameState.INPUT;
    }

    //MOVES
    private void CalculateMoves()
    {
        AudioManager.instance.PlaySound(moveClip);

        Cell currentClickedCell = cellsDictionary[startClickGrid];
        Cell endClickedCell = cellsDictionary[startClickGrid + endClickGrid];

        //VALID STARTPOS
        if (!IsValidPos(startClickGrid))
        {
            stateDelay = 0f;
            StartCoroutine(SwitchStateAfterDelay());
            return;
        }

        //VALID ENDPOS AND HAS MOVES
        if (!IsValidPos(startClickGrid + endClickGrid) || !(currentClickedCell.cellData.moves > 0))
        {
            stateDelay = 0f;
            StartCoroutine(SwitchStateAfterDelay());
            return;
        }

        //INVALID SAVE COLOR
        if (currentClickedCell.cellData.color == endClickedCell.cellData.color)
        {
            stateDelay = 0f;
            StartCoroutine(SwitchStateAfterDelay());
            return;
        }

        //MOVE FOR EMPTY CELL
        if (endClickedCell.cellData.color == -1)
        {
            currentClickedCell.cellData.moves -= 1;
            StartCoroutine(currentClickedCell.UpdateMoves());

            var temp = endClickedCell.cellData.gridPos;
            endClickedCell.cellData.gridPos = currentClickedCell.cellData.gridPos;
            currentClickedCell.cellData.gridPos = temp;

            StartCoroutine(currentClickedCell.MoveToPos());
            StartCoroutine(endClickedCell.MoveToPos());

            cellsDictionary[startClickGrid] = endClickedCell;
            cellsDictionary[startClickGrid + endClickGrid] = currentClickedCell;

            stateDelay = Constants.Values.ANIMATION_TIME;
            StartCoroutine(SwitchStateAfterDelay());

            return;
        }
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
