using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pathTest
{
    public static class Gfx
    {
        public static Texture2D floor;
        public static Texture2D wall;
        public static Texture2D path;
        public static Texture2D door;
    }
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PathTest : Game
    {
        GraphicsDeviceManager graphics;
        View activeView;
        SpriteBatch spriteBatch;
        Maze maze;

        public PathTest()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 512;
            graphics.PreferredBackBufferHeight = 512;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            activeView = new View(1600, 1600, 4, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, spriteBatch);

            // TODO: use this.Content to load your game content here
            Gfx.floor = Content.Load<Texture2D>("images/floor");
            Gfx.wall = Content.Load<Texture2D>("images/wall");
            Gfx.path = Content.Load<Texture2D>("images/path");
            Gfx.door = Content.Load<Texture2D>("images/door");

            maze = new Maze(100, 100);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                activeView.x -= activeView.speed;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                activeView.x += activeView.speed;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                activeView.y -= activeView.speed;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                activeView.y += activeView.speed;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                activeView.zoom = activeView.zoom / 0.95;
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                activeView.zoom = activeView.zoom * 0.95;

            maze.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            maze.Draw(activeView);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
