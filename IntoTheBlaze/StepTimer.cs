using System;
using Microsoft.Xna.Framework;

namespace IntoTheBlaze {

    internal class StepTimer {

        private int interval, ticks;

        private float eltime;

        public int Interval {
            get => interval;
            set {
                if (value < 1) throw new ArgumentOutOfRangeException("interval");
                interval = value;
            }
        }

        public StepTimer(int interval = 60) {
            Interval = interval;
            ticks = 0;
            eltime = 0;
        }

        public void Update(GameTime time) {
            eltime += (float)time.ElapsedGameTime.TotalMilliseconds / 100f * 6f;
            if (eltime > interval) {
                ticks += (int)(eltime / interval);
                eltime %= interval;
            }
        }

        public bool HasTick() => ticks > 0;

        public bool TakeTick() {
            if (ticks > 0) {
                --ticks;
                return true;
            }
            return false;
        }

        public int TakeTicks() {
            var c = ticks;
            ticks = 0;
            return c;
        }

        public void Reset() {
            ticks = 0;
            eltime = 0;
        }
    }
}
