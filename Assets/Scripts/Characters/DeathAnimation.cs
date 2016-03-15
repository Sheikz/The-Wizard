using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DeathAnimation : MonoBehaviour
{
    public float speed;
    [Range(0, 10)]
    public float duration;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(fadeAfterSeconds(duration));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * speed / Screen.height;
    }

    IEnumerator fadeAfterSeconds(float duration)
    {
        float startingTime = Time.time;
        while (Time.time - startingTime < duration)
        {
            Color newColor = spriteRenderer.color;
            newColor.a = Mathf.Lerp(1, 0, (Time.time - startingTime) / duration);
            spriteRenderer.color = newColor;
            yield return null;
        }
        Destroy(gameObject);
    }
}
