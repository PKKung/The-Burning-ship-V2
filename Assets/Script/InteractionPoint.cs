using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    public SignalMiniGame signalGame; // ลากตัวมินิเกมมาใส่
    public float range = 3f;
    public Transform player;

    private void OnMouseUp()
    {
        // Safety check if player is assigned
        if (player == null) return;

        if (Vector2.Distance(player.position, transform.position) > range) return;

        // Check with Manager if parts are collected for the current round
        if (SignalQuestManager.instance.IsReadyToSignal())
        {
            signalGame.StartMiniGame();
        }
        else
        {
            int current = SignalQuestManager.instance.partsCollected;
            int total = SignalQuestManager.instance.currentRound;
            int missing = total - current;

            // Log for developer (English)
            Debug.Log($"Not enough parts! Need {missing} more.");

            // Message for player (English) - This goes to the UI AlertText
            string failMessage = $"Not enough parts! Need {missing} more ({current}/{total})";
            SignalQuestManager.instance.ShowAlert(failMessage);
        }
    }
}