using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///�����Ʒ
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
            //��һ���+1���������������
            AudioSource.PlayClipAtPoint(destroyAudio,transform.position);
            GameManager.Instance.playerScore++;
            yield return new WaitForSeconds(clearAnimation.length);
            
            Destroy(gameObject);
        }

    }
}
