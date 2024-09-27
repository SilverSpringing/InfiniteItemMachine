using System;
using System.Collections;
using UnityEngine;

namespace InfiniteItems
{
    public class PurpleLocker : EnvironmentObject, IClickable<int>
    {
		public float PREVIOUS_SCALE;

		public void Start()
        {
			meshFilter = GetComponentInChildren<MeshFilter>();
			audMan = GetComponentInChildren<PropagatedAudioManager>();

			audOpen = InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("PURPLE_LOCKER_OPEN");
			audClose = InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("PURPLE_LOCKER_CLOSE");
		}

		public void Clicked(int player)
		{
			this.Toggle();
			if (this.open)
			{
				return;
			}
			base.StopAllCoroutines();
			if (InfiniteItemsPlugin.pauseWhenOpening.Value) PauseEverything(this.open);

		}

		public void ClickableSighted(int player)
		{
		}


		public void ClickableUnsighted(int player)
		{
		}

		public bool ClickableHidden()
		{
			return false;
		}

		public bool ClickableRequiresNormalHeight()
		{
			return true;
		}

		public void PauseEverything(bool pause)
        {
			if (pause)
            {
				Singleton<MusicManager>.Instance.PauseMidi(true);
				this.PREVIOUS_SCALE = Time.timeScale;
				Time.timeScale = 0f;
			}
			else
            {
				Singleton<MusicManager>.Instance.PauseMidi(false);
				Time.timeScale = PREVIOUS_SCALE;
			}
		}

		private void Toggle()
		{
			this.open = !this.open;
			BoxCollider[] array = this.openBoxCollider;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = this.open;
			}
			array = this.closedBoxCollider;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = !this.open;
			}
			if (this.open)
			{
				this.meshFilter.mesh = this.openMesh;
				this.audMan.PlaySingle(this.audOpen);
				return;
			}
			this.ec.MakeNoise(base.transform.position, this.noiseVal);
			this.meshFilter.mesh = this.closedMesh;
			this.audMan.PlaySingle(this.audClose);
		}

		private IEnumerator ShutWhenPlayerLeaves(PlayerManager player)
		{
			while (Vector3.Distance(base.transform.position, player.transform.position) < this.maxPlayerDistance)
			{
				player.RuleBreak("Lockers", 1f);
				yield return null;
			}
			if (this.open)
			{
				this.Toggle();
			}
			yield break;
		}

		public void SetMeshes(Mesh a, Mesh b)
        {
			closedMesh = a;
			openMesh = b;
		}

		[SerializeField]
		private MeshFilter meshFilter;

		[SerializeField]
		private AudioManager audMan = new AudioManager();

		[SerializeField]
		private SoundObject audOpen;

		[SerializeField]
		private SoundObject audClose;

		[SerializeField]
		private BoxCollider[] closedBoxCollider = new BoxCollider[0];

		[SerializeField]
		private BoxCollider[] openBoxCollider = new BoxCollider[0];

		[SerializeField]
		private Mesh closedMesh;

		[SerializeField]
		private Mesh openMesh;

		[SerializeField]
		private float maxPlayerDistance = 17f;

		[SerializeField]
		[Range(0f, 127f)]
		private int noiseVal = 58;

		private bool open;
	}
}