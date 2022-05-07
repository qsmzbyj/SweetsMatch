using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///清除甜品
///<summary>
public class ClearSweets : MonoBehaviour
{
    public AnimationClip clearAnimation;

    public AudioClip destroyAudio;

    protected SweetControl sweet;

    private bool isClearing;

    public bool IsClearing { get => isClearing;  }

    private void Awake()
    {
        sweet = GetComponent<SweetControl>();
    }

    public virtual void Clear()
    {
        isClearing = true;
        StartCoroutine(ClearCoroutine());
    }

    public IEnumerator ClearCoroutine()
    {
        Animator animator = sweet.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(clearAnimation.name);
            //玩家积分+1，并播放清除动画
            AudioSource.PlayClipAtPoint(destroyAudio,transform.position);
            GameManager.Instance.playerScore++;
            yield return new WaitForSeconds(clearAnimation.length);
            
            Destroy(gameObject);
        }

    }
}
