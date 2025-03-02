# SVGL For PowerToys

This [**PowerToys Run**](https://github.com/microsoft/PowerToys) plugin enables quick **searching and copying of SVG icons**, powered by [**SVGL**](https://svgl.app).

![SVGL PowerToys Run Initial Screen Mockup](https://mcp4lhyypl.ufs.sh/f/9Vzc0FiUzX7Rf77ZRLZzk764cyASxaOBWsoVzZ9vLqK8u5tY)

## Features

- Browse and search SVG logos
- Choose from multiple different variant i.e. Light, Dark or Themed Wordmark
- Copy SVG to clipboard

## Installation

1. Download the latest release of the SVGL Plugin from the [**Release page**.](https://github.com/SameerJS6/powertoys-svgl/releases)
2. Extract the zip folder to your PowerToys Plugin modules directory. Usually:

```text
%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins
```

3. Restart PowerToys

## Usage

1. Open PowerToys Run using the default shortcut **`Alt`** + **`Space`**.
2. Type `svgl` to initialize the plugin. _(If it's your first time, wait a moment for initialization.)_
3. Search for a company's logo by typing its name.
4. Select your preferred logo—if multiple variants are available, you'll see relevant options in the context menu.

- Hover or focus on a context menu item to view its **keyboard shortcut & action**.
- _(For a full list of available commands, [click here](https://svgl.sameerjs.com/commands).)_

5. Press `Enter` to copy the SVG icon and paste it wherever you need—whether in **Figma, Adobe Illustrator, or directly in your code**! ✨

## License

This project is licensed under the [MIT License](https://github.com/SameerJS6/SVGL/blob/main/LICENSE)

## Acknowledgements

1. The original [**SVGL**](https://github.com/pheralb/svgl) project by [Pablo Hdez](https://github.com/pheralb) for making this possible.
2. [Corey] for providing guidance on building a PowerToys plugin and sharing valuable resources.
3. [**Henrik Lau Erikson**](https://github.com/hlaueriksson) for writing an insightful blog on [Creating Custom PowerToys Run Plugin](https://conductofcode.io/post/creating-custom-powertoys-run-plugins/) and for developing a highly useful [Dependency Referencing NuGet Package](https://www.nuget.org/packages/Community.PowerToys.Run.Plugin.Dependencies/) for PowerToys Run plugin development.
4. The [**SVGL Raycast Extension**](https://www.raycast.com/1weiho/svgl) by [Yiewi Ho](https://github.com/1weiho), featured in a [YouTube video](https://youtu.be/dQwJQnOxyVk?si=E72TxTEnHo83_sqk&t=370) by **Theo**, which inspired me and gave me the confidence to build something similar for Windows i.e. PowerToys.

## Roadmap

### Version 1.0.0 ✅

1. **Search Enhancements**

   - [x] Debounce API calls to optimize search performance.
   - [x] Improve search ranking: prioritize exact matches, titles starting with the query, and titles containing the query.
   - [x] Cache search results for faster responses.

2. **Context Menu Improvements**

   - [x] Fix broken icons in search results.
   - [x] Show relevant options for "Request Logo" and "Submit a Logo."
   - [x] Add options for copying Wordmark icons (both themes, if available).
   - [x] Separate context menu options for copying light/dark theme SVGs.
   - [x] Dynamically display theme-based context menu options based on SVG availability.
   - [x] Add context menu actions for SVGL results (Copy SVG Logo, Copy Wordmark Logo).

3. **Functionality Enhancements**

   - [x] Add a "Copy SVG Code" function.
   - [x] Implement caching for default results.
   - [x] Add a command to manually reload/re-validate cache without restarting PowerToys.
   - [x] Automatically detect internet reconnection and refresh API results instead of showing cached `No Internet` data.

4. **Codebase & Testing**
   - [x] Restructure the codebase into two projects: Main Plugin & Unit Tests.
   - [x] Refactor code (separate types, API functions, etc.).
   - [x] Handle exceptions, particularly when fetching SVGs (e.g., tRPC dark-themed Wordmark SVG errors).
   - [x] Write unit tests to ensure reliability.


### Future Improvements 🛠️

- **Enhanced Functionality**

  - Add support for copying SVGs as `React` components.
  - Implement fuzzy search for better result matching.
  - Allow customization of all context menu keyword shortcuts via settings.

- **Code Quality & Performance**
  - Refactor `Types.cs` for better structure and maintainability.
  - Improve overall code quality, resolve build warnings, and enable `TreatWarningsAsErrors` in `csproj`.


### PowerToys Run Limitations & Workarounds ⚡

- **Performance Optimizations**

  - Implement caching for SVG code to speed up retrieval.
  - Introduce file-based caching with time-based re-validation for improved efficiency.

- **UI/UX Enhancements**
  - Improve context menu icons for better clarity.
  - Display actual SVGs as previews in search results.
  - Show a loading state when fetching results for the first time.