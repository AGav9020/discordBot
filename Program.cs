// this has been edited to remove all private info
// 

using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBotCntySupport.Properties;
using System.IO;

namespace DiscordBotCntySupport
{
    class Program
    {

        private readonly DiscordSocketClient _client;

        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task MainAsync()
        {
            // Tokens should be considered secret data, and never hard-coded.
            await _client.LoginAsync(TokenType.Bot, Resources.AuthToken);// Environment.GetEnvironmentVariable("token"));
            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        public async Task AnnounceJoinedUser(SocketGuildUser user) //Welcomes the new user
        {
            var channel = _client.GetChannel(channelNumberID) as SocketTextChannel; // Gets the channel to send the message in

            await channel.SendMessageAsync($"Welcome {user.Mention} to {channel.Guild.Name}"); //Welcomes the new user
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("pong!");

            if (message.Content == "!hello")            
                await subHello( message);
           

            if (message.Content == "!page")
                await subPageHelp(message);


            if (message.Content == "!resume")
                await subResumeSend(message);


        }

        private async Task subResumeSend(SocketMessage Discordmessage)
        {
            try
            {
                System.Net.Mail.Attachment attMail;
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("someEmailThatExists@roadrunner.com");
                message.To.Add(new MailAddress("anotherEmailThatExists@roadrunner.com"));
           
                message.Subject = "Discord Bot Sending you a Resume To Review";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = "Hello from Your friendly CntySupport. " + Discordmessage.Author.Username + " is sending you a resume";
             
                  foreach(Discord.Attachment att in Discordmessage.Attachments)// (int i = 0; i < Discordmessage.Attachments.Count; i++)
                {
                    Console.WriteLine(att.Url);
                 
                  attMail = new System.Net.Mail.Attachment(fileRead(att.Url), att.Filename);
                     
                    message.Attachments.Add(attMail);
                        }
                

                 
                smtp.Port = 587;
                smtp.Host = "smtp.roadrunner.com"; //for roadrunner host  
                smtp.EnableSsl = false;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("anotherEmailThatExists@roadrunner.com", "thePassword");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);

            }
            catch (Exception e)
            { await Discord.UserExtensions.SendMessageAsync(Discordmessage.Author, e.Message.ToString()); }
            //await Discord.UserExtensions.SendMessageAsync(message.Author, WelcomeMessage); //This is a direct message
        }

         Stream fileRead(string url)
        {
           
            HttpWebRequest aRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse aResponse = (HttpWebResponse)aRequest.GetResponse();
             return aResponse.GetResponseStream();

        }

        private async Task subPageHelp(SocketMessage Discordmessage)
        {
            try
            { 
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress("anotherEmailThatExists@roadrunner.com");
            message.To.Add(new MailAddress("aPhoneNumberWithVerizon@vtext.com"));
             
            message.Subject = "Discord Page";
            message.IsBodyHtml = true; //to make message body as html  
            message.Body = "Hello from Discord Bot. " + Discordmessage.Author.Username + " Is pinging you";
            smtp.Port = 587;
            smtp.Host = "smtp.roadrunner.com"; //for gmail host  
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("anotherEmailThatExists@roadrunner.com", "thePassword");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
                
            }
            catch (Exception e)
            { await Discord.UserExtensions.SendMessageAsync(Discordmessage.Author, e.Message.ToString()); }
            //await Discord.UserExtensions.SendMessageAsync(message.Author, WelcomeMessage); //This is a direct message
        }

        private async Task subHello(SocketMessage message)
        {
            string WelcomeMessage = Resources.WelcomeInfo.Replace("%%NAME%%", message.Author.Mention);
                
            //await message.Channel.SendMessageAsync(WelcomeMessage);//THis sends it to the board
          
            await Discord.UserExtensions.SendMessageAsync(message.Author,WelcomeMessage); //This is a direct message
        }

    }
}
