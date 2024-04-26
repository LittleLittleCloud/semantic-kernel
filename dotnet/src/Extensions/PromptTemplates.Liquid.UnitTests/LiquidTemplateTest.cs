﻿// Copyright (c) Microsoft. All rights reserved.

using System.IO;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;
using Xunit;
namespace SemanticKernel.Extensions.PromptTemplates.Liquid.UnitTests;
public class LiquidTemplateTest
{
    [Fact]
    public async Task ItRenderChatTestAsync()
    {
        // Arrange
        var liquidTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "chat.txt");
        var liquidTemplate = File.ReadAllText(liquidTemplatePath);

        var config = new PromptTemplateConfig()
        {
            TemplateFormat = LiquidPromptTemplateFactory.LiquidTemplateFormat,
            Template = liquidTemplate,
        };

        // create a dynamic customer object
        // customer contains the following properties
        // - firstName
        // - lastName
        // - age
        // - membership
        // - orders []
        //  - name
        //  - description
        var customer = new
        {
            firstName = "John",
            lastName = "Doe",
            age = 30,
            membership = "Gold",
            orders = new[]
            {
                new { name = "apple", description = "2 fuji apples", date = "2024/04/01" },
                new { name = "banana", description = "1 free banana from amazon banana hub", date = "2024/04/03" },
            },
        };

        // create a list of documents
        // documents contains the following properties
        // - id
        // - title
        // - content
        var documents = new[]
        {
            new { id = "1", title = "apple", content = "2 apples"},
            new { id = "2", title = "banana", content = "3 bananas"},
        };

        // create chat history
        // each chat message contains the following properties
        // - role (system, user, assistant)
        // - content

        var chatHistory = new[]
        {
            new { role = "user", content = "When is the last time I bought apple?" },
        };

        var arguments = new KernelArguments()
        {
            { "customer", customer },
            { "documentation", documents },
            { "history", chatHistory },
        };

        var liquidTemplateInstance = new LiquidPromptTemplate(config);

        // Act
        var result = await liquidTemplateInstance.RenderAsync(new Kernel(), arguments);

        // Assert
        await VerifyXunit.Verifier.Verify(result);
    }
}
