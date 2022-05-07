using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///��Ʒ���ƶ��ű�
///<summary>
public class SweetMovement : MonoBehaviour
{
 
    private SweetControl sweet;
    private IEnumerator moveCoroutine;
    private void Awake()
    {
        sweet = GetComponent<SweetControl>();
    }
    /// <summary>
    /// �ƶ���Ʒ��λ��
    /// </summary>
    /// <param name="newX">x��</param>
    /// <param name="newY">y��</param>
    public void Move(int newX,int newY,float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine=MoveCoroutine(newX,newY,time);
        StartCoroutine(moveCoroutine);
    }
    /// <summary>
    /// �ƶ���Ʒ Э��
    /// </summary>
    /// <param name="newX">x��</param>
    /// <param name="newY">y��</param>
    /// <param name="time">���ʱ��</param>
    /// <returns></returns>
    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        
        sweet.X = newX;
        sweet.Y = newY;
        //ÿһ֡�ƶ�һ���
        Vector3 startPos =transform.position;
        Vector3 endPos = sweet.gameManager.CorrectPosition(newX, newY);
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            sweet.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        /*while (Vector3.Distance(transform.position, endPos) != 0)
        {
            sweet.transform.position = Vector3.Lerp(StartPos, endPos, Time.deltaTime / time);
            yield return null;
        }*/
        sweet.transform.position = endPos;
    }

}
