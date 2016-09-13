using Sulakore.Communication;
using Sulakore.Modules;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangine;

namespace UnfilterExample
{
    [Module("Unfilter example", "Unfilter words in messages")]
    [Author("Mika")]
    public partial class Form1 : ExtensionForm
    {
        public Form1()
        {
            //Initialize the form components
            InitializeComponent();

            //Attach to the speech method to receive packets
            Triggers.OutAttach(1234, OutgoingSpeechReceived);
        }

        public void OutgoingSpeechReceived(DataInterceptedEventArgs e)
        {
            //Read the message from the speech packet
            string message = e.Packet.ReadString();

            //Read the color/theme integer from the speech packet
            int color = e.Packet.ReadInteger();

            //Read another irrelevant integer, which we replace with 0 later on
            e.Packet.ReadInteger();

            //Replace the packet with a new one that contains our unfiltered message
            e.Packet = new HMessage(e.Packet.Header, message.Unfilter(BobbaWords, "&#122;"), color, 0);
        }

        //This here is the list of words you want to provide with the Unfilter() method
        //You can add as many words as you want in here
        public static List<string> BobbaWords = new List<string>
    {
        "whore",
        "slut",
        "cunt",
        "ass",
        "weed",
        "fucking",
        "bitch"
    };
    }

    public static class Extension
    {
        private static string UnfilterWord(string word, string replacement)
        {
            //Divide the word length by two
            int half = word.Length / 2;

            //Split the word in half, insert the unfilter string, then return the unfiltered word
            return $"{word.Substring(0, half)}{replacement}{word.Substring(half - 1).Remove(0, 1)}";
        }

        public static string Unfilter(this string message, List<string> filtered, string replacement)
        {
            //Loop through all words we want to unfilter
            foreach (string s in filtered)
            {
                //Check if the message contains the current word
                if (message.ToLower().Contains(s))
                {
                    //Keep unfiltering as long as the message contains the current word
                    while (message.ToLower().Contains(s))
                    {
                        //Find where in the message the word starts
                        int index = message.ToLower().IndexOf(s);

                        //Insert the unfilter string in the middle of our word
                        string unfiltered = UnfilterWord(message.Substring(index, s.Length), replacement);

                        //Remove the old word and insert our new, unfiltered word
                        message = message.Remove(index, s.Length).Insert(index, unfiltered);
                    }
                }
            }

            //Return the new message with all words unfiltered
            return message;
        }
    }
}
