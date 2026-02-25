using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    //Elementos visuales de la interfaz
    private VisualElement m_HealthBar;
    private VisualElement m_NonPlayerDialogue;
    private VisualElement m_LoseScreen;
    private VisualElement m_WinScreen;

    //Instancia estática para acceder 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Obtiene el documento UIDocument adjunto al objeto
        UIDocument uiDocument = GetComponent<UIDocument>();

        //Busca los elementos en el documento
        m_HealthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealthValue(float percentage)
    {
        //m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_HealthBar.style.width = Length.Percent(100 * percentage);

    }
}
