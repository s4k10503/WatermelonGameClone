using UnityEngine;

namespace WatermelonGameClone
{
    public class Ceiling : MonoBehaviour
    {
        private float _stayTime;
        private static readonly float s_timeLimit = 0.5f;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Sphere"))
            {
                _stayTime += Time.deltaTime;
                if (_stayTime > s_timeLimit)
                {
                    GameManager.Instance.GameEvent.Execute(GameModel.GameState.GameOver);
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