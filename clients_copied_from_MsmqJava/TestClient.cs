//
// TestClient.cs
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
// Time-stamp: <2010-March-27 15:36:17>
//
// ------------------------------------------------------------------
//
// This module is just a simple utility app used during testing, to
// put and get messages onto a queue.
//
// ------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Messaging;

namespace Ionic.Msmq
{
    public class TestClient
    {
        MessageQueue myQueue= null;

        string ipAddr= "?";

        public TestClient() {
            string strHostName = System.Net.Dns.GetHostName ();
            System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry (strHostName);
            ipAddr= ipEntry.AddressList[0].ToString();
        }

        public static void Main(string[] args) {
            TestClient tc= new TestClient() ;
            tc.Run();
        }

        private void Create()
        {
            try
            {
                String qname= getLine("\nName of Queue to create? ");
                String hostname= getLine("\nName of Queue Server (ENTER for local)? ", false);
                //String fullname= getQueueFullName(hostname,qname);
                if ((hostname==null) || (hostname=="")) hostname=".";

                String fullname= hostname + "\\private$\\" + qname;
                Console.WriteLine("create ({0})", fullname);
                myQueue= MessageQueue.Create(fullname);
                myQueue.Formatter = new StringMessageFormatter();
                Console.WriteLine("Create: OK.");
            }
            catch (Exception ex1) {
                Console.WriteLine("Queue creation failure: {0}", ex1.Message);
            }
        }

        private void Delete()
        {
            try
            {
                String qname= getLine("\nName of Queue to Delete? ");
                String hostname= getLine("\nName of Queue Server (ENTER for local)? ", false);
                String fullname= getQueueFullName(hostname,qname);
                Console.WriteLine("delete ({0})", fullname);
                MessageQueue.Delete(fullname);
                Console.WriteLine("delete: OK.");
            }
            catch (Exception ex1) {
                Console.WriteLine("Queue deletion failure: {0}", ex1.Message);
            }
        }


        private void Open()
        {
            try
            {
                if (myQueue!=null) {
                    myQueue.Close();
                    myQueue= null;
                }
                String qname= getLine("\nName of Queue to open? ");
                String hostname= getLine("\nName of Queue Server (ENTER for local)? ", false);
                String fullname= getQueueFullName(hostname,qname);
                Console.WriteLine("open ({0})", fullname);
                myQueue= new MessageQueue(fullname, false);
                myQueue.Formatter = new StringMessageFormatter();
                Console.WriteLine("open: OK.");
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Queue open failure: {0}", ex1.Message);
            }
        }


        private void Close()
        {
            if (myQueue!=null) {
                try {
                    Console.WriteLine("close");
                    myQueue.Close();
                    myQueue= null;
                    Console.WriteLine("close: OK.");
                }
                catch (MessageQueueException ex1) {
                    Console.WriteLine("close failure: {0}", ex1);
                }
            }
        }

        private void Send()
        {
            string s= String.Format("[from:{0} [time:{1}] [ip:{2}]",
                                    ".NET",
                                    System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:sszzz"),
                                    ipAddr);

            try {
                Console.WriteLine("Send ({0})", s);
                // TODO: resolve this
                // Console.WriteLine("Do I need the MessageFormatter?  maybe can just send string?");
                Message msg = new Message(s, new StringMessageFormatter());
                msg.Label= "inserted by TestClient (.NET)";
                myQueue.Send(msg); // s
            }
            catch (Exception ex1) {
                Console.WriteLine("Send failure: " + ex1.Message);
            }
        }


        private void Receive()
        {
            try
            {
                Console.WriteLine("Receive...");
                Message msg= myQueue.Receive(new TimeSpan(0,0,2));  // wait 2 seconds
                Console.WriteLine(" ==> message: " + (string) msg.Body);
                Console.WriteLine("     label:   " + (string) msg.Label);
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Receive failure: " + ex1.Message);
            }
        }

