using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {

        TcpClient tcpClient;
        StreamReader reader;
        StreamWriter writer;
        bool joined;
        string channelName, userName, password, chatMessagePrefix, chatCommandId;
        string playlist, followMessage, modlist, specs;
        string botMessage = "Bot has responded to a command";


        public Form1()
        {
            InitializeComponent();
            Reconnect();

            

        }

        private void Reconnect()
        {
            tcpClient = new TcpClient("irc.twitch.tv", 6667);
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());
            using (StreamReader sr = new StreamReader("oauth.txt")) {
                this.password = sr.ReadToEnd();
            }
            JoinChannel();
            joined = false;
           


        }
        private void JoinChannel() {
            using (StreamReader sr = new StreamReader("channelName.txt"))
            {
                this.channelName = sr.ReadToEnd();
            }

            this.userName = "uni__bot";


            chatMessagePrefix = $":{userName}!{userName}@{userName}.tmi.twitch.tv PRIVMSG #{channelName} :";
            chatCommandId = "PRIVMSG";
            //login
            writer.WriteLine("PASS " + password + Environment.NewLine + "NICK " + userName + Environment.NewLine + "USER " + userName + " 8* :" + userName);
            //clear
            writer.Flush();
            //Join room
            writer.WriteLine("CAP REQ :twitch.tv/membership");
            writer.WriteLine("JOIN #" + channelName);

            writer.Flush();
        }
        private void ChangeRoom(string channelName) {
            
            //WIP
            
            writer.WriteLine("JOIN #" + channelName);

            writer.Flush();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //If not connected, connect
            if (!tcpClient.Connected)
            {
                Reconnect();
            }

            //If data is available, Display it
            if (tcpClient.Available > 0 || reader.Peek() >= 0)
            {
                var message = reader.ReadLine();

                var iCollon = message.IndexOf(":", 1);
                if (iCollon > 0)
                {
                    var command = message.Substring(1, iCollon);
                    if (command.Contains(chatCommandId))
                    {
                        var Excl = command.IndexOf("!");
                        if (Excl > 0)
                        {
                            var speaker = command.Substring(0, Excl);

                            var chatMessage = message.Substring(iCollon + 1);

                            ReceiveMessage(speaker, chatMessage);
                        }
                    }

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Chat();

            //WIP
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Reconnect();
            //WIP
        }

        private void button3_Click(object sender, EventArgs e)
        {
            channelName = channelBox.Text;
            ChangeRoom(channelName);
            channelBox.Text = "";
        }

        void ReceiveMessage(string speaker, string message) {
           
            testBox.AppendText ($"\r\n{speaker}: {message}");
            ChatCommands(speaker, message);
            SpookyWords(speaker, message);

        }
        void ChatCommands(string speaker, string message) {
            


            //!hi. Says hello to speaker
            if (message.StartsWith("!hi"))
            {
                Thread.Sleep(50);
                SendMessage($"Hello, {speaker}");
                
            }
            //!follow. Types a follower message. Editable in a text file.
            else if (message.StartsWith("!follow"))
            {
                Thread.Sleep(50);
                using (StreamReader sr = new StreamReader("followerMessage.txt"))
                {
                    this.followMessage = sr.ReadToEnd();
                }
                SendMessage($"{followMessage}");
                
            }
            //!playlist. Pastes link to music playlist. Editable in a text file.
            else if (message.StartsWith("!playlist"))
            {
                Thread.Sleep(50);
                using (StreamReader sr = new StreamReader("playlistLink.txt"))
                {
                    this.playlist = sr.ReadToEnd();
                }
                SendMessage($"{speaker}, here is my playlist: {playlist}");
                
            }
            else if (message.StartsWith("!roulette"))
            {
                Thread.Sleep(50);
                SendMessage($"/me places a gun next to {speaker}'s head.");
                Random rng = new Random();
                double chance = rng.Next(1, 100);
                double finalChance = Math.Floor(chance);
                Thread.Sleep(5000);
                if (finalChance < 17)
                {

                    SendMessage($"/timeout {speaker} 5");
                    SendMessage($"{speaker} Has been shot dead in chat. Press F to pay respects");
                    Thread.Sleep(700);
                    SendMessage("F");
                }
                else if (speaker == "purple__stealth")
                {
                    SendMessage($"/timeout {speaker} 5");
                    SendMessage($"{speaker} Has been shot dead in chat. Press F to pay respects");
                    Thread.Sleep(700);
                    SendMessage("F");
                }
                else
                {
                    SendMessage($"No bullet fires and {speaker} survives. Have a cookie :^)");
                }

            }
            else if (message.StartsWith("!mods"))
            {
                Thread.Sleep(50);
                using (StreamReader sr = new StreamReader("modlist.txt"))
                {
                    this.modlist = sr.ReadToEnd();
                }
                SendMessage($"Currently, my mods are: {modlist}.");


            }
            else if (message.StartsWith("!specs"))
            {
                Thread.Sleep(50);
                using (StreamReader sr = new StreamReader("specs.txt"))
                {
                    this.specs = sr.ReadToEnd();
                }
                SendMessage($"Here are my specs: {specs}");
            }
            else if (message.StartsWith("!stfu"))
            {
                Thread.Sleep(50);
                SendMessage($"STFU {speaker} and drink bleach");
            }
            else if (message.StartsWith("!commands")) {
                Thread.Sleep(50);
                SendMessage("Commands are: !hi, !follow, !playlist, !specs, !roulette, and !stfu");
            }
        }
        void SpookyWords(string speaker, string message) {
            string lowerMessage = message.ToLower();
            //TODO Make a "wordlist"
            if (lowerMessage.Contains("fag")) {
                Thread.Sleep(50);
                SendMessage($"Thats a bad word {speaker}. Please don't do it again");
                SendMessage($"/timeout {speaker} 1");

            }

        }
        void SendMessage(string message) {
            writer.WriteLine($"{chatMessagePrefix}{message}");
            writer.Flush();
            

           
        }

        void Chat() {
            string chatMessage = chatBox.Text;
            SendMessage(chatMessage);
            chatBox.Text = "";
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void chatbox_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)13) {
                
                Chat();
            }
        }
    }
}
