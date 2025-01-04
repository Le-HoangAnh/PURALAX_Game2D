using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Square : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private static Vector3 startScale = Vector3.one * 0.7f;
    private static Vector3 endScale = Vector3.one;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitializeSquare(int colorValue)
    {
        Color color = GameManager.instance.colors[colorValue];
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = Constants.Values.FRONT;
        StartCoroutine(StartAnimation());
    }

    public IEnumerator StartAnimation()
    {
        transform.localScale = startScale;

        float animationTime = Constants.Values.ANIMATION_TIME;
        Vector3 speed = (endScale - startScale) / animationTime;
        float endTime = 0f;
        while (endTime < animationTime)
        {
            transform.localScale += speed * Time.deltaTime;
            endTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
    }
}
