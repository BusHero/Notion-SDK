﻿using FluentAssertions;

using Notion.Model;

using Xunit;

namespace MarkdownExporter.Tests;

public class ParagraphConverterTests : ConverterTestsBase
{
    [Theory]
    [MemberData(nameof(Paragraphs))]
    public void Foo(Block.Paragraph block, string expectedText)
    {
        var actualText = Converter.Convert(block, Settings).ValueOrDefault(string.Empty);
        actualText.Should().Be(expectedText);
    }

    public static TheoryData<Block.Paragraph, string> Paragraphs { get; } = new()
    {
        {
            new Block.Paragraph
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Some text here and there",
                        PlainText = "Some text here and there"
                    }
                }
            },
            "Some text here and there"
        },
        {
            new Block.Paragraph
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Some text ",
                        PlainText = "Some text "
                    },
                    new RichText.Text
                    {
                        Content = "here and there",
                        PlainText = "here and there"
                    }
                }
            },
            "Some text here and there"
        },
        {
            new Block.Paragraph
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Some text ",
                        Annotations = new Annotations
                        {
                            Bold = true
                        },
                        PlainText = "Some text ",

                    },
                    new RichText.Text
                    {
                        Content = "here and there",
                        Annotations = new Annotations
                        {
                            Italic = true
                        },
                        PlainText = "here and there",
                    }
                }
            },
            "*Some text ***here and there**"
        },
    };
}
