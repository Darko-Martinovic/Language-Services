// Azure Language Service - Capability Showcase (C#)
// Demonstrates: Language Detection, Sentiment Analysis, Key Phrase Extraction,
// Named Entity Recognition, Entity Linking, PII Detection,
// Text Summarization, and Healthcare NER.

using Azure;
using Azure.AI.TextAnalytics;

var Endpoint =
    Environment.GetEnvironmentVariable("AZURE_LANGUAGE_ENDPOINT")
    ?? throw new InvalidOperationException("Set the AZURE_LANGUAGE_ENDPOINT environment variable.");
var Key =
    Environment.GetEnvironmentVariable("AZURE_LANGUAGE_KEY")
    ?? throw new InvalidOperationException("Set the AZURE_LANGUAGE_KEY environment variable.");
const string Sep = "======================================================================";

var client = new TextAnalyticsClient(new Uri(Endpoint), new AzureKeyCredential(Key));

void Section(string title)
{
    Console.WriteLine($"\n{Sep}");
    Console.WriteLine($"  {title}");
    Console.WriteLine(Sep);
}

// ---------------------------------------------------------------------------
// 1. Language Detection
// ---------------------------------------------------------------------------
void DemoLanguageDetection()
{
    Section("1. LANGUAGE DETECTION");

    var documents = new List<string>
    {
        "This is written in English.",
        "Bonjour, comment ca va?",
        "Hola, como estas?",
        "Guten Morgen, wie geht es Ihnen?",
        "Merhaba, nasilsin?",
        "Konnichiwa, ogenki desu ka?",
    };

    var results = client.DetectLanguageBatch(documents).Value;
    foreach (var (result, text) in results.Zip(documents))
    {
        if (!result.HasError)
        {
            var lang = result.PrimaryLanguage;
            var snippet = text.Length > 45 ? text[..45] : text;
            Console.WriteLine(
                $"  '{snippet}' -> {lang.Name} ({lang.Iso6391Name}) | confidence: {lang.ConfidenceScore:F2}"
            );
        }
        else
        {
            Console.WriteLine($"  Error: {result.Error.Message}");
        }
    }
}

// ---------------------------------------------------------------------------
// 2. Sentiment Analysis (with opinion mining)
// ---------------------------------------------------------------------------
void DemoSentimentAnalysis()
{
    Section("2. SENTIMENT ANALYSIS (with Opinion Mining)");

    var documents = new List<string>
    {
        "The hotel was absolutely wonderful! The staff were incredibly friendly.",
        "The food was terrible and the service was extremely slow.",
        "The movie was okay, nothing special.",
        "I love the new iPhone camera, but the battery life is disappointing.",
        "Azure Cognitive Services makes AI integration effortless and powerful.",
    };

    var options = new AnalyzeSentimentOptions { IncludeOpinionMining = true };
    var results = client.AnalyzeSentimentBatch(documents, options: options).Value;

    foreach (var (result, text) in results.Zip(documents))
    {
        if (!result.HasError)
        {
            var snippet = text.Length > 70 ? text[..70] : text;
            Console.WriteLine($"\n  Text     : {snippet}");
            Console.WriteLine(
                $"  Sentiment: {result.DocumentSentiment.Sentiment.ToString().ToUpper()} "
                    + $"(pos={result.DocumentSentiment.ConfidenceScores.Positive:F2}, "
                    + $"neu={result.DocumentSentiment.ConfidenceScores.Neutral:F2}, "
                    + $"neg={result.DocumentSentiment.ConfidenceScores.Negative:F2})"
            );

            foreach (var sentence in result.DocumentSentiment.Sentences)
            {
                foreach (var opinion in sentence.Opinions)
                {
                    var assessments = string.Join(
                        ", ",
                        opinion.Assessments.Select(a => $"{a.Sentiment}({a.Text})")
                    );
                    Console.WriteLine(
                        $"    Opinion -> target='{opinion.Target.Text}' | assessments=[{assessments}]"
                    );
                }
            }
        }
        else
        {
            Console.WriteLine($"  Error: {result.Error.Message}");
        }
    }
}

