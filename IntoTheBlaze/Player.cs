using System;
using IntoTheBlaze.Game;
using IntoTheBlaze.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IntoTheBlaze {

    internal class Player {

        private LevelScreen levelScreen;
        private IntoTheBlaze game;

        private float rotation;

        public float HP { get; set; }

        public bool IsDead => HP <= 0;

        public Vector2 Position;

        private Texture2D texture;

        private ExtinguishingAgent[] agentTypes;
        private float[] agentReserves;

        public void Initialize(LevelScreen levelScreen, IntoTheBlaze game) {
            this.levelScreen = levelScreen;
            this.game = game;
            Position = Vector2.Zero;
            rotation = 90;
            Reset();
        }

        public void Reset() {
            HP = 100f;
            agentTypes = new[] { null, ExtinguishingAgent.Water, null };
            agentReserves = new[] { 0f, 100f, 0f };
        }

        public void LoadContent() => texture = game.Content.Load<Texture2D>("firefighter");

        public void Update(GameTime time, HeatMap heatMap, GamePartSystem partSystem) {
            var h = 0;
            var v = 0;
            if (KeyboardHelper.Key(Keys.A)) h -= 1;
            if (KeyboardHelper.Key(Keys.D)) h += 1;
            if (KeyboardHelper.Key(Keys.W)) v -= 1;
            if (KeyboardHelper.Key(Keys.S)) v += 1;
            if (h != 0 || v != 0) {
                var vec = new Vector2(h, v);
                vec.Normalize();
                vec *= 3;
                if (!levelScreen.CollidesWithSolidOrEdge(
                        new Circle(
                            (int)Math.Round(Position.X + vec.X),
                            (int)Math.Round(Position.Y + vec.Y),
                            12
                        )
                    )
                ) Position += vec;
            }

            var hydrant = levelScreen.GetNearestHydrant(Position, 48f);
            if (hydrant != null) {
                var type = hydrant.Type.AgentSupply;
                int i;
                for (i = 0; i < 3; ++i) if (agentTypes[i] == type) break;
                if (i < 3) agentReserves[i] = Math.Min(100f, agentReserves[i] + 3);
            }

            if (KeyboardHelper.KeyPressed(Keys.E)) {
                var t = agentTypes[0];
                agentTypes[0] = agentTypes[1];
                agentTypes[1] = agentTypes[2];
                agentTypes[2] = t;
                var r = agentReserves[0];
                agentReserves[0] = agentReserves[1];
                agentReserves[1] = agentReserves[2];
                agentReserves[2] = r;
            }
            if (KeyboardHelper.KeyPressed(Keys.Q)) {
                var t = agentTypes[2];
                agentTypes[2] = agentTypes[1];
                agentTypes[1] = agentTypes[0];
                agentTypes[0] = t;
                var r = agentReserves[2];
                agentReserves[2] = agentReserves[1];
                agentReserves[1] = agentReserves[0];
                agentReserves[0] = r;
            }

            var dir = MouseHelper.Pos - Position;
            if (dir != Vector2.Zero) {
                rotation = (float)Math.Atan2(dir.Y, dir.X);
                if (MouseHelper.Left && agentTypes[1] != null && agentReserves[1] > 0) {
                    dir.Normalize();
                    var pos = Position + dir * 16 + new Vector2(-dir.Y, dir.X) * 7;
                    partSystem.AddExtAgentPart(pos, 360 - MathHelper.ToDegrees(rotation), ExtinguishingAgent.Water);
                    partSystem.AddExtAgentPart(pos, 360 - MathHelper.ToDegrees(rotation), ExtinguishingAgent.Water);
                    agentReserves[1] = Math.Max(0f, agentReserves[1] - (float)time.ElapsedGameTime.TotalMilliseconds * 0.06f * 0.06f);
                }
            }

            var heat = heatMap.GetHeat(Position.ToPoint());
            if (heat > 200f) HP -= (heat - 200f) * (float)time.ElapsedGameTime.TotalMilliseconds * 0.0006f;
        }

        public Tuple<ExtinguishingAgent, float> GetAgentAmmount(int index) {
            index = MathHelper.Clamp(index, 0, 2);
            return agentTypes[index] == null ? null : new Tuple<ExtinguishingAgent, float>(agentTypes[index], agentReserves[index]);
        }

        public void Draw(SpriteBatch batch, GameTime time) {
            batch.Draw(
                texture,
                Position,
                null,
                Color.White,
                rotation,
                new Vector2(16, 16),
                1, SpriteEffects.None, 0
            );
        }
    }
}
