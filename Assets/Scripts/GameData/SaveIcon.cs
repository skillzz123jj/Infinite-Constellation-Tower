using System.Collections;
using UnityEngine;
using TMPro;

public class SaveIcon : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text; 
    private string baseText = "Saving";
    private int dotCount = 3;

    //This runs the temporary animation for saving
    public IEnumerator AnimateDots()
    {
        int cycles = 0;

        while (cycles < 6)
        {
            dotCount = (dotCount + 1) % 4;

            text.text = baseText + new string('.', dotCount);

            yield return new WaitForSeconds(0.5f);

            cycles++;
        }

        text.enabled = false;
    }
}