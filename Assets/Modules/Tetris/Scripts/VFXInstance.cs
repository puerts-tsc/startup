using Tetris.Tools;
using UnityEngine;

namespace Tetris
{
    public class VFXInstance : MonoBehaviour, IPoolable<VFXInstance>
    {
        public bool InPooled { get; set; }
        public Pool<VFXInstance> Pool { get; set; }

        private new ParticleSystem particleSystem;
        private Timer timer;

        private void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            particleSystem.Play();
            if (timer == null) timer = Timer.Register(particleSystem.main.duration, Free, null, false);
            timer.Restart();
        }

        private void Free()
        {
            particleSystem.Stop();
            Pool.Free(this);
        }
    }
}
