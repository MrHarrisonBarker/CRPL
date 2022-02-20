import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import { Copyright } from "../../typechain";


describe("ApproveOne", function ()
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
            {owner: owner.address, share: 50},
            {owner: address1.address, share: 50}
        ];

        let meta = {
            title: "Hello world",
            expires: 0,
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

    it('Should REVERT when not valid rightId', async function ()
    {
        await expect(deployedContract.ApproveOne(2, address1.address)).to.be.revertedWith('NOT_VALID_RIGHT');
    });

    it('Should REVERT when not valid address', async function ()
    {
        await expect(deployedContract.ApproveOne(1, ethers.constants.AddressZero)).to.be.revertedWith('INVALID_ADDR');
    });

    it('Should REVERT when not sent from shareholder or approved', async function ()
    {
        await expect(deployedContract.connect(address2).ApproveOne(1, address1.address)).to.be.revertedWith('NOT_SHAREHOLDER');
    });

});