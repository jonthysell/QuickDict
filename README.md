# QuickDict #

![License](https://img.shields.io/github/license/jonthysell/QuickDict.svg) [![NuGet](https://img.shields.io/nuget/v/QuickDict.svg)](https://www.nuget.org/packages/QuickDict) [![CI Build](https://github.com/jonthysell/QuickDict/actions/workflows/ci.yml/badge.svg)](https://github.com/jonthysell/QuickDict/actions/workflows/ci.yml)

QuickDict is an open-source .NET library for generating language dictionaries in a variety of digital dictionary formats.

## Install ##

QuickDict is published on NuGet Gallery: https://www.nuget.org/packages/QuickDict

Use this command in the NuGet Package Manager console to install QuickDict manually:

```ps
Install-Package QuickDict
```

## Usage ##

Using QuickDict is as easy as:

1. Create a `StarDictDictionary` or `XdxfDictionary` instance
2. Set the dictionary's metadata (i.e. title, authors, languages, etc.)
3. (Optional) Implement any of a dictionary's hooks with any custom processing code
4. Add all of the dictionary's articles (i.e. term and definition pairs)
5. (Optional) Add all of the dictionary's abbreviations (i.e. term and definition pairs)
6. Save the dictionary to disk

**Note:** The library works best when using *plain text* in the articles and abbreviations.

### Sample Code ###

```cs
/// Create XdxfDictionary instance
var dict = new XdxfDictionary();

// Set the dictionary's metadata
dict.Metadata.LongTitle = "My Fancy French to English Dictionary";
dict.Metadata.Description = "Sample Dictionary made by QuickDict";
dict.Metadata.ArticleKeyLangCode = "FRA";
dict.Metadata.ArticleValueLangCode = "ENG";

// (Optional) Implement any of a dictionary's hooks
dict.GetXdxfKeysFromArticle = a =>
{
  // XDXF can support an article with multiple terms for the same defintions,
  // so assuming my input data has articles with multiple terms divided by commas,
  // I split them here by the comma into separate keys
  return a.Key.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
};

dict.GetXdxfKeyOptionalTerms = () =>
{
  // XDXF can support specifying parts of an article's terms which are "optional",
  // and do not need to be matched when performing a search, so assuming my input data
  // has these these gender terms "(masc)" and "(fem)" that aren't part of the actual word
  // I can specify them here to be flagged as optional
  return new HashSet<string>() { "(masc)", "(fem)" };
};

dict.GetXdxfValuesFromArticle = a =>
{
  // XDXF can support an article with multiple defintions for the same terms,
  // so assuming my input data has articles with multiple numbered definitions
  // (i.e. 1. definition 2. other definition) I use the GetDefintions helper to
  // split the single string into separate (unnumbered) definitions
  return a.Value.GetDefinitions(false).ToList();
};

// Add all of the dictionary's articles
dict.AddArticle("bonjour", "1. interj. hello 2. dated. interj. good day");
dict.AddArticle("examinateur (masc), examinatrice (fem)", "n. examiner");
dict.AddArticle("horloge", "n. clock");

// (Optional) Add all of the dictionary's abbreviations
dict.AddAbbreviation("dated.", "interjection", AbbreviationType.Stylistic);
dict.AddAbbreviation("interj.", "interjection", AbbreviationType.Grammatical);
dict.AddAbbreviation("n.", "noun", AbbreviationType.Grammatical);

// Save the dictionary to disk
dict.Save("MyFancyFrenchToEnglishDictionary.xdxf");
```

This sample code will create a file named `MyFancyFrenchToEnglishDictionary.xdxf` with the following contents:

```xml
<?xml version="1.0" encoding="utf-8"?>
<xdxf format="logical" revision="33" lang_from="FRA" lang_to="ENG">
  <meta_info>
    <full_title>My Fancy French to English Dictionary</full_title>
    <description>Sample Dictionary made by QuickDict</description>
    <abbreviations>
      <abbr_def type="stl">
        <abbr_k>dated.</abbr_k>
        <abbr_v>interjection</abbr_v>
      </abbr_def>
      <abbr_def type="grm">
        <abbr_k>interj.</abbr_k>
        <abbr_v>interjection</abbr_v>
      </abbr_def>
      <abbr_def type="grm">
        <abbr_k>n.</abbr_k>
        <abbr_v>noun</abbr_v>
      </abbr_def>
    </abbreviations>
    <creation_date>24-07-2025</creation_date>
  </meta_info>
  <lexicon>
    <ar>
      <k>bonjour</k>
      <def>
        <def>
          <deftext>
            <abbr>interj.</abbr> hello</deftext>
        </def>
        <def>
          <deftext>
            <abbr>dated.</abbr>
            <abbr>interj.</abbr> good day</deftext>
        </def>
      </def>
    </ar>
    <ar>
      <k>examinateur <opt>(masc)</opt></k>
      <k>examinatrice <opt>(fem)</opt></k>
      <def>
        <deftext>
          <abbr>n.</abbr> examiner</deftext>
      </def>
    </ar>
    <ar>
      <k>horloge</k>
      <def>
        <deftext>
          <abbr>n.</abbr> clock</deftext>
      </def>
    </ar>
  </lexicon>
</xdxf>
```

## Build ##

Building QuickDict requires:

1. A PC with the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed
2. The [QuickDict source](https://github.com/jonthysell/QuickDict)

Then you should be able to run the following command to build QuickDict from within its source folder:

```cmd
dotnet build ./src/QuickDict.sln
```

## Test ##

With the above setup, you should be able to run the following command to test QuickDict from within its source folder:

```cmd
dotnet test ./src/QuickDict.sln
```

## Errata ##

QuickDict is open-source under the MIT license.

Copyright (c) 2025 Jon Thysell.
