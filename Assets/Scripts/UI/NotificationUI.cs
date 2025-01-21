using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NotificationUI : MonoBehaviour
{
    private VisualElement _root;

    void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
    }

    /// <summary>
    /// Send an alert to user.
    /// </summary>
    /// <param name="text">Text to show.</param>
    /// <param name="duration">Duration in seconds.</param>
    public void SendAlert(string text, float duration)
    {
        StartCoroutine(Alert(text, duration));
    }

    private IEnumerator Alert(string text, float time)
    {
        Label alertText = new Label();
        string identifier = System.Guid.NewGuid().ToString();

        alertText.text = text;
        alertText.AddToClassList("notification");
        alertText.name = $"alert_{identifier}";
        alertText.style.flexGrow = 0;
        
        _root.Add(alertText);

        alertText.style.opacity = 1f;

        yield return new WaitForSeconds(time);

        alertText.style.opacity = 0f;

        yield return new WaitForSeconds(1f);

        _root.Remove(alertText);
    }
}
