using System;
using UnityEngine;

namespace Tetris.Tools
{
    public static class TimerExtention {

        public static Timer TimerRegister (this TetrisComponent behaviour, float duration, Action onComplete, Action onUpdate = null,
            bool isLooped = false, bool useRealTime = false) {

            return Timer.Register (duration, onComplete, onUpdate, isLooped, useRealTime, behaviour);
        }
    }
}