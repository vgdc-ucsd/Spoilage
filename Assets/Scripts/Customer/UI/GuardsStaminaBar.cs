using UnityEngine;
using UnityEngine.UI;

public class GuardsStaminaBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Image FillImage;
    private GameObject guardStaminaFill;
    private GameObject guardStaminaArrow;
    public int maxPresses = 5;
    public float buttonPressedReduceAmount;
    private float pressesUsed;
    /*public float endAngle = 180f;
    public float startAngle = -90f;
    */
    private float barLeftEdge;
    private float barRightEdge;
    public RectTransform rectTransform;
    void Start()
    {
        guardStaminaFill = GameObject.FindGameObjectWithTag("Stamina Bar Fill");
        guardStaminaArrow = GameObject.FindGameObjectWithTag("Stamina Bar Arrow");
        rectTransform = guardStaminaFill.GetComponent<RectTransform>();
        barLeftEdge = rectTransform.position.x - rectTransform.rect.width / 2;
        barRightEdge = rectTransform.position.x + rectTransform.rect.width / 2;
        FillImage = guardStaminaFill.GetComponent<Image>();
        buttonPressedReduceAmount = 1.0f / maxPresses;
        pressesUsed = 0;
        guardStaminaArrow.transform.position = new Vector3(barRightEdge, guardStaminaArrow.transform.position.y, guardStaminaArrow.transform.position.z);

        //rectTransform.rotation = Quaternion.Euler(0, 0, startAngle);
    }

    // Update is called once per frame
    public void buttonPressed()
    {
        if(pressesUsed >= maxPresses)
        {
            Debug.Log("Guard is too eepy to move!");
            return;
        }
        pressesUsed++;
        FillImage.fillAmount = 1 - (pressesUsed * buttonPressedReduceAmount);
        guardStaminaArrow.transform.position = new Vector3(Mathf.Lerp(barLeftEdge, barRightEdge, FillImage.fillAmount), guardStaminaArrow.transform.position.y, guardStaminaArrow.transform.position.z);
        /*
        float rotAngle = pressesUsed / maxPresses * (endAngle - startAngle) + startAngle;
        rectTransform.rotation = Quaternion.Euler(0, 0, rotAngle);
        */
    }
    public void setMaxPresses(int newMaxPresses)
    {
        maxPresses = newMaxPresses;
        buttonPressedReduceAmount = 1.0f / maxPresses;
        pressesUsed = 0;
    }
}
