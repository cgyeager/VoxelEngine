using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using VoxelGame.World;
using VoxelGame.Graphics;
using VoxelGame.Utility;

namespace VoxelGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private SpriteBatch fontBatch;
        private string output;
        private FrameCounter frameCounter;

        private int RESX = 1500;
        private int RESY = 960;
        private Texture2D t;

        private MouseState previousState;
        private bool MouseCursorVisible = false;
        bool WireframeToggle = false;

        private int BLOCK_DIM = 10;
        List<Chunk> chunks;
        LineBox linebox;
        Color SkyColor = new Color(125, 200, 255);

        Camera camera;
        Effect effect;

        //bool notAlreadyDone = true;
        bool WasModified = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = RESX,
                PreferredBackBufferHeight = RESY
            };
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
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferMultiSampling = true;
            GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            graphics.ApplyChanges();

            this.IsMouseVisible = MouseCursorVisible;

            effect = Content.Load<Effect>("shaders/shader");

            frameCounter = new FrameCounter();

            Color[] colors = {
                Color.Cornsilk,
                Color.DarkGoldenrod,
                Color.DarkOliveGreen,
                Color.DarkSlateBlue,
                Color.DarkViolet,
                Color.Ivory,
                Color.LightGreen,
                Color.YellowGreen
            };

            Vector3 chunkPosition = new Vector3(0.0f, 0.0f, 0.0f);
            chunks = new List<Chunk>();
            const float dim = 4;
            const float halfDim = dim / 2;
            const float depth = 3;
            const float scale = 1f;
            for (float y = -depth; y < 0; y++)
                for (float x = -halfDim; x < halfDim; x++)
                    for (float z = -halfDim; z < halfDim; z++)
                    {
                        chunkPosition.X = x;
                        chunkPosition.Y = y;
                        chunkPosition.Z = z;
                        Chunk chunk = new Chunk(GraphicsDevice, effect, chunkPosition, scale);

                        if (y == -1)
                            chunk.SetupLandscape();
                        else
                            chunk.SetAllToStone();
                        chunks.Add(chunk);
                    }
            chunkPosition.Y = -0;
            /*
            Chunk chunk = new Chunk(GraphicsDevice, effect, chunkPosition, (float)BLOCK_DIM);
            //chunk.SetupLandscape();
            chunk.SetAllToStone();
            chunks.Add(chunk);

            chunkPosition.X++;
            chunk = new Chunk(GraphicsDevice, effect, chunkPosition, (float)BLOCK_DIM);
            //chunk.SetupLandscape();
            chunk.SetAllToStone();
            chunks.Add(chunk);
            */

            /*

            chunkPosition.X = 0f; 
            chunkPosition.Y = 1.5f;
            chunkPosition.Z = -1f;
            */

            Chunk moon = new Chunk(GraphicsDevice, effect, chunkPosition, 10f);  
            moon.GenSphere();
            chunks.Add(moon);

            camera = new Camera(GraphicsDevice);
            camera.Position.Y += 15f;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fontBatch = new SpriteBatch(graphics.GraphicsDevice);
            font = Content.Load<SpriteFont>("Courier New");
            previousState = Mouse.GetState();
            t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(
                new Color[] { Color.White });// fill the texture with white
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            output = $"\r\nx:{camera.Position.X}, Y: {camera.Position.Y}, Z: {camera.Position.Z}";

            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.OemTilde))
            {
                WireframeToggle = !WireframeToggle;
                if (WireframeToggle)
                {
                    RasterizerState rast = new RasterizerState();
                    rast.FillMode = FillMode.WireFrame;
                    GraphicsDevice.RasterizerState = rast;
                }
                else
                {
                    RasterizerState rast = new RasterizerState();
                    rast.FillMode = FillMode.Solid;
                    GraphicsDevice.RasterizerState = rast;
                }
            }

            Vector3 moveVector = Vector3.Zero;
            if (state.IsKeyDown(Keys.W))
                moveVector += camera.GetLookVector();
            if (state.IsKeyDown(Keys.S))
                moveVector -= camera.GetLookVector();
            if (state.IsKeyDown(Keys.D))
                moveVector += camera.GetRightVector();
            if (state.IsKeyDown(Keys.A))
                moveVector -= camera.GetRightVector();
            if (state.IsKeyDown(Keys.Z))
                moveVector.Y += 1;
            if (state.IsKeyDown(Keys.X))
                moveVector.Y -= 1;

            if (state.IsKeyDown(Keys.Tab))
            {
                MouseCursorVisible = !MouseCursorVisible;
                this.IsMouseVisible = MouseCursorVisible;
            }
            camera.Position += moveVector * 1f;
            var mouseState = Mouse.GetState();

            previousState = mouseState;

            Vector2 mouseLocation = new Vector2(mouseState.X, mouseState.Y);
            Viewport viewport = this.GraphicsDevice.Viewport;

            if (mouseState.LeftButton == ButtonState.Pressed) // and has pickaxe in hand
            {
                foreach (var chunk in chunks)
                {

                    Vector3 origin = camera.Position;
                    Block block = null;
                    Vector3 dir = camera.GetLookVector();

                    Vector3 testpoint = origin;
                    float granularity = 2;
                    testpoint += dir / granularity;
                    for (int i = 0; i < 2 * granularity; i++)
                    {
                        testpoint += dir / granularity;
                        block = chunk.GetBlockAt(testpoint);
                        if (block != null && block.IsActive)
                        {
                            block.IsActive = false;
                            WasModified = !WasModified;
                            break;
                        }

                        // store position to build at here
                    }

                    if (WasModified)
                    {
                        chunk.Update();
                        WasModified = !WasModified;
                    }
                }
            }

            int dx = mouseState.X - GraphicsDevice.Viewport.Width / 2;
            int dy = mouseState.Y - GraphicsDevice.Viewport.Height / 2;

            float sensitivity = MouseCursorVisible ? 0f : 0.005f;
            camera.Yaw -= dx * sensitivity;
            camera.Pitch = (float)Math.Min(Math.PI * 0.49, Math.Max(-Math.PI * 0.49, camera.Pitch - dy * sensitivity));

            if (!MouseCursorVisible)
            {
                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            }

            camera.Update();

            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(SkyColor);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            foreach (var c in chunks)
                c.Render(camera);

            if (linebox != null)
                linebox.Render(camera);

            // 2D drawing
            if (!WireframeToggle)
            {
                GraphicsDevice.BlendState = BlendState.AlphaBlend;

                var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                frameCounter.Update(deltaTime);

                var fps = string.Format("FPS: {0}", frameCounter.AverageFramesPerSecond);

                spriteBatch.Begin();

                TempDrawCrosshairs(spriteBatch);

                spriteBatch.DrawString(font, fps, new Vector2(1, 1), Color.Black);
                spriteBatch.DrawString(font, output, new Vector2(20, 20), Color.Black);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }




        /*
         *  Temporary stuff
         *
         */
        private Ray GetRay(Vector2 mouseLocation, Camera camera, Viewport viewport)
        {
            Vector3 origin = viewport.Unproject(new Vector3(mouseLocation.X,
                                                            mouseLocation.Y,
                                                            0.0f),
                                                camera.Projection,
                                                camera.View,
                                                Matrix.Identity);

            Vector3 endPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                                                               mouseLocation.Y,
                                                               1.0f),
                                                  camera.Projection,
                                                  camera.View,
                                                  Matrix.Identity);
            Vector3 direction = endPoint - origin;
            direction.Normalize();

            return new Ray(origin, direction);
        }

        private float? IntersectDistance(BoundingBox aabb,
                                         Vector2 mouseLocation,
                                         Camera camera,
                                         Viewport viewport,
                                         ref Ray ray)
        {
            ray = GetRay(mouseLocation, camera, viewport);
            return ray.Intersects(aabb);
        }

        private float? Intersects(Vector2 mouseLocation,
                                  Chunk chunk,
                                  Matrix world,
                                  Camera camera,
                                  Viewport viewport,
                                  ref Ray ray)
        {
            BoundingBox box = chunk.aabb;
            float? distance = IntersectDistance(box, mouseLocation, camera, viewport, ref ray);

            return distance;
        }

        private void TempDrawCrosshairs(SpriteBatch sb)
        {
            int size = 10;
            int x = RESX / 2;
            int y = RESY / 2;
            DrawLine(sb, new Vector2(x - size, y), new Vector2(x + size, y));
            DrawLine(sb, new Vector2(x, y - size), new Vector2(x, y + size));
        }

        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                Color.White, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
    }
}

