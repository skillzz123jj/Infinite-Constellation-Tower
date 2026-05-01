using UnityEngine;

public class PowerBarAnimationController : MonoBehaviour
{
    [SerializeField] GameObject fullPowerbar;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFullPowerbar()
    {
        fullPowerbar.SetActive(true);
    }
}
