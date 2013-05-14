/*
 * Form1.cs
 * 
 * A GUI that communicates with a Touch Screen Device over the USB.
 * Also Recognizes Kannada Characters drawn on the Touch Screen and displays it.
 * 
 * USB Communication done with the help of (c)Simon Inns's USB Framework, distributed 
 * under the GNU General Public License.
 * 
 * Functions made use of for USB communication:
 * 1.bool writeRawReportToDevice(Byte[])
 * 2.bool readSingleReportFromDevice(Byte[])
 * 
 * Written By: Jeevan B S
 * Project:    Touch Screen Input Device for Kannada Language Script
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace myFirstApp
{
    using usbGenericHidCommunications;

    public partial class Form1 : Form
    {
        
        string winDir = System.Environment.GetEnvironmentVariable("windir");
        private void addListItem(string value)
        {
            this.listBox1.Items.Add(value);
        }

        private void addWrite(Byte[] coor, int index)
        {
            StreamWriter writer = new StreamWriter("c:\\Users\\wind\\Desktop\\KBTest.txt");
            writer.WriteLine("File created using StreamWriter class");
            writer.WriteLine(coor[index]);
        }

        
        public Form1()
        {
            
            InitializeComponent();

            // Create the USB reference device object (passing VID and PID)
            theReferenceUsbDevice = new usbReferenceDevice(0x04D8, 0x0080);

            // Add a listener for usb events
            theReferenceUsbDevice.usbEvent +=
              new usbReferenceDevice.usbEventsHandler(usbEvent_receiver);

            // Perform an initial search for the target device
            theReferenceUsbDevice.findTargetDevice();
        }

        class usbReferenceDevice : usbGenericHidCommunication
        {
            string temp, temp1;
            int a, b = 0, i = 0;
            // Class constructor - place any initialisation here
            public usbReferenceDevice(int vid, int pid)
                : base(vid, pid)
            {
            }

            public bool test1()
            {
                // Test 1 - Send a single write packet to the USB device

                // Declare our output buffer
                Byte[] outputBuffer = new Byte[65];

                // Byte 0 must be set to 0
                outputBuffer[0] = 0;

                // Byte 1 must be set to our command
                outputBuffer[1] = 0x80;

                // Fill the rest of the buffer with known data
                int bufferPointer;
                Byte data = 0;
                for (bufferPointer = 2; bufferPointer < 65; bufferPointer++)
                {
                    // We send the numbers 0 to 63 to the device
                    outputBuffer[bufferPointer] = data;
                    data++;
                }

                // Perform the write command
                bool success;
                success = writeRawReportToDevice(outputBuffer);

                // We can't tell if the device received the data ok, we are
                // only indicating that the write was error free.
                return success;
            }

            public Byte[] test2()
            {
                // Test 1 - Send a single write packet to the USB device

                // Declare our output buffer
                Byte[] outputBuffer = new Byte[65];

                // Byte 0 must be set to 0
                outputBuffer[0] = 0;

                // Byte 1 must be set to our command
                outputBuffer[1] = 0x81;

                // Fill the rest of the buffer with known data
                int bufferPointer;
                Byte data = 0;
                for (bufferPointer = 2; bufferPointer < 65; bufferPointer++)
                {
                    // We send the numbers 0 to 63 to the device
                    outputBuffer[bufferPointer] = data;
                    data++;
                }

                // Perform the write command
                bool success, success1;
                success1 = writeRawReportToDevice(outputBuffer);
                success = readSingleReportFromDevice(ref outputBuffer);
               
                StreamWriter writer = new StreamWriter("c:\\Users\\wind\\Desktop\\KBTest.txt", true);
                int o1 = Convert.ToInt32(outputBuffer[1]);
                int o2 = Convert.ToInt32(outputBuffer[2]);
                if (outputBuffer[1] > 0)
                    o2 = 255 + o2;

                writer.WriteLine(o2);
                writer.Close();

                
                // We can't tell if the device received the data ok, we are
                // only indicating that the write was error free.
                return outputBuffer;
                
            }

            public void prepareCompFile(string iFile, string iSignFile, string iVathaksharaFile)
            {                               
                string addresscPatternFile = "c:\\Users\\wind\\Desktop\\cPattern\\";
                addresscPatternFile = addresscPatternFile + iFile + ".txt";
                StreamReader cPatternFile = new StreamReader(addresscPatternFile);

                string addressSignFile = "c:\\Users\\wind\\Desktop\\cPattern\\signs\\";
                iSignFile = addressSignFile + iSignFile + ".txt";
                StreamReader SignFile = new StreamReader(iSignFile);
                

                string addressVathaksharaFile = "c:\\Users\\wind\\Desktop\\cPattern\\vathakshara\\";
                addressVathaksharaFile = addressVathaksharaFile + iVathaksharaFile + ".txt";
                StreamReader vathaksharaFile = new StreamReader(addressVathaksharaFile);

                StreamWriter addressCompFile = new StreamWriter("c:\\Users\\wind\\Desktop\\compFile.txt");

                while ((temp = cPatternFile.ReadLine()) != null)
                    addressCompFile.WriteLine(temp);
                while ((temp = SignFile.ReadLine()) != null)
                    addressCompFile.WriteLine(temp);
                while ((temp = vathaksharaFile.ReadLine()) != null)
                    addressCompFile.WriteLine(temp);

                SignFile.Close();
                addressCompFile.Close();
                cPatternFile.Close();
                vathaksharaFile.Close();
            }

            public string recognizeCharacter(string in_cPattern)
            {                
                string cHold, cHold1;
                string recChar = "";
                StreamReader indexSign = new StreamReader("c:\\Users\\wind\\Desktop\\cPattern\\signs\\index.txt");
                


                string addressOutFile = "c:\\Users\\wind\\Desktop\\out.txt";
                

                for (int i = 0; i < 17; i++)
                {
                    StreamReader indexVathakshara = new StreamReader("c:\\Users\\wind\\Desktop\\cPattern\\vathakshara\\index.txt");
                    
                    string sendSign = indexSign.ReadLine();

                    // Location of the Unicode Number of Signs
                    string addressSignNum = "c:\\Users\\wind\\Desktop\\cPattern\\signs\\";
                    addressSignNum = addressSignNum + sendSign + "Num.txt";
                    StreamReader SignNum = new StreamReader(addressSignNum);

                    
                    for (int j = 0; j < 13; j++)
                    {
                        string sendVathakshara = indexVathakshara.ReadLine();
                        prepareCompFile(in_cPattern, sendSign, sendVathakshara);
                        StreamReader outFile = new StreamReader(addressOutFile);
                    
                        // Location of Unicode Number of Vathaksharas
                        string addressVathaksharaNum = "c:\\Users\\wind\\Desktop\\cPattern\\";
                        addressVathaksharaNum = addressVathaksharaNum + sendVathakshara + "Num.txt";
                        StreamReader VathaksharaNum = new StreamReader(addressVathaksharaNum);

                        StreamReader compFile = new StreamReader("c:\\Users\\wind\\Desktop\\compFile.txt");
                        while (true)
                        {

                            cHold = outFile.ReadLine();
                            cHold1 = compFile.ReadLine();
                            if ((cHold == null) && (cHold1 == null))
                            {
                                if (sendVathakshara == "noPattern")
                                    recChar = giveNumber(in_cPattern) + SignNum.ReadLine();
                                else
                                    recChar = giveNumber(in_cPattern) + "\u0ccd" + VathaksharaNum.ReadLine() + SignNum.ReadLine();
                                outFile.Close();
                                compFile.Close();
                                return recChar;
                            }

                            if ((cHold == null) || (cHold1 == null))
                                break;

                            if (cHold != cHold1)
                                break;
                        }
                    compFile.Close();
                    outFile.Close();
                    
                    VathaksharaNum.Close();
                    }
                    SignNum.Close();
                    
                    indexVathakshara.Close();
                }
                
                indexSign.Close();

                return recChar;
            }

            public string giveNumber(string in_cPattern)
            {
                string numAdd = in_cPattern + "Num";
                StreamReader numRetriever = new StreamReader("c:\\Users\\wind\\Desktop\\cPattern\\" + numAdd + ".txt");
                temp = numRetriever.ReadLine();
                numRetriever.Close();
                return temp;
            }

            public void getCoordinates()
            {
                
                // Outputs 1s and -1s                
                StreamReader reader1 = new StreamReader("c:\\Users\\wind\\Desktop\\KBTest.txt");
                StreamWriter writer1 = new StreamWriter("c:\\Users\\wind\\Desktop\\KBTest1.txt", true);
                bool loopEntered = false;
                // If facing problems in the future: Look at the following while loop, used 
                // to remove 0s induced by Erasing process. Might be loosing one coordinate here
                while ((temp = reader1.ReadLine()) != null)
                {
                    a = Convert.ToInt32(temp);
                    b = Convert.ToInt32(temp1);
                    if (a < 20)
                    {
                        a = 0;
                        temp = Convert.ToString(a);
                    }
                    if (loopEntered)
                    {
                        if (((a - b) <= 5) && ((a - b) >= -5))
                        {
                            a = b;
                            temp = Convert.ToString(a);
                        }
                        else if (a == b)
                        {
                            a = b;
                            temp = Convert.ToString(a);
                        }
                        else
                            a = a;
                    }
                    writer1.WriteLine(temp);
                    temp1 = temp;
                    loopEntered = true;
                }

                /*bool loopEntered1 = false;
                while ((temp = reader1.ReadLine()) != null)
                {
                    /* writer1.Write(temp);
                     writer1.Write("     ");
                     writer1.WriteLine(temp1);*/
                    /*if (loopEntered1)
                    {
                        a = Convert.ToInt32(temp);
                        b = Convert.ToInt32(temp1);
                        int r = b - a;
                        if (r < 0)
                        {
                            r = -1;
                            writer1.WriteLine(r);
                        }
                        else if (r > 0)
                        {
                            r = 1;
                            writer1.WriteLine(r);
                        }
                        else
                            r = r;

                    }
                    temp1 = temp;
                    loopEntered1 = true;

                }*/
                reader1.Close();
                writer1.Close();

                StreamReader reader2 = new StreamReader("c:\\Users\\wind\\Desktop\\KBTest1.txt");
                StreamWriter writer2 = new StreamWriter("c:\\Users\\wind\\Desktop\\in.txt", true);
                bool loopEntered2 = false;
                int index = 1;
                while ((temp = reader2.ReadLine()) != null)
                {
                    if (loopEntered2)
                    {
                        if (temp == temp1)
                            index++;

                        if (temp != temp1)
                            index = 1;

                        if (index == 5)
                        {
                            writer2.WriteLine(temp);
                            index = 1;
                        }
                    }
                    temp1 = temp;
                    loopEntered2 = true;
                }
                reader2.Close();
                writer2.Close();

                StreamReader reader3 = new StreamReader("c:\\Users\\wind\\Desktop\\in.txt");
                StreamWriter writer3 = new StreamWriter("c:\\Users\\wind\\Desktop\\in1.txt", true);
                bool loopEntered3 = false;
                while ((temp = reader3.ReadLine()) != null)
                {
                    if (!loopEntered3)
                        writer3.WriteLine(temp);
                    /*if (!loopEntered3)
                    {
                        if (temp == "0")
                        {
                            tempWasZero = true;
                            continue;                            
                        }
                        else
                        {
                            writer3.WriteLine(temp);
                        }
                    }*/

                    if (loopEntered3)
                    {
                        if (temp == temp1)
                            continue;
                        else
                            writer3.WriteLine(temp);
                    }
                    loopEntered3 = true;
                    temp1 = temp;

                }
                reader3.Close();
                writer3.Close();

                StreamReader reader4 = new StreamReader("c:\\Users\\wind\\Desktop\\in1.txt");
                StreamWriter writer4 = new StreamWriter("c:\\Users\\wind\\Desktop\\in2.txt", true);
                bool loopEntered4 = false;
                while ((temp = reader4.ReadLine()) != null)
                {
                    if (loopEntered4)
                    {
                        a = Convert.ToInt32(temp);
                        b = Convert.ToInt32(temp1);
                        
                        if (temp1 == "0")
                        {
                            writer4.WriteLine(temp1);
                            temp1 = temp;                            
                            continue;
                        }

                        if (temp == "0")
                        {
                            writer4.WriteLine(temp);
                            temp1 = temp;                           
                            continue;
                        }

                        if ((a - b) > 0)
                        {
                            writer4.WriteLine(" 1");
                        }
                        else if ((a - b) < 0)
                        {
                            writer4.WriteLine("-1");
                        }
                    }
                    loopEntered4 = true;
                    temp1 = temp;
                }
                reader4.Close();
                writer4.Close();

                StreamReader reader5 = new StreamReader("c:\\Users\\wind\\Desktop\\in2.txt");
                StreamWriter writer5 = new StreamWriter("c:\\Users\\wind\\Desktop\\out.txt", true);
                bool loopEntered5 = false;
                while ((temp = reader5.ReadLine()) != null)
                {
                    if (!loopEntered5)
                        writer5.WriteLine(temp);

                    if (loopEntered5)
                    {
                        if (temp != temp1)
                            writer5.WriteLine(temp);
                    }
                    loopEntered5 = true;
                    temp1 = temp;
                }
                writer5.Close();
                reader5.Close();
            }
        }
        // Test button 1 clicked
        private void button1_Click(object sender, EventArgs e)
        {
            if (theReferenceUsbDevice.test1()) this.label1.Text = "Test passed";
            else this.label1.Text = "Test failed";
        }

        
        /*private void button2_Click(object sender, EventArgs e)
        {
            if (theReferenceUsbDevice.test2()) this.label2.Text = "Test passed";
            else this.label2.Text = "Test failed";
        }*/
        private void button2_Click(object sender, EventArgs e)
        {
            Byte[] m = new Byte[65];
            m = theReferenceUsbDevice.test2();
            /*StreamWriter writer = new StreamWriter("c:\\Users\\wind\\Desktop\\KBTest.txt", true);
            Stopwatch drawingWindow = new Stopwatch();
            Stopwatch filterCoordinates = new Stopwatch();
            drawingWindow.Start();
            while (drawingWindow.ElapsedMilliseconds < 1000)
            {
                filterCoordinates.Start();
                while (filterCoordinates.ElapsedMilliseconds < 53)
                {
                    if (filterCoordinates.ElapsedMilliseconds > 45)
                    {
                        m = theReferenceUsbDevice.test2();
                        writer.WriteLine(m[1]);
                    }
                }
                filterCoordinates.Stop();
            }
            writer.Close();
            drawingWindow.Stop();*/
            this.label2.Text = m[1].ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Byte[] n = new Byte[65];
            n = theReferenceUsbDevice.test2();
            int oi1 = Convert.ToInt32(n[1]);
            int oi2 = Convert.ToInt32(n[2]);
            if (n[1] > 0)
                oi2 = 255 + oi2;
            this.label3.Text = oi2.ToString();
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            theReferenceUsbDevice.getCoordinates();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StreamWriter eraser1 = new StreamWriter("c:\\Users\\wind\\Desktop\\KBTest.txt");
            eraser1.WriteLine(0);
            eraser1.Close();

            StreamWriter eraser2 = new StreamWriter("c:\\Users\\wind\\Desktop\\KBTest1.txt");
            eraser2.WriteLine(0);
            eraser2.Close();

            StreamWriter eraser3 = new StreamWriter("c:\\Users\\wind\\Desktop\\in.txt");
            eraser3.WriteLine(0);
            eraser3.Close();

            StreamWriter eraser4 = new StreamWriter("c:\\Users\\wind\\Desktop\\in1.txt");
            eraser4.WriteLine(0);
            eraser4.Close();

            StreamWriter eraser5 = new StreamWriter("c:\\Users\\wind\\Desktop\\in2.txt");
            eraser5.WriteLine(0);
            eraser5.Close();

            StreamWriter eraser6 = new StreamWriter("c:\\Users\\wind\\Desktop\\out.txt");
            eraser6.WriteLine(0);
            eraser6.Close();

        }

        // Create an instance of the USB reference device
        private usbReferenceDevice theReferenceUsbDevice;

        // Listener for USB events
        private void usbEvent_receiver(object o, EventArgs e)
        {
            this.button1.Enabled = true;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string recognized = "";
            string in_cPattern = "";
            StreamReader indexFile = new StreamReader("c:\\Users\\wind\\Desktop\\cPattern\\index.txt");
            for (int i = 0; i < 45; i++)
            {
                in_cPattern = indexFile.ReadLine();
                recognized = theReferenceUsbDevice.recognizeCharacter(in_cPattern);
                if (recognized != "")
                    break;
            }
            
            this.richTextBox1.Text = this.richTextBox1.Text + recognized;
            indexFile.Close();
        }
    }
}
