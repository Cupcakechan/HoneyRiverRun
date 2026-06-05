using UnityEngine;
using UnityEngine.EventSystems;

/// Plays UI hover/click SFX through the AudioManager. Add to any Button.
/// Covers mouse (pointer) and keyboard/gamepad navigation (select/submit).
public class ButtonSound : MonoBehaviour,
    IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
{
    public void OnPointerEnter(PointerEventData e) => Hover();
    public void OnSelect(BaseEventData e)          => Hover();
    public void OnPointerClick(PointerEventData e) => Click();
    public void OnSubmit(BaseEventData e)          => Click();

    private void Hover()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIHover();
    }

    private void Click()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayUIClick();
    }
}