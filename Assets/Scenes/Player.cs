using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

using LSL;
using LSL4Unity.Utils;
using System.Collections;

public class Player : MonoBehaviour
{
    private Animator animator;

    #region LSL4Unity_inlet
    public string StreamName;
    ContinuousResolver resolver;
    double max_chunk_duration = 2.0;
    private StreamInlet inlet;

    // buffers to pass to LSL when pulling data
    private float[,] data_buffer;
    private double[] timestamp_buffer;
    public float baseline_pow = -999.0f;
    float EEGpow;
    bool isSatisfied = false;

    string StreamName_marker = "LSL4Unity_marker";
    string StreamType_marker = "Markers";
    public bool IrregularRate = true;
    public MomentForSampling moment;
    public static StreamOutlet outlet;
    public static int OV_marker = 811;
    public static double startTime;
    #endregion

    public int wave;
    float timer;
    int watingTime;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (!StreamName.Equals(""))
            resolver = new ContinuousResolver("name", StreamName);
        else
        {
            UnityEngine.Debug.LogError("Object must specify a name for resolver to lookup a stream.");
            this.enabled = false;
            return;
        }
        StartCoroutine(ResolveExpectedStream());

        var hash_marker = new Hash128();
        hash_marker.Append(StreamName_marker);
        hash_marker.Append(StreamType_marker);
        hash_marker.Append(gameObject.GetInstanceID());
        StreamInfo streamInfo_marker = new StreamInfo(StreamName_marker, StreamType_marker, 1, LSL.LSL.IRREGULAR_RATE,
        channel_format_t.cf_int32, hash_marker.ToString());
        outlet = new StreamOutlet(streamInfo_marker);
        startTime = LSL.LSL.local_clock();

        timer = 0.0f;
        watingTime = 4;
    }

    IEnumerator ResolveExpectedStream()
    {
        var results = resolver.results();
        while (results.Length == 0)
        {
            yield return new WaitForSeconds(.1f);
            results = resolver.results();
        }
        inlet = new StreamInlet(results[0]);
        int n_channels = inlet.info().channel_count();
        // float s_freq = inlet.info().nominal_srate();
        int buf_samples = (int)Mathf.Ceil((float)(inlet.info().nominal_srate() * max_chunk_duration));
        // int buf_samples = (int)Mathf.Ceil((float)(inlet.info().nominal_srate()));
        data_buffer = new float[buf_samples, n_channels]; // float �迭
        timestamp_buffer = new double[buf_samples];
    }

/*     void PerformAnimationAndIncreaseScore(string triggerName)
    {
        animator.SetTrigger(triggerName);

        if (triggerName == "doJump")
        {
            Score.Cat = Score.Cat + 1;

        }
        else if (triggerName == "doHi")
        {
            Score.Cat = Score.Cat + 10;
        }
        else if (triggerName == "doSit")
        {
            Score.Cat = Score.Cat + 100;
        }
        else if (triggerName == "isVictory")
        {
            Score.Cat = Score.Cat + 1000;
        }
    } */

    void IncreaseScore(int wave)
    {
        if (wave == 0)
        {
            Score.Cat = Score.Cat + 1;
        }
        else if (wave == 1)
        {
            Score.Cat = Score.Cat + 10;
        }
        else if (wave == 2)
        {
            Score.Cat = Score.Cat + 100;
        }
        else if (wave == 3)
        {
            Score.Cat = Score.Cat + 1000;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > watingTime)
        {
            Process process = new Process();

            process.StartInfo.FileName = @"pythonw";
            process.StartInfo.Arguments = @"..\Suriyun\Scripts\bpf.py";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;

            #region LSL_inlet_update
            // LSL - inlet
            if (inlet != null)
            {
                process.Start();

                int samples_returned = inlet.pull_chunk(data_buffer, timestamp_buffer);
                //Debug.Log("data_buffer: " + data_buffer);
                //Debug.Log("timestamp_buffer: " + timestamp_buffer);
                
                if (samples_returned > 0)
                {
                    //UnityEngine.Debug.Log("timestamp_buffer: " + timestamp_buffer);

                    float x = data_buffer[samples_returned - 1, 0] * 100;
                    float y = data_buffer[samples_returned - 1, 1] * 100;
                    float z = data_buffer[samples_returned - 1, 2] * 100;
                    float a = data_buffer[samples_returned - 1, 3] * 100;

                    //Debug.Log("samples_returned: " + samples_returned);
                    //Debug.Log("x: " + data_buffer[samples_returned - 1, 0]);
                    //Debug.Log("x: " + (data_buffer[samples_returned - 1, 0] * 100));

                    var new_scale = new Vector3(x, y, z);
                    EEGpow = new_scale.y;

                    //process.StandardInput.WriteLine(EEGpow);
                    //process.StandardInput.Flush();
                    //process.StandardInput.Close();

                    string output = process.StandardOutput.ReadToEnd();
                    //double f_data = Double.Parse(output);
                    UnityEngine.Debug.Log(output);

                    UnityEngine.Debug.Log("EEGpow: " + EEGpow);
                    //double s_freq = #;
                    if (baseline_pow == -999.0f) // unset
                    {
                        if (EEGpow != 0.0f)
                        {
                            //Debug.Log(EEGpow);
                            outlet.push_sample(new int[1] { OV_marker });
                            if (EEGpow < -200.0f)
                            {
                                wave = 0;
                                isSatisfied = true;
                            }
                            else if (EEGpow < -160.0f)
                            {
                                wave = 1;
                                isSatisfied = true;
                            }
                            else if (EEGpow < -80.0f)
                            {
                                wave = 2;
                                isSatisfied = true;
                            }
                            else if (EEGpow < 0.0f)
                            {
                                wave = 3;
                                isSatisfied = true;
                            }
                        }
                        else
                            isSatisfied = false;
                    }
                    else
                    {
                        if (EEGpow < baseline_pow)
                        {
                            //UnityEngine.Debug.Log(LSL.LSL.local_clock() - startTime);
                            outlet.push_sample(new int[1] { OV_marker });
                            isSatisfied = true;
                        }
                        else
                            isSatisfied = false;
                    }
                }
                #endregion

                if (isSatisfied)
                {
                    if (wave == 0)
                    {
                        //PerformAnimationAndIncreaseScore("doJump");
                        animator.SetTrigger("doJump");
                        UnityEngine.Debug.Log("doJump + 1");
                    }
                    else if (wave == 1)
                    {
                        //PerformAnimationAndIncreaseScore("doHi");
                        animator.SetTrigger("doHi");
                        UnityEngine.Debug.Log("doHi + 10");
                    }
                    else if (wave == 2)
                    {
                        //PerformAnimationAndIncreaseScore("doSit");
                        animator.SetTrigger("doSit");
                        UnityEngine.Debug.Log("doSit + 100");
                    }
                    else if (wave == 3)
                    {
                        //PerformAnimationAndIncreaseScore("isVictory");
                        animator.SetTrigger("isVictory");
                        UnityEngine.Debug.Log("isVictory + 1000");
                    }
                    StartCoroutine("Delay", wave);
                }
            }
            timer = 0;
            isSatisfied = false;
        }
    }

    IEnumerator Delay(int wave)
    {
        yield return new WaitForSeconds(2.25f);
        IncreaseScore(wave);
    }
}

