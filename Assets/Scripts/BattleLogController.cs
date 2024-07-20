using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

    public void AddToLog(List<Argument> list)
    {
        if (_battleLog.childCount == 5)
            _battleLog.RemoveAt(1);
        VisualElement log = UiComponent.Instantiate();
        log.Q<Label>("text").text = list[0].Parameter.ToString();
        _battleLog.Add(log);

    }
}
