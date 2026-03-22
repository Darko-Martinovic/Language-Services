# Azure Language Services — C# Capability Showcase

A .NET 9 console application demonstrating all major capabilities of the [Azure AI Language](https://learn.microsoft.com/en-us/azure/ai-services/language-service/) service using the `Azure.AI.TextAnalytics` SDK.

## Features

| # | Capability | API Method |
|---|-----------|-----------|
| 1 | **Language Detection** | `DetectLanguageBatch` |
| 2 | **Sentiment Analysis** with Opinion Mining | `AnalyzeSentimentBatch` |
| 3 | **Key Phrase Extraction** | `ExtractKeyPhrasesBatch` |
| 4 | **Named Entity Recognition (NER)** | `RecognizeEntitiesBatch` |
| 5 | **Entity Linking** (Wikipedia) | `RecognizeLinkedEntitiesBatch` |
| 6 | **PII Detection** with redaction | `RecognizePiiEntitiesBatch` |
| 7 | **Abstractive Text Summarization** | `AbstractiveSummarizeAsync` |
| 8 | **Extractive Text Summarization** | `ExtractiveSummarizeAsync` |
| 9 | **Healthcare NER** with entity relations | `AnalyzeHealthcareEntitiesAsync` |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- An Azure AI Language resource ([create one](https://portal.azure.com))

## Configuration

Set the following environment variables before running:

```bash
export AZURE_LANGUAGE_ENDPOINT="https://<your-resource>.cognitiveservices.azure.com/"
export AZURE_LANGUAGE_KEY="<your-key>"
```

On Windows (PowerShell):

```powershell
$env:AZURE_LANGUAGE_ENDPOINT = "https://<your-resource>.cognitiveservices.azure.com/"
$env:AZURE_LANGUAGE_KEY      = "<your-key>"
```

## Run

```bash
dotnet run
```

## Dependencies

- [`Azure.AI.TextAnalytics`](https://www.nuget.org/packages/Azure.AI.TextAnalytics) 5.3.0
