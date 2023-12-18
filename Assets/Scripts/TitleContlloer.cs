using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WatermelonGameClone
{
    public class TitleContller : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnStartButtonClicked()
        {
            SceneManager.LoadScene("Main");
        }
    }
}