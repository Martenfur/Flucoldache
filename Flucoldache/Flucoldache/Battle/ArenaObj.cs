using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Battle
{
	public class ArenaObj : GameObj
	{
		public string Name;

		public Vector2 Pos;
		
		public int Health;
		public int MaxHealth;
		public int MinAttack;
		public int MaxAttack;
		public int Defence;
		public int Speed;

		public Color ForegroundColor = Color.Gray;
		public Color BackgroundColor = Color.Black;

		
		bool _dmgAnim = false;
		Color[] _dmgFgColor = {Color.Black, Color.White};
		Color[] _dmgBgColor = {Color.Gray, Color.White};
		double _dmgFrame = 0;
		int _dmgMaxFrame = 2;
		double _dmgAnimSpd = 8f;

		public bool Selected;

		/// <summary>
		/// Tells if it is unit's turn now.
		/// </summary>
		public bool Initiative;

		public List<StatEffect> Effects = new List<StatEffect>();

		public override void Update()
		{
			Selected = false;
			if (_dmgAnim)
			{
				_dmgFrame += GameCntrl.Time(_dmgAnimSpd);

				if (_dmgFrame >= _dmgMaxFrame)
				{
					_dmgFrame = 0;
					_dmgAnim = false;
				}
			}
		}

		public override void Draw()
		{
			if (Initiative)
			{
				GameConsole.ForegroundColor = Color.Yellow;
				GameConsole.BackgroundColor = Color.Black;

				GameConsole.DrawChar('!', Pos - Vector2.UnitY);
			}

			if (_dmgAnim)
			{
				GameConsole.ForegroundColor = _dmgFgColor[(int)_dmgFrame];
				GameConsole.BackgroundColor = _dmgBgColor[(int)_dmgFrame];
			}
			else
			{
				GameConsole.ForegroundColor = ForegroundColor;
				GameConsole.BackgroundColor = BackgroundColor;
			}

			if (Selected)
			{
				GameConsole.ForegroundColor = BackgroundColor;
				GameConsole.BackgroundColor = ForegroundColor;
			}

		}

		public int Attack(ArenaObj obj)
		{
			int resDmg = Math.Max(1, ((int)MathHelper.Lerp(MinAttack, MaxAttack + 1, (float)GameplayController.Random.NextDouble()) - obj.Defence));

			obj.Health -= resDmg;
			if (obj.Health < 0)
			{
				obj.Health = 0;
			}
			obj._dmgAnim = true;
			return resDmg;
		}

		public virtual void ReceiveInitiative()
		{
			Initiative = true;
		}

	}
}
