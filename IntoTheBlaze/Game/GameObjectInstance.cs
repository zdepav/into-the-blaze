using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IntoTheBlaze.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntoTheBlaze.Game {

    public class GameObjectInstance {

        public GameObject Type { get; }

        public Point Position { get; }

        public float Rotation { get; }

        public float StartHP { get; }

        public float HP { get; private set; }

        public float Burning { get; private set; }

        public bool IsBurning => !IsDestroyed && Burning > 0;

        public bool IsDestroyed => HP <= 0;

        public IShape FireArea { get; }

        public ReadOnlyCollection<IShape> CollisionMasks { get; }

        public GameObjectInstance(GameObject type, Point position, float rotation) {
            Type = type;
            Position = position;
            Rotation = rotation < 0 ? rotation % 360 + 360 : rotation % 360;
            var simpleRotation = // ×90
                Rotation < 45 || Rotation >= 315 ? 0 :
                Rotation < 135 || Rotation >= 45 ? 1 :
                Rotation < 225 || Rotation >= 135 ? 2 : 3;
            Rotation = MathHelper.ToRadians(Rotation);
            StartHP = HP = type.MaxHP > 0 ? Utils.Random.Next(type.MinHP, type.MaxHP + 1) : int.MaxValue;
            var collisionMasks = new IShape[type.CollisionMasks.Count];
            for (var i = 0; i < collisionMasks.Length; ++i) {
                int x = 0, y = 0;
                var s = type.CollisionMasks[i];
                switch (simpleRotation) {
                    case 0:
                        x = s.X;
                        y = s.Y;
                        break;
                    case 1:
                        x = s.Y;
                        y = -s.X;
                        break;
                    case 2:
                        x = -s.X;
                        y = -s.Y;
                        break;
                    case 3:
                        x = -s.Y;
                        y = s.X;
                        break;
                }
                if (s is Circle c) {
                    collisionMasks[i] = new Circle(x + position.X, y + position.Y, c.Radius);
                } else {
                    var r = (Rect)s;
                    int w, h;
                    if (simpleRotation == 1 || simpleRotation == 3) {
                        w = r.Height;
                        h = r.Width;
                    } else {
                        w = r.Width;
                        h = r.Height;
                    }
                    collisionMasks[i] = new Rect(x + position.X, y + position.Y, w, h);
                }
            }
            FireArea = Type.FireArea;
            if (FireArea is Rect rect && (simpleRotation == 1 || simpleRotation == 3))
                FireArea = new Rect(rect.X, rect.Y, rect.Height, rect.Width);
            CollisionMasks = Array.AsReadOnly(collisionMasks);
        }

        public void Update(GameTime time, HeatMap heatMap, GamePartSystem partSystem, LevelScreen levelScreen) {
            if (IsDestroyed) return;
            if (Type.Type == ObjectType.Hydrant) return;
            if (IsBurning) {
                Burning = Math.Min(Burning + 0.5f, Type.FireLimit);
                heatMap.RadiateHeat(Position, Burning);
                HP -= Burning / 1000f;
                if (IsDestroyed) {
                    if (Type.Type == ObjectType.Explosive) {
                        heatMap.Explode(Position);
                        partSystem.Explode(Position);
                        levelScreen.ExplosionSound.Play(1f, 0f, Position.X / 512f - 1f);
                        levelScreen.ExplosionSound.Play(1f, 0f, Position.X / 512f - 1f);
                        levelScreen.ExplosionSound.Play(1f, 0f, Position.X / 512f - 1f);
                    }
                } else {
                    var count = (int)(Burning / 100f) + 1;
                    if (FireArea is Circle c) {
                        for (var i = 0; i < count; ++i) {
                            var d = MathHelper.ToRadians(Utils.Random.Next(360));
                            var l = Utils.Random.Next(c.Radius);
                            partSystem.AddFirePart(
                                new Vector2(
                                    Position.X + c.X + (float)Math.Cos(d) * l,
                                    Position.Y + c.Y - (float)Math.Sin(d) * l
                                ),
                                Type.FireType
                            );
                        }
                    } else if (FireArea is Rect r) {
                        for (var i = 0; i < count; ++i) {
                            partSystem.AddFirePart(
                                new Vector2(
                                    Position.X + r.X - r.Width / 2 + Utils.Random.Next(r.Width),
                                    Position.Y + r.Y - r.Height / 2 + Utils.Random.Next(r.Height)
                                ),
                                Type.FireType
                            );
                        }
                    }
                }
            }
        }

        public void UpdateFromHeatmap(HeatMap heatMap) {
            if (Type.Type == ObjectType.Hydrant) return;
            if (!IsBurning && heatMap.GetHeat(Position) > Type.FireLimit / 10) SetAlight();
        }

        public void Extinguish(ExtinguishingAgent agent) {
            if (IsBurning) {
                //if (agent.Extinguishes(Type.FireType))
                Burning = Math.Max(0, Burning - 1);
                //if (agent.IsEffectiveAgainst(Type.FireType))
                Burning = Math.Max(0, Burning - 1);
            }
        }

        public void Draw(SpriteBatch batch, GameTime time) {
            batch.Draw(
                Type.Texture,
                Position.ToVector2(),
                null,
                Color.Lerp(
                    Utils.DarkGray,
                    Color.White,
                    HP / StartHP
                ),
                MathHelper.TwoPi - Rotation,
                Type.TextureOrigin,
                1,
                SpriteEffects.None,
                0
            );
        }

        public void SetAlight() {
            if (Type.Type == ObjectType.Hydrant) return;
            Burning = 1;
        }

        public bool CollidesWith(IShape shape) => CollisionMasks.Any(s => s.CollidesWith(shape));

        public IEnumerable<IShape> Collisions(IShape shape) => CollisionMasks.Where(s => s.CollidesWith(shape));
    }

}
