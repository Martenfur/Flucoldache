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
		private static string _soundsDir = "Content/Sounds/";

		
		public static Sound Title;
		public static Sound House;
		public static Sound Overworld;
		public static Sound Battle;
		public static Sound FinalBattle;
		
		public static Sound CurrentSong {get; private set;}

		private static List<Sound> _queue = new List<Sound>();

		private static FMOD.ChannelGroup _channelGroup; 

		public static void Init(ContentManager content)
		{
			_channelGroup = AudioMgr.CreateChannelGroup("group");

			Title = AudioMgr.LoadStreamedSound(_musicDir + "Title");
			House = AudioMgr.LoadStreamedSound(_musicDir + "House");
			Overworld = AudioMgr.LoadStreamedSound(_musicDir + "Overworld");
			Battle = AudioMgr.LoadStreamedSound(_musicDir + "Battle");
			FinalBattle = AudioMgr.LoadStreamedSound(_musicDir + "FinalBattle");
		}
		
		public static void PlaySong(Sound song)
		{
			if (CurrentSong != song)
			{
				if (CurrentSong != null)
				{
					CurrentSong.Stop();
				}
				CurrentSong = AudioMgr.PlaySound(song, _channelGroup);
				CurrentSong.Channel.setMode(FMOD.MODE.LOOP_NORMAL);
				CurrentSong.Loops = -1;

			}
		}

		public static void QueueSong(Sound song)
		{
			_queue.Add(song);
		}
		
		
		public static void Unload()
		{
		}



	}
}
