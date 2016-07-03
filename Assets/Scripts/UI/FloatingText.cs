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
    public Countf randomVerticalOffset;

    private Text text;
    private Image img;
	private RectTransform rectTransform;
	private Vector3 speedOffset;
	private GameObject parent;
	private Vector3 parentPosition;
	private float parentRadius;
    private float horizontalOffset;
    private float verticalOffset;

    void Awake()
	{
		text = GetComponentInChildren<Text>();
        img = GetComponentInChildren<Image>();
		transform.SetParent(UIManager.instance.floatingTextHolder.transform);
		rectTransform = GetComponent<RectTransform>();
		speedOffset = Vector3.zero;
        horizontalOffset = randomHorizontalOffset.getRandom();
        verticalOffset = randomVerticalOffset.getRandom();
        if (duration > 0)
            StartCoroutine(fadeAfterSeconds(duration));
	}

    internal void show(bool v)
    {
        if (text)
            text.enabled = v;
        if (img)
            img.enabled = v;
    }

    // Update is called once per frame
    void FixedUpdate()
	{
		if (parent)
			parentPosition = parent.transform.position;

        speedOffset += Vector3.up * speed;
		rectTransform.position = Camera.main.WorldToScreenPoint
			(parentPosition
			+ Vector3.up * parentRadius * verticalOffset
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
        CircleCollider2D circleCol = parent.GetComponent<CircleCollider2D>();
        if (circleCol)
        {
            parentRadius = circleCol.radius;
            return;
        }
        BoxCollider2D boxCol = parent.GetComponent<BoxCollider2D>();
        if (boxCol)
            parentRadius = Mathf.Max(boxCol.size.x + Mathf.Abs(boxCol.offset.x), boxCol.size.y + Mathf.Abs(boxCol.offset.y));

    }

    /// <summary>
    /// Initialize at a specified position in space
    /// </summary>
    /// <param name="position"></param>
    /// <param name="t"></param>
    public void initialize(Vector3 position, string t)
    {
        text.text = t;
        parentPosition = position;
        parentRadius = 0;
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
        CircleCollider2D col = parent.GetComponent<CircleCollider2D>();
        if (col)
            parentRadius = col.radius;
        else
            parentRadius = 1.0f;
	}

    public void setCriticalHit()
    {
        text.color = Color.red;
        text.fontSize = 24;
        text.fontStyle = FontStyle.Bold;
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

    public void setText(String t)
    {
        text.text = t;
    }
}
