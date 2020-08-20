using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBehavior : MonoBehaviour
{
    private Text myTextComponent;
    private CanvasGroup canvasGroup;    

    // Start is called before the first frame update
    void Start()
    {        
        myTextComponent = GetComponent<Text>();
        canvasGroup = GameObject.Find("MessageCanvas").GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMessage(string message)
    {
        myTextComponent.text = message;
        canvasGroup.alpha = 100;
    }

    public void HideMessage()
    {        
        myTextComponent.text = "";
        canvasGroup.alpha = 0;
    }
}
