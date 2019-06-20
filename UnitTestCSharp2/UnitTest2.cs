using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Messaging;

//Most code from TestClient.cs of sourceCode of Msmqjava - https://archive.codeplex.com/?p=MsmqJava
//(not MsmqJava-master whid is this dll project)
namespace UnitTestCSharp2
{
    [TestClass]
    public class UnitTest2
    {
        MessageQueue myQueue = null;
        string ipAddr = "?";

        [TestMethod]
        public void TestOpenQueue2()
        {
            try
            {
                if (myQueue != null)
                {
                    myQueue.Close();
                    myQueue = null;
                }
                //String qname = getLine("\nName of Queue to open? ");
                String qname = "q_by_j";
                //String hostname = getLine("\nName of Queue Server (ENTER for local)? ", false);
                String hostname = "tsd-t019004";
                //String hostname = "local";
                String fullname = getQueueFullName(hostname, qname);
                Console.WriteLine("open ({0})", fullname);
                myQueue = new MessageQueue(fullname, false);
                myQueue.Formatter = new StringMessageFormatter();
                Console.WriteLine("open: OK. UnitTestCSharp2");
                System.Threading.Thread.Sleep(5000);
                Send();
                System.Threading.Thread.Sleep(5000);
                Peek();
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Queue open failure: {0}", ex1.Message);
            }
        }

        public class StringMessageFormatter : IMessageFormatter
        {
            public readonly Encoding encoding = System.Text.Encoding.GetEncoding("UTF-16");

            public StringMessageFormatter() { }
            public object Clone() { return new StringMessageFormatter(); }
            public bool CanRead(System.Messaging.Message oMessage) { return true; }

            public void Write(System.Messaging.Message m, object o)
            {
                try
                {
                    byte[] buffer = encoding.GetBytes(o.ToString());
                    m.BodyStream = new MemoryStream(buffer);
                }
                catch (Exception ex1)
                {
                    Console.WriteLine("Exception writing: " + ex1);
                }
            }

            public object Read(System.Messaging.Message message)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(message.BodyStream, encoding))
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

        private String getQueueFullName(String queueShortName)
        {
            return getQueueFullName(".", queueShortName);
        }

        private String getQueueFullName(String hostname, String queueShortName)
        {
            String h1 = hostname;
            if ((h1 == null) || (h1 == "")) h1 = ".";
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
            String s = Console.ReadLine();
            //Console.WriteLine();
            return s;
        }

        private void Peek()
        {
            try
            {
                Console.WriteLine("Peek...");
                Message msg = myQueue.Peek(new TimeSpan(0, 0, 2));  // wait 2 seconds
                Console.WriteLine(" ==> message: " + (string)msg.Body);
                Console.WriteLine("     label:   " + (string)msg.Label);
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Peek failure: " + ex1.Message);
            }
        }


        private void Send()
        {
            string s = String.Format("[from:{0} [time:{1}] [ip:{2}]",
                                    ".NET",
                                    System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:sszzz"),
                                    ipAddr);

            try
            {
                Console.WriteLine("Send ({0})", s);
                // TODO: resolve this
                // Console.WriteLine("Do I need the MessageFormatter?  maybe can just send string?");
                Message msg = new Message(s, new StringMessageFormatter());
                msg.Label = "inserted by TestClient (.NET)";
                myQueue.Send(msg); // s
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Send failure: " + ex1.Message);
            }
        }

    }
}
