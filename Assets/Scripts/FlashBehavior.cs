using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashBehavior : MonoBehaviour
{
    private float flashDurationStop;
    private CanvasGroup myCanvasGroup;
    private Image flashImage;
    private bool isFlashing = false;
    private float flashDuration = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        myCanvasGroup = GetComponent<CanvasGroup>();
        flashImage = GameObject.Find("FlashPanel").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFlashing)
        {
            myCanvasGroup.alpha = 1;
            flashDuration += Time.deltaTime;
            if (flashDuration > flashDurationStop)
            {
                myCanvasGroup.alpha = 0;
                isFlashing = false;
                flashDuration = 0.0f;
            }
        }
    }

    public bool IsFlashing()
    {
        return isFlashing;
    }

    public void DoFlash(Color flashRgb, float setStopTime = 0.15f)
    {
        if (!isFlashing)
        {
            flashDurationStop = setStopTime;
            flashImage.color = flashRgb;
            isFlashing = true;
        }
    }
}
