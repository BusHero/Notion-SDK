﻿using Notion.Model;

namespace MarkdownExporter;

public class RichTextConverter : Converter<RichText>
{
    public IAplicable Applier { get; }

    public RichTextConverter(IAplicable aplicable) => Applier = aplicable ?? throw new ArgumentNullException(nameof(aplicable));

    public override IOption<string> Convert(RichText text, ConverterSettings? settings) => text switch
    {
        not null => Applier.Apply(text, text.PlainText).ToOption(),
        _ => default(string).ToOption()
    };

    public override IOption<List<string>> Convert2(RichText text, ConverterSettings? settings) => (text switch
    {
        not null => new List<string> { Applier.Apply(text, text.PlainText) },
        _ => default,
    }).ToOption();
}
