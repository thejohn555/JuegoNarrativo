using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CodingTools
{
    //Not finished
    internal class Timer 
    {
        public async Task Cooldown(float waitTime, bool check)
        {
            check = false;
            float currentTime = Time.time;
            while(Time.time - currentTime < waitTime)
            {
                await Task.Yield();
            }
            check = true;
        }
    }
}