        private void Peek()
        {
            try
            {
                Console.WriteLine("Peek...");
                Message msg= myQueue.Peek(new TimeSpan(0,0,2));  // wait 2 seconds
                Console.WriteLine(" ==> message: " + (string) msg.Body);
                Console.WriteLine("     label:   " + (string) msg.Label);
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Peek failure: " + ex1.Message);
            }
        }


        private String getQueueFullName( String queueShortName )
        {
            return getQueueFullName(".", queueShortName);
        }

        private String getQueueFullName( String hostname, String queueShortName )
        {
            String h1= hostname;
            if ((h1==null) || (h1=="")) h1=".";
            return "FormatName:DIRECT=OS:" + h1 + "\\private$\\" + queueShortName;
        }

        private string getLine(String prompt)
        {
            return getLine(prompt, true);
        }

        private string getLine(String prompt, bool discardPending)
        {
            if (discardPending)
                Console.ReadLine();  // discard the pending <ENTER>
            Console.Write(prompt);
            String s= Console.ReadLine();
            //Console.WriteLine();
            return s;
        }

        private void Info()
        {
            System.Console.Write("\nHello from the .NET MSMQ Test Client.\n");
            System.Reflection.Assembly caller= System.Reflection.Assembly.GetExecutingAssembly();
            string[]  props1=
            {
                "System Name: " + System.Net.Dns.GetHostName(),
                "Assembly: " + caller.GetName().Name + ", Version " + caller.GetName().Version ,
                "CLR: "  + System.Environment.Version.ToString(),
                "Running on " + System.Environment.OSVersion.ToString(),
            };

            for(int i=0; i < props1.Length; i++) {
                System.Console.WriteLine("  {0}", props1[i]);
            }

        }

        public void Run()
        {
            try
            {
                System.Console.Write("\nHello from the .NET MSMQ Test Client.\n");

                int state=1;
                do
                {
                    // only prompt after processing a command.
                    if (state==1) {
                        System.Console.Write("\n[C]reate [O]pen [R]eceive [P]eek [S]end [X]close [D]elete [I]nfo or [Q]uit?\n==> ");
                        state=0;
                    }
                    int c= Console.Read(); // read one char at a time
                    if ((c=='C')||(c=='c')) { Create();state=1;}
                    else if ((c=='D')||(c=='d')) { Delete(); state=1;}
                    else if ((c=='O')||(c=='o')) { Open(); state=1; }
                    else if ((c=='R')||(c=='r')) Receive();
                    else if ((c=='S')||(c=='s')) Send();
                    else if ((c=='I')||(c=='i')) Info();
                    else if ((c=='X')||(c=='x')) Close();
                    else if ((c=='P')||(c=='p')) Peek();
                    else if ((c=='Q')||(c=='q')) state=99;
                    else if (c==13) state= 1;  // ENTER
                } while (state!=99);

                Console.WriteLine("\n");
            }

            catch (Exception ex1)
            {
                Console.WriteLine("Exception: " + ex1);
            }

        }



        public class StringMessageFormatter : IMessageFormatter
        {
            public readonly Encoding encoding = System.Text.Encoding.GetEncoding("UTF-16");

            public StringMessageFormatter() { }
            public object Clone() { return new StringMessageFormatter();  }
            public bool CanRead (System.Messaging.Message oMessage){ return true;}

            public void Write (System.Messaging.Message m, object o)
            {
                try
                {
                    byte[] buffer = encoding.GetBytes (o.ToString());
                    m.BodyStream = new MemoryStream(buffer);
                }
                catch (Exception ex1) {
                    Console.WriteLine("Exception writing: " + ex1);
                }
            }

            public object Read (System.Messaging.Message message)
            {
                try
                {
                    using (StreamReader reader = new StreamReader (message.BodyStream, encoding))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (Exception ex1)
                {
                    Console.WriteLine("Exception reading: " + ex1);
                }

                return null;
            }

        }
    }
}
