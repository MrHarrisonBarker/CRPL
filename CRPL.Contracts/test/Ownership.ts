import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import {Standard} from "../typechain";

describe("Ownership", function ()
{
    let contractFactory: ContractFactory;
    let deployedContract: Standard;

    let owner: SignerWithAddress;
    let address1: SignerWithAddress;
    let address2: SignerWithAddress;
    let addresses: SignerWithAddress[];

    beforeEach(async function ()
    {
        contractFactory = await ethers.getContractFactory("Standard");

        [owner, address1, address2, ...addresses] = await ethers.getSigners();
        let ownershipStructure: { owner: string; share: BigNumberish } = {owner: owner.address, share: 100};

        deployedContract = await contractFactory.deploy() as Standard;

        await deployedContract.deployed();
        await deployedContract.Register([ownershipStructure]);
    });

    it('Should', async function ()
    {

    });

    it('Should have no proposal', async function ()
    {
        expect(await deployedContract.Proposal(1)).to.eql([[[owner.address, 100]],[]]);
    });

    it('Should have ownership structure', async function ()
    {
        expect(await deployedContract.OwnershipOf(1)).to.eql([[owner.address, 100]]);
    });

    it('Should propose new structure', async function ()
    {
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 50},
            {owner: address1.address, share: 50}
        ]

        let res = await deployedContract.ProposeRestructure(1, ownershipStructure);

        res.wait().then(value =>
        {
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("ProposedRestructure")
        });

        expect(await deployedContract.Proposal(1)).to.eql([
            [[owner.address, 100]],
            [
                [owner.address, 50],
                [address1.address, 50]
            ]
        ]);
    });

    it('Should bind a restructure', async function ()
    {
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 50},
            {owner: address1.address, share: 50}
        ]

        await deployedContract.ProposeRestructure(1, ownershipStructure);

        let res = await deployedContract.BindRestructure(1, true);

        res.wait().then(value =>
        {
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("Restructured")
        });
    });

});