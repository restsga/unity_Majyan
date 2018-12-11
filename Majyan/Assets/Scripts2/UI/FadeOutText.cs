using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutText : MonoBehaviour {

    private const float speed = 1.0f;

    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Color newColor = spriteRenderer.color;
        newColor.a = Mathf.Max(0f, newColor.a - speed * Time.deltaTime);
        spriteRenderer.color = newColor;
    }
}
