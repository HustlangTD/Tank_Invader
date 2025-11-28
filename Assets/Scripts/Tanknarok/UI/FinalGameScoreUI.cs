using FusionExamples.Utility;
using FusionHelpers;
using UnityEngine;
using TMPro;

namespace FusionExamples.Tanknarok
{
	public class FinalGameScoreUI : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _crown;
		[SerializeField] private TextMeshPro _score;
		[SerializeField] private TextMeshPro _playerName;

		public void SetPlayerName(Player player)
		{
			// [SỬA LẠI]: Lấy biến NetName từ Player thay vì dùng PlayerIndex
			if (player.Object != null && !string.IsNullOrEmpty(player.NetName.ToString()))
			{
				_playerName.text = player.NetName.ToString();
			}
			else
			{
				// Fallback: Nếu chưa load được tên thì mới hiện Player Index
				_playerName.text = $"Player {player.PlayerIndex}";
			}

			Color textColor = player.playerMaterial.GetColor("_SilhouetteColor");
			_score.color = textColor;
			_playerName.color = textColor;
		}

		public void SetScore(int newScore)
		{
			_score.text = newScore.ToString();
		}

		public void ToggleCrown(bool on)
		{
			_crown.enabled = on;
		}
	}
}