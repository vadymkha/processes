using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace WindowsFormsApp12
{
    public partial class Form1 : Form
    {
        Semaphore s;
        List<Thread> threadsCreated = new List<Thread>();
        List<Thread> threadsReady = new List<Thread>();
        List<Thread> threadsExecuted = new List<Thread>();
        int numWorkingThreads = 0;int j = 0;
        public Form1()
        {
            InitializeComponent();
           // numericUpDown1.Value = 1;
            numWorkingThreads = (int)numericUpDown1.Value;
            s = new Semaphore(numWorkingThreads, numWorkingThreads);
        }

        private void listBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int ind = listBox3.SelectedIndex;

            listBox3.Items.RemoveAt(ind);
            threadsReady.Add(threadsCreated[ind]);
            threadsCreated.RemoveAt(ind);
            listBox2.Items.Add("thread " + threadsReady[threadsReady.Count - 1].ManagedThreadId + " is ready");
            GoToWork();
        }
        public void GoToWork()
        {
            numWorkingThreads = (int)numericUpDown1.Value;
            if (j < numWorkingThreads&&listBox2.Items.Count!=0)
            {
                listBox2.Items.RemoveAt(0);
                threadsExecuted.Add(threadsReady[0]);
                threadsReady.RemoveAt(0);
                listBox1.Items.Add("thread " + threadsExecuted[threadsExecuted.Count - 1].ManagedThreadId + " is working");
                if (threadsExecuted[threadsExecuted.Count - 1].ThreadState == ThreadState.Aborted)
                    threadsExecuted[threadsExecuted.Count - 1].Resume();
                else
                threadsExecuted[threadsExecuted.Count - 1].Start();
                j++;
            }
            else return;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(incCounter);

            threadsCreated.Add(t);
            listBox3.Items.Add("thread " + t.ManagedThreadId + " was created");
        }
        public void incCounter()
        {
           

           

            if (s.WaitOne())
            {
                for (int i = 0; i < 10; i++)
                {
                    //listBox1.Items.RemoveAt(j-1);
                    //listBox1.Items.Add("thread " + Thread.CurrentThread.ManagedThreadId + " is working" );
                  if(Thread.CurrentThread.ThreadState!= ThreadState.Aborted)
                    Thread.Sleep(1000);
                  else
                        s.Release();
                }
                s.Release();
                threadsExecuted.RemoveAt(0);
                listBox1.Items.RemoveAt(0);
                j--;
                GoToWork();
            }

        }

        private void listBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //int ind = listBox2.SelectedIndex;

            //listBox2.Items.RemoveAt(ind);
            //threadsExecuted.Add(threadsReady[ind]);
            //threadsReady.RemoveAt(ind);
            //listBox1.Items.Add("thread " + threadsExecuted[threadsExecuted.Count - 1].ManagedThreadId + " is working");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GoToWork();
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int ind = listBox1.SelectedIndex;
            threadsExecuted[ind].Abort();
            listBox1.Items.RemoveAt(ind);
            threadsReady.Add(threadsExecuted[ind]);
            threadsExecuted.RemoveAt(ind);
            listBox2.Items.Add("thread " + threadsReady[threadsReady.Count - 1].ManagedThreadId + " is ready");
            j--;
            GoToWork();
        }
    }
}
