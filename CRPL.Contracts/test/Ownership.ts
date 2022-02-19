import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import { Copyright } from "../typechain";

describe("Ownership", function ()
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

        deployedContract = await contractFactory.deploy() as Copyright;

        await deployedContract.deployed();
        await deployedContract.Register(ownershipStructure);
    });

    it('Should', async function ()
    {

    });

    it('Should have no proposal', async function ()
    {
        expect(await deployedContract.Proposal(1)).to.eql([[[owner.address, 70],[address1.address, 30]],[]]);
    });

    it('Should have ownership structure', async function ()
    {
        expect(await deployedContract.OwnershipOf(1)).to.eql([[owner.address, 70],[address1.address, 30]]);
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
            [
                [owner.address, 70],
                [address1.address, 30]
            ],
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

        await deployedContract.connect(address1).BindRestructure(1, true);
        let res = await deployedContract.BindRestructure(1, true);

        res.wait().then(value =>
        {
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("Restructured")
        });
    });

    it('Should allow shareholder', async function()
    {
        let res = await deployedContract.connect(address1).ApproveOne(1, address1.address);

        res.wait().then(value =>
        {
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("Approved")
        });
    });

});