using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    // 기존의 코드는 디아블로와 같이 마우스 입력이 중요한 게임을 만들 때 사용되는 코드.
    // VR 애플리케이션은 이 System을 사용할 수 없기 때문에 주석으로 처리함.
    // 코드는 참고용으로 활용될 수 있을 것이라 판단해 삭제하지 않음.
    
    // public Action KeyAction = null;
    // public Action<Define.MouseEvent> MouseAction = null;
    //
    // private bool _pressed = false;
    // // Click과 PointUp을 구분하기 위해, 마우스 버튼을 누르고 있는 시간을 기록하는 float
    // private float _pressedTime = 0;
    //
    // public void OnUpdate()
    // {
    //     if (EventSystem.current.IsPointerOverGameObject())  // UI가 클릭된 상황이라면 동작 X
    //         return;
    //     
    //     if (Input.anyKey && KeyAction != null)        // 어떠한 입력이라도 있고, KeyAction이 비어있지 않다면,
    //         KeyAction.Invoke();
    //
    //     if (MouseAction != null)
    //     {
    //         if (Input.GetMouseButton(0))                            // 마우스가 눌린다면,
    //         {
    //             if (!_pressed)      // 마우스 버튼이 안 눌리다가 처음 눌림  => PointerDown
    //             {
    //                 MouseAction.Invoke(Define.MouseEvent.PointerDown);
    //                 // 이 때 부터, 시간을 기록.
    //                 _pressedTime = Time.time;
    //             }
    //             MouseAction.Invoke(Define.MouseEvent.Press);    // (1)일단 Press에 해당하는 이벤트를 Invoke
    //             _pressed = true;                                   // (2)눌렸다가,
    //         }
    //         else                    
    //         {
    //             if (_pressed)       // 마우스가 눌리다가 안눌릴 때
    //             {
    //                 // 0.2초 내에 마우스 버튼을 다시 올리면, 그것을 클릭으로 인식
    //                 if(Time.time < _pressedTime + 0.2f)     
    //                     MouseAction.Invoke(Define.MouseEvent.Click);   // 떨어질 때, Click 이벤트를 Invoke
    //                 
    //                 // 그냥 PointerUp은 무조건 실행
    //                 MouseAction.Invoke(Define.MouseEvent.PointerUp);
    //             }
    //
    //             _pressed = false;
    //             _pressedTime = 0;       // 마우스 쿨릭 시간 기록 초기화
    //         }
    //     }
    // }
    //
    // public void Clear()
    // {
    //     KeyAction = null;
    //     MouseAction = null;
    // }
}
