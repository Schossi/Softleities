[![npm package](https://img.shields.io/npm/v/com.softleitner.softleities)](https://www.npmjs.com/package/com.softleitner.softleities)
[![openupm](https://img.shields.io/npm/v/com.softleitner.softleities?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.softleitner.softleities/)
![Tests](https://github.com/softleitner/softleities/workflows/Tests/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

# Softleities

Softleitner Utilities for Unity

- [How to use](#how-to-use)
- [Install](#install)
  - [via npm](#via-npm)
  - [via OpenUPM](#via-openupm)
  - [via Git URL](#via-git-url)
  - [Tests](#tests)
- [Configuration](#configuration)

<!-- toc -->

## How to use

*Work In Progress*

## Install

### via npm

Open `Packages/manifest.json` with your favorite text editor. Add a [scoped registry](https://docs.unity3d.com/Manual/upm-scoped.html) and following line to dependencies block:
```json
{
  "scopedRegistries": [
    {
      "name": "npmjs",
      "url": "https://registry.npmjs.org/",
      "scopes": [
        "com.softleitner"
      ]
    }
  ],
  "dependencies": {
    "com.softleitner.softleities": "1.0.0"
  }
}
```
Package should now appear in package manager.

### via OpenUPM

The package is also available on the [openupm registry](https://openupm.com/packages/com.softleitner.softleities). You can install it eg. via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.softleitner.softleities
```

### via Git URL

Open `Packages/manifest.json` with your favorite text editor. Add following line to the dependencies block:
```json
{
  "dependencies": {
    "com.softleitner.softleities": "https://github.com/softleitner/softleities.git"
  }
}
```

### Tests

The package can optionally be set as *testable*.
In practice this means that tests in the package will be visible in the [Unity Test Runner](https://docs.unity3d.com/2017.4/Documentation/Manual/testing-editortestsrunner.html).

Open `Packages/manifest.json` with your favorite text editor. Add following line **after** the dependencies block:
```json
{
  "dependencies": {
  },
  "testables": [ "com.softleitner.softleities" ]
}
```

## Configuration

*Work In Progress*

## License

MIT License

Copyright © 2024 SoftLeitner
