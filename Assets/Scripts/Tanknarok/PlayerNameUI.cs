using UnityEngine;
using TMPro;

namespace FusionExamples.Tanknarok
{
    public class PlayerNameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _nameText; // Sử dụng TMP 3D (không phải UI)

        public void SetName(string name)
        {
            if (_nameText != null)
                _nameText.text = name;
        }

        // Billboard: Luôn hướng về camera
        void LateUpdate()
        {
            // Cách đơn giản nhất để UI luôn nhìn về phía Camera
            transform.forward = Camera.main.transform.forward;
        }
    }
}