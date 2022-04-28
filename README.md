# CRPL

[![Build Status](https://dev.azure.com/harrison0099/CRPL/_apis/build/status/MrHarrisonBarker.CRPL?branchName=main)](https://dev.azure.com/harrison0099/CRPL/_build/latest?definitionId=3&branchName=main)

CRPL or **C**opy**r**ight on a **p**ublic **l**edger is an open platform for registering and managing the copyrights of
a creators intellectual property secured on the publicly distributed ledger Ethereum. The immutability and public
distribution of a blockchain is the essential component to the success of this platform and allows copyright
registration to be placed in everyone's hands.

## How to run

### Prerequisites

#### MySql
---

Install MySql server version 8+ [guide](https://dev.mysql.com/doc/mysql-installation-excerpt/5.7/en/)

#### .NET6
---

Install the .NET6 SDK from Microsoft [download](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

#### Node.js
---

Install the latest version of Node.js [download](https://nodejs.org/en/download/)

#### (Optional) Angular
---

Should'nt be necessary to run the application but if there's errors install [version 12](https://www.npmjs.com/package/@angular/cli/v/12.0.0)

## Usage

### Setup

#### Config
---

First you'll need to create an **appsettings.json** file in the **CRPL.Web** directory following this template.

```json
{
  "AppSettings" : {
    "ConnectionString": "Server=localhost;Database=crpl;Uid=crpl;Pwd=** PASSWORD **;",
    "EncryptionKey": "** 32 Character randomly generated string used for hashing **",
    "SeqKey": "** OPTIONAL **",
    "IpfsHost": "http://ipfs.harrisonbarker.co.uk",
    "EtherscanHost": "https://etherscan.io",
    "Chains": [
      {
        "Name": "LOCAL",
        "Url": "http://localhost:8545",
        "Id": "444444444500",
        "SystemAccount": {
          "AccountId": "0x12890d2cce102216644c59daE5baed380d84830c",
          "PrivateKey": "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7"
        }
      }
    ]
  },
  "Logging": {
      "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
      }
    },
"AllowedHosts": "*"
}
```

* If you're using the included test chain don't change and chain settings!
* Create a user in mysql that has enough permissions called "crpl" and paste password.
* Generate a random string of length 32 [I used this](http://www.unit-conversion.info/texttools/random-string-generator/).

#### Building the database
---

Now you need to build and migrate the database. To do this you'll need the entity framework cli tool so run.

```console
$ dotnet tool install --global dotnet-ef
```

then migrate!

```console
$ cd CRPL.Web
$ dotnet ef database update --context=ContractContext # creates all tables for holding the deployed smart contract.
$ dotnet ef database update --context=ApplicationContext # creates tables for all application logic.
```

#### Starting the test chain
---

Now you need a test blockchain, I'm using the geth versions of [these](https://github.com/Nethereum/TestChains). You should'nt need to install geth as the executable comes with the test chain I've provided.

```console
$ cd PIPO/TestChains
$ cd geth-clique-mac # mac version
$ ./startgeth.sh
$ cd geth-clique-windows # windows version
$ ./startgeth.bat
```

Now once the database is running, migrated and the test chain is running it's time to start the application.

### Running
---

```console
$ dotnet restore // install dependences
$ dotnet run // run the app
```

This will start the web server it may take a few moments to start up.

Then navigate to [https://localhost:7145/](https://localhost:7145/) which should display:

![](./Operational%20diagrams/spa.png)

This means Angular is starting and will take a moment to start and you'll be automatically redirected to the application so be patient!

### Using
---

I've made a user guide if you're stuck at any point [https://github.com/MrHarrisonBarker/Crpl/wiki/user-guide](https://github.com/MrHarrisonBarker/Crpl/wiki/user-guide)

### Test

```console
$ cd CRPL.Web
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
class that all subsequent applications
inherit ([CopyrightRegistrationApplication](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Data/Applications/DataModels/CopyrightRegistrationApplication.cs))
these application models are then
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

### Sprint 3 (20th Feb – 27th Feb)

![burndown-3](./Sprint%20Reviews/burndown-3.png)

#### Focus

- Dispute filing
- Dispute resolution
- User focused UI elements
- Copyright expiry
- ChainSync&trade;
- Account delete
- Wallet transfer

#### What was achieved

All tasks and functionality assigned to this sprint have been completed.

The first and most important piece of functionality to be implemented for this sprint was to file copyright disputes,
this was done using my existing applications framework with
a [DisputeApplication](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Data/Applications/DataModels/DisputeApplication.cs)
data model representing what will be needed to file a dispute. Because of the existing framework this side of the
implementation was quick and most of the development time was spent on changing and improving the data model for the
needs of the application. Next was to create a
new [dispute service](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/Services/DisputeService.cs) for
handling resolution of the dispute, a rights
holder can then accept or reject the expected recourse described in the application. I've built two expected recourses:
change of ownership that transfers the copyright to the accusing party and payment which transfers a set amount of Eth
to the accuser stated in the application.

Next was handling expiry of copyrights, instead of an explicit state expiry happens via a modifer or check that happens
on requests and transactions with the contract and throws an error if the right is expired I catch this error and set
the work as expired silently in a background service and queue.

```Solidity
modifier isExpired(uint256 rightId)
{
    require(_metadata[rightId].expires > block.timestamp, EXPIRED);
    _;
}
```

Now with a lot of state change and interaction with the chain I need a way of ensuring the system is in sync and
consistent with the blockchain, to do this I
created [ChainSync](https://github.com/MrHarrisonBarker/CRPL/tree/main/CRPL.Web/Core/ChainSync) which is made up of a
background service that runs batches of synchronisation this synchronisation comes from any number
of [ISynchronisers](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/Core/ChainSync/Synchronisers/ISynchroniser.cs)
and at this time
an [OwnershipSynchroniser](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/Core/ChainSync/Synchronisers/OwnershipSynchroniser.cs)
has been implemented which checks the ownership structure on the chain and compares it with our database.

The final feature to implement was two account configuration methods that: transfer ownership and delete account. These
again used the application framework and a
new [AccountManagementService](https://github.com/MrHarrisonBarker/CRPL/blob/main/CRPL.Web/Services/AccountManagementService.cs)
.

#### What was not achieved

All work assigned was completed however I am not confident all edge cases have been covered therefore the reliability of
the code could be questioned.

#### What went wrong

When building out the dispute filing and resolution system the design and operation of the system was not throughout so
significant time was spent trying different paths.

#### Conclusion

This sprint was very successful dispute a lot of work to be done in only a week, it gave the user the ability to file
and resolve disputes, delete account and transfer assets to wallet. The system now recognises expired copyrights and
keeps in sync with the blockchain.

### Sprint 4 (1st March – 8th March)

![burndown-4](./Sprint%20Reviews/burndown-4.png)

#### Focus

- Bug fixes
- Work CDN & Usage (Extra feature)
- Websocket
- Continuous deployment

#### What was achieved

All tasks an functionality assigned to this was implemented in the time period.

This sprint was focused first on two extra features I wanted to get implemented first was a decentralised blockchain
based content delivery network, after researching a number of solutions one stood out as the best option
being [IPFS](https://ipfs.io/) (InterPlanetary File System) which is a peer to peer hypermedia protocol to retrieve
files saved across multiple nodes/peers in chunks. So I installed Ipfs and started up my own node needed to interact and
most importantly add files to the network, using
the [Ipfs.Http.Client](https://github.com/richardschneider/net-ipfs-http-client) library it was then easy to add the
digitally signed to this network returning a [CID](https://docs.ipfs.io/concepts/content-addressing/) which is a unique
identifier (essentially a hash of the file) used to retrieve the file from the Ipfs network. This identifier is saved on
the work.

Once saved to the network a link to the work is created but I wanted a way for creators to measure the number of times
their work has been used, I decided on creating a reverse proxy which takes a request for a work records the use and
then passes the ipfs request onto the client.

The second extra feature implemented was websocket integration which allows for realtime updates to the client which in
a blockchain based application sending transactions that can possibly take any amount of time to complete greatly
improves the usability of the application. This was implemented using register and push methods, when a user loads an
application the client will send a listen command to the backend, whenever an application or work is updated those
updates will pushed out to listeners.

#### What was not achieved

Although all bugs registered in this sprint were fixed a level of polish was and has not been obtained, everything
technically works but the software is still not "user proof"

#### What went wrong

Work on implementing the websocket radically changed a large portion of the frontend dataflow to accommodate the now
realtime asynchronous data that can now change on the fly at anytime and therefore has to change and adapt. This caused
more problems than expected and therefore implementing this feature took considerably more time and change.

#### Conclusion

This sprint was very successful implementing extra quality of life features, the project now feels like a more complete
and holistic product ready to be used.

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