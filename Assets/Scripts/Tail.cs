﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    public enum TaleType { empty, barrier, railway, start, finish, gasstation}
    public TaleType taleType;
    public bool rotatable, movable;
    public Vector3 specialPos;
    protected Vector3 cursorPos;
    protected int posX, posZ;
    protected int myNum;
    protected GameObject specialGraphic;
    public GameObject peron;
    protected char special;

    protected bool blockTail;
    // Start is called before the first frame update


    public void RememberPositionInArray(int x, int z)
    {
        posX = x;
        posZ = z;
    }
    
    public void BlockTail(bool block_)
    {
        blockTail = block_;
    }
    public bool IsBlocked()
    {
        return blockTail;
    }

    public void RememberNum(int num)
    {
        myNum = num;
    }
    public int GetNum()
    {
        return myNum;
    }
    public PositionTale GetPos()
    {
        PositionTale myPos;
        myPos.X = posX;
        myPos.Z = posZ;
        return myPos;
    }
    public PositionTale ChangeOtherTale(Tail newTale)
    {
        TailsTable.talesTable.ReplaceTale(newTale, posX, posZ);
        PositionTale myPos;
        myPos.X = posX;
        myPos.Z = posZ;
        return myPos;
    }

    public void PrepareSpecialForRotate(bool before)
    {
        if (!specialGraphic)
            return;
        if(before)
            specialGraphic.transform.parent = transform;
        else
        {
            specialGraphic.transform.parent = null;

        }
    }
    
    public void AddNewSpecial(GameObject specialNew, char name_char)
    {

        if(!specialGraphic)
        {
            specialGraphic = Instantiate(specialNew,transform);
            //specialGraphic = specialNew;
            specialGraphic.transform.localPosition = specialPos;
            //specialGraphic.transform.rotation = new Quaternion();
            special = name_char;
            if (peron)
                peron.SetActive(true);
            //specialGraphic.transform.parent = null;
        }
        else
        {
            //Destroy(specialGraphic);
            special = 'n';
        }
    }

    public char getSpecialName()
    {
        return special;
    }
}


public struct PositionTale
{
    public int X;
    public int Z;
}
