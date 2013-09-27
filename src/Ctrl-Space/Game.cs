using System;
using Ctrl_Space.Gameplay;
using Ctrl_Space.Graphics;
using Ctrl_Space.Input;
using Ctrl_Space.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Ctrl_Space
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static readonly int WorldWidthInClusters = 8;
        public static readonly int WorldHeihgtInClusters = 8;
        public static readonly int ClusterSizeInPowerOfTwo = 8;
        public static readonly int ClusterSize = 1 << ClusterSizeInPowerOfTwo;
        public static readonly int WorldWidth = WorldWidthInClusters * ClusterSize;
        public static readonly int WorldHeight = WorldHeihgtInClusters * ClusterSize;

        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Ship _ship;
        private Ship _enemyShip;

        private Camera _camera;
        private InputManager _inputManager = null;

        private World _world = new World();

        private Particles _particles = new Particles();

        private WorldLoop _worldLoop = new WorldLoop();
        private WorldLoop _worldLoopParticles = new WorldLoop();

        private Song _song;

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            Random r = new Random();

            InitializeInputManager();

            _ship = new Ship(new Vector2(WorldWidth / 2, WorldHeight / 2), _world);
            _enemyShip = new EnemyShip(new Vector2(0.0f, 0.0f), _world, _ship);
            _camera = new Camera(_ship);

            for (int i = 0; i < 100; ++i)
            {
                Asteroid asteroid = new Asteroid();
                asteroid.Size = (float)(r.NextDouble() * 60 + 20);
                asteroid.Mass = asteroid.Size;
                asteroid.Position = new Vector2((float)(r.NextDouble() * WorldWidth), (float)(r.NextDouble() * WorldHeight));
                asteroid.Speed = new Vector2((float)(r.NextDouble() * 4 - 2), (float)(r.NextDouble() * 4 - 2));
                asteroid.Rotation = (float)(r.NextDouble() * 6.28);
                asteroid.RotationSpeed = (float)(r.NextDouble() * .1 - .05);
                _world.Add(asteroid);
            }

            for (int i = 0; i < 25; ++i)
            {
                SpeedBonus bonus = new SpeedBonus(
                    new Vector2((float)(r.NextDouble() * WorldWidth),
                    (float)(r.NextDouble() * WorldHeight)));
                _world.Add(bonus);
            }

            _world.Add(_enemyShip);
            _world.Add(_ship);

            Components.Add(new FPS(this, "Fonts/FPSFont", Vector2.Zero));

            _worldLoopParticles.Clusterize(_particles.ParticlesList);
            _worldLoop.Clusterize(_world);

            base.Initialize();
        }

        private void InitializeInputManager()
        {
            _inputManager = new InputManager(this);
            _inputManager.StartUpdate();

            this.Activated += new EventHandler<EventArgs>(Game_Activated);
            this.Deactivated += new EventHandler<EventArgs>(Game_Deactivated);

            _inputManager.ExitGame += () => Exit();
            _inputManager.DebugMode += () => GameOptions.IsDebugMode = !GameOptions.IsDebugMode;
            _inputManager.PlayStopMediaPlayer += () =>
                {
                    if (MediaPlayer.Queue.ActiveSong != null)
                    {
                        if (MediaPlayer.State == MediaState.Paused)
                            MediaPlayer.Resume();
                        else
                            MediaPlayer.Pause();
                    }
                    else MediaPlayer.Play(_song);
                };

            var strafeAcceleration = 0.5f;
            _inputManager.MoveRightLeft += e => _ship.Strafe(strafeAcceleration * e.Value);

            var moveAcceleration = 0.5f;
            _inputManager.MoveUpDown += e => _ship.SpeedUp(moveAcceleration * e.Value);

            _inputManager.PrimaryWeapon += e => _ship.Shoot(e.State);
            _inputManager.SecondaryWeapon += e => _ship.ShootAlt(e.State);

            _inputManager.Rotate += e => _ship.Rotate(e.Value * 0.1f);
        }

        void Game_Activated(object sender, EventArgs e)
        {
            this.IsMouseVisible = false;
            _inputManager.StartUpdate();
        }

        void Game_Deactivated(object sender, EventArgs e)
        {
            this.IsMouseVisible = true;
            _inputManager.StopUpdate();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            TextureManager.LoadTextures(Content);
            _song = Content.Load<Song>("Music/SOUP - Q7");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);

            Collisions.Detect(_worldLoop.Clusters, _world);

            for (int i = 0; i < _world.Count; i++)
            {
                var obj = _world[i];
                obj.Update(_world, _particles);
                if (obj.IsDestroyed)
                {
                    _world[i] = null;
                }
            }
            _world.RemoveAll(o => o == null);

            _particles.Update(_world, _particles);

            _worldLoopParticles.Clusterize(_particles.ParticlesList);
            _worldLoop.Clusterize(_world);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _camera.GetParallaxTransform());
            _spriteBatch.Draw(TextureManager.SpaceTexture, new Rectangle(0, 0, 1024, 1024), null, Color.White, 0.0f, new Vector2(512, 512), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(TextureManager.SpaceTexture, new Rectangle(0, 0, 1024, 1024), null, Color.White, 0.0f, new Vector2(1536, 512), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(TextureManager.SpaceTexture, new Rectangle(0, 0, 1024, 1024), null, Color.White, 0.0f, new Vector2(1536, 1536), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(TextureManager.SpaceTexture, new Rectangle(0, 0, 1024, 1024), null, Color.White, 0.0f, new Vector2(512, 1536), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(TextureManager.SpaceTexture, new Rectangle(0, 0, 1024, 1024), null, Color.White, 0.0f, new Vector2(-512, 1536), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(TextureManager.SpaceTexture, new Rectangle(0, 0, 1024, 1024), null, Color.White, 0.0f, new Vector2(-512, 512), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(TextureManager.SpaceTexture, new Rectangle(0, 0, 1024, 1024), null, Color.White, 0.0f, new Vector2(-512, -512), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(TextureManager.SpaceTexture, new Rectangle(0, 0, 1024, 1024), null, Color.White, 0.0f, new Vector2(512, -512), SpriteEffects.None, 0.0f);
            _spriteBatch.Draw(TextureManager.SpaceTexture, new Rectangle(0, 0, 1024, 1024), null, Color.White, 0.0f, new Vector2(1536, -512), SpriteEffects.None, 0.0f);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _camera.GetTransform());

            foreach (var cluster in _worldLoopParticles.GetClustersAroundPosition(_ship.Position, 600f))
            {
                Vector2 offset = new Vector2(cluster.ShiftX * WorldWidth, cluster.ShiftY * WorldHeight);
                foreach (var obj in cluster.GameObjects)
                    obj.Draw(_spriteBatch, gameTime, offset);
            }

            foreach (var cluster in _worldLoop.GetClustersAroundPosition(_ship.Position, 600f))
            {
                Vector2 offset = new Vector2(cluster.ShiftX * WorldWidth, cluster.ShiftY * WorldHeight);
                foreach (var obj in cluster.GameObjects)
                    obj.Draw(_spriteBatch, gameTime, offset);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
