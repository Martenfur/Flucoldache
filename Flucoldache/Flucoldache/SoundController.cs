using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Monofoxe.FMODAudio;

namespace Flucoldache
{
	public static class SoundController
	{
		private static string _musicDir = "Content/Music/";
		private static string _soundDir = "Content/Sounds/";

		
		public static Sound Title;
		public static Sound Overworld;
		public static Sound Battle;
		public static Sound FinalBattle;
		
		public static Sound Blip;
		public static Sound Back;
		public static Sound Hit;
		public static Sound Death;
		public static Sound ArenaAppearEffect;
		public static Sound Voice;
		public static Sound OpenGate;
		public static Sound OpenChest;
		public static Sound Heal;
		public static Sound NewGame;
		public static Sound Win;
		public static Sound Potion;


		public static Sound CurrentSong {get; private set;}
		private static Sound _currentSongTemplate;

		private static FMOD.ChannelGroup _channelGroup; 

		public static void Init(ContentManager content)
		{
			_channelGroup = AudioMgr.CreateChannelGroup("group");
			
			Title = AudioMgr.LoadStreamedSound(_musicDir + "Title");
			Overworld = AudioMgr.LoadStreamedSound(_musicDir + "Overworld");
			Battle = AudioMgr.LoadStreamedSound(_musicDir + "Battle");
			FinalBattle = AudioMgr.LoadStreamedSound(_musicDir + "FinalBattle");

			Blip = AudioMgr.LoadSound(_soundDir + "Blip");
			Back = AudioMgr.LoadSound(_soundDir + "Back");
			Hit = AudioMgr.LoadSound(_soundDir + "Hit");
			Death = AudioMgr.LoadSound(_soundDir + "Death");
			ArenaAppearEffect = AudioMgr.LoadSound(_soundDir + "ArenaAppearEffect");
			Voice = AudioMgr.LoadSound(_soundDir + "Voice");
			OpenGate = AudioMgr.LoadSound(_soundDir + "OpenGate");
			OpenChest = AudioMgr.LoadSound(_soundDir + "OpenChest");
			Heal = AudioMgr.LoadSound(_soundDir + "Heal");
			NewGame = AudioMgr.LoadSound(_soundDir + "NewGame");
			Win = AudioMgr.LoadSound(_soundDir + "Win");
			Potion = AudioMgr.LoadSound(_soundDir + "Potion");

		}
		
		public static void PlaySong(Sound song)
		{
			if (_currentSongTemplate != song)
			{
				if (CurrentSong != null)
				{
					CurrentSong.Stop();
				}
				_currentSongTemplate = song;
				CurrentSong = AudioMgr.PlaySound(song, _channelGroup);
				CurrentSong.Channel.setMode(FMOD.MODE.LOOP_NORMAL);
				CurrentSong.Loops = -1;
			}
		}
		
		public static Sound PlaySound(Sound sound)
		{
			var s = AudioMgr.PlaySound(sound, _channelGroup);
			s.Loops = 0;
			return s;
		}
		
		public static void Unload()
		{
		}



	}
}
