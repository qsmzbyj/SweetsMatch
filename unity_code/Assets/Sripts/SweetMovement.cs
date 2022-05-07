using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///甜品的移动脚本
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
    /// 移动甜品的位置
    /// </summary>
    /// <param name="newX">x轴</param>
    /// <param name="newY">y轴</param>
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
    /// 移动甜品 协程
    /// </summary>
    /// <param name="newX">x轴</param>
    /// <param name="newY">y轴</param>
    /// <param name="time">完成时间</param>
    /// <returns></returns>
    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        
        sweet.X = newX;
        sweet.Y = newY;
        //每一帧移动一点点
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
