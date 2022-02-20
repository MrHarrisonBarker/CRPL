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
fill out personal info, upload work and download a digitally signed master file.

### Sprint 2 (4th Feb – 18nd Feb)

![burndown-2](./Sprint%20Reviews/burndown-2.png)

#### Focus

- Copyright registration
- Restructure ownership
- Applications
- Multi-party proposal binding
- Search
- Blockchain data injection

#### What was achieved

All tasks and functionality assigned to this sprint have been completed excluding copyright un-registration which was
removed from the sprint part way through over concerns that the feature is unnecessary and would need substantial
thought and debate before implementing.

The first focus of this sprint was the "application" framework which resulted in
the [FormsService](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/Services/FormsService.cs) as generic
interface for interacting and manipulating model driven forms (application), this code allows me to add new forms very
easily with infrastructure already built. The working principle is leveraging the power of **OOP** to create a
base [Application](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Data/Applications/DataModels/Application.cs)
class that all subsequent applications inherit ([CopyrightRegistrationApplication]()) these application models are then
manipulated
with [Submitters](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/Core/Applications/ApplicationSubmitter.cs)
and [Updaters](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/Core/Applications/ApplicationUpdater.cs)
which contain any extra logic to be processed when an application is updated or submitted custom to that type of
application. All these different applications are even stored in the same table on the database thanks to EFCores
support for abstract classes which adds columns for all properties of all inheritors plus a discriminator column to
represent the inherited class.

Once the application framework was built it was time to build specific applications and of course the first was the
copyright registration application which used Angular reactive
forms ([cpy-registration-form](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/ClientApp/src/app/Forms/cpy-registration-form/cpy-registration-form.component.ts))
and
the [input model](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/ClientApp/src/app/_Models/Applications/CopyrightRegistrationInputModel.ts)
to update and submit the finished application.

After submitting these applications the system was going to need to communicate with the blockchain sending
registration, propose and bind transactions. Using the [BlockchainConnection]() from the first sprint and the generated
contract service (thank you Nethereum) it was relatively easy to send these transactions but the processing of them in
the real world can take time (as a result of the Ethereum blockchain) so I didn't want wait for the transaction to be
proofed and mined all within the same http call. My solution for this was to setup
event [listeners](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/Services/Background/BlockchainEventListener.cs)
which poll the blockchain for specific "Events" and
then [process](https://github.com/MrHarrisonBarker/CRPL/tree/main/CRPL.Web/Core/EventProcessors) them running all the
necessary logic based on the type of event and payload data.

However once the copyrights are registered onto the blockchain I need a way of querying it to present relevant data to
the user, this was simple as Nethereum generates all the necessary infrastructure and all I have to do is call the
service methods, you can see this in the get works methods which use
the [injectFromChain](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/Services/QueryService.cs) method to
get data from the chain.

Search has been implemented as an endpoint with all logic tested ready for the frontend ui to be built, I chose to focus
all my ui and design efforts towards the applications and flow of registration and restructures therefore only the
backend was deemed workable for this sprint.

#### What was not achieved

As stated above the unregister feature was dropped this sprint as its worth was called into question as currently theres
no real way of removing a copyright registration in the legal world in practice a copyright is no longer enforced by a
rights holder when the holder no longer actively pursues claims of infringement or the work moves into the public
domain. Therefore giving more reason and though to expiry functions in my contract over a specific un register method.

#### What went wrong

There was many tasks and features that conflicted with underlying design changes made in the first sprint, this is
because all the work sanctioned for this sprint was backlog and chosen before any development work started. This is a
failure on my part to properly review the upcoming work for this sprint after the last, when partaking in the sprint
review I thought thoroughly on what was achieved but not the consequences that work had on the entire system and the
work to be completed.

To mitigate this failure for the upcoming sprint all tasks and features already submitted will be used as a guide not as
a list of what to do next hopefully producing a more accurate backlog of issues to complete.

#### Conclusion

This second sprint was very successful giving users the ability to register copyrights, restructure the ownership of
those copyrights and injection of data stored on the blockchain.

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