import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import {Copyright} from "../../typechain";

describe("CurrentVotes", function ()
{
    let contractFactory: ContractFactory;
    let deployedContract: Copyright;

    let owner: SignerWithAddress;
    let address1: SignerWithAddress;
    let address2: SignerWithAddress;
    let addresses: SignerWithAddress[];

    beforeEach(async function ()
    {
        contractFactory = await ethers.getContractFactory("Copyright");

        [owner, address1, address2, ...addresses] = await ethers.getSigners();
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 70},
            {owner: address1.address, share: 30}
        ];

        let meta = {
            title: "Hello world",
            expires: Date.now() + 999999,
            registered: 0,
            workHash: "",
            workUri: "",
            legalMeta: "",
            workType: "",
            protections: {
                authorship: false,
                commercialAdaptation: false,
                nonCommercialAdaptation: false,
                reviewOrCrit: false,
                commercialPerformance: false,
                nonCommercialPerformance: false,
                commercialReproduction: false,
                nonCommercialReproduction: false,
                commercialDistribution: false,
                nonCommercialDistribution: false
            }
        };

        deployedContract = await contractFactory.deploy() as Copyright;

        await deployedContract.deployed();
        await deployedContract.Register(ownershipStructure, meta);
    });

    it('Should get no current votes', async function ()
    {
        expect(await deployedContract.CurrentVotes(1)).to.eql([]);
    });

    it('Should get current votes', async function ()
    {
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 50},
            {owner: address1.address, share: 50}
        ]

        await deployedContract.ProposeRestructure(1, ownershipStructure);
        await deployedContract.BindRestructure(1, true);

        expect(await deployedContract.CurrentVotes(1)).to.eql([[owner.address, true]]);
    });

    it('Should REVERT when not valid rightId', async function () {
        await expect(deployedContract.CurrentVotes(2)).to.be.revertedWith('NOT_VALID_RIGHT');
    });

});