# I-DOT Assay Studio

I-DOT One graphical control software

---

❗ This README only contains project-specific guidance. Please refer to the [README](https://github.com/Dispendix/dx-idot-sw-codebase/blob/develop/README.md) in the repository root for general information.

---

## Purpose

Assay Studio is the front end for the I.DOT device and, as of now, the central project of the codebase repo - it is where all the different libraries come together.

## Getting started

The project has no additional prerequisites. Open `AssayStudio.sln` to get started.

## Building and Running

---

❗ _This section needs reviewing._

---

It is advisable to use the software installer on your development machine before your first build. This makes sure some configs are copied in the AppData folder. Use the most current version that can be found in the I.DOT One Software Sharepoint: I-DOT One - Documents\05 Design\03 Software\02 SoftwareVersions\2_I-DOT Assay Studio (internal releases)

# Notes / comments

**Log files:**

- You can set "Superdebug" as a flag for a higher resultion in the error logs (produces very big files so only use for short debugging sessions). You can find this setting at the bottom of the Dispendix_IDOTOne.cfg config file. It can also be set at runtime with 'base.LogHandler.LogLevel = DDK.Log.LogLevel.xyz'.
- After the driver config file is read, logs are written to the designated directory - %localappdata%\Dispendix\I-DOT Assay Studio\Logs. Before that, they're written in %localappdata%\Temp\Dispendix_IDOTOne, but that will only store the same 6 entries that aren't needed for troubleshooting.
- Testing showed, that setting "SuperDebug" in the constructur in the code (to have superdebug level logs even before the config file is read) does not provide more entries.

# Release Management

For the general software release workflow, please see the [Software Release SOP](https://cellink.sharepoint.com/:w:/r/sites/DxQMS/Delade%20dokument/02%20Organization,%20roles,%20processes%20and%20SOPs/02%20Processes%20and%20SOPs/Software/01%20Source%20Files/SW22_DX-SW_SoftwareReleaseSOP/SW22_DX-SW_SoftwareReleaseSOP.docx?d=wbb72c5bfd9aa43b0b2310ef9bd4f5609&csf=1&web=1&e=p9d672)

## Release Notes

Every feature branch should contain an update to releaseNotes.txt, where applicable. This is part of the Pull Request checklist. This process helps with writing the release notes before each software release.

Please refer to the file for information regarding formatting and formulation.

## Versioning

The software follows semantic versioning and generally follows the branching model defined in the [SCM Guideline](https://cellink.sharepoint.com/:w:/r/sites/DxSoftwareDevelopment/Files/01_Guidelines/SW17_DX-IDOT-SW_SourceCodeManagementGuidelin_v1.3.docx?d=w2800a3dbf58d48bfae0cfcfd23ec106e&csf=1&web=1&e=DjdQzg). The tool [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning) is used for version management.

The current major, minor and patch version is always specified in the file `version.json`. For the build, GitVersioning calculates the "git height", which is the number of commits since the last major/minor/patch change. This ensures that every commit in a branch results in a unique version number, making setups tracable and reproducible.

Commits based on a release branch are considered "release candiates". Therefore, on release branches, the version must be postfixed with "-rc" (f.e. 1.7.9-rc).

### Upgrade Paths

The software upgrade paths are configured in the following way:

- Major, minor and patch versions can be upgraded, but not downgraded (for example: 1.7.10 -> 1.7.11 is possible, 1.7.10 -> 1.8.0 is possible, 1.7.11 -> 1.7.10 is not possible)
- Upgrading/Downgrading between _Build_ versions can be done freely. For example, 1.7.10.145 can be installed over 1.7.10.148. This is because Windows Installer doesn't understand build versions - only the first 3 version segments are considered. While this is a technical limitation, it helps simplifying the development workflow because these (strictly internal) builds can be easily installed/replaced with each other without having to check / uninstall existing installations on the system first.

### Creating a new version / release branch

In order to faciliate this, a helper script named `prepare-release.ps1` can be found in the Assay Studio folder. This script takes the desired new version as an argument and:

1. Creates a new release branch using the given version (eg. idot-1.7.9)
2. Updates `version.json` with the new version, plus the "rc" postfix
3. Commits the update to `version.json` on the new release branch as the initial commit

**The script only runs if you are currently on the 'develop' branch, since this is the only allowed base branch for release branches, according to our branching model.**

_Example:_

    ./prepare-release.ps1 1.7.9

## Continuous Delivery

Assay Studio is delivered to customers using .msi setups, created with AdvancedInstaller.
The AdvancedInstaller project is located in `Installer/I-DOT Assay Studio Installer.aip`.

Due to licensing reasons, the project can currently opened by @nico-dispendix and our Azure DevOps pipeline.

By default, .msi Setups are published as pipeline artifacts for all commits on a release branch (prefixed with `idot-`) and can be downloaded from the pipeline artifacts. In order to build a setup for other branches, run the pipeline manually in Azure DevOps and check `Force Assay Studio Setup Build`.

For release branches, setup files contain an automatically incremented "release candidate" number (rc). This is calculated using the git height (see Versioning) and ensures unique and reproducible setup builds. This format helps with communication, as every new commit on the release branch because a numbered release candidate.

For non-release branches, the setup contains the current commit hash, following a more "developer friendly" convention.

### Code Signing

The Assay Studio setup is digitally signed with an EV code signing certificate provided by GlobalSign. This prevents Microsoft SmartScreen from popping up, shows our company name instead of "Unknown" in the UAC prompt and helps build user trust.
The certificate is stored in an Azure Key Vault in HSM format. In order to access it for code signing, a tool named "AzureSignTool" is used. The relevant parameters are saved as secret variables in the pipeline library.

Local signing of the setup is also possible using the same tool. If you require the access information to the Azure Key Vault for this purpose, please contact Nico Borchert or BICO Cyber Security.

### Rebranding

The script `RebrandToFlexDrop.py` renames and replaces files and folders to faciliate rebranding of Assay Studio to FlexDrop iQ. The rebranding process is integrated into the CI/CD pipeline. Just check "Rebrand to FlexDrop iQ" when executing the pipeline.

## Release Testing

TBD

