using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/**
 * <summary>
 *
 * </summary>
 * <version>
 *  <date here>/07/2024
 * </version>
 * <author>
 *  Diogo Capela (<Email here>)
 * </author>
 */
public class BattleLogController : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset UiComponent;
    private UIDocument _uiDocument;
    private VisualElement _rootVE;
    private VisualElement _battleLog;

    void Start()
    {
        _uiDocument = GetComponent<UIDocument>();
        _rootVE = _uiDocument.rootVisualElement;
        _battleLog = _rootVE.Q<VisualElement>("BattleLog");
    }

    /**
         * <summary>
         *  Adds a new line to combat log and removes oldest line if needed
         * </summary>
         * <param name="list">
         *    list of arguments passed by event, contains text to add to battle log
         * </param>
         */
    public void AddToLog(List<Argument> list)
    {
        if (_battleLog.childCount == 5)
            _battleLog.RemoveAt(1);
        VisualElement log = UiComponent.Instantiate();
        log.Q<Label>("text").text = list[0].Parameter.ToString();
        _battleLog.Add(log);

    }
}
