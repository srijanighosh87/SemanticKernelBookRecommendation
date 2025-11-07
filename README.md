# Semantic Kernel Book Recommendation

## Overview
The **Semantic Kernel Book Recommendation** is a .NET 8 console application that combines AI-powered book recommendations with real-time rating validation. It leverages the **Microsoft Semantic Kernel** to orchestrate Azure OpenAI (GPT-4) for personalized book suggestions based on user preferences, and filters the results using the Google Books API to ensure only highly-rated books (4.0+ stars) are recommended.

## Features
- **AI-Powered Recommendations**: Uses Azure OpenAI to generate book suggestions based on user-provided genres or themes.
- **Rating Validation**: Integrates with the Google Books API to validate and filter books by a minimum rating threshold.
- **Custom Plugins**:
  - **Semantic Plugin**: Prompt-based AI recommendations.
  - **Native Plugin**: C#-based integration with external APIs.
- **JSON Parsing**: Processes and validates AI responses for structured output.
- **Prompt Engineering**: Tailored prompts for generating insightful and relevant book recommendations.

## Prerequisites
- .NET 8 SDK installed.
- Azure OpenAI account with access to GPT-4.
- Google Books API (no authentication required for basic usage).
