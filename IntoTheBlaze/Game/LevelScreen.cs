using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntoTheBlaze.Menu;
using IntoTheBlaze.Physics;
using IntoTheBlaze.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IntoTheBlaze.Game {

    public class LevelScreen : IGameScreen {

        private static readonly byte[] levelFileMagicNumber = { 73, 84, 66, 45, 76, 69, 86, 69, 76 };

        private IntoTheBlaze game;

        internal Texture2D
            cursor,
            floorTex,
            wallsTex,
            fireworkRocket,
            hud,
            hudBars,
            pauseScreen;

        internal StepTimer winTimer, fireSndTimer, fireExtSndTimer;

        private RenderTarget2D background;

        private bool backgroundGenerated, paused;

        private byte[,] floor, walls;

        private Dictionary<string, GameObject> gameObjects;

        private List<Rect> wallColliders;

        private List<GameObjectInstance> gameObjectInstances;

        private HeatMap heatMap;

        private GamePartSystem partSystem;

        internal SoundEffect FireExtinguisherSound, FireSound, ExplosionSound;

        private int startingCost;

        private Player player;
        private int levelId;

        public Song Music { get; private set; }
        
        public void Initialize(IntoTheBlaze game) {
            this.game = game;
            winTimer = new StepTimer(180);
            fireSndTimer = new StepTimer(108);
            fireExtSndTimer = new StepTimer(440);
            heatMap = new HeatMap();
            partSystem = new GamePartSystem();
            partSystem.Initialize(game);
            startingCost = 0;
            player = new Player();
            player.Initialize(this, game);
            background = new RenderTarget2D(
                game.GraphicsDevice,
                1024, 640,
                false,
                game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            floor = new byte[32, 17];
            walls = new byte[32, 17];
            backgroundGenerated = false;
            gameObjects = new Dictionary<string, GameObject>();
            wallColliders = new List<Rect>();
            gameObjectInstances = new List<GameObjectInstance>();
        }

        public void LoadContent() {
            cursor = game.Content.Load<Texture2D>("game_cursor");
            floorTex = game.Content.Load<Texture2D>("floor");
            wallsTex = game.Content.Load<Texture2D>("walls");
            hud = game.Content.Load<Texture2D>("hud");
            hudBars = game.Content.Load<Texture2D>("hud_bars");
            fireworkRocket = game.Content.Load<Texture2D>("firework_rocket");
            pauseScreen = game.Content.Load<Texture2D>("pause");
            Music = game.Content.Load<Song>("armies_on_the_ground");
            FireExtinguisherSound = game.Content.Load<SoundEffect>("snd_fire_extinguisher");
            FireSound = game.Content.Load<SoundEffect>("snd_fire");
            ExplosionSound = game.Content.Load<SoundEffect>("snd_explosion");
            LoadGameObjects();
            player.LoadContent();
            partSystem.LoadContent();
        }

        private IShape JsonParseShape(JToken json, bool withOffset) {
            var type = (string)((JValue)json["type"]).Value;
            var x = 0;
            var y = 0;
            if (withOffset) {
                var offset = json["offset"];
                x = (int)(long)((JValue)offset["x"]).Value;
                y = (int)(long)((JValue)offset["y"]).Value;
            }
            switch (type) {
                case "circ":
                    return new Circle(
                        x, y,
                        (int)(long)((JValue)json["radius"]).Value
                    );
                case "rect":
                    return new Rect(
                        x, y,
                        json["width"].Value<int>(),
                        json["height"].Value<int>()
                        //(int)(long)((JValue)json["width"]).Value,
                        //(int)(long)((JValue)json["height"]).Value
                    );
                default:
                    return null;
            }
        }

        private ExtinguishingAgent JsonParseExtinguishingAgent(JToken json) {
            if (json == null) return null;
            switch ((string)((JValue)json).Value) {
                case "water":        return ExtinguishingAgent.Water;
                case "water-mist":   return ExtinguishingAgent.WaterMist;
                case "CO2":          return ExtinguishingAgent.CO2;
                case "foam":         return ExtinguishingAgent.Foam;
                case "dry-powder":   return ExtinguishingAgent.DryPowder;
                case "wet-chemical": return ExtinguishingAgent.WetChemical;
                default:             return null;
            }
        }

        private ObjectType JsonParseObjectType(JToken json) {
            if (json == null) return ObjectType.Normal;
            switch ((string)((JValue)json).Value) {
                case "normal":    return ObjectType.Normal;
                case "explosive": return ObjectType.Explosive;
                case "fireworks": return ObjectType.Fireworks;
                case "hydrant":   return ObjectType.Hydrant;
                default:          return ObjectType.Normal;
            }
        }

        private void LoadGameObjects() {
            var objects = (JArray)JsonConvert.DeserializeObject(Resources.objects);
            foreach (var obj in objects) {
                var name = (string)((JValue)obj["name"]).Value;
                var type = JsonParseObjectType(obj["type"]);
                var fire = obj["fire"];
                var fireTypeString = fire == null ? "None" : (string)((JValue)fire["type"]).Value;
                if (!Enum.TryParse(fireTypeString, out FireType fireType))
                    fireType = FireType.None;
                var maxFireIntensity = fire == null ? -1 : (int)(long)((JValue)fire["maxIntensity"]).Value;
                var fireShape = fire == null ? null : JsonParseShape(fire["area"], false);
                var hpmin = obj["hpmin"] == null ? -1 : (int)(long)((JValue)obj["hpmin"]).Value;
                var hpmax = obj["hpmax"] == null ? -1 : (int)(long)((JValue)obj["hpmax"]).Value;
                var width = (int)(long)((JValue)obj["width"]).Value;
                var height = (int)(long)((JValue)obj["height"]).Value;
                var zheight = (int)(long)((JValue)obj["zheight"]).Value;
                var cost = obj["cost"] == null ? 0 : (int)(long)((JValue)obj["cost"]).Value;
                var supply = JsonParseExtinguishingAgent(obj["agent"]);
                var collisionMasks = ((JArray)obj["collisionMasks"]).Select(t => JsonParseShape(t, true));
                var solid = (bool)((JValue)obj["solid"]).Value;
                gameObjects.Add(
                    name,
                    new GameObject(
                        name,
                        type,
                        fireType,
                        maxFireIntensity,
                        fireShape,
                        game.Content.Load<Texture2D>(name),
                        width,
                        height,
                        zheight,
                        hpmin,
                        hpmax,
                        supply,
                        collisionMasks,
                        cost,
                        solid
                    )
                );
            }
        }

        public void Update(GameTime time) {
            if (KeyboardHelper.KeyPressed(Keys.Escape)) {
                paused = !paused;
                if (paused) return;
            }
            heatMap.UpdateBegin(time);
            winTimer.Update(time);
            fireSndTimer.Update(time);
            fireExtSndTimer.Update(time);
            if (winTimer.HasTick()) {
                if (game.LevelSelectScreen.Progress < levelId)
                    game.LevelSelectScreen.Progress = levelId;
                game.ChangeScreen(game.LevelEndScreen.SetWin(true));
            } else if (gameObjectInstances.Where(inst => !inst.IsDestroyed).Sum(inst => inst.Type.Cost) < startingCost / 3 || player.IsDead)
                game.ChangeScreen(game.LevelEndScreen.SetWin(false));
            foreach (var inst in gameObjectInstances) inst.Update(time, heatMap, partSystem, this);
            foreach (var inst in gameObjectInstances) inst.UpdateFromHeatmap(heatMap);
            partSystem.Update(time);
            foreach (var agent in partSystem.GetAgents()) {
                var c = agent.CollisionMask;
                foreach (var inst in gameObjectInstances)
                    if (inst.IsBurning && inst.CollidesWith(c))
                        inst.Extinguish(agent.Type);
                heatMap.CoolDown(new Point(c.X, c.Y));
                if (wallColliders.Any(w => w.CollidesWith(c))) agent.Kill();
            }
            player.Update(time, heatMap, partSystem);
            if (gameObjectInstances.Any(inst => inst.IsBurning)) winTimer.Reset();
            if (fireSndTimer.HasTick()) {
                var b = Math.Min(2000f, gameObjectInstances.Sum(inst => inst.Burning));
                if (b > 0) FireSound.Play(b / 2000f, 0f, 0f);
            }
            fireSndTimer.TakeTicks();
            fireExtSndTimer.TakeTicks();
        }

        public void Draw(SpriteBatch batch, GameTime time) {
            // pre-render floor
            if (!backgroundGenerated) {
                game.GraphicsDevice.SetRenderTarget(background);
                game.GraphicsDevice.Clear(Color.Black);
                batch.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
                for (var x = 0; x < 32; ++x)
                for (var y = 0; y < 17; ++y) {
                    var tx = x % 8 * 32 + floor[x, y] * 256;
                    var ty = y % 8 * 32;
                    batch.Draw(
                        floorTex,
                        new Vector2(x * 32, y * 32),
                        new Rectangle(tx, ty, 32, 32),
                        Color.White
                    );
                }
                batch.Draw(hud, new Vector2(0, 544), Color.White);
                batch.End();
                game.GraphicsDevice.SetRenderTarget(null);
                backgroundGenerated = true;
            }

            batch.Begin();
            // floor
            batch.Draw(background, Vector2.Zero, Color.White);
            batch.End();
            batch.Begin(SpriteSortMode.Texture);
            // layer 1
            foreach (var inst in gameObjectInstances)
                if (inst.Type.ZHeight == 1)
                    inst.Draw(batch, time);
            batch.End();
            batch.Begin(SpriteSortMode.Texture);
            // layer 2
            foreach (var inst in gameObjectInstances)
                if (inst.Type.ZHeight == 2)
                    inst.Draw(batch, time);
            batch.End();
            batch.Begin();
            player.Draw(batch, time);
            batch.End();
            batch.Begin(SpriteSortMode.Texture);
            // layer 3
            foreach (var inst in gameObjectInstances)
                if (inst.Type.ZHeight == 3)
                    inst.Draw(batch, time);
            batch.End();
            // particles
            partSystem.Draw(batch, time);
            batch.Begin();
            // --- HUD ---

            // agents
            DrawExtAgent(batch, player.GetAgentAmmount(0), 16);
            DrawExtAgent(batch, player.GetAgentAmmount(1), 34);
            DrawExtAgent(batch, player.GetAgentAmmount(2), 52);
            // health
            var hp = Math.Min(64, (int)(player.HP * 0.64));
            if (hp > 0) batch.Draw(hudBars, new Vector2(96, 624 - hp), new Rectangle(48, 64 - hp, 12, hp), Color.White);
            // temperature
            var temp = heatMap.GetHeat(player.Position.ToPoint());
            var h = Math.Min(50, (int)(temp / 4f));
            if (h > 0) batch.Draw(hudBars, new Vector2(115, 617 - h), new Rectangle(60, 64 - h, 4, h), Color.White);

            batch.End();
            batch.Begin();
            // walls
            for (var x = 0; x < 32; ++x)
            for (var y = 0; y < 17; ++y) {
                if (walls[x, y] == 0) continue;
                batch.Draw(
                    wallsTex,
                    new Vector2(x * 32, y * 32),
                    new Rectangle((walls[x, y] & 0x0F) * 32, 0, 32, 32),
                    Color.White
                );
            }
            if (paused) batch.Draw(pauseScreen, Vector2.Zero, Color.White);
            // cursor
            batch.Draw(
                cursor,
                MouseHelper.Pos,
                new Rectangle(0, 0, 16, 16),
                Color.White,
                (
                    time.TotalGameTime.Milliseconds / 1000f +
                    (time.TotalGameTime.Seconds % 2 == 1 ? 1 : 0)
                ) *
                MathHelper.Pi,
                new Vector2(8),
                1, SpriteEffects.None, 0);
            batch.End();
        }

        private void DrawExtAgent(SpriteBatch batch, Tuple<ExtinguishingAgent, float> a, int x) {
            if (a == null) batch.Draw(hudBars, new Vector2(x, 560), new Rectangle(0, 0, 12, 64), Color.White);
            else {
                var h = Math.Min(64, (int)(a.Item2 * 0.64f));
                if (h == 0) return;
                var t = 0;
                if (a.Item1 == ExtinguishingAgent.Water) t = 12;
                else if (a.Item1 == ExtinguishingAgent.Foam) t = 24;
                else if (a.Item1 == ExtinguishingAgent.DryPowder) t = 36;
                batch.Draw(hudBars, new Vector2(x, 624 - h), new Rectangle(t, 64 - h, 12, h), Color.White);
            }
        }

        public void Enter(IGameScreen previous) {
            MediaPlayer.Play(Music);
            MediaPlayer.IsRepeating = true;
        }

        public void Leave(IGameScreen next) {
            MediaPlayer.Stop();
            winTimer.Reset();
        }

        /// <returns>this</returns>
        public LevelScreen LoadLevel(int id) {
            try {
                levelId = id;
                backgroundGenerated = false;
                player.Reset();
                using (var br = new BinaryReader(File.OpenRead($"Levels/{id}.level"))) {

                    // magic number
                    var bytes = br.ReadBytes(9);
                    for (var i = 0; i < 9; ++i)
                        if (bytes[i] != levelFileMagicNumber[i])
                            throw new FormatException();
                    // walls
                    for (var y = 0; y < 17; ++y) {
                        var u = br.ReadUInt32();
                        for (var x = 0; x < 32; ++x)
                            walls[x, y] = (byte)(BinaryHelper.GetBit(u, x) ? 0xF0 : 0);
                    }
                    for (var x = 0; x < 32; ++x)
                    for (var y = 0; y < 17; ++y) {
                        if (walls[x, y] > 0) {
                            BinaryHelper.SetBit(ref walls[x, y], 0, WallAt(x - 1, y));
                            BinaryHelper.SetBit(ref walls[x, y], 1, WallAt(x, y + 1));
                            BinaryHelper.SetBit(ref walls[x, y], 2, WallAt(x + 1, y));
                            BinaryHelper.SetBit(ref walls[x, y], 3, WallAt(x, y - 1));
                        }
                    }
                    wallColliders.Clear();
                    var processed = new bool[32, 17];
                    for (var x = 0; x < 32; ++x)
                    for (var y = 0; y < 17; ++y) {
                        if (processed[x, y]) continue;
                        if (walls[x, y] > 0) {
                            if (WallAt(x + 1, y)) {
                                var w = 2;
                                processed[x + 1, y] = true;
                                for (var _x = x + 2; WallAt(_x, y); processed[_x, y] = true, ++_x, ++w) { }
                                wallColliders.Add(new Rect(x * 32 + w * 16, y * 32 + 16, w * 32, 32));
                            } else if (WallAt(x, y + 1)) {
                                var h = 2;
                                processed[x, y + 1] = true;
                                for (var _y = y + 2; WallAt(x, _y); processed[x, _y] = true, ++_y, ++h) { }
                                wallColliders.Add(new Rect(x * 32 + 16, y * 32 + h * 16, 32, h * 32));
                            } else wallColliders.Add(new Rect(x * 32 + 16, y * 32 + 16, 32, 32));
                        }
                        processed[x, y] = true;
                    }

                    // floor
                    for (var y = 0; y < 17; ++y) {
                        var u = br.ReadUInt64();
                        for (var x = 0; x < 32; ++x) {
                            floor[x, y] = (byte)(
                                (BinaryHelper.GetBit(u, x * 2) ? 1 : 0) +
                                (BinaryHelper.GetBit(u, x * 2 + 1) ? 2 : 0)
                            );
                        }
                    }

                    // objects
                    gameObjectInstances.Clear();
                    var count = br.ReadInt16();
                    for (var i = 0; i < count; ++i) {
                        var name = br.ReadString();
                        var x = br.ReadUInt16();
                        var y = br.ReadUInt16();
                        var rotation = br.ReadUInt16();
                        if (name == "player") {
                            br.ReadBoolean(); // discard 1 bool
                            player.Position = new Vector2(x, y);
                        } else {
                            var inst = new GameObjectInstance(
                                gameObjects[name],
                                new Point(x, y),
                                rotation
                            );
                            if (br.ReadBoolean())
                                inst.SetAlight();
                            gameObjectInstances.Add(inst);
                        }
                    }
                    startingCost = gameObjectInstances.Sum(inst => inst.Type.Cost);
                }
                return this;
            } catch (Exception e) {
                return null;
            }
        }

        private bool WallAtRealCoords(float x, float y) => WallAt((int)x / 32, (int)y / 32);

        private bool WallAt(int x, int y) => x >= 0 && x < 32 && y >= 0 && y < 17 && walls[x, y] > 0;

        public bool CollidesWithSolidOrEdge(IShape s) =>
            Utils.CollidesWithEdge(s) ||
            wallColliders.Any(w => w.CollidesWith(s)) ||
            gameObjectInstances.Any(i => i.Type.Solid && i.CollidesWith(s));

        public IEnumerable<IShape> WallCollisions(IShape s) =>
            wallColliders.Where(w => w.CollidesWith(s));

        public IEnumerable<IShape> SolidObjectCollisions(IShape s) =>
            gameObjectInstances.Where(i => i.Type.Solid).SelectMany(i => i.Collisions(s));

        public GameObjectInstance GetNearestHydrant(Vector2 pos, float distanceLimit = 10_000) {
            distanceLimit *= distanceLimit;
            GameObjectInstance nearest = null;
            var dist = float.PositiveInfinity;
            foreach (var h in gameObjectInstances.Where(i => i.Type.Type == ObjectType.Hydrant)) {
                var d = (h.Position.ToVector2() - pos).LengthSquared();
                if (d < dist && d < distanceLimit) {
                    dist = d;
                    nearest = h;
                }
            }
            return nearest;
        }
    }
}
