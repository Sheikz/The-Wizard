using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FloatingText : MonoBehaviour
{
	public float speed;
	[Range(0, 10)]
	public float duration;
    public Countf randomHorizontalOffset;

	private Text text;
	private RectTransform rectTransform;
	private Vector3 speedOffset;
	private GameObject parent;
	private Vector3 parentPosition;
	private float parentRadius;
    private float horizontalOffset;

    void Awake()
	{
		text = GetComponent<Text>();
		transform.SetParent(UIManager.instance.floatingTextHolder.transform);
		rectTransform = GetComponent<RectTransform>();
		speedOffset = Vector3.zero;
        horizontalOffset = Random.Range(randomHorizontalOffset.minimum, randomHorizontalOffset.maximum);
		StartCoroutine(fadeAfterSeconds(duration));
	}

    // Update is called once per frame
    void Update ()
	{
		if (parent)
			parentPosition = parent.transform.position;

        speedOffset += Vector3.up * speed / Screen.height;
		rectTransform.position = Camera.main.WorldToScreenPoint
			(parentPosition
			+ Vector3.up * parentRadius
			+ Vector3.right * parentRadius * horizontalOffset
            ) 
			+ speedOffset;
	}

    /// <summary>
    /// Initialize the specified parent and text.
    /// </summary>
    /// <param name="parent">Parent.</param>
    /// <param name="text">Text.</param>
    public void initialize(GameObject parent, string t)
    {
        text.text = t;
        this.parent = parent;
        parentPosition = parent.transform.position;
        parentRadius = parent.GetComponent<CircleCollider2D>().radius;
    }

    /// <summary>
    ///  Version with numbers, apply the division if necessary
    /// </summary>
    /// <param name="parent">Parent.</param>
    /// <param name="value">Value.</param>
	public void initialize(GameObject parent, int value) // Initialization attached to a parent
	{
        if (value >= 1000000)
            text.text = (value / 1000000).ToString() + "M";
        else if (value >= 1000)
            text.text = (value / 1000).ToString() + "K";
        else
            text.text = value.ToString();
        
		this.parent = parent;
		parentPosition = parent.transform.position;
		parentRadius = parent.GetComponent<CircleCollider2D>().radius;
	}

	IEnumerator fadeAfterSeconds(float duration)
	{
		float startingTime = Time.time;
		while (Time.time - startingTime < duration)
		{
			Color newColor = text.color;
			newColor.a = Mathf.Lerp(1, 0, (Time.time - startingTime) / duration);
			text.color = newColor;
			yield return null;
		}
		Destroy(gameObject);
	}

	public void setColor(Color color)
	{
		text.color = color;
	}
}
