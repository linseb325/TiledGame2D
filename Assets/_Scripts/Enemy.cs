using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public AudioClip[] audioClips;
    private EnemyContainer myContainer;



	// Use this for initialization
	void Start () {

        this.myContainer = new EnemyContainer(new NPCStats(8, 3, 12), this.gameObject, this.gameObject.GetComponent<LerpBackForth>());
        GameCore.currentEnemy = this.myContainer;

        // Play all audio clips back-to-back.
        float currDelay = 0;
        AudioSource currAudioPlayer;
        foreach(AudioClip currClip in audioClips)
        {
            currAudioPlayer = this.gameObject.AddComponent<AudioSource>();
            currAudioPlayer.clip = currClip;
            currAudioPlayer.PlayDelayed(currDelay);
            currDelay += currClip.length;
            StartCoroutine(removeAudioSource(currAudioPlayer, currDelay));
        }
	}


    private IEnumerator removeAudioSource(AudioSource source, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Destroy(source);
    }



	
	// Update is called once per frame
	void Update () {
		
	}
}
