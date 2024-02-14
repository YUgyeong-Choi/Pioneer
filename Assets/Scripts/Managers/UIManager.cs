public class UIManager
{
    // // UI Popup의 order를 관리하는 기능
    // private int _order = 10;
    //
    // // 팝업 목록을 저장 -> Stack 형태로
    // private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    // UI_Scene _sceneUI = null;
    //
    // public GameObject Root
    // {
    //     get
    //     {
    //         GameObject root = GameObject.Find("@UI_Root");
    //         if (root == null)
    //             root = new GameObject { name = "@UI_Root" };
    //         return root;
    //     }
    // }
    //
    // /// <summary>
    // /// Canvas 설정
    // /// </summary>
    // /// <param name="go"></param>
    // /// <param name="sort">PopupSystem과 연관이 없는 일반 Popup이라면 false</param>
    // public void SetCanvas(GameObject go, bool sort = true)
    // {
    //     Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
    //     canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    //     canvas.overrideSorting = true;
    //     if (sort)
    //     {
    //         canvas.sortingOrder = _order;
    //         _order++;
    //     }
    //     else    // PopupSystem과 연관이 없는 일반 Popup 
    //     {
    //         canvas.sortingOrder = 0;
    //     }
    // }
    //
    // public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    // {
    //     if (string.IsNullOrEmpty(name))
    //         name = typeof(T).Name;
    //
    //     GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");
    //     
    //     if(parent != null)
    //         go.transform.SetParent(parent);
    //         
    //     return Util.GetOrAddComponent<T>(go);
    // }
    //
    // public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    // {
    //     if (string.IsNullOrEmpty(name))
    //         name = typeof(T).Name;
    //
    //     GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
    //     T sceneUI = Util.GetOrAddComponent<T>(go);
    //     _sceneUI = sceneUI;
    //     
    //     go.transform.SetParent(Root.transform);
    //     
    //     return sceneUI;
    // }
    //
    // /// <summary>
    // /// 팝업을 띄우는 메서드
    // /// </summary>
    // /// <param name="name">UI Prefab의 이름 (선택) </param>
    // /// <typeparam name="T">UI_Popup 타입</typeparam>
    // /// <returns></returns>
    // public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    // {
    //     if (string.IsNullOrEmpty(name))
    //         name = typeof(T).Name;
    //
    //     GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
    //     T popup = Util.GetOrAddComponent<T>(go);
    //     _popupStack.Push(popup);
    //     
    //     go.transform.SetParent(Root.transform);
    //     
    //     return popup;
    // }
    //
    // /// <summary>
    // /// Stack의 가장 상단에 있는 popup을 지운다.
    // /// </summary>
    // public void ClosePopupUI()
    // {
    //     // Stack을 건드릴때는 항상 팝업을 건드리는 것을 습관화하자.
    //     if (_popupStack.Count == 0)
    //         return;
    //
    //     UI_Popup popup = _popupStack.Pop();
    //     Managers.Resource.Destroy(popup.gameObject);
    //     popup = null;
    //
    //     _order--;
    // }
    //
    // /// <summary>
    // /// ClosePopupUI의 좀 더 안전한 버전
    // /// </summary>
    // /// <param name="popup"></param>
    // public void ClosePopupUI(UI_Popup popup)
    // {
    //     if (_popupStack.Count == 0)
    //         return;
    //
    //     if (_popupStack.Peek() != popup)
    //     {
    //         Debug.Log("Close Popup Failed!");
    //         return;
    //     }
    //     
    //     ClosePopupUI();
    // }
    //
    // public void CloseAllPopupUI()
    // {
    //     while (_popupStack.Count > 0)
    //         ClosePopupUI();
    // }
    //
    // public void Clear()
    // {
    //     CloseAllPopupUI();
    //     _sceneUI = null;
    // }
}
