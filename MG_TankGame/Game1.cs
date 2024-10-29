using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
// ReSharper disable All

namespace MG_TankGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _tankTexture;
        private Texture2D _bulletTexture;
        private Vector2 _tankPosition;
        private float _tankSpeed = 200f;
        private List<Bullet> _bullets;
        private bool _canShoot = true; 

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _tankPosition = new Vector2(50, 50);
            _bullets = new List<Bullet>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _tankTexture = new Texture2D(GraphicsDevice, 50, 50);
            Color[] data = new Color[50 * 50];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Green;
            _tankTexture.SetData(data);
            
            _bulletTexture = new Texture2D(GraphicsDevice, 10, 10);
            Color[] bulletData = new Color[10 * 10];
            for (int i = 0; i < bulletData.Length; ++i) bulletData[i] = Color.Red;
            _bulletTexture.SetData(bulletData);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W) && _tankPosition.Y > 0)
                _tankPosition.Y -= _tankSpeed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.S) && _tankPosition.Y < _graphics.PreferredBackBufferHeight - _tankTexture.Height)
                _tankPosition.Y += _tankSpeed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.A) && _tankPosition.X > 0)
                _tankPosition.X -= _tankSpeed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.D) && _tankPosition.X < _graphics.PreferredBackBufferWidth - _tankTexture.Width)
                _tankPosition.X += _tankSpeed * deltaTime;
            
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && _canShoot)
            {
                Vector2 tankCenter = _tankPosition + new Vector2(_tankTexture.Width / 2, _tankTexture.Height / 2);
                Vector2 direction = Vector2.Normalize(new Vector2(mouseState.X, mouseState.Y) - tankCenter);
                _bullets.Add(new Bullet(tankCenter, direction * 300)); 
                _canShoot = false; 
            }
            
            if (mouseState.LeftButton == ButtonState.Released)
            {
                _canShoot = true;
            }
            
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(deltaTime);
                
                if (_bullets[i].Position.X < 0 || _bullets[i].Position.X > _graphics.PreferredBackBufferWidth ||
                    _bullets[i].Position.Y < 0 || _bullets[i].Position.Y > _graphics.PreferredBackBufferHeight)
                {
                    _bullets.RemoveAt(i);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();
            _spriteBatch.Draw(_tankTexture, _tankPosition, Color.White);
            foreach (var bullet in _bullets)
            {
                _spriteBatch.Draw(_bulletTexture, bullet.Position, Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
    
    public class Bullet
    {
        public Vector2 Position { get; private set; }
        private Vector2 _velocity;

        public Bullet(Vector2 position, Vector2 velocity)
        {
            Position = position;
            _velocity = velocity;
        }
        
        public void Update(float deltaTime)
        {
            Position += _velocity * deltaTime;
        }
    }
}