// ---------------------------------------------------------------------------
// 3. Key Phrase Extraction
// ---------------------------------------------------------------------------
void DemoKeyPhrases()
{
    Section("3. KEY PHRASE EXTRACTION");

    var documents = new List<string>
    {
        "Microsoft Azure is a cloud computing service created by Microsoft for building, testing, deploying, and managing applications via Microsoft-managed data centers.",
        "Machine learning and artificial intelligence are transforming industries ranging from healthcare to finance and autonomous vehicles.",
        "The Great Wall of China is one of the greatest wonders of the ancient world, stretching over 13,000 miles across northern China.",
    };

    var results = client.ExtractKeyPhrasesBatch(documents).Value;
    foreach (var (result, text) in results.Zip(documents))
    {
        if (!result.HasError)
        {
            var snippet = text.Length > 80 ? text[..80] : text;
            Console.WriteLine($"\n  Text       : {snippet}...");
            Console.WriteLine($"  Key Phrases: {string.Join(", ", result.KeyPhrases)}");
        }
        else
        {
            Console.WriteLine($"  Error: {result.Error.Message}");
        }
    }
}

// ---------------------------------------------------------------------------
// 4. Named Entity Recognition (NER)
// ---------------------------------------------------------------------------
void DemoNer()
{
    Section("4. NAMED ENTITY RECOGNITION (NER)");

    var documents = new List<string>
    {
        "Elon Musk founded SpaceX in 2002 and Tesla in 2003. He was born in Pretoria, South Africa.",
        "The Eiffel Tower in Paris was designed by Gustave Eiffel and completed in 1889.",
        "Amazon was founded by Jeff Bezos on July 5, 1994, in Bellevue, Washington.",
    };

    var results = client.RecognizeEntitiesBatch(documents).Value;
    foreach (var (result, text) in results.Zip(documents))
    {
        if (!result.HasError)
        {
            var snippet = text.Length > 80 ? text[..80] : text;
            Console.WriteLine($"\n  Text: {snippet}");
            foreach (var entity in result.Entities)
            {
                Console.WriteLine(
                    $"    [{entity.Category, -20}] '{entity.Text}' (confidence: {entity.ConfidenceScore:F2})"
                );
            }
        }
        else
        {
            Console.WriteLine($"  Error: {result.Error.Message}");
        }
    }
}

// ---------------------------------------------------------------------------
// 5. Entity Linking (linking to Wikipedia)
// ---------------------------------------------------------------------------
void DemoEntityLinking()
{
    Section("5. ENTITY LINKING (Wikipedia)");

    var documents = new List<string>
    {
        "I visited the Louvre Museum in Paris last summer.",
        "Bill Gates co-founded Microsoft and is now a philanthropist.",
    };

    var results = client.RecognizeLinkedEntitiesBatch(documents).Value;
    foreach (var (result, text) in results.Zip(documents))
    {
        if (!result.HasError)
        {
            Console.WriteLine($"\n  Text: {text}");
            foreach (var entity in result.Entities)
            {
                Console.WriteLine($"    '{entity.Name}' -> {entity.Url}");
            }
        }
        else
        {
            Console.WriteLine($"  Error: {result.Error.Message}");
        }
    }
}

// ---------------------------------------------------------------------------
// 6. PII Detection
// ---------------------------------------------------------------------------
void DemoPiiDetection()
{
    Section("6. PII DETECTION (Personally Identifiable Information)");

    var documents = new List<string>
    {
        "My name is John Smith and my email is john.smith@example.com. My phone is 425-555-0189.",
        "Please charge my Visa card 4111 1111 1111 1111, expiry 12/26, CVV 123.",
        "Patient DOB: 01/15/1985. SSN: 123-45-6789. Address: 123 Main St, Seattle, WA 98101.",
    };

    var results = client.RecognizePiiEntitiesBatch(documents).Value;
    foreach (var (result, text) in results.Zip(documents))
    {
        if (!result.HasError)
        {
            var snippet = text.Length > 80 ? text[..80] : text;
            var redactedText = result.Entities.RedactedText;
            var redacted = redactedText.Length > 80 ? redactedText[..80] : redactedText;
            Console.WriteLine($"\n  Original : {snippet}");
            Console.WriteLine($"  Redacted : {redacted}");
            foreach (var entity in result.Entities)
            {
                Console.WriteLine($"    [{entity.Category, -30}] '{entity.Text}'");
            }
        }
        else
        {
            Console.WriteLine($"  Error: {result.Error.Message}");
        }
    }
}

