using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntoTheBlaze {

    internal interface IParticle {

        void Initialize(float x, float y, Random r);

        void Update(float steps);

        void Draw(SpriteBatch batch);

        bool IsAlive { get; }

        bool IsGlowing { get; }
    }
}
