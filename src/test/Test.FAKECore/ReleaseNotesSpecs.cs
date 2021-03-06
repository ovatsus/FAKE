﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Fake;
using Machine.Specifications;
using Microsoft.FSharp.Collections;

namespace Test.FAKECore
{
    public class when_parsing_release_notes
    {
        static IEnumerable<string> Notes(string data)
        {
            return data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        const string validData = @" 

### New in 1.1.9 (Released 2013/07/21)
* Infer booleans for ints that only manifest 0 and 1.

* Support for partially overriding the Schema in CsvProvider.
* PreferOptionals and SafeMode parameters for CsvProvider.

### New in 1.1.10 (Released 2013/09/12)
* Support for heterogeneous XML attributes.
* Make CsvFile re-entrant. 
* Support for compressed HTTP responses. 
* Fix JSON conversion of 0 and 1 to booleans.
* Fix XmlProvider problems with nested elements and elements with same name in different namespaces.

";

        static readonly ReleaseNotesHelper.ReleaseNotes expected = new ReleaseNotesHelper.ReleaseNotes("1.1.10", "1.1.10",
            new [] {"Support for heterogeneous XML attributes.", 
                    "Make CsvFile re-entrant.",
                    "Support for compressed HTTP responses.",
                    "Fix JSON conversion of 0 and 1 to booleans.",
                    "Fix XmlProvider problems with nested elements and elements with same name in different namespaces."}
            .ToFSharpList());

        It should_parse_complex =
            () => ReleaseNotesHelper.parseReleaseNotes(Notes(validData)).ShouldEqual(expected);

        It should_parse_empty_notes =
            () => ReleaseNotesHelper.parseReleaseNotes(Notes(@"### New in 1.1.10 (Released 2013/09/12)"))
                .ShouldEqual(new ReleaseNotesHelper.ReleaseNotes("1.1.10", "1.1.10", FSharpList<string>.Empty));

        It should_throws_on_wrong_formatted_data =
            () => Catch.Exception(() =>
                ReleaseNotesHelper.parseReleaseNotes(Notes(@"New in 1.1.10 (Released 2013/09/12)")));

        It should_throws_on_empty_seq_input =
            () => Catch.Exception(() => ReleaseNotesHelper.parseReleaseNotes(new string[] { }));

        It should_throws_on_single_empty_string_input =
            () => Catch.Exception(() => ReleaseNotesHelper.parseReleaseNotes(new[] { "" }));

        It should_throws_on_null_input =
            () => Catch.Exception(() => ReleaseNotesHelper.parseReleaseNotes(null));

        It should_parse_simple_format =
            () => ReleaseNotesHelper.parseReleaseNotes(Notes(@"
* 1.1.9 - Infer booleans for ints that only manifest 0 and 1. Support for partially overriding the Schema in CsvProvider.
* 1.1.10 - Support for heterogeneous XML attributes. Make CsvFile re-entrant."))
                .ShouldEqual(new ReleaseNotesHelper.ReleaseNotes("1.1.10", "1.1.10",
                    new[] { "Support for heterogeneous XML attributes.", "Make CsvFile re-entrant." }.ToFSharpList()));

		It should_parse_simple_format_with_dots =
			() => ReleaseNotesHelper.parseReleaseNotes (Notes (@"
* 1.2.0 - Allow currency symbols on decimals. Remove .AsTuple member from CsvProvider. CsvProvider now uses GetSample instead of constructor like the other providers."))
				.ShouldEqual (new ReleaseNotesHelper.ReleaseNotes ("1.2.0", "1.2.0",
					new [] { "Allow currency symbols on decimals.", "Remove .AsTuple member from CsvProvider.", "CsvProvider now uses GetSample instead of constructor like the other providers." }.ToFSharpList ()));
	}
}