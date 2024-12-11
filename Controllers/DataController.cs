using intelliBot.Controllers;
using intelliBot.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace intelliBot
{
    public class DataController
    {
        private static readonly string connectionString = "server=192.168.154.189;database=intelliGuide;user=Daan;password=Daanpassword@22";

        private static List<T> ExecuteQuery<T>(string query, Action<MySqlCommand> parameterize, Func<MySqlDataReader, T> readData)
        {
            var results = new List<T>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            using var command = new MySqlCommand(query, connection);
            parameterize(command);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(readData(reader));
            }

            return results;
        }

        public static List<Context> GetContexts()
        {
            string query = "SELECT * FROM context WHERE bot_id = @botId";
            return ExecuteQuery(query, 
                cmd => cmd.Parameters.AddWithValue("@botId", BotController.botId),
                reader => new Context
                {
                    Id = reader["context_id"].ToString() ?? string.Empty,
                    BotId = reader["bot_id"].ToString() ?? string.Empty,
                    Title = reader["title"].ToString() ?? string.Empty,
                    Body = reader["body"].ToString() ?? string.Empty,
                    IsActive = Convert.ToBoolean(reader["status"])
                });
        }

        public static List<Answer> GetAnswers(string conversationId)
        {
            string query = "SELECT * FROM answer WHERE conversation_id = @conversationId";
            return ExecuteQuery(query, 
                cmd => cmd.Parameters.AddWithValue("@conversationId", conversationId),
                reader => new Answer
                {
                    Id = reader["answer_id"].ToString() ?? string.Empty,
                    ConversationId = reader["conversation_id"].ToString() ?? string.Empty,
                    Order = Convert.ToInt32(reader["order"]),
                    Body = reader["body"].ToString() ?? string.Empty
                });
        }

        public static List<Question> GetQuestions(string conversationId)
        {
            string query = "SELECT * FROM question WHERE conversation_id = @conversationId";
            return ExecuteQuery(query, 
                cmd => cmd.Parameters.AddWithValue("@conversationId", conversationId),
                reader => new Question
                {
                    Id = reader["question_id"].ToString() ?? string.Empty,
                    ConversationId = reader["conversation_id"].ToString() ?? string.Empty,
                    Order = Convert.ToInt32(reader["order"]),
                    Body = reader["body"].ToString() ?? string.Empty
                });
        }

        public static List<Conversation> GetConversations()
        {
            string query = "SELECT * FROM conversation WHERE bot_id = @botId";
            return ExecuteQuery(query, 
                cmd => cmd.Parameters.AddWithValue("@botId", BotController.botId),
                reader => new Conversation
                {
                    Id = reader["conversation_id"].ToString() ?? string.Empty,
                    BotId = reader["bot_id"].ToString() ?? string.Empty,
                    Date = Convert.ToDateTime(reader["date"]),
                    Review = reader["review"] as int?,
                    Comment = reader["comment"] as string,
                    Tokens = Convert.ToInt32(reader["tokens"])
                });
        }

        public static Bot? GetBot()
        {
            string query = "SELECT * FROM bot WHERE bot_id = @botId";
            var bots = ExecuteQuery(query, 
                cmd => cmd.Parameters.AddWithValue("@botId", BotController.botId),
                reader => new Bot
                {
                    Id = reader["bot_id"].ToString() ?? string.Empty,
                    Name = reader["name"].ToString() ?? string.Empty,
                    Voice = reader["voice"].ToString() ?? string.Empty,
                    Avatar = Convert.ToInt32(reader["avatar"]),
                    Language = reader["language"].ToString() ?? string.Empty,
                    Style = Convert.ToInt32(reader["style"]),
                    MaxTokens = Convert.ToInt32(reader["max_tokens"]),
                    ConvLength = Convert.ToInt32(reader["conv_length"]),
                    AnswerLength = Convert.ToInt32(reader["answer_length"]),
                    GreetUser = Convert.ToBoolean(reader["greet_users"]),
                    GenerateQr = Convert.ToBoolean(reader["generate_qr"]),
                    StoreConv = Convert.ToBoolean(reader["store_conv"]),
                    RequestReviews = Convert.ToBoolean(reader["request_reviews"]),
                    IsActive = Convert.ToBoolean(reader["status"])
                });

            return bots.FirstOrDefault();
        }
    }
}