import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import { Copyright } from "../../typechain";

describe("SingleOwner", function ()
{
    let contractFactory: ContractFactory;
    let deployedContract: Copyright;

    let owner: SignerWithAddress;
    let address1: SignerWithAddress;

    beforeEach(async function ()
    {
        contractFactory = await ethers.getContractFactory("Copyright");

        [owner, address1] = await ethers.getSigners();
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 100}
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

    it('Should approve for right', async function ()
    {
        let res = await deployedContract.ApproveOne(1, address1.address);

        res.wait().then(value =>
        {
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("Approved")
        });
    });

    it('Should have ownership structure of one address', async function ()
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
            [
                [owner.address, 100]
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

        let res = await deployedContract.BindRestructure(1, true);

        res.wait().then(value =>
        {
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("Restructured")
        });
    });
});