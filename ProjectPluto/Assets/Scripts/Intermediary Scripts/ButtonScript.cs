using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        GameObject.Find("IntermediaryManager").GetComponent<IntermediaryManager>().UISelectSFX();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        GameObject.Find("IntermediaryManager").GetComponent<IntermediaryManager>().UIPressSFX();
    }
}
