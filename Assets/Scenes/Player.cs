using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;

public class Player : MonoBehaviour
{
    private Animator animator;

    TcpClient client;
    string serverIP = "127.0.0.1";
    int port = 8000;
    byte[] receivedBuffer;
    StreamReader reader;
    bool socketReady = false;
    NetworkStream stream;

    bool isSatisfied = false;
    float timer;
    int watingTime;

    private void Start()
    {
        animator = GetComponent<Animator>();

        CheckRecieve();

        timer = 0.0f;
        watingTime = 6;
    }

    void CheckRecieve()
    {
        if(socketReady) return;

        try
        {
            client = new TcpClient(serverIP, port);

            if(client.Connected)
            {
                stream = client.GetStream();
                UnityEngine.Debug.Log("Connect Success");
                socketReady = true;
            }
        }

        catch (Exception e)
        {
            UnityEngine.Debug.Log("On client connect exception" + e);
        }
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if(!socketReady) return;

        reader.Close();
        client.Close();
        socketReady = false;
    }

    void IncreaseScore(char wave)
    {
        if (wave == '0')
        {
            Score.Cat = Score.Cat + 1;
            UnityEngine.Debug.Log("score update");
        }
        else if (wave == '1')
        {
            Score.Cat = Score.Cat + 10;
            UnityEngine.Debug.Log("score update");
        }
        else if (wave == '2')
        {
            Score.Cat = Score.Cat + 100;
            UnityEngine.Debug.Log("score update");
        }
        else if (wave == '3')
        {
            Score.Cat = Score.Cat + 1000;
            UnityEngine.Debug.Log("score update");
        }
    }

    private void Update()
    {
        //if (timer > watingTime)
        //{
            if (socketReady)
            {
                if (stream.DataAvailable)
                {
                    receivedBuffer = new byte[100];
                    stream.Read(receivedBuffer, 0, receivedBuffer.Length);
                    string str = Encoding.UTF8.GetString(receivedBuffer, 0, receivedBuffer.Length);
                    char wave = str[26];
                    UnityEngine.Debug.Log(wave);
                    isSatisfied = true;
                
                    if (isSatisfied)
                    {
                        if (wave == '0')
                        {
                            //PerformAnimationAndIncreaseScore("doJump");
                            animator.SetTrigger("doJump");
                            UnityEngine.Debug.Log("doJump + 1");
                        }
                        else if (wave == '1')
                        {
                            //PerformAnimationAndIncreaseScore("doHi");
                            animator.SetTrigger("doHi");
                            UnityEngine.Debug.Log("doHi + 10");
                        }
                        else if (wave == '2')
                        {
                            //PerformAnimationAndIncreaseScore("doSit");
                            animator.SetTrigger("doSit");
                            UnityEngine.Debug.Log("doSit + 100");
                        }
                        else if (wave == '3')
                        {
                            //PerformAnimationAndIncreaseScore("isVictory");
                            animator.SetTrigger("isVictory");
                            UnityEngine.Debug.Log("isVictory + 1000");
                        }
                    }
                    StartCoroutine("Delay", wave);
                }
            }
            timer = 0;
            isSatisfied = false;
        /* }
        else
        {
            timer += Time.deltaTime;
        } */
    }

    IEnumerator Delay(char wave)
    {
        yield return new WaitForSeconds(2f);
        IncreaseScore(wave);
    }
}

