using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public LayerMask Player;
    public Transform player;

    public void SetMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }
    public void SetStamina(int stamina)
    {
        slider.value = stamina;
    }
}
