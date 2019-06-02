using System.IO;
using System;

namespace Flucoldache
{
	public static class Strings
	{
		
		public static string Localization {get; private set;}
		
		public static string 
			MenuTitle,
			MenuNewGame,
			MenuLoad,
			MenuMapEditor,
			MenuLoadMap,
			MenuExit,
			MenuControls,
			MenuCredits1,
			MenuCredits2,
			
			PlayerMenu,
			Inventory,
			Potions,
			Back,

			BackTip,
			NothingHappened,
			NoTimeToRead,
			CantUsePotionsNow,
			EatingFood,
			EatingDeny,
			CantUseLab,

			OpenGateLine,
			BonfireLine1,
			BonfireLine2,
			BonfireLine3,
			OpenChestLine1,
			OpenChestLine2,
			OpenEmptyChestLine,
			
			BattleTip,
			Attack,
			PlayerName1,
			PlayerName2,
			PlayerAttackDialogue,
			PlayerPotionDialogue,

			BattleWin,
			BattleDefeat,
			EnemySkipTurn,
			EnemyRabid,
			EnemyAttack,
			BearName1,
			BearName2,
			KnightName1,
			KnightName2,
			RabbitName1,
			RabbitName2,
			VillagerName1,
			VillagerName2,
			WolfName1,
			WolfName2,

			Continue,
			FullscreenOn,
			FullscreenOff,
			Exit
		;

		private static int _index = 0; 
		private static string[] _strings;

		private static bool _loaded = false;

		public static void Load(string localization)
		{
			if (_loaded)
			{
				return;
			}

			//_loaded = true;

			Localization = localization;
			_strings = File.ReadAllLines(Environment.CurrentDirectory + "/Resources/" + Localization + "/Strings.txt");
			
			MenuTitle = NextString();
			MenuNewGame = NextString();
			MenuLoad = NextString();
			MenuMapEditor = NextString();
			MenuLoadMap = NextString();
			MenuExit = NextString();
			MenuControls = NextString();
			MenuCredits1 = NextString();
			MenuCredits2 = NextString();

			PlayerMenu = NextString();
			Inventory = NextString();
			Potions = NextString();
			Back = NextString();
			
			BackTip = NextString();
			NothingHappened = NextString();
			NoTimeToRead = NextString();
			CantUsePotionsNow = NextString();
			EatingFood = NextString();
			EatingDeny = NextString();
			CantUseLab = NextString();

			OpenGateLine = NextString();
			BonfireLine1 = NextString();
			BonfireLine2 = NextString();
			BonfireLine3 = NextString();
			OpenChestLine1 = NextString();
			OpenChestLine2 = NextString();
			OpenEmptyChestLine = NextString();

			BattleTip = NextString();
			Attack = NextString();
			PlayerName1 = NextString();
			PlayerName2 = NextString();
			PlayerAttackDialogue = NextString();
			PlayerPotionDialogue = NextString();

			BattleWin = NextString();
			BattleDefeat = NextString();

			EnemySkipTurn = NextString();
			EnemyRabid = NextString();
			EnemyAttack = NextString();

			BearName1 = NextString();
			BearName2 = NextString();
			KnightName1 = NextString();
			KnightName2 = NextString();
			RabbitName1 = NextString();
			RabbitName2 = NextString();
			VillagerName1 = NextString();
			VillagerName2 = NextString();
			WolfName1 = NextString();
			WolfName2 = NextString();

			Continue = NextString();
			FullscreenOn = NextString();
			FullscreenOff = NextString();
			Exit = NextString();

			_index = 0;
		}

		private static string NextString()
		{
			_index += 1;
			return _strings[_index - 1];
		}

	}
}
