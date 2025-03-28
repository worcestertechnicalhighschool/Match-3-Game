using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] SpriteRenderer bg;
    [SerializeField] Color color;
    private int changeTimer;
    private float hue;

    private void Start()
    {
        hue = Random.Range(0, 254);
        bg = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        changeTimer++;

        if (changeTimer >= Time.deltaTime * 300)
        {
            color = Color.HSVToRGB(hue / 255, 1, 1);
            bg.color = color;
            hue++;
            changeTimer = 0;
            if (hue == 255) hue = 0;
        }
    }
}