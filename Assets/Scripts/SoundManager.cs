using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioClip buffSound;
    private AudioClip castSpellSound;
    private AudioClip crossbowSound;
    private AudioClip lootDropSound;
    private AudioClip meleeSound;
    private AudioClip noTargetSound;
    private AudioClip pickupItemSound;
    private AudioSource audioSource;
    void Start()
    {
        buffSound = Resources.Load<AudioClip>("buff");
        castSpellSound = Resources.Load<AudioClip>("cast_spell");
        crossbowSound = Resources.Load<AudioClip>("crossbow");
        lootDropSound = Resources.Load<AudioClip>("loot_drop");
        meleeSound = Resources.Load<AudioClip>("melee");
        noTargetSound = Resources.Load<AudioClip>("no_target");
        pickupItemSound = Resources.Load<AudioClip>("pickup_item");
        audioSource = GetComponent<AudioSource>();
    }

    // Make sure this is the only SoundManager in the scene
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Sound");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void PlaySound (string sound)
    {
        if (sound == "buff")
        {
            audioSource.PlayOneShot(buffSound);
        }
        else if (sound == "castSpell")
        {
            audioSource.PlayOneShot(castSpellSound);
        }
        else if (sound == "crossbow")
        {
            audioSource.PlayOneShot(crossbowSound);
        }
        else if (sound == "lootDrop")
        {
            audioSource.PlayOneShot(lootDropSound);
        }
        else if (sound == "melee")
        {
            audioSource.PlayOneShot(meleeSound);
        }
        else if (sound == "noTarget")
        {
            audioSource.PlayOneShot(noTargetSound);
        }
        else if (sound == "pickupItem")
        {
            audioSource.PlayOneShot(pickupItemSound);
        }
        else
        {
            Debug.Log($"Error: {sound} is not a recognised sound");
        }
    }
}
