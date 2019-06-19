//
// EnumQueues.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2006-2010 Dino Chiesa.
// All rights reserved.
//
// This code module is part of MsmqJava, a JNI library that provides
// access to MSMQ for Java on Windows.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License.
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// last saved (in emacs):
// Time-stamp: <2010-March-27 10:34:20>
//
// ------------------------------------------------------------------
//
// This module is just a simple utility app used during testing, to
// enumerate the queues.
//
// ------------------------------------------------------------------


using System;
using System.Messaging;

namespace Ionic.Msmq
{
    public class TestDriver
    {
        static void Main(String[] args)
        {
            try
            {
                string machineName= ".";
                System.Console.WriteLine("\nEnumerating the private queues on machine ({0})...\n", machineName);

                // get the list of message queues
                MessageQueue[] MQList = MessageQueue.GetPrivateQueuesByMachine(machineName);

                // check to make sure we found some private queues on that machine
                if (MQList.Length >0)
                {

                    string[,] MQNameList = new string[MQList.Length, 4];

                    // loop through all message queues and get the name, path, etc.
                    for (int i = 0; i < MQList.Length; i++)
                    {
                        try {
                            MQNameList[i,0]= MQList[i].QueueName;
                        }
                        catch {
                            MQNameList[i,0]= "???";
                        }
                        try {
                            MQNameList[i,1]= MQList[i].Label;
                        }
                        catch {
                            MQNameList[i,1]= "???";
                        }
                        try {
                            MQNameList[i,2]= MQList[i].Transactional.ToString();
                        }
                        catch {
                            MQNameList[i,2]= "???";
                        }
                        try {
                            MQNameList[i,3]= MQList[i].FormatName;
                        }
                        catch {
                            MQNameList[i,3]= "???";
                        }

                        System.Console.WriteLine("{0} '{1}'  {2}  {3}",
                                                 MQNameList[i,0],
                                                 MQNameList[i,1],
                                                 MQNameList[i,2],
                                                 MQNameList[i,3]);
                    }
                }

            }
            catch (System.Exception exc1)
            {
                System.Console.WriteLine("Exception:\n" + exc1);
            }
        }
    }

}
