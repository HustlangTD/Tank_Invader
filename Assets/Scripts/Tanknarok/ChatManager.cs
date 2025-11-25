using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FusionExamples.Tanknarok
{
    public class ChatManager : NetworkBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField _chatInput;
        [SerializeField] private TextMeshProUGUI _chatContent;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _chatPanel; // Panel chứa toàn bộ chat UI để ẩn hiện nếu muốn

        [Header("Settings")]
        [SerializeField] private int _maxMessages = 20;

        private List<string> _messages = new List<string>();
        private bool _isTyping = false;

        public static ChatManager Instance { get; private set; }

        private void Awake()
        {
            // Singleton pattern đơn giản để dễ gọi từ nơi khác
            if (Instance != null && Instance != this)
            {
                // Nếu đã có 1 cái (từ Lobby lần trước còn lưu lại), thì hủy cái mới này đi
                Destroy(gameObject);
                return;
            }

            // Nếu chưa có, gán cái này là Instance duy nhất
            Instance = this;

            // Giữ cho khung chat này sống sót khi chuyển sang Level 1, Level 2
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Ẩn input field lúc đầu
            _chatInput.gameObject.SetActive(false);
            
            // Lắng nghe sự kiện khi người dùng submit tin nhắn (ấn Enter khi đang gõ)
            _chatInput.onSubmit.AddListener(OnSubmitMessage);
        }

        private void Update()
        {
            // Xử lý phím Enter để bật/tắt chat
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (_isTyping)
                {
                    // Nếu đang gõ mà ấn Enter -> Gửi tin nhắn (đã xử lý ở OnSubmitMessage) hoặc đóng nếu rỗng
                    if (string.IsNullOrWhiteSpace(_chatInput.text))
                    {
                        ToggleChat(false);
                    }
                }
                else
                {
                    // Nếu chưa gõ -> Mở khung chat
                    ToggleChat(true);
                }
            }
        }

        private void ToggleChat(bool status)
        {
            _isTyping = status;
            _chatInput.gameObject.SetActive(status);

            if (status)
            {
                _chatInput.ActivateInputField(); // Focus vào ô nhập
                Cursor.lockState = CursorLockMode.None; // Mở khóa chuột
                Cursor.visible = true;
                
                // Tạm thời vô hiệu hóa input điều khiển xe tăng (nếu cần)
                InputController.fetchInput = false; 
            }
            else
            {
                _chatInput.text = ""; // Xóa text cũ
                _chatInput.DeactivateInputField();
                
                // Khóa chuột lại để chơi game (tùy logic game của bạn)
                // Cursor.lockState = CursorLockMode.Locked; 
                // Cursor.visible = false;

                // Bật lại điều khiển xe tăng
                InputController.fetchInput = true;
            }
        }

        private void OnSubmitMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                // Lấy tên người chơi hiện tại từ App.cs (biến static bạn đã làm ở bước trước)
                string senderName = App.LocalPlayerName;

                // Gửi RPC đến tất cả mọi người (RpcTargets.All)
                RPC_SendMessage(senderName, message);
            }

            // Gửi xong thì đóng khung input
            ToggleChat(false);
        }

        // [Rpc] đánh dấu đây là hàm gửi qua mạng
        // RpcSources.All: Ai cũng có thể gọi
        // RpcTargets.All: Gửi tới tất cả mọi người
        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SendMessage(string name, string message)
        {
            // Format tin nhắn: [Tên]: Nội dung
            string formattedMsg = $"<b><color=#FFD700>{name}</color></b>: {message}";
            AddMessageToChat(formattedMsg);
        }

        private void AddMessageToChat(string msg)
        {
            _messages.Add(msg);

            // Giới hạn số lượng tin nhắn để tránh đầy bộ nhớ
            if (_messages.Count > _maxMessages)
            {
                _messages.RemoveAt(0);
            }

            // Cập nhật UI Text
            _chatContent.text = string.Join("\n", _messages);

            // Tự động cuộn xuống dưới cùng
            StartCoroutine(ScrollToBottom());
        }

        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        // [THÊM]: Hàm này dùng để xóa Chat khi quay về Main Menu (nếu cần)
        public void ResetChat()
        {
            Destroy(gameObject);
        }
    }
}