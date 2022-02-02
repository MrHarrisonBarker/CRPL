import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import {Standard} from "../typechain";

describe("Standard", function ()
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
        deployedContract = await contractFactory.deploy() as Standard;
        await deployedContract.deployed();
    });

    it("Should construct standard copyright contract with legal def", async function ()
    {
        expect(await deployedContract.LegalDefinition()).to.equal("Standard Copyright legal definition");
    });

    it("The account Should have no portfolio for new contract", async function ()
    {
        expect(await deployedContract.PortfolioSize(owner.address)).to.equal(0);
    });

    it("Should register new copyright", async function ()
    {
        // 
        let ownershipStructure: { owner: string; share: BigNumberish } = {owner: owner.address, share: 100};

        let res = await deployedContract.Register([ownershipStructure]);

        expect(await deployedContract.PortfolioSize(owner.address)).to.equal(1);
    });

    it('Should register with metadata', async function ()
    {
        let metadata = {
            title: "TEST TITLE",
            expires: 1,
            registered: 0,
            workHash: "string",
            workUri: "string",
            legalMeta: "string"
        }
        let ownershipStructure: { owner: string; share: BigNumberish } = {owner: owner.address, share: 100};

        let res = await deployedContract.RegisterWithMeta([ownershipStructure],metadata);

        expect(await deployedContract.PortfolioSize(owner.address)).to.equal(1);
        expect(await deployedContract.Title(1)).to.equal(metadata.title);
    });

    // it("Should propose ownership restructure", async function ()
    // {
    //     let ownershipStructure: { owner: string; share: BigNumberish } = {owner: owner.address, share: 100}
    //     await deployedContract.ProposeRestructure(1, [ownershipStructure]);
    // });
});