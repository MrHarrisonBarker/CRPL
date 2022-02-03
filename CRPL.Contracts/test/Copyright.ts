import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import {Standard} from "../typechain";


describe("Copyright", function ()
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
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 50},
            {owner: address1.address, share: 50}
        ];

        deployedContract = await contractFactory.deploy() as Standard;

        await deployedContract.deployed();
        await deployedContract.Register(ownershipStructure);
    });

    it('Should have portfolio', async function ()
    {
        expect(await deployedContract.PortfolioSize(owner.address)).to.equal(1);
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

    it('Should be approved', async function ()
    {
        await deployedContract.connect(address1).ApproveOne(1, address2.address);
        expect(await deployedContract.GetApproved(1)).to.equal(address2.address);
    });

    it('Should approve manager for user', async function ()
    {
        let res = await deployedContract.ApproveManager(address1.address, true);

        res.wait().then(value =>
        {
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("ApprovedManager")
        });
    });

    it('Should be manager', async function ()
    {
        await deployedContract.ApproveManager(address1.address, true);
        expect(await deployedContract.IsManager(owner.address, address1.address)).to.equal(true);
    });
});