
using UnityEngine;
using TMPro;

public class VidaUIController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private TMP_Text vidasText;

    private void OnEnable()
    {
        PlayerObserverManager.OnVidasChanged += UpdateVidasText;
    }

    

    private void OnDisable()
    {
        PlayerObserverManager.OnVidasChanged -= UpdateVidasText;
    }

    private void UpdateVidasText(int newVidasValue)
    {
        vidasText.text = newVidasValue.ToString();
    }
    
    
}




