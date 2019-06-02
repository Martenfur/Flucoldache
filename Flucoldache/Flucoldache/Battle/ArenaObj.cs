using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;
using Flucoldache.Overworld;

namespace Flucoldache.Battle
{
	public class ArenaObj : GameObj
	{
		public string Name;
		public string Name1;

		public Vector2 Pos;
		
		public int Health;
		public int MaxHealth;
		public int MinAttack;
		public int MaxAttack;
		public int Defence;
		public int Speed;

		public Color ForegroundColor = GameConsole.BaseForegroundColor;
		public Color BackgroundColor = GameConsole.BaseBackgroundColor;

		public bool PreserveBackground = false;

		public bool DmgAnim = false;
		Color[] _dmgFgColor = {GameConsole.BaseBackgroundColor, Color.White};
		Color[] _dmgBgColor = {GameConsole.BaseForegroundColor, Color.White};
		double _dmgFrame = 0;
		int _dmgMaxFrame = 2;
		double _dmgAnimSpd = 8f;

		public bool Selected;

		/// <summary>
		/// Tells if it is unit's turn now.
		/// </summary>
		public bool Initiative;

		public bool Waiting;
		public Dialogue BuffDialogue;


		public List<StatEffect> Effects = new List<StatEffect>();
		public List<StatEffect> NewEffects = new List<StatEffect>();


		public override void Update()
		{
			Selected = false;
			if (DmgAnim)
			{
				_dmgFrame += GameCntrl.Time(_dmgAnimSpd);

				if (_dmgFrame >= _dmgMaxFrame)
				{
					_dmgFrame = 0;
					DmgAnim = false;
				}
			}

			if (Waiting)
			{
				if (BuffDialogue.Destroyed)
				{
					Waiting = false;
					BuffDialogue = null;
				}
			}

			Effects.RemoveAll(o => o.Duration <= 0);
			
			Effects.AddRange(NewEffects);
			NewEffects.Clear();
		}

		public override void Draw()
		{
			if (DmgAnim)
			{
				GameConsole.ForegroundColor = _dmgFgColor[(int)_dmgFrame];
				GameConsole.BackgroundColor = _dmgBgColor[(int)_dmgFrame];
			}
			else
			{
				GameConsole.ForegroundColor = ForegroundColor;
				Color color = Color.Transparent;
				if (PreserveBackground)
				{
					color = BackgroundColor;
				}
				foreach(StatEffect effect in Effects)
				{
					color = Color.Lerp(color, effect.Color, 0.5f);

					if (effect.IsInfective)
					{
						GameConsole.BackgroundColor = new Color(effect.Color, 0.3f);
						foreach(Vector2 v in Player.Rotation)
						{
							GameConsole.DrawChar(' ', Pos + v);
						}
					}

				}
				GameConsole.BackgroundColor = color;
			}

			if (Selected)
			{
				GameConsole.ForegroundColor = BackgroundColor;
				GameConsole.BackgroundColor = ForegroundColor;
			}


		}

		public override void DrawEnd()
		{
			if (Initiative)
			{
				GameConsole.ForegroundColor = new Color(216, 166, 0);
				GameConsole.BackgroundColor = Color.Transparent;

				GameConsole.DrawChar('!', Pos - Vector2.UnitY);
			}
		}

		public int Attack(ArenaObj obj)
		{
			
			int buffAttack = 0;
			foreach(StatEffect effect in Effects)
			{
				buffAttack += effect.AttackBuff;
			}
			int buffDefence = 0;
			foreach(StatEffect effect in obj.Effects)
			{
				buffDefence += effect.DefenceBuff;
			}

			int resDmg = Math.Max(1, 
				(int)MathHelper.Lerp(
					MinAttack + buffAttack, 
					MaxAttack + 1 + buffAttack, 
					(float)GameplayController.Random.NextDouble()
				) - (obj.Defence + buffDefence)
			);

			obj.Health -= resDmg;
			if (obj.Health <= 0)
			{
				obj.Health = 0;
				SoundController.PlaySound(SoundController.Death);
			}
			else
			{
				SoundController.PlaySound(SoundController.Hit);
			}
			obj.DmgAnim = true;
			return resDmg;
		}

		public virtual void ReceiveInitiative()
		{
			Initiative = true;
			List<string> effectMessages = new List<string>();
			foreach(StatEffect effect in Effects)
			{
				effectMessages.AddRange(effect.Update());
			}

			// Yeah, I regret this too.
			List<string> emptyStr = new List<string>();
			foreach(string str in effectMessages)
			{
				emptyStr.Add("");
			}
			// Yeah, I regret this too.

			if (effectMessages.Count > 0)
			{
				Waiting = true;
				BuffDialogue = new Dialogue(emptyStr.ToArray(), effectMessages.ToArray());
			}
		}

		public void AddStatEffect(StatEffect effect, int stage)
		{
			if (effect != null)
			{
				foreach(StatEffect myEffect in Effects)
				{
					if (myEffect.Token == effect.Token)
					{
						myEffect.Duration = effect.Duration; // Renewing effect.
						myEffect.InfectionStage = stage;
						return;
					}
				}

				foreach(StatEffect myEffect in NewEffects)
				{
					if (myEffect.Token == effect.Token)
					{
						return;
					}
				}
				NewEffects.Add(new StatEffect(effect, this, stage));
			}
		}
	}
}
