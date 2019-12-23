# Paket Chain

[![Release](https://img.shields.io/github/release/BlythMeister/PaketChain.svg?style=flat)](https://github.com/BlythMeister/PaketSorter/releases/latest)
[![Issues](https://img.shields.io/github/issues/BlythMeister/PaketChain.svg?style=flat)](https://github.com/BlythMeister/PaketSorter/issues)

A command line application call paket in standard folder structures and chain multiple commands together.

With added ability to sort dependencies and references files alphabetically prior to install.

# Installation

TODO

# Usage

```
Usage: PaketChain [options]

Options:
  -d|--dir <PATH>             The path to a root of a repository, defaults to current directory if not provided (Note: <PATH> should be in quotes)
  -u|--update                 Include a paket update
  -ua|--update-args <ARGS>    Args to pass to paket update (Note: <ARGS> should be in quotes)
  -co|--clean-obj             Clean obj folders to force a full update
  -i|--install                Include a paket install
  -ia|--install-args <ARGS>   Args to pass to paket install (Note: <ARGS> should be in quotes)
  -r|--reinstall              Delete the lock file and create from scratch
  -s|--simplify               Include a paket simplify
  -sa|--simplify-args <ARGS>  Args to pass to paket simplify (Note: <ARGS> should be in quotes)
  -so|--sort                  Sort paket files alphabetically
  -cc|--clear-cache           Clear caches before running
  -np|--no-prompt             Never prompt user input
  -v|--verbose                Verbose logging
  -?|-h|--help                Show help information
```

# 3rd Party Libraries

* [CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils)
* [Paket](https://github.com/fsprojects/Paket)
