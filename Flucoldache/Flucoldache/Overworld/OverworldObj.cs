using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	public class OverworldObj : GameObj
	{
		public Vector2 Pos;

		/// <summary>
		/// Used in the editor. 
		/// </summary>
		public string Argument = "";
		public string ArgumentName = "argument";
		
		/// <summary>
		/// In editor mode overwold objects should be inactive.
		/// </summary>
		public bool EditorMode = false;

		public virtual void ProccessArgument() {}

		public virtual bool TriggerAction() => false;
	}
}
