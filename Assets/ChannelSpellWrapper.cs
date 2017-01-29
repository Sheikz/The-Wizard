using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelSpellWrapper : SpellController {

    public ChannelSpell[] channelSpell;
    public int numberOfSpells = 1;
    public float angleBetweenSpells = 0;
    public float angleOffset = 0;
    public float rotationSpeed = 0;

    private float currentAngle;
    private Vector3 direction;
    private ChannelSpell[] instances;


    public override SpellController castSpell(SpellCaster emitter, Vector3 target)
    {
        var newSpell = Instantiate(this);
        newSpell.emitter = emitter;
        newSpell.start(target);
        return this;
    }


    void start(Vector3 target)
    {
        this.target = target;
        direction = (target - emitter.transform.position).normalized;
        direction = Quaternion.Euler(0, 0, angleOffset) * direction;
        instances = new ChannelSpell[channelSpell.Length];

        for (int i=0; i < channelSpell.Length; i++)
        {
            var currentDir = Quaternion.Euler(0, 0, angleBetweenSpells * i) * target;
            instances[i] = channelSpell[i].castSpell(emitter, this.emitter.transform.position + currentDir) as ChannelSpell;
        }

        StartCoroutine(channelSpellRoutine());
    }

    IEnumerator channelSpellRoutine()
    {
        var movingChar = emitter.GetComponent<MovingCharacter>();
        //movingChar.enableAction(false);
        float startTime = Time.realtimeSinceStartup;
        
        while(Time.realtimeSinceStartup -startTime < duration)
        {
            direction = Quaternion.Euler(0, 0, rotationSpeed) * direction;
            for (int i=0; i < instances.Length; i++)
            {
                Vector3 currentDir = Quaternion.Euler(0, 0, i * angleBetweenSpells) * direction;
                instances[i].update(this.emitter.transform.position + currentDir);
            }
            yield return new WaitForFixedUpdate();
        }

        foreach (ChannelSpell sp in instances)
            sp.stop();

        //movingChar.enableAction(true);

        Destroy(gameObject);
        yield break;
    }
}
