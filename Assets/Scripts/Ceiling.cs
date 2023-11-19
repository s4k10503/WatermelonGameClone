using UnityEngine;

namespace SuikaGameClone
{
    public class Ceiling : MonoBehaviour
    {
        private float _stayTime;
        private readonly float _timeLimit = 0.5f;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Sphere"))
            {
                _stayTime += Time.deltaTime;
                if (_stayTime > _timeLimit)
                {
                    GameManager.Instance.SetGameState(GameModel.GameState.GameOver);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Sphere"))
                _stayTime = 0;
        }
    }
}