// ---------------------------------------------------------------------------
// 7. Abstractive Text Summarization
// ---------------------------------------------------------------------------
async Task DemoAbstractiveSummaryAsync()
{
    Section("7. ABSTRACTIVE TEXT SUMMARIZATION");

    var documents = new List<TextDocumentInput>
    {
        new TextDocumentInput(
            "1",
            "The COVID-19 pandemic, caused by the SARS-CoV-2 coronavirus, began in late 2019 "
                + "in Wuhan, China, and rapidly spread worldwide, leading the World Health Organization "
                + "to declare a global pandemic in March 2020. Governments around the world implemented "
                + "lockdowns, travel restrictions, and mask mandates to slow transmission. The pandemic "
                + "accelerated the development of mRNA vaccines, with Pfizer-BioNTech and Moderna "
                + "receiving emergency authorization by late 2020. The economic impact was severe, "
                + "causing the sharpest recession since the Great Depression. By 2022, widespread "
                + "vaccination campaigns had significantly reduced severe illness and death, though "
                + "variants like Delta and Omicron continued to challenge public health systems globally."
        )
        {
            Language = "en",
        },
    };

    var operation = await client.AbstractiveSummarizeAsync(WaitUntil.Completed, documents);
    await foreach (var page in operation.Value)
    {
        foreach (var result in page)
        {
            if (!result.HasError)
            {
                foreach (var summary in result.Summaries)
                    Console.WriteLine($"\n  Summary: {summary.Text}");
            }
            else
            {
                Console.WriteLine($"  Error: {result.Error.Message}");
            }
        }
    }
}

// ---------------------------------------------------------------------------
// 8. Extractive Text Summarization
// ---------------------------------------------------------------------------
async Task DemoExtractiveSummaryAsync()
{
    Section("8. EXTRACTIVE TEXT SUMMARIZATION");

    var documents = new List<TextDocumentInput>
    {
        new TextDocumentInput(
            "1",
            "Renewable energy sources are becoming increasingly important in the fight against "
                + "climate change. Solar power has seen dramatic cost reductions over the past decade, "
                + "making it one of the cheapest forms of electricity in many regions. Wind energy has "
                + "also grown substantially, with offshore wind farms capable of generating electricity "
                + "for millions of homes. Battery storage technology is improving rapidly, addressing "
                + "the intermittency challenge of renewables. Governments worldwide are setting ambitious "
                + "net-zero targets, with many pledging to phase out fossil fuel subsidies and invest "
                + "heavily in clean energy infrastructure. The transition to renewables is also creating "
                + "millions of new jobs in manufacturing, installation, and maintenance."
        )
        {
            Language = "en",
        },
    };

    var options = new ExtractiveSummarizeOptions { MaxSentenceCount = 3 };
    var operation = await client.ExtractiveSummarizeAsync(
        WaitUntil.Completed,
        documents,
        options: options
    );
    await foreach (var page in operation.Value)
    {
        foreach (var result in page)
        {
            if (!result.HasError)
            {
                var sentences = string.Join(" ", result.Sentences.Select(s => s.Text));
                Console.WriteLine($"\n  Key Sentences:\n  {sentences}");
            }
            else
            {
                Console.WriteLine($"  Error: {result.Error.Message}");
            }
        }
    }
}