/*
            long? chunkId = null;
            bool mouseOverSomething = false;
            float? distance = 0f;
            Ray ray = new Ray();
            foreach (var chunk in chunks)
            {
                if ((distance = Intersects(mouseLocation, chunk, Matrix.Identity, camera, viewport, ref ray)) > 0 && distance < 200f)
                {
                    mouseOverSomething = true;
                    //chunkId = chunk.GetId();
                    if (mouseState.LeftButton == ButtonState.Pressed)// || notAlreadyDone)
                    {
                        chunk.Selected = true;
                        linebox = new LineBox(GraphicsDevice, effect, chunk.Min, chunk.Max, Color.Red);
                        notAlreadyDone = false;
                        Vector3 origin = camera.Position;
                        Block block = null; 
                        Vector3 dir = camera.GetLookVector();
                        Vector3 testpoint = origin;

                        for (int i = 0; i < 64; i++)
                        {
                            testpoint += dir * 5f; 
                            block = chunk.GetBlockAt(testpoint);
                            if (block != null)
                                block.IsActive = false; 
                        }
    
                        chunk.Update();
                    }
                }
                else if (chunk.Selected == true)
                {
                    notAlreadyDone = true;
                    chunk.Selected = false;
                    if (linebox != null)
                        linebox.Dispose();
                    linebox = null;
                }

            }

                        */ 
