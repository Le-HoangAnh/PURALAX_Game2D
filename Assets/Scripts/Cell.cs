using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public CellData cellData;
    public Vector2Int currentPos => new((int)transform.position.x, (int)transform.position.y);

    [SerializeField] private GameObject first, second;
    [SerializeField] private List<GameObject> circles;

    private Square frontSquare, backSquare;

    private void Awake()
    {
        cellData = new();
        cellData.color = -1;
        cellData.moves = 0;
        cellData.gridPos = currentPos;

        first.SetActive(false);
        second.SetActive(false);
        frontSquare = first.GetComponent<Square>();
        backSquare = second.GetComponent<Square>();

        for (int i = 0; i < circles.Count; i++)
        {
            circles[i].SetActive(false);
        }
    }

    public void InitializeCell(CellData data)
    {
        cellData = data;
        frontSquare.gameObject.SetActive(true);
        frontSquare.InitializeSquare(cellData.color);
        StartCoroutine(UpdateMoves());
    }

    public IEnumerator UpdateMoves()
    {
        yield return new WaitForSeconds(Constants.Values.ANIMATION_TIME);

        for (int i = 0; i < circles.Count;i++)
        {
            circles[i].SetActive(i < cellData.moves);
        }
    }

    public IEnumerator MoveToPos()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = new(cellData.gridPos.x + 0.5f, cellData.gridPos.y + 0.5f);
        float animationTime = Constants.Values.ANIMATION_TIME;
        Vector3 speed = (endPos - startPos) / animationTime;
        float endTime = 0f;
        while (endTime < animationTime)
        {
            transform.position += speed * Time.deltaTime;
            endTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
    }

    public IEnumerator ChangeColor(float delay)
    {
        yield return new WaitForSeconds(delay);

        backSquare.gameObject.SetActive(true);
        backSquare.StartColorSettings(cellData.color, true);
        frontSquare.StartColorSettings(cellData.color, false);

        var temp = frontSquare;
        frontSquare = backSquare;
        backSquare = temp;

        yield return frontSquare.StartAnimation();

        backSquare.gameObject.SetActive(false);
    }
}

[System.Serializable]
public struct CellData
{
    public int moves;
    public int color;
    public Vector2Int gridPos;
}
