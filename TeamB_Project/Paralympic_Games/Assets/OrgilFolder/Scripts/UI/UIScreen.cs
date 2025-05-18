using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(CanvasGroup))]
public class UIScreen : MonoBehaviour
{
    public bool isModal = false;
    public UnityEvent onFocused;
    public UnityEvent onDefocused;

    CanvasGroup _group = null;
    public CanvasGroup Group
    {
        get
        {
            if (_group) return _group;
            return _group = GetComponent<CanvasGroup>();
        }
    }

    public virtual void Show(object data = null)
    {
        Focus();
    }

    public virtual void Hide()
    {
        Defocus();
    }

    public void Focus()
    {
        Group.interactable = true;
        gameObject.SetActive(true);
        onFocused?.Invoke();
    }

    public void Defocus()
    {
        gameObject.SetActive(false);
        onDefocused?.Invoke();
    }
}