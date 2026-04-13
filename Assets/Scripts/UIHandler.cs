using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    //Elementos visuales de la interfaz
    private VisualElement m_HealthBar;
    private VisualElement m_NonPlayerDialogue;
    private VisualElement m_LoseScreen;
    private VisualElement m_WinScreen;

    [Header("Elementos Bosses")]
    private VisualElement m_BossHealtBar;
    private VisualElement m_BossHealtBarBackground;
    public enum BossHealthBarIndex { Boss1 = 1, Boss2 = 2 }
    public BossHealthBarIndex m_bossHealthBarIndex; // Índice para seleccionar la barra de salud del jefe en el UI Document

    public float displayTime = 4.0f;
    private float m_TimerDisplay;

    //Instancia estática para acceder 
    public static UIHandler instance { get; private set; }


    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Obtiene el documento UIDocument adjunto al objeto
        UIDocument uiDocument = GetComponent<UIDocument>();

        //Busca los elementos en el documento
        m_HealthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_LoseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
        m_WinScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");

        // Configuracion inicial 
        //SetHealthValue(0.5f);
        m_NonPlayerDialogue.style.display = DisplayStyle.None;
        m_TimerDisplay = -1.0f;

        // Busca la barra de salud del jefe según el índice seleccionado
        if (m_bossHealthBarIndex == BossHealthBarIndex.Boss1)
        {
            m_BossHealtBar = uiDocument.rootVisualElement.Q<VisualElement>("BossHealthBar");
            m_BossHealtBarBackground = uiDocument.rootVisualElement.Q<VisualElement>("BossHealthBarBackground");
        } 
        else if (m_bossHealthBarIndex == BossHealthBarIndex.Boss2)
        {
            m_BossHealtBar = uiDocument.rootVisualElement.Q<VisualElement>("BossHealthBar2");
            m_BossHealtBarBackground = uiDocument.rootVisualElement.Q<VisualElement>("BossHealthBarBackground2");
        }

        if(m_BossHealtBar != null && m_BossHealtBarBackground != null)
            m_BossHealtBarBackground.style.visibility = Visibility.Visible; 
    }

    // Update is called once per frame
    void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    public void SetHealthValue(float percentage)
    {
        //m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_HealthBar.style.width = Length.Percent(100 * percentage);

    }

    public void SetBossHealthValue(float percentage)
    {
        //m_BossHealtBar.style.height = Length.Percent(100 * percentage);
        m_BossHealtBar.style.scale = new Vector3(1f, percentage, 1f);
    }


    public void DisplayDialogue()
    {
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    // Hace visible las pantallas de fin con la opacidad (Negro a visible)----------------
    public void DisplayWinScreen()
    {
        m_WinScreen.style.opacity = 1.0f;
    }
    public void DisplayLoseScreen()
    {
        m_LoseScreen.style.opacity = 1.0f;
    }
    //----------------------------------------------------------------------------------
}
