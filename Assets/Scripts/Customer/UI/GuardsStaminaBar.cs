using UnityEngine;
using UnityEngine.UI;

public class GuardsStaminaBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Image FillImage;
    public float fillRate = 0.01f;
    public float buttonPressedReduceAmount = 0.5f;
    void Start()
    {
        FillImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        FillImage.fillAmount += fillRate * Time.deltaTime;
    }
    public void buttonPressed()
    {
        FillImage.fillAmount -= buttonPressedReduceAmount;

        if(FillImage.fillAmount <= 0)
        {
            Debug.Log("Guard is exhausted!");
        }
    }
}
