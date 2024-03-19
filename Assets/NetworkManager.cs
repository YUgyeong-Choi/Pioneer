// using UnityEngine;
// using Photon.Pun;
// using Photon.Realtime;
//
// public class NetworkManager : MonoBehaviourPunCallbacks
// {
//     // 싱글톤 인스턴스
//     public static NetworkManager instance = null;
//     public Transform centerEyeAnchor;
//     public GameObject playerPrefab;
//
//     // 플레이어가 생성될 위치의 배열
//     [SerializeField] private Transform[] m_SpawnPoints;
//
//     // 생성된 플레이어 오브젝트 참조
//     private GameObject m_Player;
//
//     private void Awake()
//     {
//         // GameManager 인스턴스 초기화
//         if (instance == null)
//         {
//             instance = this;
//         }
//     }
//
//     // Start is called before the first frame update
//     void Start()
//     {
//         Debug.Log("실행");
//         Connect();
//     }
//
//     public void Connect()
//     {
//         // 연결 되었는지를 체크해서, 룸에 참여할지 재연결을 시도할지 결정
//         if (PhotonNetwork.IsConnected)
//         {
//             // 랜덤 룸에 접속.
//             // 접속에 실패하면 OnJoinRandomFailed()이 실행되어 실패 알림.
//             PhotonNetwork.JoinRandomRoom();
//         }
//         else
//         {
//             // 서버 연결에 실패하면 서버에 연결 시도
//             PhotonNetwork.ConnectUsingSettings();
//         }
//     }
//
//     public override void OnConnectedToMaster()
//     {
//         Debug.Log("클라이언트가 마스터에 연결됨");
//         // 마스터에 연결되면 방에 랜덤으로 입장
//         PhotonNetwork.JoinRandomRoom();
//     }
//
//     public override void OnDisconnected(DisconnectCause cause)
//     {
//         Debug.LogWarningFormat("서버와의 연결이 끊어짐. 사유 : {0}", cause);
//     }
//     // 랜덤 방 입장에 실패할 경우 호출됨
//     public override void OnJoinRandomFailed(short returnCode, string message)
//     {
//         Debug.Log("랜덤 방 입장에 실패함. 새로운 방 생성");
//         // 랜덤 방 입장에 실패하면 서버 연결이 끊기지 않았다면, 방이 가득 찼거나 방이 없거나이므로 방을 새로 생성
//         PhotonNetwork.CreateRoom(null, new RoomOptions());
//     }
//
//     // 방에 입장하면 호출됨 
//     public override void OnJoinedRoom()
//     {
//         Debug.Log("방 입장 완료");
//         // 현재 방의 플레이어 수에 따라 적절한 스폰 포인트 선택
//         Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
//         // Vector3 pos = m_SpawnPoints[PhotonNetwork.CurrentRoom.PlayerCount - 1].position;
//         // Quaternion rot = m_SpawnPoints[PhotonNetwork.CurrentRoom.PlayerCount - 1].rotation;
//
//         spawnPlayer();
//     }
//
//     public void spawnPlayer()
//     {
//         GameObject playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
//         if (playerInstance.GetComponent<PhotonView>().IsMine)
//         {
//             SyncPosition syncScript = playerInstance.GetComponent<SyncPosition>();
//             if (syncScript != null)
//             {
//                 syncScript.target = centerEyeAnchor;
//             }
//         }
//     }
//
//     public override void OnLeftRoom()
//     {
//         // 방을 떠날 때 플레이어 오브젝트 파괴
//         base.OnLeftRoom();
//         // PhotonNetwork.Destroy(m_Player);
//     }
// }