// ---------------------------------------------------------------------------
// 9. Healthcare NER
// ---------------------------------------------------------------------------
async Task DemoHealthcareNerAsync()
{
    Section("9. HEALTHCARE ENTITY RECOGNITION");

    var inputText =
        "The patient was prescribed 500mg of Amoxicillin twice daily for a bacterial infection. "
        + "She has a known allergy to Penicillin. Blood pressure was 130/85 mmHg. "
        + "She was also diagnosed with Type 2 Diabetes and prescribed Metformin 1000mg.";

    var documents = new List<TextDocumentInput>
    {
        new TextDocumentInput("1", inputText) { Language = "en" },
    };

    var operation = await client.AnalyzeHealthcareEntitiesAsync(WaitUntil.Completed, documents);
    await foreach (var page in operation.Value)
    {
        foreach (var result in page)
        {
            if (!result.HasError)
            {
                Console.WriteLine(
                    $"\n  Text: {(inputText.Length > 90 ? inputText[..90] : inputText)}..."
                );
                Console.WriteLine("  Entities:");
                foreach (var entity in result.Entities)
                    Console.WriteLine(
                        $"    [{entity.Category, -30}] '{entity.Text}' (confidence: {entity.ConfidenceScore:F2})"
                    );

                if (result.EntityRelations.Count > 0)
                {
                    Console.WriteLine("  Relations:");
                    foreach (var relation in result.EntityRelations)
                    {
                        var roles = string.Join(
                            " -> ",
                            relation.Roles.Select(r => $"'{r.Entity.Text}'({r.Name})")
                        );
                        Console.WriteLine($"    {relation.RelationType}: {roles}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"  Error: {result.Error.Message}");
            }
        }
    }
}

// ---------------------------------------------------------------------------
// 10. Custom Single-Label Text Classification
// ---------------------------------------------------------------------------
async Task DemoCustomSingleLabelClassificationAsync()
{
    Section("10. CUSTOM SINGLE-LABEL TEXT CLASSIFICATION");

    const string projectName    = "singlelabelclassification";
    const string deploymentName = "learn-text-classfication";
    const string dataFolder     = "Data/Classification";

    var fileNames = new[] { "Test1.txt", "Test2.txt", "Test3.txt" };

    var documents = fileNames
        .Select((name, i) => new TextDocumentInput((i + 1).ToString(), File.ReadAllText(Path.Combine(dataFolder, name))) { Language = "en" })
        .ToList();

    Console.WriteLine($"  Classifying {documents.Count} documents using project '{projectName}', deployment '{deploymentName}'...\n");

    var operation = await client.SingleLabelClassifyAsync(
        WaitUntil.Completed,
        documents,
        projectName,
        deploymentName
    );

    int docIndex = 0;
    await foreach (var page in operation.Value)
    {
        foreach (var result in page)
        {
            var fileName = fileNames[docIndex++];
            if (!result.HasError)
            {
                var best = result.ClassificationCategories
                    .OrderByDescending(c => c.ConfidenceScore)
                    .First();
                Console.WriteLine($"  [{fileName,-10}]  Category: {best.Category,-30} Confidence: {best.ConfidenceScore:F2}");
            }
            else
            {
                Console.WriteLine($"  [{fileName,-10}]  Error: {result.Error.Message}");
            }
        }
    }
}

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------
Console.OutputEncoding = System.Text.Encoding.UTF8;

var menu = new (string Label, Func<Task> Action)[]
{
    (
        "Language Detection",
        () =>
        {
            DemoLanguageDetection();
            return Task.CompletedTask;
        }
    ),
    (
        "Sentiment Analysis",
        () =>
        {
            DemoSentimentAnalysis();
            return Task.CompletedTask;
        }
    ),
    (
        "Key Phrase Extraction",
        () =>
        {
            DemoKeyPhrases();
            return Task.CompletedTask;
        }
    ),
    (
        "Named Entity Recognition (NER)",
        () =>
        {
            DemoNer();
            return Task.CompletedTask;
        }
    ),
    (
        "Entity Linking (Wikipedia)",
        () =>
        {
            DemoEntityLinking();
            return Task.CompletedTask;
        }
    ),
    (
        "PII Detection",
        () =>
        {
            DemoPiiDetection();
            return Task.CompletedTask;
        }
    ),
    ("Abstractive Text Summarization", DemoAbstractiveSummaryAsync),
    ("Extractive Text Summarization", DemoExtractiveSummaryAsync),
    ("Healthcare Entity Recognition", DemoHealthcareNerAsync),
    ("Custom Single-Label Classification", DemoCustomSingleLabelClassificationAsync),
};

while (true)
{
    Console.WriteLine($"\n{Sep}");
    Console.WriteLine("  AZURE LANGUAGE SERVICE — CAPABILITY SHOWCASE (C#)");
    Console.WriteLine(Sep);
    for (int i = 0; i < menu.Length; i++)
        Console.WriteLine($"  {i + 1}. {menu[i].Label}");
    Console.WriteLine($"  0. Exit");
    Console.WriteLine(Sep);
    Console.Write($"  Select (0-{menu.Length}): ");

    var input = Console.ReadLine()?.Trim();
    if (input == "0" || input == null)
        break;

    if (int.TryParse(input, out int choice) && choice >= 1 && choice <= menu.Length)
    {
        await menu[choice - 1].Action();
        Console.WriteLine($"\n{Sep}");
        Console.Write("  Press Enter to return to menu...");
        Console.ReadLine();
    }
    else
    {
        Console.WriteLine(
            $"  Invalid selection. Please enter a number between 0 and {menu.Length}."
        );
    }
}

Console.WriteLine($"\n{Sep}");
Console.WriteLine("  Goodbye!");
Console.WriteLine($"{Sep}\n");
