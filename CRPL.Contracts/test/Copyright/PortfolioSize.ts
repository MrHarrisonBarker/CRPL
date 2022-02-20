import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import { Copyright } from "../../typechain";


describe("PortfolioSize", function ()
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

    it('Should have portfolio', async function ()
    {
        expect(await deployedContract.PortfolioSize(owner.address)).to.equal(1);
    });

    it('Should REVERT when not valid address', async function ()
    {
        await expect(deployedContract.PortfolioSize(ethers.constants.AddressZero)).to.be.revertedWith('INVALID_ADDR');
    });
});