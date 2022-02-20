import {assert, expect, use} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import {Copyright} from "../../typechain";

describe("BindRestructure", function ()
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

        let newStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 50},
            {owner: address1.address, share: 50}
        ]

        await deployedContract.ProposeRestructure(1, newStructure);
    });

    it('Should bind a restructure', async function ()
    {
        await deployedContract.connect(address1).BindRestructure(1, true);
        let res = await deployedContract.BindRestructure(1, true);

        res.wait().then(value =>
        {
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("Restructured")
        });
    });

    it('Should REVERT when not sent from shareholder or approved', async function ()
    {
        await expect(deployedContract.connect(address2).BindRestructure(1,true)).to.be.revertedWith('NOT_SHAREHOLDER');
    });

    it('Should REVERT when not valid rightId', async function ()
    {
        await expect(deployedContract.BindRestructure(2,true)).to.be.revertedWith('NOT_VALID_RIGHT');
    });

    it('Should REVERT when not valid rightId', async function ()
    {
        await deployedContract.BindRestructure(1,true)
        await expect(deployedContract.BindRestructure(1,true)).to.be.revertedWith('ALREADY_VOTED');
    });

});