using System;
using UnityEngine;
using Zenject;

namespace _Project.Code.Runtime.CommonServices.TimeManagement
{
    public class CountdownTimer
    {
        public event Action OnElapsed;
        public event Action<int> OnSecondElapsed;

        private float _duration;
        private float _remaining;
        private bool _isRunning;
        private int _lastSecond;
        
        public float Duration => _duration;
        public float Remaining => _remaining;
        
        public void SetUp(float duration)
        {
            _duration = duration;
            _remaining = duration;
            _lastSecond = Mathf.CeilToInt(duration);
        }

        public void Start() => _isRunning = true;

        public void Stop() => _isRunning = false;

        public void Tick()
        {
            if (!_isRunning) return;

            _remaining -= Time.deltaTime;

            var currentSecond = Mathf.CeilToInt(_remaining);
            if (currentSecond < _lastSecond)
            {
                _lastSecond = currentSecond;
                OnSecondElapsed?.Invoke(currentSecond);
            }

            if (_remaining > 0) return;

            _remaining = 0;
            _isRunning = false;
            OnElapsed?.Invoke();
        }
    }
}