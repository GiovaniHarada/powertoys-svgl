## Roadmap

### Version 0.0.1
- [x] Add Debounce API Calls from Search
- [x] Fix Context Menu Options.
- [x] Fix Broken Icon from each result.
- [x] Caching or something similar to that for results (atleast for default results).
- [x] Fix the Multiple action keyword being displayed for default results.
- [x] Add seperate Context Menu Options for copying light or Dark theme SVG.
- [x] Conditional Display Light or Dark Theme Context Menu based on the does svg have light/dark theme or not.
- [x] Add Copy SVG code Function.
- [x] Add Wordmark Icon Copying options in Context Menu (For both themes if present).
- [x] Add Context Menu for SVGL Results (i.e Copy Actual SVG Logo, Copy Actual Wordmark Logo).
- [x] Restructure the entire codebase, to have two projects i.e Main Plugin & Unit Tests.
- [x] Caching for Searched Results.
- [x] Improve Search Results by adding some clause i.e perfect match, title startsWiths search query and title contains search query in earlier indexes.
- [ ] Write Unit Tests for the Codebase.
- [ ] Handle Error/Exception, when fetching actual SVG Code (Especially for tRPC's dark themed Wordmark SVG, where there's a error from SVGL's side).
- [x] Show releavant context menu options for request logo and submit a logo result.
- [ ] Add Loading State/Result, when fetching for the first time.
- [ ] Add a reload/re-validate cache command. Goal: In order to get latest results from the API, even though the cache gets re-validated whenever we start a new PowerToys Session but having a command to do is beneficial. Since, SVGL gets updated frequently, having a command which could just re-validate cache with starting a new session is nice to have.
- [x] Refactor Codebase (Seperate Types, API Functions, etc).

### Improvements for later versions
- [ ] Better Icon Previewing (Using the Actual SVG as Preview for that result).
- [ ] File based caching for results and time based re-validation.
- [ ] Add React SVG Commponent Function.
- [ ] Add Caching for SVG Code.
- [ ] Implement better searching i.e Fuzzy Search
- [ ] Improve `Types.cs`, since it's not that good for sure.

