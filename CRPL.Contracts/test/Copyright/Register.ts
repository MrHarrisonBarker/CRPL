import {expect} from "chai";
// @ts-ignore
import {ethers} from "hardhat";
import {SignerWithAddress} from "@nomiclabs/hardhat-ethers/dist/src/signer-with-address";
import {BigNumberish, ContractFactory} from "ethers";
import { Copyright } from "../../typechain";


describe("Register", function ()
{
    let contractFactory: ContractFactory;
    let deployedContract: Copyright;

    let owner: SignerWithAddress;
    let address1: SignerWithAddress;
    let address2: SignerWithAddress;
    let addresses: SignerWithAddress[];

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

    beforeEach(async function ()
    {
        contractFactory = await ethers.getContractFactory("Copyright");

        [owner, address1, address2, ...addresses] = await ethers.getSigners();

        deployedContract = await contractFactory.deploy() as Copyright;

        await deployedContract.deployed();
    });

    it('Should register new copyright', async function() 
    {
        let ownershipStructure: { owner: string; share: BigNumberish }[] = [
            {owner: owner.address, share: 50},
            {owner: address1.address, share: 50}
        ];

        let res = await deployedContract.Register(ownershipStructure, meta);

        res.wait().then(value =>
        {
            expect(value.status).to.eql(1);
            expect(value.events).to.not.be.null;
            expect(value.events != null && value.events[0].event).to.equal("Registered")
        });
    });

    it('Should REVERT no shareholders', async function ()
    {
        await expect(deployedContract.Register([], meta)).to.be.revertedWith('NO_SHAREHOLDERS');
    });

    it('Should REVERT when invalid shareholders', async function ()
    {
        await expect(deployedContract.Register([{owner: ethers.constants.AddressZero, share: 1}], meta)).to.be.revertedWith('INVALID_ADDR');
    });

});