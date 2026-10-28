using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.CodingTools
{
    internal class Timer : MonoBehaviour
    {
        public static Timer Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
                //    else
                //    {
                //        Destroy(gameObject);
                //    }
        }

        public IEnumerator Countdown(float time, bool check)
        {
            check = false;
            yield return new WaitForSeconds(time);
            check = true;
        }
    }
}
