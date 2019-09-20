# Contributing

HI! Thanks you for your interest in RdpMon! We'd love to accept your patches and contributions. Please remember that this project was started first and foremost to serve users of cloud machines directly connected to the Internet, i.e. not behind gateways.

## New feature guidelines

When authoring new features or extending existing ones, consider the following:
- All new features should be accompanied first with a Github issues describing the feature and its necessity.
- Aim for simplicity. Options, buttons, panels etc. detract from that.
- Features should serve the general public. Very specific things for your use case are frowned upon.

## Getting set up

1. Clone this repository

```bash
git clone https://github.com/cameyo/rdpmon
```

2. Open the .sln file with Visual Studio and build.


## Code reviews

All submissions, including submissions by project members, require review. We
use GitHub pull requests for this purpose. Consult
[GitHub Help](https://help.github.com/articles/about-pull-requests/) for more
information on using pull requests.

> Note: one pull request should cover one, atomic feature and/or bug fix. Do not submit pull requests with a plethora of updates, tweaks, fixes and new features.

## Code Style

- Coding style is yet to be documented, so meanwhile please just follow the existing style.
- Comments should be generally avoided except when necessary.

## Commit Messages

Commit messages should follow the Semantic Commit Messages format:

```
label: title

description (optional)

footer
```

1. *label* is one of the following:
    - `fix` - bug fixes.
    - `feat` - features.
    - `docs` - changes to docs, e.g. `docs(api.md): ..` to change documentation.
    - `test` - changes to test infrastructure.
    - `style` - code style: spaces/alignment/wrapping etc.
    - `chore` - build-related work.
2. *title* is a brief summary of changes.
3. *description* is optional.

Example:

```
fix: fix LogIt() method for Windows 2020

This patch fixes logging for Windows Server 2020.

Fixes #123, Fixes #234
```
If your commit fixes something trivial / obvious, no need for #2 and #3. For example if you fix a typo or an obvious issue, we don't want to put you through extra bureaucracy :)

## Dependencies

One of the nice things about RdpMon.exe is its portability and lightness. We'd like to keep it like this, as a single standalone EXE.
If you believe an additional non-system assembly is truly valuable, please make sure it works with 'ilmerge' (the tool we use for merging RdpMon with LiteDB's assembly). What we call 'non-system assembly' is any 3rd party assembly adding extra DLL requirements. Keep in mind that additional dependencies may be a barrier for inclusion.
