A .NET 8 console application that combines AI-powered book recommendations with real-time rating validation. 
Uses Microsoft Semantic Kernel to orchestrate Azure OpenAI (GPT-4) for personalized book suggestions based on 
genre preferences, then filters results through the Google Books API to ensure only highly-rated books (4.0+ stars) 
are recommended. Features custom Semantic Kernel plugins for both AI-driven recommendations and external API integration.

Semantic Kernel-based book recommendation engine demonstrating:
- Azure OpenAI integration for natural language processing
- Custom kernel functions and prompt engineering
- Plugin architecture with both native (C#) and semantic (prompt-based) plugins
- External API integration (Google Books API) for rating validation
- JSON-based response parsing and data validation
