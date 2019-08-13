using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace IntoTheBlaze {

    public interface IGameScreen {

        Song Music { get; }

        void Initialize(IntoTheBlaze game);

        void LoadContent();

        void Update(GameTime time);

        void Draw(SpriteBatch batch, GameTime time);

        void Enter(IGameScreen previous);

        void Leave(IGameScreen next);
    }
}
