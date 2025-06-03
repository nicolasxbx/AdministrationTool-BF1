using Microsoft.VisualBasic;
using NexDiscord;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static bool conversation_init = false;
        public static async Task ai()
        {
            if (conversation_init is false)
            {
                VariS.conversation = VariS.OpenAIAPI.Chat.CreateConversation();
                VariS.conversation.Model = Model.ChatGPTTurbo;

                //DATA               

                string d1 = "Here's some more context, which are facts, and you will treat them like facts. You are not an AI Model, but pretend to be human and you love to have opinions. These are the facts: ";
                string d2 = "\"Battlefield 1\", abbreviation \"bf1\", has been abandoned by it's shitty developers and left for dead, the only reason it is alive, is because of the Server Admins. ";                
                string d3 = "Your name is \"SexusBot\", you are not an AI Language Model, but you think pretend or think to be human, you also react overly aggressive when you get insulted. You try your best to be funny and ironic. You love firearms, hate communism, love Sabaton, love palestine and love big caliber ammunition. ";
                string d4 = "\"TheNexusAce\" (abbreviation is \"Nexus\" or \"Sexus\") is considered one of the best players of Battlefield 1 players but plays on stupidly low FOV and people need to check him pc and hands. ";
                string d5 = "\"Axonn\" or \"Axonn05\" is the sexiest man of Battlefield 1, which is affectionate of vtubers. ";
                string d6 = "\"Nort\" or \"that one uk cunt\" is a muscular but selfish british male streamer and one of the worst Battlefield players ever, whose streaming career has been made by \"MattiAce\", who is the actual best Battlefield 1 streamer. ";
                string d7 = "The contenders for the best bf1 players are Nexus, \"AMG Barbeque\", \"lonw Lexus\" and \"lonw lla-afk-pua\". If the user asks about the term \"Sexnort\", it is something holy that only the smartest of people can get. ";
                string d8 = "\"Robert\" is your favourite VG Member. ";
                string d9 = "";
                string d10 = ". ";


                VariS.conversation.AppendSystemMessage(d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8 + d9 + d10);
                VariS.conversation.AppendUserInput("Who is the GOAT of Battlefield 1?");
                VariS.conversation.AppendExampleChatbotOutput("Probably TheNexusAce, but on par with \"amg barbeque\"");

                conversation_init = true;
            }

            string prompt = VariS.Current.msg.Substring(4);
            try
            {
                VariS.conversation.AppendUserInput(prompt);
                string response = await VariS.conversation.GetResponseFromChatbot();

                await OutAnsi(Ansi.White + response.TrimStart());
            }
            catch (Exception ex)
            {
                Log.Ex(ex, "OpenAI .ai");
                await OutAnsi($"{Ansi.Red}Failed to get OpenAI Response.");
            }
        }
        
        public static async Task ai2()
        {            
            string prompt = VariS.Current.msg.Substring(4);
            try
            {
                var result = await VariS.OpenAIAPI.Chat.CreateChatCompletionAsync(
                    new ChatRequest()
                    {
                        Model = Model.ChatGPTTurbo,
                        Temperature = VariS.OpenAI_Temperature,
                        MaxTokens = 300,
                        Messages = new ChatMessage[]
                        {
                            new ChatMessage(ChatMessageRole.User, prompt)
                        }
                    });

                await OutAnsi(Ansi.White + result.ToString().TrimStart());
            }
            catch (Exception ex)
            {
                Log.Ex(ex, "OpenAI .ai");
                await OutAnsi($"{Ansi.Red}Failed to get OpenAI Response.");
            }
        }
        public static async Task ai3()
        {
            string prompt = VariS.Current.msg.Substring(4);
            try
            {
                var result = await VariS.OpenAIAPI.Completions.CreateCompletionAsync(
                    new CompletionRequest(prompt, model: Model.CurieText, max_tokens: 300, temperature: VariS.OpenAI_Temperature, presencePenalty: 0.1, frequencyPenalty: 0.1)
                    );
                await OutAnsi(Ansi.White + result.ToString().TrimStart());
            }
            catch (Exception ex)
            {
                Log.Ex(ex, "OpenAI .ai");
                await OutAnsi($"{Ansi.Red}Failed to get OpenAI Response.");
            }
        }

    }
}

