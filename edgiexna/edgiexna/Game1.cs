using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace edgiexna
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // static constants
        static float overlayHeight = 0.5f;
        static int drawCallSize = 60000;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        VertexPositionTexture[] textureVerts;
        VertexPositionColor[] overlayVerts;
        VertexBuffer vertexPositionTextureBuffer;
        VertexBuffer vertexPositionColorBuffer;
        BasicEffect effect;
        Texture2D img;
        Matrix world;

        private Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// 
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(0.5f, 0.5f, 3), Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 800;
            graphics.ApplyChanges();
            world = Matrix.Identity;
            base.Initialize();
        }

        protected void loadOverlayVertices()
        {
            overlayVerts = new VertexPositionColor[img.Width * img.Height];
            Color[] colorData = new Color[img.Width * img.Height];
            img.GetData<Color>(colorData);
            for (int j = 0; j < img.Height; j++)
            {
                for (int i = 0; i < img.Width; i++)
                {
                    Color curColor = colorData[(j * img.Height) + i];
                    float curIntensity = (curColor.R*0.2125f + curColor.G*0.7152f + curColor.B*.0722f) / 255;
                    Color drawColor = new Color(curIntensity, curIntensity, curIntensity);

                    overlayVerts[(j * img.Height) + i] = new VertexPositionColor(new Vector3((float)i/img.Width, 
                        (img.Height - (float)j) / img.Height, overlayHeight + curIntensity), drawColor);
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            img = Content.Load<Texture2D>("smalnia");

            // Set cullmode to none
            RasterizerState rs = new RasterizerState(); rs.CullMode = CullMode.None; GraphicsDevice.RasterizerState = rs;

            // Initialize vertices
            textureVerts = new VertexPositionTexture[4]; 
            textureVerts[0] = new VertexPositionTexture(new Vector3(0, 1f, 0f), new Vector2(0, 0)); 
            textureVerts[1] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)); 
            textureVerts[2] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 1));
            textureVerts[3] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 1));
            loadOverlayVertices();

            vertexPositionTextureBuffer = new VertexBuffer(this.GraphicsDevice, typeof(VertexPositionTexture), textureVerts.Length, BufferUsage.None);
            vertexPositionTextureBuffer.SetData(textureVerts);
                        
            vertexPositionColorBuffer = new VertexBuffer(this.GraphicsDevice, typeof(VertexPositionColor), overlayVerts.Length, BufferUsage.None);
            vertexPositionColorBuffer.SetData(overlayVerts);


            effect = new BasicEffect(this.GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            KeyboardState st = Keyboard.GetState();
            if (st.IsKeyDown(Keys.Escape))
                this.Exit();

            if (st.IsKeyDown(Keys.W))
                world *= Matrix.CreateRotationX(0.02f);
            if (st.IsKeyDown(Keys.S))
                world *= Matrix.CreateRotationX(-0.02f);
            if (st.IsKeyDown(Keys.D))
                world *= Matrix.CreateRotationY(0.02f);
            if (st.IsKeyDown(Keys.A))
                world *= Matrix.CreateRotationY(-0.02f);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// 
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SetVertexBuffer(vertexPositionTextureBuffer);
            //Set object and camera info 
            effect.World = world; 
            effect.View = camera.view;
            effect.Projection = camera.projection;
            effect.VertexColorEnabled = false;
            effect.Texture = img;
            effect.TextureEnabled = true;
            // Begin effect and draw for each pass
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture> (PrimitiveType.TriangleStrip, textureVerts, 0, 2);
            }

            GraphicsDevice.SetVertexBuffer(vertexPositionColorBuffer);
            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                int curDrawCount = 0;
                while (curDrawCount < img.Width * img.Height)   // this is to correct for the fact that Reach has a limited # of draw calls.
                {
                    int drawCount = drawCallSize;
                    if(drawCount + curDrawCount > (img.Width * img.Height - 1))
                        drawCount = (img.Width * img.Height) - curDrawCount - 1; // extra minus 1, we are 0 based

                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, overlayVerts, curDrawCount,
                        drawCount);
                    curDrawCount += drawCallSize;
                }
            }


            base.Draw(gameTime);
        }
    }
}
