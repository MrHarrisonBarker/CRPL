# CRPL
[![Build Status](https://dev.azure.com/harrison0099/CRPL/_apis/build/status/MrHarrisonBarker.CRPL?branchName=main)](https://dev.azure.com/harrison0099/CRPL/_build/latest?definitionId=3&branchName=main)

CRPL or **C**opy**r**ight on a **p**ublic **l**edger is an open platform for registering and managing the copyrights of
a creators intellectual property secured on the publicly distributed ledger Ethereum. The immutability and public
distribution of a blockchain is the essential component to the success of this platform and allows copyright
registration to be placed in everyone's hands.

## How to run

### Prerequisites

#### .NET6

Install the .NET6 SDK from Microsoft [download](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

#### Node.js

Install the latest version of Node.js [download](https://nodejs.org/en/download/)

## Usage

### Build, run and test

```console
$ dotnet restore // install dependences
$ dotnet run // run the app
$ dotnet test // run all .net tests
```

### Test Angular

```console
$ cd CRPL.Web/ClientApp
$ ng test // run Angular tests
```

### Test smart contracts

```console
$ cd CRPL.Contracts
$ npm install // install dependences
$ npx hardhat test // run smart contract tests
```

## Sprint Reviews

### Sprint 1 (19th Jan – 2nd Feb)

![burndown-1](./Sprint%20Reviews/burndown-1.png)

#### Focus

- Smart contracts
- User accounts
- Authentication
- File hashing
- File signing

#### What was achieved

All tasks and functionality assigned to this sprint have been completed.
A [Copyright](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Contracts/contracts/ICopyright.sol) interface was
written drawing inspiration from the [EIP-721](https://eips.ethereum.org/EIPS/eip-721) contract interface built for
non-fungible tokens, many of the core concepts of this token are applicable to copyright contracts and was a firm base
to start from. However this contract doesn't support multi-party ownership which is core tenant of this system (a book
can obviously have two authors), to solve this issue I built
the [IStructuredOwnership](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Contracts/contracts/IStructuredOwnership.sol)
interface which outlines multi-party ownership of a "
work" using a share based system lifted from limited companies.

Then an abstract implementation of these interfaces was built simply
called [Copyright](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Contracts/contracts/Copyrights/Copyright.sol)
this was then combined with
an [ICopyrightMeta](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Contracts/contracts/ICopyrightMeta.sol)
interface to create derived contracts representing three types of copyright licenses
found [here](https://github.com/MrHarrisonBarker/CRPL/tree/main/CRPL.Contracts/contracts/Copyrights)

A [user account](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Data/Account/UserAccount.cs) model was
developed and migrated into a MySQL database representing a user in the system, to authenticate these users I used
MetaMask to get a users wallet address sign a message with a randomly generated nonce which then gets decoded on the
server to verify the ownership of the original wallet address. All this results in a quick and easy one click login flow
for the system simplifying authentication for user and the system itself.

File hashing is handled by a basic sha-512 implementation offered in .NET, the user uploaded file is also signed both by
a number of individual [WorkSigners](https://github.com/MrHarrisonBarker/CRPL/tree/main/CRPL.Web/WorkSigners) specially
built for different content types (image, sound, video, text/pdf) and a universal signer that encodes a digital
signature into the file.

#### What was not achieved

Expansive unit testing of Angular components and services was not completed only basic sanity checks, this is justified
as frontend flow is subject to change based on user feedback and when the frontend if fully fleshed out (only simple
login, logout buttons, file upload and user info wizard components were built this sprint).

#### What went wrong

Proper forethought into user authentication and the interplay between Ethereum accounts and accounts track on the system
was not taken which resulted in more development time wasted figuring this logic out.

#### Conclusion

This first sprint was very successful achieving all goals set out, smart contracts are in a good place, users can login,
fill out personal info, upload work and download a digitally signed master file.~~~~

## Licence

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

Harrison Barker --- mail@harrisonbarker.co.uk

## Acknowledgments

* https://angular.io/
* https://dotnet.microsoft.com/en-us/apps/aspnet
* https://dotnet.microsoft.com/en-us/
* https://nethereum.com/
* https://ethereum.org/en/
* https://docs.soliditylang.org/en/v0.8.11/
* https://metamask.io/
* https://hardhat.org/
* https://getwaffle.io/
* https://rxjs.dev/
* https://clarity.design/
* https://www.jetbrains.com/youtrack/https://www.jetbrains.com/youtrack/
* https://www.mysql.com/
* https://docs.microsoft.com/en-us/ef/core/