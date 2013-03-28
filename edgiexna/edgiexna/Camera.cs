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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        public Vector3 curPos { get; protected set; }
        public Vector3 curOrientation { get; protected set; }
        public Vector3 curTarget { get; protected set; }

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            view = Matrix.CreateLookAt(pos, target, up);
            curPos = pos;
            curOrientation = up;
            curTarget = target;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 
                (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height,
            1, 100);
        }

        public void LookAt(Vector3 target)
        {
            curTarget = target;
            view = Matrix.CreateLookAt(curPos, target, curOrientation);
        }

        public void MoveTo(Vector3 pos)
        {
            curPos = pos;
            view = Matrix.CreateLookAt(curPos, curTarget, curOrientation);
        }

        public void ApplyViewTransformation(Matrix transformation)
        {
            view = view * transformation; 
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
