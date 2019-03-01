using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Flucoldache.Overworld;
using System.Diagnostics;

namespace Flucoldache.Battle
{
	public class StatEffect
	{

		public static Dictionary<string, StatEffect> Effects = new Dictionary<string, StatEffect>();

		/// <summary>
		/// Replenishes health every turn.
		/// </summary>
		public int RegenerationBuff;

		/// <summary>
		/// Damages every turn.
		/// </summary>
		public int PoisonBuff;
		

		public int AttackBuff;
		public int DefenceBuff;
		public int SpeedBuff;

		/// <summary>
		/// Buff duration. Decreases every turn. 
		/// </summary>
		public int Duration;
		

		public bool IsInfective;
		public float InfectionChance;
		public int InfectionStage;
		public int InfectionMaxStage;

		public ArenaObj Owner;
		public string Name;
		public string Token;

		public bool Proc;
		public float ProcChance;

		public StatEffect() {}

		public StatEffect(StatEffect effect, ArenaObj owner, int stage)
		{
			RegenerationBuff = effect.RegenerationBuff;
			PoisonBuff = effect.PoisonBuff;

			AttackBuff = effect.AttackBuff;
			DefenceBuff = effect.DefenceBuff;
			SpeedBuff = effect.SpeedBuff;
			Duration = effect.Duration;
		
			IsInfective = effect.IsInfective;
			InfectionChance = effect.InfectionChance;
			
			InfectionStage = stage;
			InfectionMaxStage = effect.InfectionMaxStage;

			Color = effect.Color;

			Owner = owner;
			Name = effect.Name;
			Token = effect.Token;

			ProcChance = effect.ProcChance;
		}

		public Color Color = GameConsole.BaseBackgroundColor;

		public string[] Update()
		{
			Duration -= 1;
			List<string> lines = new List<string>();

			Owner.Health += RegenerationBuff - PoisonBuff;
			if (Owner.Health > Owner.MaxHealth)
			{
				Owner.Health = Owner.MaxHealth;
			}
			if (Owner.Health < 0)
			{
				Owner.Health = 0;
			}

			if (PoisonBuff > 0)
			{
				//lines.Add(Name + " берёт свое, и " + Owner.Name + " получает " + PoisonBuff + " урона.");
				Owner.DmgAnim = true;
			}
			if (RegenerationBuff > 0)
			{
				lines.Add("Благодаря " + Name + " " + Owner.Name + " восстанавливает " + PoisonBuff + " здоровья.");
			}

			Proc = (GameplayController.Random.NextDouble() <= ProcChance);

			return lines.ToArray();
		}

		public void Infect()
		{
			if (IsInfective && InfectionStage < InfectionMaxStage)
			{
				List<Enemy> enemies = Objects.GetList<Enemy>();

				foreach(Vector2 v in Player.Rotation)
				{
					foreach(Enemy enemy in enemies)
					{
						if (GameplayController.Random.NextDouble() <= InfectionChance 
						&& enemy.Pos == Owner.Pos + v * (Arena.UnitSpacing + Vector2.One))
						{
							enemy.AddStatEffect(Effects[Token], InfectionStage + 1);
						}
					}
				}
				InfectionStage = InfectionMaxStage;
			}
		}

		public static void InitStatEffects()
		{
			StatEffect effect;

			effect = new StatEffect();
			effect.Token = "cold";
			effect.Name = Inventory.ItemPool[effect.Token].Name;//"Зелье простуды";
			effect.PoisonBuff = 15;
			effect.Duration = 3;
			effect.Color = Color.YellowGreen;
			effect.IsInfective = true;
			effect.InfectionChance = 0.75f;
			effect.InfectionMaxStage = 7;
			Effects.Add(effect.Token, effect);

			effect = new StatEffect();
			effect.Token = "weakness";
			effect.Name = Inventory.ItemPool[effect.Token].Name;
			effect.Duration = 3;
			effect.DefenceBuff = -10;
			effect.AttackBuff = -10;
			effect.Color = Color.MediumBlue;
			effect.IsInfective = true;
			effect.InfectionChance = 0.99f;
			effect.InfectionMaxStage = 12;
			Effects.Add(effect.Token, effect);

			effect = new StatEffect();
			effect.Token = "flu";
			effect.Name = Inventory.ItemPool[effect.Token].Name;
			effect.Duration = 3;
			effect.AttackBuff = -3;
			effect.Color = Color.Yellow;
			effect.IsInfective = true;
			effect.InfectionChance = 0.75f;
			effect.InfectionMaxStage = 4;
			effect.ProcChance = 0.33f;
			Effects.Add(effect.Token, effect);

			effect = new StatEffect();
			effect.Token = "rabies";
			effect.Name = Inventory.ItemPool[effect.Token].Name;
			effect.Duration = 3;
			effect.AttackBuff = 10;
			effect.Color = Color.Red;
			effect.IsInfective = true;
			effect.InfectionChance = 0.1f;
			effect.InfectionMaxStage = 2;
			effect.ProcChance = 1f;
			Effects.Add(effect.Token, effect);
		}


	}
}
