---
title: Home
description: "A command line application call paket in standard folder structures and chain multiple commands together."
---

A command line application call paket in standard folder structures and chain multiple commands together.

With added ability to sort dependencies and references files alphabetically prior to install.

## Usage

```cmd
Usage: PaketChain [options]

Options:
  -d|--dir <PATH>                          The path to a root of a repository, defaults to current directory if not provided (Note: <PATH> should be in quotes)
  -u|--update                              Run a paket update
  -up|--update-package <PackageName>       Run a paket update for a single package
  -ua|--update-args <ARGS>                 Args to pass to paket update (Note: <ARGS> should be in quotes)
  -i|--install                             Run a paket install
  -ia|--install-args <ARGS>                Args to pass to paket install (Note: <ARGS> should be in quotes)
  -a|--add <PackageName>                   Run a paket add for package
  -ai|--add-interactive <PackageName>      Run a paket add for package interactive mode
  -ap|--add-project <ProjectName>          Projects to add package to. (Requires -a/--add with package)
  -aa|--add-args <ARGS>                    Args to pass to paket add (Note: <ARGS> should be in quotes)
  -re|--remove <PackageName>               Run a paket remove for package
  -rei|--remove-interactive <PackageName>  Run a paket remove for package interactive mode
  -rep|--remove-project <ProjectName>      Projects to remove package from. (Requires -re/--remove with package) (Note: can be used multiple times for multiple projects)
  -rea|--remove-args <ARGS>                Args to pass to paket remove (Note: <ARGS> should be in quotes)
  -r|--restore                             Run a paket restore
  -ra|--restore-args <ARGS>                Args to pass to paket restore (Note: <ARGS> should be in quotes)
  -s|--simplify                            Run a paket simplify
  -si|--simplify-interactive               Run a paket simplify interactive mode
  -sa|--simplify-args <ARGS>               Args to pass to paket simplify (Note: <ARGS> should be in quotes)
  -rd|--redirects                          Force redirects on install/update/add
  -ri|--reinstall                          Delete the lock file and create from scratch
  -so|--sort                               Sort paket files alphabetically
  -cc|--clear-cache                        Clear caches before running
  -gc|--git-clean                          Run git clean
  -co|--clean-obj                          Clean obj folders to force a full restore
  -upt|--update-paket-tool                 update the paket tool
  -f|--force                               Include force
  -pc|--prompt-close                       Prompt user input
  -v|--verbose                             Verbose logging
  -?|-h|--help                             Show help information
```
