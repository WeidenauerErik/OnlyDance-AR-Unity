using System;
using UnityEngine;
using System.Collections.Generic;

public class GeneralSerializables
{
    [Serializable]
    public class Response
    {
        public bool success;
        public string error;
        public string message;
        public string password;
    }
    
    [Serializable]
    public class User
    {
        public string email;
        public string password;

        public User(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }

    [Serializable]
    public class DanceResponse
    {
        public bool success;
        public Step[] data;
    }

    [Serializable]
    public class Step
    {
        public int id;

        public float m1_x;
        public float m1_y;
        public bool m1_toe;
        public bool m1_heel;
        public float m1_rotate;

        public float m2_x;
        public float m2_y;
        public bool m2_toe;
        public bool m2_heel;
        public float m2_rotate;
    }
    
    [Serializable]
    public class StepDanceAnimator
    {
        public Vector3 leftFootPosition;
        public Vector3 rightFootPosition;

        public float leftRotation;
        public float rightRotation;

        public bool leftToe;
        public bool leftHeel;
        public bool rightToe;
        public bool rightHeel;
    }
    
    [Serializable]
    public class DanceData
    {
        public int id;
        public string name;
        public int BPM;
        public List<Step> data;
    }

    [Serializable]
    public class DanceCollection
    {
        public List<DanceData> dances = new List<DanceData>();
    }
    
    [Serializable]
    public class Dance
    {
        public int id;
        public string name;
    }

    [Serializable]
    public class DanceWrapper
    {
        public Dance[] data;
    }
    
    [Serializable]
    public class ChangePwdRequest
    {
        public string email;
        public string oldPassword;
        public string newPassword;

        public ChangePwdRequest(string email, string oldPassword, string newPassword)
        {
            this.email = email;
            this.oldPassword = oldPassword;
            this.newPassword = newPassword;
        }
    }

    [Serializable]
    public class DeleteAccountRequest
    {
        public string email;
        public string password;

        public DeleteAccountRequest(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}