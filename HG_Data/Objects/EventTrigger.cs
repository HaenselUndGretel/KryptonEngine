using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.FModAudio;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HanselAndGretel.Data
{
	public class EventTrigger : GameObject
	{
		public enum EEvent
		{
			SpawnWitch,
			TreeFalling,
			PlaySound,
			ActivateTrigger
		}

		#region Properties

		// Trigger kann wegfallen, da von GameObject erbt
		//protected Rectangle mTrigger;
		// mIdToTrigger = Baum der umfällt.
		protected int mIdToTrigger;
		protected EEvent mEvent;
		protected bool mActivated;

		//////////////////////////////////////////
		// Variabeln für die vorhandenen Events //
		protected Vector2 mWitchSpawnPosition;
		// Soundname wird geändert sobald FMOD integriert ist,
		// und somit ein Sound abgespielt werden kann wenn der Spieler sich in der Eventzone befindet.
		protected String mSoundName;
		#endregion

		#region Getter & Setter
		// Trigger kann wegfallen, da von GameObject erbt
		// public Rectangle Trigger { get { return mTrigger; } set { mTrigger = value; } }
		public EEvent Event { get { return mEvent; } set { mEvent = value; } }
		public int Target { get { return mIdToTrigger; } set { mIdToTrigger = value; } }
		public bool IsAcitvated { get { return mActivated; } set { mActivated = value; } }

		//////////////////////////////////////////
		// Variabeln für die vorhandenen Events //
		public Vector2 WitchSpawnPosition { get { return mWitchSpawnPosition; } set { mWitchSpawnPosition = value; } }
		public String SoundName { get { return mSoundName; } set { mSoundName = value; } }
		[XmlIgnoreAttribute]
		public bool Activated { get { return mActivated; } }
		#endregion

		#region Constructor
		public EventTrigger() 
			: base()
		{
			mIdToTrigger = -1;
			mWitchSpawnPosition = Vector2.Zero;
			SoundName = "";
		}
		#endregion

		#region OverrideMethods

		// EditorFunktion
		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(TextureManager.Instance.GetElementByString("IconEventArea"), CollisionBox, Color.White);
		}
		#endregion

		#region Methods

		public void TriggerActivity()
		{
			switch(this.mEvent)
			{
				case EEvent.PlaySound: PlaySound();
					break;
				case EEvent.SpawnWitch: SpawnWitch();
					break;
			}
			mActivated = true;
		}

		private void PlaySound()
		{
			FmodMediaPlayer.Instance.AddSong(this.mSoundName, 0.8f);
		}

		private void SpawnWitch()
		{
			//FmodMediaPlayer.Instance.AddSong("WitchSpawn");
			//FmodMediaPlayer.Instance.FadeBackgroundChannelIn(WitchChannelIndex);
			Witch witch = new Witch("witch");
			witch.LoadContent();
			witch.Position = mWitchSpawnPosition;
			witch.ApplySettings();
			witch.IsAiActive = true;
			GameReferenzes.Level.Enemies.Add(witch);
			GameReferenzes.Level.RenderList.Add(witch);
		}
		#endregion
	}
}
