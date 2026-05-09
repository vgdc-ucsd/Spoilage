using UnityEngine;
using UnityEngine.UI;

public class GuardsStaminaBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //private RawImage FillImage;
    public int maxPresses = 5;
    public float buttonPressedReduceAmount;
    private float pressesUsed;
    public float radialAngle = 180f;
    public float startAngle = -90f;
    public RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //FillImage = GetComponent<RawImage>();
        buttonPressedReduceAmount = 1.0f / maxPresses;
        pressesUsed = 0;
        
    }

    // Update is called once per frame
    public void buttonPressed()
    {
        /*if(FillImage.fillAmount <= 0)
        {
            Debug.Log("Guard is too eepy to move!");
        }*/
        //FillImage.fillAmount -= buttonPressedReduceAmount;
        pressesUsed++;
        if(pressesUsed > maxPresses)
        {
            Debug.Log("Guard is too eepy to move!");
            return;
        }
        float rotAngle = pressesUsed / maxPresses * radialAngle + startAngle;
        rectTransform.rotation = Quaternion.Euler(0, 0, rotAngle);

    }
    public void setMaxPresses(int newMaxPresses)
    {
        maxPresses = newMaxPresses;
        buttonPressedReduceAmount = 1.0f / maxPresses;
        pressesUsed = 0;
    }
}
