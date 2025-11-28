using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FusionExamples.Tanknarok
{
    public class ChatManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField _chatInput;
        [SerializeField] private TextMeshProUGUI _chatContent;
        [SerializeField] private ScrollRect _scrollRect;

        [Header("Settings")]
        [SerializeField] private int _maxMessages = 20;

        private List<string> _messages = new List<string>();
        private bool _isTyping = false;

        public static ChatManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Ẩn khung nhập lúc đầu
            if (_chatInput != null)
                _chatInput.gameObject.SetActive(false);

            if (_chatInput != null)
                _chatInput.onSubmit.AddListener(OnSubmitMessage);
        }

        private void Update()
        {
            // Bấm Enter để bật/tắt khung chat
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (_isTyping)
                {
                    // Nếu đang gõ mà bấm Enter (và ô trống) thì đóng lại
                    if (string.IsNullOrWhiteSpace(_chatInput.text))
                    {
                        ToggleChat(false);
                    }
                    // Nếu có chữ thì OnSubmitMessage sẽ được gọi tự động bởi InputField
                }
                else
                {
                    // Mở khung chat
                    ToggleChat(true);
                }
            }
        }

        // Hàm bị thiếu đây ạ
        private void ToggleChat(bool status)
        {
            _isTyping = status;
            
            if (_chatInput != null)
                _chatInput.gameObject.SetActive(status);

            if (status)
            {
                _chatInput.ActivateInputField();
                
                // Hiện con trỏ chuột để chat
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // Tắt điều khiển xe tăng khi đang chat
                InputController.fetchInput = false;
            }
            else
            {
                _chatInput.text = ""; // Xóa chữ vừa nhập
                _chatInput.DeactivateInputField();

                // Bật lại điều khiển xe tăng
                InputController.fetchInput = true;
            }
        }

        private void OnSubmitMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                string senderName = App.LocalPlayerName;
                SendChatViaPlayer(senderName, message);
            }
            // Gửi xong thì đóng khung nhập lại
            ToggleChat(false);
        }

        private void SendChatViaPlayer(string name, string message)
        {
            // Tìm Player của chính mình để nhờ gửi RPC
            var players = FindObjectsByType<Player>(FindObjectsSortMode.None);
            foreach (var p in players)
            {
                // HasInputAuthority nghĩa là Player do máy này điều khiển
                if (p.Object != null && p.Object.HasInputAuthority)
                {
                    p.RPC_SendChatMessage(name, message);
                    break;
                }
            }
        }

        public void DisplayMessage(string name, string message)
        {
            string formattedMsg = $"<b><color=#FFD700>{name}</color></b>: {message}";
            AddMessageToChat(formattedMsg);
        }

        private void AddMessageToChat(string msg)
        {
            _messages.Add(msg);
            if (_messages.Count > _maxMessages)
            {
                _messages.RemoveAt(0);
            }
            _chatContent.text = string.Join("\n", _messages);
            StartCoroutine(ScrollToBottom());
        }

        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            _scrollRect.verticalNormalizedPosition = 0f;
        }

        public void ResetChat()
        {
            Destroy(gameObject);
        }
    }
}