using Azure;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;


namespace CombinedExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Question Answering Setup
            Uri qaEndpoint = new Uri("https://westeuropelanguageqna.cognitiveservices.azure.com/");
            AzureKeyCredential qaCredential = new AzureKeyCredential("e567feaeae184b0896574db8911fd677");
            string projectName = "LearnFAQ";
            string deploymentName = "production";
            QuestionAnsweringClient qaClient = new QuestionAnsweringClient(qaEndpoint, qaCredential);
            QuestionAnsweringProject qaProject = new QuestionAnsweringProject(projectName, deploymentName);

            // Sentiment Analysis Setup
            string languageKey = "4fd40311e221408a8177c082f266f6da";
            string languageEndpoint = "https://servicescogeuropewest.cognitiveservices.azure.com/";
            AzureKeyCredential sentimentCredentials = new AzureKeyCredential(languageKey);
            Uri sentimentEndpoint = new Uri(languageEndpoint);
            TextAnalyticsClient sentimentClient = new TextAnalyticsClient(sentimentEndpoint, sentimentCredentials);

            var question = "";
            while (question.ToLower() != "quit")
            {
                Console.WriteLine("Hi how can i help you today? or write quit to exit");
                question = Console.ReadLine();

                // Question Answering
                Response<AnswersResult> qaResponse = qaClient.GetAnswers(question, qaProject);
                foreach (KnowledgeBaseAnswer answer in qaResponse.Value.Answers)
                {
                    Console.WriteLine($"Q&A: {answer.Answer}");
                }

                // Sentiment Analysis
                var documents = new List<string> { question };
                AnalyzeSentimentResultCollection reviews = sentimentClient.AnalyzeSentimentBatch(documents, options: new AnalyzeSentimentOptions()
                {
                    IncludeOpinionMining = true
                });

                foreach (AnalyzeSentimentResult review in reviews)
                {
                    var ds = review.DocumentSentiment;
                    Console.WriteLine($"Sentiment: {ds.Sentiment}");
                    Console.WriteLine($"Positive score: {ds.ConfidenceScores.Positive:0.00}");
                    Console.WriteLine($"Negative score: {ds.ConfidenceScores.Negative:0.00}");
                    Console.WriteLine($"Neutral score: {ds.ConfidenceScores.Neutral:0.00}");
                    Console.WriteLine();
                }
            }
        }
    }
}

    

