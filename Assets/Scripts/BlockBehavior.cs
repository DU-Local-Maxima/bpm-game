using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    private float _destroyThreshold = 3.0f;
    private AudioSource _gmas;
    private SpriteRenderer _sr;
    private GameMasterBehavior _gmb;
    private SoundEffectsSourceBehavior sfx;

    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        
        GameObject gm = GameObject.Find("GameMaster");
        _gmas = gm.GetComponent<AudioSource>();        
        _gmb = gm.GetComponent<GameMasterBehavior>();
        sfx = GameObject.Find("SoundEffectsSource").GetComponent<SoundEffectsSourceBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gmas.isPlaying && transform.position.y <= _destroyThreshold && _gmas.clip.name.Contains(NameOfColor()))
        {
            _gmb.BlockDestroyed();
            Object.Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _gmb.SpeedUp();
        }
        sfx.PlayCollisionFx();
    }

    public string NameOfColor() {
        var props = _sr.material.color.GetType().GetProperties(BindingFlags.Public | BindingFlags.Static);

        foreach (var prop in props) {
            if ((Color) prop.GetValue(null) == _sr.material.color) { return prop.Name; }  
        }
        return _sr.material.color.ToString();
    }
}
