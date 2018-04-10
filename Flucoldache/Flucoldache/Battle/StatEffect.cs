using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flucoldache.Battle
{
	public class StatEffect
	{
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
		public int InfectionChance;
		public int InfectionDecrease;

	}
}
